using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Features.Authentication.Commands;
using LoginRequest = UserService.Application.DTOs.Request.LoginRequest;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request));
        return Ok(result);
    }
    
    [HttpPost("customer-registration")]
    public async Task<IActionResult> CustomerRegistrationAsync(RegisterCustomerRequest request)
    {
        var result = await _mediator.Send(new RegisterCustomerCommand(request));
        return Ok(result);
    }
}