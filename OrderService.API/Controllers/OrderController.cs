using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Order.Commands;
using OrderService.Application.Features.Order.Queries;
using OrderService.Domain.Enums;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Customer")]
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? createdAtFrom,
        [FromQuery] DateTime? createdAtTo)
    {
        var result = await _mediator.Send(new GetOrdersQuery(
            pageNumber,
            pageSize,
            status,
            createdAtFrom,
            createdAtTo));
        
        return Ok(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync()
    {
        var result = await _mediator.Send(new CreateOrderCommand());
        return Ok(result);
    }
}