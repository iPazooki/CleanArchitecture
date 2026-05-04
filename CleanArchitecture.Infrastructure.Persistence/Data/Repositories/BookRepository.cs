namespace CleanArchitecture.Infrastructure.Persistence.Data.Repositories;

internal sealed class BookRepository(ApplicationDbContext context) : IBookRepository
{
    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Set<Book>().FindAsync([id], cancellationToken).AsTask();

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => context.Set<Book>().AsNoTracking().CountAsync(cancellationToken);

    public async Task<IReadOnlyList<Book>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
        => await context.Set<Book>()
            .AsNoTracking()
            .OrderBy(b => b.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
        => await context.Set<Book>().AddAsync(book, cancellationToken).ConfigureAwait(false);

    public void Remove(Book book)
        => context.Set<Book>().Remove(book);
}
