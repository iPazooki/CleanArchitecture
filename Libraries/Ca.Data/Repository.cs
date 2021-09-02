using Ca.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ca.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, IAggregateRoot
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression = default)
        {
            async Task<List<TEntity>> getList()
            {
                if (expression is null)
                    return await _context.Set<TEntity>().ToListAsync();
                else
                    return await _context.Set<TEntity>().Where(expression).ToListAsync();
            }

            return (await getList()).AsReadOnly();
        }

        public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }
}