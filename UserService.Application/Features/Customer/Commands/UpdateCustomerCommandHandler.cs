using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.Customer.Commands;

public record UpdateCustomerCommand(long Id, UpdateCustomerRequest Request) : IRequest<CustomerResponse>;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(IUserServiceDbContext dbContext, ILogger<UpdateCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<CustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {Id} not found", request.Id);
            throw new CustomerNotFoundException();
        }

        customer.Name = request.Request.Name;
        customer.Surname = request.Request.Surname;
        customer.Username = request.Request.Username;
        customer.Email = request.Request.Email;
        customer.PhoneNumber = request.Request.PhoneNumber;
        customer.UpdatedAt = DateTime.UtcNow;
        
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Surname = customer.Surname,
            Birthdate = customer.Birthdate,
            Username = customer.Username,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Role = customer.Role,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            DeletedAt = customer.DeletedAt
        };
    }
}