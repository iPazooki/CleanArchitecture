using CleanArchitecture.Domain.Entities.Order;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

public sealed class OrderConfiguration : BaseAggregateRootConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);
        
        builder.Property(o => o.OrderStatus)
            .HasConversion<string>()
            .IsRequired();
        
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(o => o.PurchasedDateTime)
            .IsRequired();
        
        builder.Property(o => o.RowVersion)
            .IsRowVersion();
        
        builder.HasIndex(x=>x.CustomerId);
    }
}