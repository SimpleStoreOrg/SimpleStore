using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class InsufficientStockException : BaseException
{
    public InsufficientStockException(long id) : base($"Insufficient stock for product Id {id}",
        StatusCodes.Status400BadRequest)

    {
    }
}