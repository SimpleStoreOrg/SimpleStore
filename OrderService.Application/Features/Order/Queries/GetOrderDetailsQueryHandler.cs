using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Order.Queries;

public record GetOrderDetailsQuery(long Id) : IRequest<OrderResponse>;

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<GetOrderDetailsQueryHandler> _logger;
    private readonly IHttpContextAccessor _accessor;

    public GetOrderDetailsQueryHandler(
        IOrderServiceDbContext dbContext,
        ILogger<GetOrderDetailsQueryHandler> logger, 
        IHttpContextAccessor accessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
    }
    public async Task<OrderResponse> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var role = _accessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        if (role == null)
        {
            throw new NotAuthorizedException();
        }

        var query = _dbContext.Orders.Where(o => o.Id == request.Id).AsQueryable();

        if (role == "MarketAdmin" || role == "ShopperAssistant")
        {
            var marketIdStr = _accessor.HttpContext?.User.FindFirst("MarketId")?.Value;
            if (marketIdStr == null)
            {
                throw new NotAuthorizedException();
            }
            
            long.TryParse(marketIdStr, out var marketId);

            query = query.Where(o => o.MarketId == marketId);
        }

        if (role == "Customer")
        {
            var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerIdStr == null)
            {
                throw new NotAuthorizedException();
            }
        
            long.TryParse(customerIdStr, out var customerId);
            
            query = query.Where(o => o.CustomerId == customerId);
        }

        var order = await query.Include(o=>o.OrderItems).FirstOrDefaultAsync(cancellationToken);
        if (order == null)
        {
            throw new OrderNotFoundException();
        }

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
                TotalItemPrice = oi.Quantity * oi.Price
            }).ToList(),
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            CompletedAt = order.CompletedAt,
        };
    }
}