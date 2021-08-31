using Ca.Core.Domain.Blog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ca.Data.Mapping.Blog
{
    public class BlogCommentMapping : IEntityTypeConfiguration<BlogComment>
    {
        public void Configure(EntityTypeBuilder<BlogComment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CommentText)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(x => x.BlogPost)
                .WithMany(b => b.Comments);
        }
    }
}