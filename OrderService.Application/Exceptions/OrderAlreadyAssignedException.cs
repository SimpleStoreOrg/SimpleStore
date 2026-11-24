using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class OrderAlreadyAssignedException : BaseException
{
    public OrderAlreadyAssignedException() : base("Order is already assigned", StatusCodes.Status400BadRequest)
    {
    }
}