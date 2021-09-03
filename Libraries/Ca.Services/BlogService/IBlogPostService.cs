using Ca.Core.Domain.Blog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ca.Services.BlogService
{
    public interface IBlogPostService
    {
        Task<BlogPost> GetBlogPostById(Guid id);

        Task<BlogPost> GetBlogPostByIdWithComments(Guid id);

        Task<IReadOnlyList<BlogPost>> GetAll(Expression<Func<BlogPost, bool>> expression = default);

        Task<IReadOnlyList<BlogComment>> GetAllComments(Guid blogPostId);

        ValueTask<BlogPost> AddBlogPost(BlogPost entity);

        ValueTask<BlogPost> UpdateBlogPost(BlogPost entity);

        Task DeleteBlogPost(BlogPost entity);

        Task AddComment(BlogComment comment);

        Task UpdateComment(BlogComment comment);

        Task DeleteComment(BlogComment comment);

        Task<BlogComment> GetCommentById(Guid id);
    }
}