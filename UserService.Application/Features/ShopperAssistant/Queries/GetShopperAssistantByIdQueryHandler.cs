using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.ShopperAssistant.Queries;

public record GetShopperAssistantByIdQuery(long Id) : IRequest<ShopperAssistantResponse>;

public class GetShopperAssistantByIdQueryHandler : IRequestHandler<GetShopperAssistantByIdQuery, ShopperAssistantResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<GetShopperAssistantByIdQueryHandler> _logger;

    public GetShopperAssistantByIdQueryHandler(
        IUserServiceDbContext dbContext,
        ILogger<GetShopperAssistantByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<ShopperAssistantResponse> Handle(GetShopperAssistantByIdQuery request, CancellationToken cancellationToken)
    {
        var shAssistant = await _dbContext.ShopperAssistants.FirstOrDefaultAsync(
            sh => sh.Id == request.Id, cancellationToken);
        if (shAssistant == null)
        {
            throw new ShopperAssistantNotFoundException();
        }

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
            UpdatedAt = shAssistant.UpdatedAt,
            DeletedAt = shAssistant.DeletedAt
        };
    }
}