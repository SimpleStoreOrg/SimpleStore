using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Admin.Queries;

public record GetAdminByIdQuery(long Id) : IRequest<AdminResponse>;

public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, AdminResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<GetAdminByIdQueryHandler> _logger;

    public GetAdminByIdQueryHandler(IUserServiceDbContext dbContext, ILogger<GetAdminByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<AdminResponse> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (admin == null)
        {
            _logger.LogWarning("Admin with Id {Id} not found", request.Id);
            throw new AdminNotFoundException();
        }

        return new AdminResponse
        {
            Id = admin.Id,
            Name = admin.Name,
            Surname = admin.Surname,
            Username = admin.Username,
            Email = admin.Email,
            PhoneNumber = admin.PhoneNumber,
            Role = admin.Role,
            MarketId = admin.MarketId,
            CreatedAt = admin.CreatedAt,
            UpdatedAt = admin.UpdatedAt,
            DeletedAt = admin.DeletedAt
        };
    }
}