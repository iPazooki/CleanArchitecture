using Serilog;
using CleanArchitecture.Presentation.Endpoints;
using CleanArchitecture.Presentation.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOpenApi();

// API Endpoints
app.MapBookEndpoints();
app.MapOrderEndpoints();
app.MapUserEndpoints();
app.MapMemberEndpoints();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync().ConfigureAwait(false);
