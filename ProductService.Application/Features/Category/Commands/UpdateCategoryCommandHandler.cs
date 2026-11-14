using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Category.Commands;

public record UpdateCategoryCommand(long Id, UpdateCategoryRequest Request) : IRequest<CategoryResponse>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IProductServiceDbContext dbContext, ILogger<UpdateCategoryCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category with Id {Id} not found", request.Id);
            throw new CategoryNotFoundException();
        }

        category.Name = request.Request.Name;
        category.ParentCategoryId = request.Request.ParentCategoryId;
        category.UpdatedAt = DateTime.UtcNow;
        
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}