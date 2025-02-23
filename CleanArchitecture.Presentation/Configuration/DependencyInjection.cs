using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Presentation.Configuration;

internal static class DependencyInjection
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(s =>
        {
            s.EnableAnnotations();
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "Clean Architecture", Version = "v1" });

            // Add JWT Authentication to Swagger
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });
            
            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, []
                }
            });
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddProblemDetails();

        services.AddHealthChecks();
    }
}