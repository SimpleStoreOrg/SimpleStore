using Microsoft.AspNetCore.Http;

namespace UserService.Application.Exceptions;

public class WrongPasswordException : BaseException
{
    public WrongPasswordException() : base("Wrong password", StatusCodes.Status400BadRequest)
    {
    }
}