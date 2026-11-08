using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Application.Features.Admin.Commands;

public record CreateSuperAdminCommand(CreateAdminRequest Request) : IRequest<AdminResponse>;

public class CreateSuperAdminCommandHandler : IRequestHandler<CreateSuperAdminCommand, AdminResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<CreateSuperAdminCommandHandler> _logger;

    public CreateSuperAdminCommandHandler(IUserServiceDbContext dbContext, ILogger<CreateSuperAdminCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<AdminResponse> Handle(CreateSuperAdminCommand request, CancellationToken cancellationToken)
    {
        var username = request.Request.Username?.Trim().ToLower();
        var email = request.Request.Email?.Trim().ToLower();

        var adminExists = await _dbContext.Admins.AnyAsync(
            a => a.Username.Trim().ToLower() == username && a.Email.Trim().ToLower() == email, cancellationToken);

        if (adminExists)
        {
            _logger.LogWarning("SuperAdmin already exists");
            throw new AdminAlreadyExistsException();
        }

        var admin = new AdminEntity
        {
            Name = request.Request.Name,
            Surname = request.Request.Surname,
            Username = request.Request.Username,
            Email = request.Request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            MarketId = request.Request.MarketId,
            PhoneNumber = request.Request.PhoneNumber,
            Role = RoleStatus.SuperAdmin,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Admins.AddAsync(admin, cancellationToken);
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
            CreatedAt = admin.CreatedAt
        };
    }
}