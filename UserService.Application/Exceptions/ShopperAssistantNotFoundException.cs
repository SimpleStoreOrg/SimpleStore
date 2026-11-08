using Microsoft.AspNetCore.Http;

namespace UserService.Application.Exceptions;

public class ShopperAssistantNotFoundException : BaseException
{
    public ShopperAssistantNotFoundException() : base("ShopperAssistant not found", StatusCodes.Status404NotFound)
    {
    }
}