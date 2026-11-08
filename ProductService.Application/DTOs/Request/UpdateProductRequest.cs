namespace ProductService.Application.DTOs.Request;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public long? CategoryId { get; set; }
    public decimal Price { get; set; }
    public decimal Stock { get; set; }
}