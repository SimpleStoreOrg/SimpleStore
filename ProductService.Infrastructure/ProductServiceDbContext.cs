using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure;

public class ProductServiceDbContext : DbContext, IProductServiceDbContext
{
    public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
}