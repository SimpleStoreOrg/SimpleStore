using OrderService.Domain.Entities;

namespace OrderService.Application.DTOs.Response;

public class CartResponse
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public List<CartItemsResponse> CartItems { get; set; }
    public decimal TotalPrice { get; set; }
}