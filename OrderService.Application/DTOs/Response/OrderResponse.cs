using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Response;

public class OrderResponse
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public OrderStatus? Status { get; set; }
    public long? ShopperAssistantId { get; set; }
    public List<OrderItemsResponse> OrderItems { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CompletedAt { get; set; }
}