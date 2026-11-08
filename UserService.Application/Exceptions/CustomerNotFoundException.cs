using Microsoft.AspNetCore.Http;

namespace UserService.Application.Exceptions;

public class CustomerNotFoundException : BaseException
{
    public CustomerNotFoundException() : base("Customer not found", StatusCodes.Status404NotFound)
    {
    }
}