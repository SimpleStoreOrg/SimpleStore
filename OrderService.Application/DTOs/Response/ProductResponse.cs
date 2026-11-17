namespace OrderService.Application.DTOs.Response;

public class ProductResponse
{
    public long Id { get; set; }
    public long MarketId { get; set; }
    public string Name { get; set; }
    public long CategoryId { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}