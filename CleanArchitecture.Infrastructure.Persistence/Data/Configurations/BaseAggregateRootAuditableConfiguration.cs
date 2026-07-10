using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

internal abstract class BaseAggregateRootAuditableConfiguration<T> : IEntityTypeConfiguration<T> where T : AggregateRootAuditable
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.UpdatedDate)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Ignore(o => o.DomainEvents);
    }
}
