using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class OrderEntity : BaseEntity
{
    public long CustomerId { get; set; }
    public long? ShopperAssistantId { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemsEntity> OrderItems { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CompletedAt { get; set; }
}