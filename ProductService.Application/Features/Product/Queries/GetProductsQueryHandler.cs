using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Common;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Product.Queries;

public record GetProductsQuery(int? PageNumber = null, int? PageSize = null) : IRequest<Pagination<ProductResponse>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Pagination<ProductResponse>>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(IProductServiceDbContext dbContext, ILogger<GetProductsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Pagination<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber.HasValue && request.PageNumber.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }
        
        if (request.PageSize.HasValue && request.PageSize.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }

        var query = _dbContext.Products.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            query = query.OrderBy(c => c.Id)
                .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value);
        }

        var product = await query.OrderBy(p => p.Id)
            .Select(c => new ProductResponse
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
            }).ToListAsync(cancellationToken);

        return new Pagination<ProductResponse>
        {
            Items = product,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}