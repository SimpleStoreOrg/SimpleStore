using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Admin.Commands;

public record UpdateAdminCommand(long Id, UpdateAdminRequest Request) : IRequest<AdminResponse>;

public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, AdminResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<UpdateAdminCommandHandler> _logger;

    public UpdateAdminCommandHandler(IUserServiceDbContext dbContext, ILogger<UpdateAdminCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<AdminResponse> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (admin == null)
        {
            _logger.LogWarning("Admin with Id {Id} not found", request.Id);
            throw new AdminNotFoundException();
        }

        admin.Name = request.Request.Name;
        admin.Surname = request.Request.Surname;
        admin.Username = request.Request.Username;
        admin.Email = request.Request.Email;
        admin.PhoneNumber = request.Request.PhoneNumber;
        admin.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new AdminResponse
        {
            Id = admin.Id,
            Name = admin.Name,
            Surname = admin.Surname,
            Username = admin.Username,
            Email = admin.Email,
            MarketId = admin.MarketId,
            PhoneNumber = admin.PhoneNumber,
            Role = admin.Role,
            CreatedAt = admin.CreatedAt,
            UpdatedAt = admin.UpdatedAt
        };
    }
}