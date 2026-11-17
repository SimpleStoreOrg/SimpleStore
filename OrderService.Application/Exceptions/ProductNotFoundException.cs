using Microsoft.AspNetCore.Http;

namespace OrderService.Application.Exceptions;

public class ProductNotFoundException : BaseException
{
    public ProductNotFoundException() : base("Product not found", StatusCodes.Status404NotFound)
    {
    }
}