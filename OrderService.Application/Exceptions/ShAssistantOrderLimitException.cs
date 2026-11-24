using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class ShAssistantOrderLimitException : BaseException
{
    public ShAssistantOrderLimitException() : base("To accept the order finish one of your previous orders",
        StatusCodes.Status400BadRequest)

    {
    }
}