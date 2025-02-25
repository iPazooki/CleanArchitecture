using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Domain.Entities.Security;
using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public partial class ApplicationUnitOfWork
{
    public DbSet<User> Users => context.Set<User>();
    
    public DbSet<Book> Books => context.Set<Book>();
    
    public DbSet<Order> Orders => context.Set<Order>();
    
    public DbSet<Role> Roles => context.Set<Role>();
    
    public DbSet<Permission> Permissions => context.Set<Permission>();
}