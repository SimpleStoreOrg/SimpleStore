using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Cart.Queries;

public record GetMyCartQuery : IRequest<CartResponse>;

public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, CartResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<GetMyCartQueryHandler> _logger;
    private readonly IHttpContextAccessor _accessor;

    public GetMyCartQueryHandler(
        IOrderServiceDbContext dbContext,
        ILogger<GetMyCartQueryHandler> logger,
        IHttpContextAccessor accessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
    }
    public async Task<CartResponse> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(customerIdStr, out var customerId);
        
        var cart = await _dbContext.Carts.Include(c=>c.CartItems).FirstOrDefaultAsync(
            c=>c.CustomerId == customerId, cancellationToken);
        
        if (cart == null)
        {
            throw new CartNotFoundException();
        }
        
        return new CartResponse
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            CartItems = cart.CartItems.Select(ci => new CartItemsResponse
            {
                Id = ci.Id,
                CartId = ci.CartId,
                MarketId = ci.MarketId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Price,
                TotalItemPrice = ci.TotalItemPrice
            }).ToList(),
            TotalPrice = cart.TotalPrice
        };
    }
}