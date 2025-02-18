using CleanArchitecture.Presentation.Configuration;
using CleanArchitecture.Presentation.Endpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseHealthChecks("/health");

// API Endpoints
app.MapBookEndpoints();
app.MapOrderEndpoints();
app.MapPersonEndpoints();

app.UseExceptionHandler();

app.Run();