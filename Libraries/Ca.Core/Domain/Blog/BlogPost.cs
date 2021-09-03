using Ca.SharedKernel;
using System;
using System.Collections.Generic;

namespace Ca.Core.Domain.Blog
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public ICollection<BlogComment> Comments { get; set; }
    }
}