using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Application.Interfaces.External;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Order.Commands;

public record CreateOrderCommand : IRequest<OrderResponse>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IHttpContextAccessor _accessor;
    private readonly IProductApi _productApi;

    public CreateOrderCommandHandler(
        IOrderServiceDbContext dbContext,
        ILogger<CreateOrderCommandHandler> logger, 
        IHttpContextAccessor accessor,
        IProductApi productApi)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
        _productApi = productApi;
    }
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(customerIdStr, out var customerId);
        
        var cart = await _dbContext.Carts.Include(c=>c.CartItems).FirstOrDefaultAsync(
            c => c.CustomerId == customerId, cancellationToken);
        if (cart == null)
        {
            throw new CartNotFoundException();
        }

        if (cart.CartItems.Count == 0)
        {
            throw new EmptyCartException();
        }

        var token = _accessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (token == null)
        {
            throw new NotAuthorizedException();
        }

        var productIds = cart.CartItems.Select(ci => ci.ProductId).ToArray();
        
        var getProductsByIds = new IProductApi.GetProductsByIdsRequest
        {
            ProductIds = productIds,
            MarketId = cart.MarketId
        };

        var products = await _productApi.GetProductByIds(getProductsByIds, token);
        
        foreach (var item in cart.CartItems)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
        
            if (product == null)
            {
                throw new ProductNotFoundException();
            }
            
            if (item.Quantity > product.Stock)
            {
                throw new InsufficientStockException(product.Id);
            }
        }
        
        var order = new OrderEntity
        {
            CustomerId = customerId,
            MarketId = cart.MarketId,
            Status = OrderStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        var orderItems = new List<OrderItemsEntity>();
        
        foreach (var cartItem in cart.CartItems)
        {
            var orderItem = new OrderItemsEntity
            {
                OrderId = order.Id,
                MarketId = cartItem.MarketId,
                ProductId = cartItem.ProductId,
                Price = cartItem.Price,
                Quantity = cartItem.Quantity,
                TotalItemPrice = cartItem.TotalItemPrice
            };
            orderItems.Add(orderItem);
        }

        order.OrderItems = orderItems;
        order.TotalPrice = cart.TotalPrice;

        _dbContext.Carts.Remove(cart);
        
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var item in order.OrderItems)
        {
            await _productApi.UpdateStock(item.ProductId, new IProductApi.UpdateStockRequest()
            {
                Quantity = item.Quantity
            }, token);
        }

        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = customerId,
            MarketId = order.MarketId,
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
        };
    }
}