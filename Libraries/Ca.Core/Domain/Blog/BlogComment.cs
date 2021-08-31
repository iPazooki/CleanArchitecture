using System;

namespace Ca.Core.Domain.Blog
{
    public class BlogComment : BaseEntity
    {
        public Guid BlogPostId { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public BlogPost BlogPost { get; set; }
    }
}