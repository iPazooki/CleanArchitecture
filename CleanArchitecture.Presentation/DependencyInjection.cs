using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(s=>
        {
            s.EnableAnnotations();
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "Clean Architecture", Version = "v1" });
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddProblemDetails();

        return services;
    }
}