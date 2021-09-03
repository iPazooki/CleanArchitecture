using System.Collections.Generic;

namespace Ca.Services.DTOs.Blog
{
    public class BlogPostWithCommentDto
    {
        public BlogPostWithCommentDto()
        {
            Comments = new List<BlogCommentDto>();
        }

        public string Title { get; set; }

        public string Body { get; set; }

        public List<BlogCommentDto> Comments { get; set; }
    }
}