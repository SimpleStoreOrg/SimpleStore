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

public record CreateCartCommand(CreateCartRequest Request) : IRequest<CartResponse>;

public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, CartResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<CreateCartCommandHandler> _logger;
    private readonly IHttpContextAccessor _accessor;
    private readonly IProductApi _productApi;

    public CreateCartCommandHandler(
        IOrderServiceDbContext dbContext,
        ILogger<CreateCartCommandHandler> logger,
        IHttpContextAccessor accessor,
        IProductApi productApi
        )
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
        _productApi = productApi;
    }
    public async Task<CartResponse> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdStr == null)
        {
            throw new NotAuthorizedException();
        }
        
        long.TryParse(customerIdStr, out var customerId);
        
        var cartExists = await _dbContext.Carts
            .AnyAsync(c => c.CustomerId == customerId, cancellationToken);
        
        if (cartExists)
        {
            throw new CartAlreadyExistsException();
        }

        var token = _accessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (token == null)
        {
            throw new NotAuthorizedException();
        }
        
        var productIds = request.Request.CartItemsRequest.Select(c => c.ProductId).ToArray();

        var getProductByIdsRequest = new IProductApi.GetProductsByIdsRequest
        {
            ProductIds = productIds,
            MarketId = request.Request.MarketId
        };
        var products = await _productApi.GetProductByIds(getProductByIdsRequest, token);
            
        var cartItems = new List<CartItemsEntity>();

        // var cartId = Guid.NewGuid(); will need after changing to GUID
        
        var cart = new CartEntity
        {
            CustomerId = customerId,
            MarketId = request.Request.MarketId,
            CreatedAt = DateTime.UtcNow
        };

        var createdCartResult = await _dbContext.Carts.AddAsync(cart, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var cartItemRequest in request.Request.CartItemsRequest)
        {
            var product = products.FirstOrDefault(p => p.Id == cartItemRequest.ProductId);
            CartItemsEntity cartItem = new CartItemsEntity
            {
                CartId = createdCartResult.Entity.Id,
                ProductId = cartItemRequest.ProductId,
                MarketId = request.Request.MarketId,
                Price = product.Price,
                Quantity = cartItemRequest.Quantity,
                TotalItemPrice = cartItemRequest.Quantity * product.Price
            };
            cartItems.Add(cartItem);
        }

        createdCartResult.Entity.CartItems = cartItems;
        createdCartResult.Entity.TotalPrice = cartItems.Sum(item => item.TotalItemPrice);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        var cartItemResponse = new List<CartItemsResponse>();
        foreach (var item in cartItems)
        {
            CartItemsResponse response = new CartItemsResponse
            {
                Id = item.Id,
                CartId = item.CartId,
                ProductId = item.ProductId,
                Price = item.Price,
                Quantity = item.Quantity,
                MarketId = item.MarketId,
                TotalItemPrice = item.TotalItemPrice
            };
            
            cartItemResponse.Add(response);
        }
        
        return new CartResponse
        {
            Id = createdCartResult.Entity.Id,
            CustomerId = cart.CustomerId,
            CartItems = cartItemResponse,
            TotalPrice = createdCartResult.Entity.TotalPrice
        };
    }
}