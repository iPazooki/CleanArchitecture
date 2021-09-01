using Ca.Data;
using Ca.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Configuration;

namespace Ca.UnitTests
{
    public abstract class BaseTest
    {
        private static readonly ServiceProvider _serviceProvider;

        static BaseTest()
        {
            var services = new ServiceCollection();

            // Create a new service provider.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Add a database context (ApplicationDbContext) using an in-memory
            // database for testing.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
                options.UseInternalServiceProvider(serviceProvider);
            }, ServiceLifetime.Singleton);

            var myConfiguration = new Dictionary<string, string>
                                    {
                                        {"Cache:SlidingExpirationSec", "10"},
                                        {"Cache:AbsoluteExpirationSec", "10"}
                                    };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            services.AddSingleton<IMemoryCache>(memoryCache);

            services.RegisterAllServices(configuration);

            _serviceProvider = services.BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}