namespace OrderService.Application.DTOs.Response;

public class CartItemsResponse
{
    public long Id { get; set; }
    public long CartId { get; set; }
    public long ProductId { get; set; }
    public long MarketId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalItemPrice { get; set; }
}