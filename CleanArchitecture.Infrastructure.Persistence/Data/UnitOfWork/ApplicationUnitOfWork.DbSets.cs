using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Domain.Entities.Person;

namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public partial class ApplicationUnitOfWork
{
    public DbSet<Person> Persons => context.Set<Person>();
    
    public DbSet<Book> Books => context.Set<Book>();
    
    public DbSet<Order> Orders => context.Set<Order>();
}