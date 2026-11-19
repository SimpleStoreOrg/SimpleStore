using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class InvalidQuantityException : BaseException
{
    public InvalidQuantityException() : base("Quantity cannot be small or equal to 0", StatusCodes.Status400BadRequest)
    {
    }
}