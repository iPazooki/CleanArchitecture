using Ca.Core.Domain.Blog;
using Ca.Services.Caching;
using Ca.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ca.Services.BlogService
{
    public class BlogPostService : IBlogPostService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<BlogPost> _blogRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;

        public BlogPostService(
            ICacheManager cacheManager,
            IRepository<BlogPost> blogRepository,
            IRepository<BlogComment> blogCommentRepository)
        {
            _cacheManager = cacheManager;
            _blogRepository = blogRepository;
            _blogCommentRepository = blogCommentRepository;
        }

        public async Task<IReadOnlyList<BlogPost>> GetAll(Expression<Func<BlogPost, bool>> expression = default)
        {
            return await _blogRepository.GetAllAsync(expression);
        }

        public virtual async Task<BlogPost> GetBlogPostById(Guid id)
        {
            return await _cacheManager.Get(id.ToString(), async _ => await _blogRepository.GetByIdAsync(id));
        }

        public virtual async ValueTask<BlogPost> UpdateBlogPost(BlogPost entity)
        {
            _cacheManager.Delete(entity.Id.ToString());

            return await _blogRepository.UpdateAsync(entity);
        }

        public virtual async ValueTask<BlogPost> AddBlogPost(BlogPost entity)
        {
            return await _blogRepository.AddAsync(entity);
        }

        public virtual async Task DeleteBlogPost(BlogPost entity)
        {
            _cacheManager.Delete(entity.Id.ToString());

            await _blogRepository.DeleteAsync(entity);
        }

        public async Task AddComment(BlogComment comment)
        {
            await _blogCommentRepository.AddAsync(comment);
        }

        public async Task UpdateComment(BlogComment comment)
        {
            await _blogCommentRepository.UpdateAsync(comment);
        }

        public async Task DeleteComment(BlogComment comment)
        {
            await _blogCommentRepository.DeleteAsync(comment);
        }

        public async Task<BlogPost> GetBlogPostByIdWithComments(Guid id)
        {
            return await _blogRepository.GetByIdAsync(id, x => x.Comments);
        }

        public async Task<IReadOnlyList<BlogComment>> GetAllComments(Guid blogPostId)
        {
            return await _blogCommentRepository.GetAllAsync(x => x.BlogPostId == blogPostId);
        }

        public async Task<BlogComment> GetCommentById(Guid id)
        {
            return await _blogCommentRepository.GetByIdAsync(id);
        }
    }
}