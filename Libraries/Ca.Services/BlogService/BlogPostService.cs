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
        private readonly IRepository<BlogPost> _repository;

        public BlogPostService(ICacheManager cacheManager, IRepository<BlogPost> repository)
        {
            _cacheManager = cacheManager;
            _repository = repository;
        }

        public async Task<IReadOnlyList<BlogPost>> GetAll(Expression<Func<BlogPost, bool>> expression)
        {
            return await _repository.GetAllAsync(expression);
        }

        public virtual async Task<BlogPost> GetBlogPostById(Guid id)
        {
            return await _cacheManager.Get(id.ToString(), async _ => await _repository.GetByIdAsync(id));
        }

        public virtual async ValueTask<BlogPost> UpdateBlogPost(BlogPost entity)
        {
            _cacheManager.Delete(entity.Id.ToString());

            return await _repository.UpdateAsync(entity);
        }

        public virtual async ValueTask<BlogPost> InsertBlogPost(BlogPost entity)
        {
            return await _repository.AddAsync(entity);
        }

        public virtual async Task DeleteBlogPost(BlogPost entity)
        {
            _cacheManager.Delete(entity.Id.ToString());

            await _repository.DeleteAsync(entity);
        }
    }
}