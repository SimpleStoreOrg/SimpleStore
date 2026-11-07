using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Features.Admin.Commands;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSuperAdminAsync(CreateAdminRequest request)
    {
        var result = await _mediator.Send(new CreateMarketAdminCommand(request));
        return Ok(result);
    }
}