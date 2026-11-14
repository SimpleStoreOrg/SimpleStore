using OrderService.Application.DTOs.Response;
using Refit;

namespace OrderService.Application.Interfaces.External;

public interface IProductApi
{
    [Get("/api/Product/{id}")]
    public Task<ProductResponse> GetProductById(long id, [Header("Authentication")] string token);
}