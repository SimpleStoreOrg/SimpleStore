namespace OrderService.Domain.Entities;

public class CartEntity : BaseEntity
{
    public long CustomerId { get; set; }
    public List<CartItemsEntity> CartItems { get; set; }
    public decimal TotalPrice { get; set; }
}