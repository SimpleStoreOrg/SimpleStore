using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class CartAlreadyExistsException : BaseException
{
    public CartAlreadyExistsException() : base("Cart already exists for this customer", StatusCodes.Status400BadRequest)
    {
    }
}