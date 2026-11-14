using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Entities;

namespace ProductService.Application.Features.Product.Commands;

public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductResponse>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IProductServiceDbContext dbContext, ILogger<CreateProductCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var name = request.Request.Name?.Trim().ToLower();

        var productExists = await _dbContext.Products.AnyAsync(
            p => p.Name.Trim().ToLower() == name, cancellationToken);
        if (productExists)
        {
            _logger.LogWarning("Product already exists");
            throw new ProductAlreadyExistsException();
        }

        var product = new ProductEntity
        {
            Name = request.Request.Name,
            CategoryId = request.Request.CategoryId,
            Price = request.Request.Price,
            Stock = request.Request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            CategoryId = product.CategoryId,
            MarketId = product.MarketId,
            Price = product.Price,
            ExpiresAt = product.ExpiresAt,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };
    }
}