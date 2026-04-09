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

builder.Services.AddLocalization();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.NoCache());
    options.AddPolicy("GetBooks", policy => policy.Expire(TimeSpan.FromSeconds(30)).Tag("books"));
    options.AddPolicy("GetBook", policy => policy.Expire(TimeSpan.FromSeconds(60)).Tag("books"));
});

builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseResponseCompression();

string[] supportedCultures = ["en", "fa", "ar"];

app.UseRequestLocalization(options =>
{
    options.SetDefaultCulture(supportedCultures[0])
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

// Configure development-specific features
app.ConfigureEnvironments();

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
