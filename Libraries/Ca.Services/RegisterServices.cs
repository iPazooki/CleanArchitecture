using Ca.Data;
using Ca.Services.BlogService;
using Ca.Services.Caching;
using Ca.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ca.Services
{
    public static class RegisterServices
    {
        public static void RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<CacheOptions>()
                .Bind(configuration.GetSection(CacheOptions.Cache))
                .ValidateDataAnnotations();

            services.AddMemoryCache();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(ICacheManager), typeof(MemoryCacheManager));
            services.AddScoped(typeof(IBlogPostService), typeof(BlogPostService));
        }
    }
}