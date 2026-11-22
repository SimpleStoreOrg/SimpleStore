using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class EmptyCartException : BaseException
{
    public EmptyCartException() : base("Cart is empty", StatusCodes.Status400BadRequest)
    {
    }
}