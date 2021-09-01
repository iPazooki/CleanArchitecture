using Ca.Core.Domain.Blog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ca.Data.Mapping
{
    public class BlogPostMapping : BaseEntityMapping<BlogPost>
    {
        public override void ConfigureDetail(EntityTypeBuilder<BlogPost> builder)
        {
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