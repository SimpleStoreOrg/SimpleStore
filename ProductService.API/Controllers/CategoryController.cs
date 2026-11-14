using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Request;
using ProductService.Application.Features.Category.Commands;
using ProductService.Application.Features.Category.Queries;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesAsync(int? pageNumber, int? pageSize)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(pageNumber, pageSize));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(request));
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategoryAsync(long id, UpdateCategoryRequest request)
    {
        var result = await _mediator.Send(new UpdateCategoryCommand(id, request));
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoryAsync(long id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id));
        return Ok(result);
    }
}