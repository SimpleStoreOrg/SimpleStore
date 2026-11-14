using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Data;

public interface IProductServiceDbContext
{
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken token);
}