using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Common;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Admin.Queries;

public record GetAdminsQuery(int? PageNumber = null, int? PageSize = null) : IRequest<Pagination<AdminResponse>>;

public class GetAdminsQueryHandler : IRequestHandler<GetAdminsQuery, Pagination<AdminResponse>>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<GetAdminsQueryHandler> _logger;

    public GetAdminsQueryHandler(IUserServiceDbContext dbContext, ILogger<GetAdminsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Pagination<AdminResponse>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber.HasValue && request.PageNumber.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }

        if (request.PageSize.HasValue && request.PageSize.Value <= 0)
        {
            throw new IncorrectPaginationException("Page size must be greater than 0");
        }

        var query = _dbContext.Admins.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            query = query.OrderBy(a => a.Id)
                .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value);
        }

        var admin = await query.OrderBy(a => a.Id)
            .Select(a => new AdminResponse
            {
                Id = a.Id,
                Name = a.Name,
                Surname = a.Surname,
                Username = a.Username,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                Role = a.Role,
                MarketId = a.MarketId,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                DeletedAt = a.DeletedAt
            }).ToListAsync(cancellationToken);

        return new Pagination<AdminResponse>
        {
            Items = admin,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalCount
        };
    }
}