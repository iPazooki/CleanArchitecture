using Ca.Core;
using Ca.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ca.Data.Mapping
{
    public abstract class BaseEntityMapping<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);

            ConfigureDetail(builder);
        }

        public abstract void ConfigureDetail(EntityTypeBuilder<TEntity> builder);
    }
}