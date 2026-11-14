using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Exceptions;

public class ProductNotFoundException : BaseException
{
    public ProductNotFoundException() : base("Product not found", StatusCodes.Status404NotFound)
    {
    }
}