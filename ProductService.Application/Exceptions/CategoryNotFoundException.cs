using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Exceptions;

public class CategoryNotFoundException : BaseException
{
    public CategoryNotFoundException() : base("Category not found", StatusCodes.Status404NotFound)
    {
    }
}