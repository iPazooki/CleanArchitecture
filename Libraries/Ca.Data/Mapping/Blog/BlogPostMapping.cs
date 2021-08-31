using Ca.Core.Domain.Blog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ca.Data.Mapping
{
    public class BlogPostMapping : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Body)
                .IsRequired()
                .HasColumnType("nvarchar(MAX)");

            builder.HasMany(x => x.Comments)
                .WithOne(b => b.BlogPost)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}