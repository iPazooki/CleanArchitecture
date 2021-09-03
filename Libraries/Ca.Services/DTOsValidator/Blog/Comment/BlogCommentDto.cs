using System;

namespace Ca.Services.DTOs.Blog
{
    public class BlogCommentDto : BaseDto
    {
        public Guid BlogPostId { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}