namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

internal sealed class RefreshTokenConfiguration : BaseEntityAuditableConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.HasIndex(rt=> rt.Token).IsUnique();

        builder.Property(rt => rt.ExpiresDate)
            .IsRequired();

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(rt => rt.IsExpired);
    }
}
