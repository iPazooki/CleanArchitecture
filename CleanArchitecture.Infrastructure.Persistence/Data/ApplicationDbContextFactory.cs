using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;

namespace CleanArchitecture.Infrastructure.Persistence.Data;

/// <summary>
/// Builds an <see cref="ApplicationDbContext"/> for design-time tooling (<c>dotnet ef</c>).
/// </summary>
/// <remarks>
/// Never used at run time, so it wires neither interceptors nor real logging. The connection
/// string only has to be parseable — migrations are scaffolded from the model, not from the
/// database — but it can be pointed at a real one through the environment variable.
/// </remarks>
internal sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private const string ConnectionStringVariable = "ConnectionStrings__postgresDatabaseResource";
    private const string PlaceholderConnectionString = "Host=localhost;Database=dummy;";

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(ConnectionStringVariable) ?? PlaceholderConnectionString;

        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options, NullLogger<ApplicationDbContext>.Instance);
    }
}
