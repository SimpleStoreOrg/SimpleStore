using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Request;
using ProductService.Application.Features.Product.Commands;
using ProductService.Application.Features.Product.Queries;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllProductsAsync(int? pageNumber, int? pageSize)
    {
        var result = await _mediator.Send(new GetProductsQuery(pageNumber, pageSize));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(CreateProductRequest request)
    {
        var result = await _mediator.Send(new CreateProductCommand(request));
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductAsync(long id, UpdateProductRequest request)
    {
        var result = await _mediator.Send(new UpdateProductCommand(id, request));
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductAsync(long id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));
        return Ok(result);
    }
}