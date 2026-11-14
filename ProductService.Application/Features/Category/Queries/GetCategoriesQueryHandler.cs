using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Common;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Category.Queries;

public record GetCategoriesQuery(int? PageNumber = null, int? PageSize = null) : IRequest<Pagination<CategoryResponse>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Pagination<CategoryResponse>>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<GetCategoriesQueryHandler> _logger;

    public GetCategoriesQueryHandler(IProductServiceDbContext dbContext, ILogger<GetCategoriesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Pagination<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber.HasValue && request.PageNumber.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }
        
        if (request.PageSize.HasValue && request.PageSize.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }

        var query = _dbContext.Categories.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            query = query.OrderBy(c => c.Id)
                .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value);
        }

        var category = await query.OrderBy(c => c.Id)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            }).ToListAsync(cancellationToken);

        return new Pagination<CategoryResponse>
        {
            Items = category,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}