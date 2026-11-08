using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.ShopperAssistant.Commands;

public record UpdateShopperAssistantCommand(long Id, UpdateShopperAssistantRequest Request)
    : IRequest<ShopperAssistantResponse>;

public class UpdateShopperAssistantCommandHandler : IRequestHandler<UpdateShopperAssistantCommand, ShopperAssistantResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<UpdateShopperAssistantCommandHandler> _logger;

    public UpdateShopperAssistantCommandHandler(IUserServiceDbContext dbContext, ILogger<UpdateShopperAssistantCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<ShopperAssistantResponse> Handle(UpdateShopperAssistantCommand request, CancellationToken cancellationToken)
    {
        var username = request.Request.Username?.Trim().ToLower();

        var shAssistant = await _dbContext.ShopperAssistants.FirstOrDefaultAsync(
            sh => sh.Id == request.Id &&
                  sh.Username.Trim().ToLower() == username, cancellationToken);
        
        if (shAssistant == null)
        {
            throw new ShopperAssistantNotFoundException();
        }

        shAssistant.Name = request.Request.Name;
        shAssistant.Surname = request.Request.Surname;
        shAssistant.Username = request.Request.Username;
        shAssistant.Email = request.Request.Email;
        shAssistant.PhoneNumber = request.Request.PhoneNumber;
        shAssistant.Position = request.Request.Position;
        shAssistant.UpdatedAt = DateTime.UtcNow;

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
            CreatedAt = shAssistant.CreatedAt,
            UpdatedAt = shAssistant.UpdatedAt
        };
    }
}