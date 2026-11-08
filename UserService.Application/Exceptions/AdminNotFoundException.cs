using Microsoft.AspNetCore.Http;

namespace UserService.Application.Exceptions;

public class AdminNotFoundException : BaseException
{
    public AdminNotFoundException() : base("Admin not found", StatusCodes.Status404NotFound)
    {
    }
}