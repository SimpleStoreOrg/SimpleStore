using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Features.ShopperAssistant.Commands;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShopperAssistantController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ShopperAssistantController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Roles = "MarketAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateShopperAssistantAsync(CreateShopperAssistantRequest request)
    {
        var result = await _mediator.Send(new CreateShopperAssistantCommand(request));
        return Ok(result);
    }
}