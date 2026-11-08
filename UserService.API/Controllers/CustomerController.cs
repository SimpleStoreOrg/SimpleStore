using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Features.Customer.Commands;
using UserService.Application.Features.Customer.Queries;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomersAsync(int? pageNumber, int? pageSize)
    {
        var result = await _mediator.Send(new GetCustomersQuery(pageNumber, pageSize));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id));
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomerAsync(long id, UpdateCustomerRequest request)
    {
        var result = await _mediator.Send(new UpdateCustomerCommand(id, request));
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomerAsync(long id)
    {
        var result = await _mediator.Send(new DeleteCustomerCommand(id));
        return Ok(result);
    }
}