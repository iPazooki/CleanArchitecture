using Ca.Core.Domain.Blog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ca.Data.Mapping.Blog
{
    public class BlogCommentMapping : BaseEntityMapping<BlogComment>
    {
        public override void ConfigureDetail(EntityTypeBuilder<BlogComment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CommentText)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(x => x.BlogPost)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .IsRequired();
        }
    }
}