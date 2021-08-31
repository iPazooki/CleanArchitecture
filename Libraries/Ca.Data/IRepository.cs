using Ca.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ca.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(Guid id);

        Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression);

        Task InsertAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        void Delete(TEntity entity);
    }
}