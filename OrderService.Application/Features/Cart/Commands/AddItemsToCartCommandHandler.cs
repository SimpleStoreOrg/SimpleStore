using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Request;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Application.Interfaces.External;
using OrderService.Domain.Entities;

namespace OrderService.Application.Features.Cart.Commands;

public record AddItemToCartCommand(AddItemToCartRequest Request) : IRequest<CartResponse>;

public class AddItemsToCartCommandHandler : IRequestHandler<AddItemToCartCommand, CartResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<AddItemsToCartCommandHandler> _logger;
    private readonly IProductApi _productApi;
    private readonly IHttpContextAccessor _accessor;

    public AddItemsToCartCommandHandler(
        IOrderServiceDbContext dbContext,
        ILogger<AddItemsToCartCommandHandler> logger,
        IProductApi productApi, 
        IHttpContextAccessor accessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _productApi = productApi;
        _accessor = accessor;
    }
    public async Task<CartResponse> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(customerIdStr, out var customerId);
        
        var cart = await _dbContext.Carts.Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
        
        if (cart == null)
        {
            throw new CartNotFoundException();
        }

        var marketId = cart.CartItems.Select(ci => ci.MarketId).FirstOrDefault();

        var token = _accessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (token == null)
        {
            throw new Exception();
        }

        foreach (var item in request.Request.CreateCartItemRequest)
        {
            ProductResponse product;
            try
            {
                product = await _productApi.GetProductById(item.ProductId, token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (marketId != product.MarketId)
            {
                throw new InvalidMarketException("All products must be from the same market");
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                
                if (existingItem.Quantity > product.Stock)
                {
                    throw new InsufficientStockException(product.Id);
                }

                existingItem.Price = product.Price;
                existingItem.TotalItemPrice = existingItem.Quantity * product.Price;
            }
            else
            {
                if (item.Quantity > product.Stock)
                {
                    throw new InsufficientStockException(product.Id);
                }

                var cartItem = new CartItemsEntity
                {
                    CartId = cart.Id,
                    ProductId = item.ProductId,
                    MarketId = product.MarketId,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    TotalItemPrice = product.Price * item.Quantity,
                };
                
                cart.CartItems.Add(cartItem);
            }
        }
        
        cart.TotalPrice = cart.CartItems.Sum(item => item.TotalItemPrice);
        
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