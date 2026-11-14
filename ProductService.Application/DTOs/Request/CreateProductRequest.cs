namespace ProductService.Application.DTOs.Request;

public class CreateProductRequest
{
    public string Name { get; set; }
    public long CategoryId { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}