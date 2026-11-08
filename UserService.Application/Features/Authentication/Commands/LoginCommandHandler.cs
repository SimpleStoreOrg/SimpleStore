using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;
using UserService.Application.Services;
using UserService.Domain.Entities;
using LoginRequest = UserService.Application.DTOs.Request.LoginRequest;

namespace UserService.Application.Features.Authentication.Commands;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly JwtService _service;

    public LoginCommandHandler(
        IUserServiceDbContext dbContext,
        ILogger<LoginCommandHandler> logger,
        JwtService service)
    {
        _dbContext = dbContext;
        _logger = logger;
        _service = service;
    }
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = request.Request.Username?.Trim().ToLower();
        var email = request.Request.Email?.Trim().ToLower();
        var role = request.Request.Role;
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Username.Trim().ToLower() == username && u.Email.Trim().ToLower() == email && u.Role == role, cancellationToken);

        if (user == null)
        {
            _logger.LogError("User with Username {Username} not found", username);
            throw new UserNotFoundException();
        }

        var validUser = BCrypt.Net.BCrypt.Verify(request.Request.Password, user.PasswordHash);
        if (!validUser)
        {
            _logger.LogError("Wrong password for User: {Username}", username);
            throw new WrongPasswordException();
        }

        var accessToken = _service.GenerateToken(user);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }
}