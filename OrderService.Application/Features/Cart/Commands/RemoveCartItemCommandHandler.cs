using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Cart.Commands;

public record RemoveCartItemCommand(long[] CartItemIds) : IRequest<CartResponse>;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, CartResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<RemoveCartItemCommandHandler> _logger;
    private readonly IHttpContextAccessor _accessor;

    public RemoveCartItemCommandHandler(
        IOrderServiceDbContext dbContext,
        ILogger<RemoveCartItemCommandHandler> logger,
        IHttpContextAccessor accessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
    }
    public async Task<CartResponse> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(customerIdStr, out var customerId);

        var cart = await _dbContext.Carts.Include(c=>c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

        if (cart == null)
        {
            throw new CartNotFoundException();
        }

        if (request.CartItemIds.Length > 0) 
        {
            foreach (var itemId in request.CartItemIds.Distinct())
            {
                var item = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
                        
                if (item == null)
                {
                    throw new ItemNotFoundException(itemId);
                }

                cart.CartItems.Remove(item);
                _dbContext.CartItems.Remove(item);
            }
        }

        if (cart.CartItems.Count == 0)
        {
            _dbContext.Carts.Remove(cart);
        }
        
        cart.TotalPrice = cart.CartItems.Sum(ci => ci.TotalItemPrice);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new CartResponse
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            CartItems = cart.CartItems.Select(ci => new CartItemsResponse
            {
                Id = ci.Id,
                CartId = cart.Id,
                ProductId = ci.ProductId,
                MarketId = ci.MarketId,
                Quantity = ci.Quantity,
                Price = ci.Price,
                TotalItemPrice = ci.TotalItemPrice
            }).ToList(),
            TotalPrice = cart.TotalPrice
        };
    }
}