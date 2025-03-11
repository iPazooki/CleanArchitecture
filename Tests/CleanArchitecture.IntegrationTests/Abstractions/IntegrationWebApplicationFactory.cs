using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Infrastructure.Persistence.Data;

namespace CleanArchitecture.IntegrationTests.Abstractions;

internal sealed class IntegrationWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using IServiceScope scope = serviceProvider.CreateScope();

            IServiceProvider scopedServices = scope.ServiceProvider;

            ApplicationDbContext db = scopedServices.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
        });
    }
}
