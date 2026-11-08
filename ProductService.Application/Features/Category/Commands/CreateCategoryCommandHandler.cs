using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Entities;

namespace ProductService.Application.Features.Category.Commands;

public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryResponse>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IProductServiceDbContext dbContext, ILogger<CreateCategoryCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Request.Name?.Trim().ToLower();

        var categoryExists = await _dbContext.Categories.AnyAsync(
            c => c.Name.Trim().ToLower() == name, cancellationToken);
        if (categoryExists)
        {
            throw new CategoryAlreadyExistsException();
        }

        var category = new CategoryEntity
        {
            Name = request.Request.Name,
            ParentCategoryId = request.Request.ParentCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId,
            CreatedAt = category.CreatedAt
        };
    }
}