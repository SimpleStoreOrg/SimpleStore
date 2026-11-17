using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class ItemNotFoundException : BaseException
{
    public ItemNotFoundException(long id) : base($"Item with Id {id} not found", StatusCodes.Status404NotFound)
    {
    }
}