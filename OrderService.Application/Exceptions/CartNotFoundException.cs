using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class CartNotFoundException : BaseException
{
    public CartNotFoundException() : base("Cart not found", StatusCodes.Status404NotFound)
    {
    }
}