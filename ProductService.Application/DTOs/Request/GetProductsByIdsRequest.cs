namespace ProductService.Application.DTOs.Request;

public class GetProductsByIdsRequest
{
    public long MarketId { get; set; }
    public long[] ProductIds { get; set; }
}