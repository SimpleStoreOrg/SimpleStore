using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Common;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Customer.Queries;

public record GetCustomersQuery(int? PageNumber, int? PageSize) : IRequest<Pagination<CustomerResponse>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Pagination<CustomerResponse>>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(IUserServiceDbContext dbContext, ILogger<GetCustomersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Pagination<CustomerResponse>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber.HasValue && request.PageNumber.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }

        if (request.PageSize.HasValue && request.PageSize.Value <= 0)
        {
            throw new IncorrectPaginationException("Page size must be greater than 0");
        }

        var query = _dbContext.Customers.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            query = query.OrderBy(c => c.Id)
                .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value);
        }

        var customer = await query.OrderBy(c => c.Id)
            .Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Surname = c.Surname,
                Birthdate = c.Birthdate,
                Username = c.Username,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Role = c.Role,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                DeletedAt = c.DeletedAt
            }).ToListAsync(cancellationToken);

        return new Pagination<CustomerResponse>
        {
            Items = customer,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalCount
        };
    }
}