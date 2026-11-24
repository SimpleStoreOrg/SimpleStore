using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Order.Commands;

public record AssignToShopperAssistantCommand(long OrderId) : IRequest<OrderResponse>;

public class AssignToShopperAssistantCommandHandler : IRequestHandler<AssignToShopperAssistantCommand, OrderResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<AssignToShopperAssistantCommandHandler> _logger;
    private readonly IHttpContextAccessor _accessor;
    private readonly TotalOrderCount _count;

    public AssignToShopperAssistantCommandHandler(
        IOrderServiceDbContext dbContext,
        ILogger<AssignToShopperAssistantCommandHandler> logger, 
        IHttpContextAccessor accessor,
        IOptions<TotalOrderCount> count)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
        _count = count.Value;
    }
    public async Task<OrderResponse> Handle(AssignToShopperAssistantCommand request, CancellationToken cancellationToken)
    {
        var shAssistantIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (shAssistantIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(shAssistantIdStr, out var shAssistantId);

        var marketIdStr = _accessor.HttpContext?.User.FindFirst("MarketId")?.Value;
        if (marketIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(marketIdStr, out var marketId);

        var order = await _dbContext.Orders.FirstOrDefaultAsync(
            o => o.Id == request.OrderId && o.MarketId == marketId, cancellationToken);
        
        if (order == null)
        {
            throw new OrderNotFoundException();
        }
        
        if (order.ShopperAssistantId != null)
        {
            throw new OrderAlreadyAssignedException();
        }
        
        var totalCount = await _dbContext.Orders.CountAsync(o =>
            o.ShopperAssistantId == shAssistantId &&
            (o.Status == OrderStatus.Accepted ||
             o.Status == OrderStatus.Collecting), cancellationToken);
        
        if (totalCount >= _count.TotalCount)
        {
            throw new ShAssistantOrderLimitException();
        }
        
        order.Status = OrderStatus.Accepted;
        order.ShopperAssistantId = shAssistantId;
        order.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            MarketId = order.MarketId,
            ShopperAssistantId = order.ShopperAssistantId,
            Status = order.Status,
            OrderItems = order.OrderItems.Select(oi => new OrderItemsResponse
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                MarketId = oi.MarketId,
                ProductId = oi.ProductId,
                Price = oi.Price,
                Quantity = oi.Quantity,
                TotalItemPrice = oi.TotalItemPrice
            }).ToList(),
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            CompletedAt = order.CompletedAt
        };
    }
}

public class TotalOrderCount
{
    public int TotalCount { get; set; }
}