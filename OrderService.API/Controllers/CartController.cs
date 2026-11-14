using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Request;
using OrderService.Application.Features.Cart.Commands;
using OrderService.Application.Features.Cart.Queries;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Roles = "Customer")]
    [HttpGet("get-my-cart")]
    public async Task<IActionResult> GetMyCartAsync()
    {
        var result = await _mediator.Send(new GetMyCartQuery());
        return Ok(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> CreateCartAsync()
    {
        var result = await _mediator.Send(new CreateCartCommand());
        return Ok(result);
    }
    
    [Authorize(Roles = "Customer")]
    [HttpPost("Add-Item-To-Cart")]
    public async Task<IActionResult> AddItemToCartAsync(AddItemToCartRequest request)
    {
        var result = await _mediator.Send(new AddItemToCartCommand(request));
        return Ok(result);
    }
}