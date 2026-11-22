using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderServiceDbContext
{
    public DbSet<CartEntity> Carts { get; set; }
    public DbSet<CartItemsEntity> CartItems { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemsEntity> OrderItems { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken token);
}