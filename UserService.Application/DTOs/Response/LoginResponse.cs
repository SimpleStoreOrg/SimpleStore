namespace UserService.Application.DTOs.Response;

public class LoginResponse
{
    public string? AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}