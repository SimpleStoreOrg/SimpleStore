using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.ShopperAssistant.Commands;

public record DeleteShopperAssistantCommand(long Id) : IRequest<bool>;

public class DeleteShopperAssistantCommandHandler : IRequestHandler<DeleteShopperAssistantCommand, bool>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<DeleteShopperAssistantCommandHandler> _logger;

    public DeleteShopperAssistantCommandHandler(IUserServiceDbContext dbContext, ILogger<DeleteShopperAssistantCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteShopperAssistantCommand request, CancellationToken cancellationToken)
    {
        var shAssistant = await _dbContext.ShopperAssistants.FirstOrDefaultAsync(
            sh => sh.Id == request.Id, cancellationToken);
        if (shAssistant == null)
        {
            _logger.LogWarning("ShAs with id {Id} not found", request.Id);
            throw new ShopperAssistantNotFoundException();
        }

        _dbContext.ShopperAssistants.Remove(shAssistant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}