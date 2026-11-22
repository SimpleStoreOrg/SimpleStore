namespace OrderService.Application.DTOs.Request;

public class CreateCartRequest
{
    public long MarketId { get; set; }
    public List<UpdateOrAddCartItemRequest> CartItemsRequest { get; set; }
}