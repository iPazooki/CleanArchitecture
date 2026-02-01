using CleanArchitecture.Api.Configuration;
using CleanArchitecture.Api.Endpoints;
using CleanArchitecture.Infrastructure.Persistence.Data;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (!IsDesignTime())
{
    builder.AddServiceDefaults();
}

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices(builder);

builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

WebApplication app = builder.Build();

if (!IsDesignTime())
{
    app.MapDefaultEndpoints();
}

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options => options
    .AddPreferredSecuritySchemes("OAuth2")
    .AddAuthorizationCodeFlow("OAuth2", flow =>
    {
        flow.ClientId = "scalar-client";
        flow.ClientSecret = "p1dBWWBebmZQyE96axyyXvHtxpPEd9xN";
        flow.Pkce = Pkce.Sha256;
        flow.SelectedScopes = ["clean_api.all"];
    }));

    using IServiceScope scope = app.Services.CreateScope();

    ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
}

// API Endpoints
app.MapBookEndpoints();
app.MapOrderEndpoints();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync().ConfigureAwait(false);

static bool IsDesignTime()
{
    // This is true when invoked by `dotnet ef` tools
    return AppDomain.CurrentDomain.GetAssemblies()
        .Any(a => a.FullName != null &&
                  a.FullName.StartsWith("Microsoft.EntityFrameworkCore.Design", StringComparison.OrdinalIgnoreCase));
}
