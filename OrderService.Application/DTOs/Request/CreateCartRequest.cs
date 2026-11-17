namespace OrderService.Application.DTOs.Request;

public class CreateCartRequest
{
    public long MarketId { get; set; }
    public List<CreateCartItemRequest> CartItemsRequest { get; set; }
}