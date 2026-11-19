namespace OrderService.Application.DTOs.Request;

public class AddOrUpdateCartItemRequest
{ 
    public List<UpdateOrAddCartItemRequest> UpdateOrAddCartItemRequest { get; set; }
}

public class UpdateOrAddCartItemRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}