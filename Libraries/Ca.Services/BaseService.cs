using Ca.Services.Caching;
using Ca.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ca.Services
{
    public abstract class BaseService<TEntity> where TEntity : BaseEntity, IAggregateRoot
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<TEntity> _repository;

        public BaseService(ICacheManager cacheManager, IRepository<TEntity> repository)
        {
            _cacheManager = cacheManager;
            _repository = repository;
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await _cacheManager.Get(id.ToString(), async _ => await _repository.GetByIdAsync(id));
        }
    }
}