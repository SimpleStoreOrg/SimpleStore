using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Category.Commands;

public record DeleteCategoryCommand(long Id) : IRequest<bool>;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IProductServiceDbContext dbContext, ILogger<DeleteCategoryCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category with Id {Id} not found", request.Id);
            throw new CategoryNotFoundException();
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}