using Ca.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ca.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbSet<TEntity> Table;

        public Repository(EfDbContext context)
        {
            Table = context.Set<TEntity>();
        }

        public void Delete(TEntity entity) => Table.Remove(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            return (await Table.Where(expression).ToListAsync()).AsReadOnly();
        }

        public Task<TEntity> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}