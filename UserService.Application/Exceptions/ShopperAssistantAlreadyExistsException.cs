using Microsoft.AspNetCore.Http;

namespace UserService.Application.Exceptions;

public class ShopperAssistantAlreadyExistsException : BaseException
{
    public ShopperAssistantAlreadyExistsException() : base("ShopperAssistant already exists", StatusCodes.Status400BadRequest)

    {
    }
}