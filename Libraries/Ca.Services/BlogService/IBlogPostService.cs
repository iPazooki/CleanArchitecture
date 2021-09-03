using Ca.Core.Domain.Blog;
using Ca.Services.DTOs.Blog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ca.Services.BlogService
{
    public interface IBlogPostService
    {
        Task<BlogPostDto> GetBlogPostById(Guid id);

        Task<BlogPostWithCommentDto> GetBlogPostByIdWithComments(Guid id);

        Task<IReadOnlyList<BlogPostDto>> GetAll(Expression<Func<BlogPost, bool>> expression = default);

        Task<IReadOnlyList<BlogCommentDto>> GetAllComments(Guid blogPostId);

        ValueTask<BlogPostDto> AddBlogPost(BlogPostDto entity);

        ValueTask<BlogPostDto> UpdateBlogPost(BlogPostDto entity);

        Task DeleteBlogPost(Guid entity);

        Task AddComment(BlogCommentDto comment);

        Task UpdateComment(BlogCommentDto comment);

        Task DeleteComment(Guid comment);

        Task<BlogCommentDto> GetCommentById(Guid id);
    }
}