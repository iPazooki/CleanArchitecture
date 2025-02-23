using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;
using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public partial class ApplicationUnitOfWork
{
    public DbSet<User> Users => context.Set<User>();
    
    public DbSet<Book> Books => context.Set<Book>();
    
    public DbSet<Order> Orders => context.Set<Order>();
}