using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Application.Features.ShopperAssistant.Commands;

public record CreateShopperAssistantCommand(CreateShopperAssistantRequest Request) : IRequest<ShopperAssistantResponse>;

public class CreateShopperAssistantCommandHandler : IRequestHandler<CreateShopperAssistantCommand, ShopperAssistantResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<CreateShopperAssistantCommandHandler> _logger;
    private readonly IHttpContextAccessor _accessor;

    public CreateShopperAssistantCommandHandler(
        IUserServiceDbContext dbContext,
        ILogger<CreateShopperAssistantCommandHandler> logger,
        IHttpContextAccessor accessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
    }
    
    public async Task<ShopperAssistantResponse> Handle(CreateShopperAssistantCommand request, CancellationToken cancellationToken)
    {
        var name = request.Request.Name?.Trim().ToLower();
        var surname = request.Request.Surname?.Trim().ToLower();
        var username = request.Request.Username?.Trim().ToLower();

        var exists = await _dbContext.ShopperAssistants.AnyAsync(
            sh => sh.Name.Trim().ToLower() == name &&
                  sh.Surname.Trim().ToLower() == surname &&
                  sh.Username.Trim().ToLower() == username, cancellationToken);
        
        if (exists)
        {
            throw new ShopperAssistantAlreadyExistsException();
        }

        var marketIdStr = _accessor.HttpContext?.User.FindFirst("MarketId")?.Value;
        if (marketIdStr == null)
        {
            throw new NotAuthorizedException("Admin is not authorized");
        }

        long marketId = long.Parse(marketIdStr);

        var shAssistant = new ShopperAssistantEntity
        {
            Name = request.Request.Name,
            Surname = request.Request.Surname,
            Username = request.Request.Username,
            Email = request.Request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            PhoneNumber = request.Request.PhoneNumber,
            MarketId = marketId,
            Role = RoleStatus.ShopperAssistant,
            Position = request.Request.Position,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.ShopperAssistants.AddAsync(shAssistant, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ShopperAssistantResponse
        {
            Id = shAssistant.Id,
            Name = shAssistant.Name,
            Surname = shAssistant.Surname,
            Username = shAssistant.Username,
            Email = shAssistant.Email,
            PhoneNumber = shAssistant.PhoneNumber,
            MarketId = shAssistant.MarketId,
            Role = shAssistant.Role,
            Position = shAssistant.Position,
            CreatedAt = shAssistant.CreatedAt
        };
    }
}