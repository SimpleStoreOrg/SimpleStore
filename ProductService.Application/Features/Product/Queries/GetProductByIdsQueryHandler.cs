using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Product.Queries;

public record GetProductByIdsQuery(GetProductsByIdsRequest Request) : IRequest<List<ProductResponse>>;

public class GetProductByIdsQueryHandler : IRequestHandler<GetProductByIdsQuery, List<ProductResponse>> 
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<GetProductByIdsQueryHandler> _logger;

    public GetProductByIdsQueryHandler(IProductServiceDbContext dbContext, ILogger<GetProductByIdsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<List<ProductResponse>> Handle(GetProductByIdsQuery request, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products
            .Where(p =>p.MarketId == request.Request.MarketId &&
                       request.Request.ProductIds.Contains(p.Id)).ToListAsync(cancellationToken);
        
        var result = new List<ProductResponse>();
        
        foreach (var c in products)
        {
            ProductResponse response = new ProductResponse
            {
                Id = c.Id,
                Name = c.Name,
                CategoryId = c.CategoryId,
                MarketId = c.MarketId,
                Price = c.Price,
                ExpiresAt = c.ExpiresAt,
                Stock = c.Stock,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                DeletedAt = c.DeletedAt
            };
            
            result.Add(response);
        }

        return result;
    }
}