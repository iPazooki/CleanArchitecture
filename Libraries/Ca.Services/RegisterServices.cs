using Ca.Data;
using Ca.Services.BlogService;
using Ca.Services.Caching;
using Ca.Services.DTOs.Blog;
using Ca.SharedKernel;
using FluentValidation.AspNetCore;
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
            services.AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<BlogPostDtoValidator>());
            services.AddAutoMapper(typeof(RegisterServices));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(ICacheManager), typeof(MemoryCacheManager));
            services.AddScoped(typeof(IBlogPostService), typeof(BlogPostService));
        }
    }
}