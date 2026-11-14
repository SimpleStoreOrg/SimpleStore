using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class NotAuthorizedException : BaseException
{
    public NotAuthorizedException() : base("User is not authorized", StatusCodes.Status400BadRequest)
    {
    }
}