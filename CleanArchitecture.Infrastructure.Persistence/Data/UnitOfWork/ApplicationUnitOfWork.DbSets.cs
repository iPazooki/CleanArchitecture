namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public sealed partial class ApplicationUnitOfWork
{
    public DbSet<Book> Books => context.Set<Book>();
}
