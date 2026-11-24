using OrderService.Application.DTOs.Response;
using Refit;

namespace OrderService.Application.Interfaces.External;

public interface IProductApi
{
    [Get("/api/Product/{id}")]
    public Task<ProductResponse> GetProductById(long id, [Header("Authentication")] string token);

    [Put("/api/Product/{id}/update-stock")]
    public Task UpdateStock(long id, UpdateStockRequest request, [Header("Authorization")] string token);
    
    [Post("/api/Product/get-by-ids")]
    public Task<List<ProductResponse>> GetProductByIds(GetProductsByIdsRequest request, [Header("Authentication")] string token);

    class GetProductsByIdsRequest
    {
        public long MarketId { get; set; }
        public long[] ProductIds { get; set; }
    }

    class UpdateStockRequest
    {
        public int Quantity { get; set; }
    }
}