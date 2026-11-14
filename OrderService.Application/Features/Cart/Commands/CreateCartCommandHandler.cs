using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Features.Cart.Commands;

public record CreateCartCommand : IRequest<CartResponse>;

public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, CartResponse>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<CreateCartCommandHandler> _logger;
    private readonly IHttpContextAccessor _accessor;

    public CreateCartCommandHandler(
        IOrderServiceDbContext dbContext,
        ILogger<CreateCartCommandHandler> logger,
        IHttpContextAccessor accessor
        )
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
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

        var cart = new CartEntity
        {
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Carts.AddAsync(cart, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CartResponse
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            TotalPrice = cart.TotalPrice
        };
    }
}