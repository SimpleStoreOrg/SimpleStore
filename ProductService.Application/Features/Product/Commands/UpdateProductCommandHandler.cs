using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Product.Commands;

public record UpdateProductCommand(long Id, UpdateProductRequest Request) : IRequest<ProductResponse>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IProductServiceDbContext dbContext, ILogger<UpdateProductCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with Id {Id} not found", request.Id);
            throw new ProductNotFoundException();
        }

        product.Name = request.Request.Name;
        product.CategoryId = request.Request.CategoryId;
        product.Price = request.Request.Price;
        product.Stock = request.Request.Stock;
        product.UpdatedAt = DateTime.UtcNow;
        ;
        
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            CategoryId = product.CategoryId,
            MarketId = product.MarketId,
            Price = product.Price,
            ExpiresAt = product.ExpiresAt,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            DeletedAt = product.DeletedAt
        };
    }
}