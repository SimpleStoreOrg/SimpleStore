using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Customer.Commands;

public record DeleteCustomerCommand(long Id) : IRequest<bool>;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(IUserServiceDbContext dbContext, ILogger<DeleteCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {Id} not found", request.Id);
            throw new CustomerNotFoundException();
        }

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}