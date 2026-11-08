using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Features.Admin.Commands;
using UserService.Application.Features.Admin.Queries;

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

    [HttpGet]
    public async Task<IActionResult> GetAdminsAsync(int? pageNumber, int? pageSize)
    {
        var result = await _mediator.Send(new GetAdminsQuery(pageNumber, pageSize));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAdminByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetAdminByIdQuery(id));
        return Ok(result);
    }

    [HttpPost("super-admin")]
    public async Task<IActionResult> CreateSuperAdminAsync(CreateAdminRequest request)
    {
        var result = await _mediator.Send(new CreateSuperAdminCommand(request));
        return Ok(result);
    }
    
    [HttpPost("market-admin")]
    public async Task<IActionResult> CreateMarketAdminAsync(CreateAdminRequest request)
    {
        var result = await _mediator.Send(new CreateMarketAdminCommand(request));
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdminAsync(long id, UpdateAdminRequest request)
    {
        var result = await _mediator.Send(new UpdateAdminCommand(id, request));
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdminAsync(long id)
    {
        var result = await _mediator.Send(new DeleteAdminCommand(id));
        return Ok(result);
    }
}