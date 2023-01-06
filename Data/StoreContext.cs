using Microsoft.EntityFrameworkCore;
using Savana.Order.API.Entities;

namespace Savana.Order.API.Data;

public class StoreContext : DbContext {
    public DbSet<ProductEntity> Products { get; set; } = null!;
    public DbSet<AddressEntity> Addresses { get; set; } = null!;
    public DbSet<VoucherEntity> Vouchers { get; set; } = null!;
    public DbSet<OrderEntity> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<DeliveryEntity> DeliveryMethods { get; set; } = null!;

    public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<ProductEntity>().ToTable("products");
        builder.Entity<AddressEntity>().ToTable("addresses");
        builder.Entity<VoucherEntity>().ToTable("vouchers");
        builder.Entity<OrderEntity>().ToTable("orders").Ignore(o => o.Active);
        builder.Entity<OrderItem>().ToTable("order_items");
        builder.Entity<DeliveryEntity>().ToTable("delivery_methods");
    }
}