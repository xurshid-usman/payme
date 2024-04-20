using Microsoft.EntityFrameworkCore;
using Payme.Entities;

namespace Payme.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Order).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OrderTransaction> OrderTransactions { get; set; } = null!;
} 