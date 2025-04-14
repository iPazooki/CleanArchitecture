using CleanArchitecture.Domain.Entities.Security;

namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public sealed partial class ApplicationUnitOfWork
{
    public DbSet<User> Users => context.Set<User>();

    public DbSet<Book> Books => context.Set<Book>();

    public DbSet<Order> Orders => context.Set<Order>();

    public DbSet<Role> Roles => context.Set<Role>();

    public DbSet<Permission> Permissions => context.Set<Permission>();

    public DbSet<RefreshToken> RefreshTokens => context.Set<RefreshToken>();
}
