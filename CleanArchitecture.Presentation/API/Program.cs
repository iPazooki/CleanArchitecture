using CleanArchitecture.Api.Configuration;
using CleanArchitecture.Api.Endpoints;
using CleanArchitecture.Infrastructure.Persistence.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices(builder);

builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

// Configure development-specific features and ensure the database is created
await app.ConfigureFeaturesAsync().ConfigureAwait(false);

// API Versioning
Asp.Versioning.Builder.ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

// API Endpoints with versioning
RouteGroupBuilder versionedApi = app.MapGroup("/api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet)
    .WithGroupName("v1");

versionedApi.MapBookEndpoints();

await app.RunAsync().ConfigureAwait(false);
