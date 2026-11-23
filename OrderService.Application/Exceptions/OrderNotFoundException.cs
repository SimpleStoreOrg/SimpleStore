using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class OrderNotFoundException : BaseException
{
    public OrderNotFoundException() : base("Order not found", StatusCodes.Status404NotFound)
    {
    }
}