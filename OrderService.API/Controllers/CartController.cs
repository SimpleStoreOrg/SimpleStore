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
    public async Task<IActionResult> CreateCartAsync(CreateCartRequest request)
    {
        var result = await _mediator.Send(new CreateCartCommand(request));
        return Ok(result);
    }
    
    [Authorize(Roles = "Customer")]
    [HttpPut("add-or-update-cart-item")]
    public async Task<IActionResult> AddOrUpdateCartItemAsync(AddOrUpdateCartItemRequest request)
    {
        var result = await _mediator.Send(new AddOrUpdateCartItemCommand(request));
        return Ok(result);
    }
    
    [Authorize(Roles = "Customer")]
    [HttpDelete("remove-cart-item")]
    public async Task<IActionResult> RemoveCartItemAsync(long[] cartItemIds)
    {
        var result = await _mediator.Send(new RemoveCartItemCommand(cartItemIds));
        return Ok(result);
    }
}