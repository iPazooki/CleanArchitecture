using CleanArchitecture.Domain.Entities.Order;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

public sealed class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        
        base.Configure(builder);

        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.HasOne(oi => oi.Book)
            .WithMany()
            .HasForeignKey(oi => oi.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(oi => oi.BookId);

        builder.Property(oi => oi.RowVersion)
            .IsRowVersion();
    }
}