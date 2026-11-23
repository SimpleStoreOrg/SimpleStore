using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Request;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Data;

namespace ProductService.Application.Features.Product.Commands;

public record UpdateStockCommand(long Id, UpdateStockRequest Request) : IRequest<bool>;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, bool>
{
    private readonly IProductServiceDbContext _dbContext;
    private readonly ILogger<UpdateStockCommandHandler> _logger;

    public UpdateStockCommandHandler(IProductServiceDbContext dbContext, ILogger<UpdateStockCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p=>p.Id == request.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with Id {Id} not found", request.Id);
            throw new ProductNotFoundException();
        }

        product.Stock -= request.Request.Quantity;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}