namespace OrderService.Application.DTOs.Response;

public class OrderItemsResponse
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long MarketId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalItemPrice { get; set; }
}