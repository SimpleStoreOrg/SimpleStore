namespace OrderService.Application.DTOs.Request;

public class AddItemToCartRequest
{ 
    public List<CreateCartItemRequest> CreateCartItemRequest { get; set; }
}

public class CreateCartItemRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}