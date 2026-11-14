using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Product.Queries;

public record GetProductByIdQuery(long Id) : IRequest<ProductResponse>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(IProductServiceDbContext dbContext, ILogger<GetProductByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with Id {Id} not found", request.Id);
            throw new ProductNotFoundException();
        }

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