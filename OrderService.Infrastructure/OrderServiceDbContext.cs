using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure;

public class OrderServiceDbContext : DbContext, IOrderServiceDbContext
{
    public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CartEntity> Carts { get; set; }
    public DbSet<CartItemsEntity> CartItems { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemsEntity> OrderItems { get; set; }
}