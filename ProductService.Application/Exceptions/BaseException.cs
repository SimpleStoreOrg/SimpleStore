namespace ProductService.Application.Exceptions;

public class BaseException : Exception
{
    public int StatusCode { get; set; }

    public BaseException(string message, int statusCodes) : base(message)
    {
        StatusCode = statusCodes;
    }
}