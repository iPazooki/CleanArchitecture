using Serilog;
using CleanArchitecture.Presentation.Endpoints;
using CleanArchitecture.Presentation.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)));

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.MapHealthChecks("/health");

// API Endpoints
app.MapBookEndpoints();
app.MapOrderEndpoints();
app.MapUserEndpoints();
app.MapMemberEndpoints();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync().ConfigureAwait(false);
