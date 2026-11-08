using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Admin.Commands;

public record DeleteAdminCommand(long Id) : IRequest<bool>;

public class DeleteAdminCommandHandler  : IRequestHandler<DeleteAdminCommand, bool>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<DeleteAdminCommandHandler> _logger;

    public DeleteAdminCommandHandler(IUserServiceDbContext dbContext, ILogger<DeleteAdminCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (admin == null)
        {
            _logger.LogWarning("Admin with Id {Id} not found", request.Id);
            throw new AdminNotFoundException();
        }

        _dbContext.Admins.Remove(admin);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}