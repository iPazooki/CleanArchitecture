using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ca.SharedKernel
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TEntity> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] include);

        Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression = default);

        ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}