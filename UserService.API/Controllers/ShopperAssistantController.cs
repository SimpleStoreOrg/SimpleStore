using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Features.ShopperAssistant.Commands;
using UserService.Application.Features.ShopperAssistant.Queries;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopperAssistantController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ShopperAssistantController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetShopperAssistantsAsync(int? pageNumber, int? pageSize)
    {
        var result = await _mediator.Send(new GetShopperAssistantsQuery(pageNumber, pageSize));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetShopperAssistantByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetShopperAssistantByIdQuery(id));
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateShopperAssistantAsync(CreateShopperAssistantRequest request)
    {
        var result = await _mediator.Send(new CreateShopperAssistantCommand(request));
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShopperAssistantAsync(long id, UpdateShopperAssistantRequest request)
    {
        var result = await _mediator.Send(new UpdateShopperAssistantCommand(id, request));
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShopperAssistantAsync(long id)
    {
        var result = await _mediator.Send(new DeleteShopperAssistantCommand(id));
        return Ok(result);
    }
}