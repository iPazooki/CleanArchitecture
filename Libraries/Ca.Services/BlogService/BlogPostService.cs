using Ca.Core.Domain.Blog;
using Ca.Services.Caching;
using Ca.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ca.Services.BlogService
{
    public class BlogPostService : BaseService<BlogPost>, IBlogPostService
    {
        private readonly IRepository<BlogPost> _repository;

        public BlogPostService(ICacheManager cacheManager, IRepository<BlogPost> repository)
            : base(cacheManager, repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<BlogPost>> GetAll(Expression<Func<BlogPost, bool>> expression)
        {
            return await _repository.GetAllAsync(expression);
        }
    }
}