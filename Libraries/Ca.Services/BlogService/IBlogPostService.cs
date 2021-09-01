using Ca.Core.Domain.Blog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ca.Services.BlogService
{
    public interface IBlogPostService
    {
        Task<IReadOnlyList<BlogPost>> GetAll(Expression<Func<BlogPost, bool>> expression);

        Task DeleteBlogPost(BlogPost entity);

        ValueTask<BlogPost> InsertBlogPost(BlogPost entity);

        ValueTask<BlogPost> UpdateBlogPost(BlogPost entity);

        Task<BlogPost> GetBlogPostById(Guid id);
    }
}