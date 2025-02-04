namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public partial class ApplicationUnitOfWork
{
    public DbSet<Person> Persons => context.Set<Person>();
    
    public DbSet<Book> Books => context.Set<Book>();
}