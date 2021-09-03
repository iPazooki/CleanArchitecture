using System;

namespace Ca.Services.DTOs.Blog
{
    public class BlogPostDto : BaseDto
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}