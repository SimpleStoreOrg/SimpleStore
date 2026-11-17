using OrderService.Application.DTOs.Response;
using Refit;

namespace OrderService.Application.Interfaces.External;

public interface IProductApi
{
    [Get("/api/Product/{id}")]
    public Task<ProductResponse> GetProductById(long id, [Header("Authentication")] string token);
    
    [Post("/api/Product/Get-By-Ids")]
    public Task<List<ProductResponse>> GetProductByIds(GetProductsByIdsRequest request, [Header("Authentication")] string token);

    class GetProductsByIdsRequest
    {
        public long MarketId { get; set; }
        public long[] ProductIds { get; set; }
    }
}