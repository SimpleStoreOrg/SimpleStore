using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Application.Features.Authentication.Commands;

public record RegisterCustomerCommand(RegisterCustomerRequest Request) : IRequest<CustomerResponse>;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, CustomerResponse>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;

    public RegisterCustomerCommandHandler(IUserServiceDbContext dbContext, ILogger<RegisterCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<CustomerResponse> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var username = request.Request.Username?.Trim().ToLower();
        var email = request.Request.Email?.Trim().ToLower();

        var customerExists = await _dbContext.Customers.AnyAsync(
            c => c.Username.Trim().ToLower() == username && c.Email.Trim().ToLower() == email, cancellationToken);

        if (customerExists)
        {
            throw new CustomerAlreadyExistsException();
        }

        var customer = new CustomerEntity
        {
            Username = request.Request.Username,
            Email = request.Request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            PhoneNumber = request.Request.PhoneNumber,
            Role = RoleStatus.Customer,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Surname = customer.Surname,
            Username = customer.Username,
            Birthdate = customer.Birthdate,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Role = customer.Role,
            CreatedAt = customer.CreatedAt
        };
    }
}