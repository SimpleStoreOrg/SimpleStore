namespace ProductService.Application.DTOs.Response;

public class ProductResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public long? CategoryId { get; set; }
    public long? MarketId { get; set; }
    public decimal Price { get; set; }
    public DateOnly? ExpiresAt { get; set; }
    public decimal Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}