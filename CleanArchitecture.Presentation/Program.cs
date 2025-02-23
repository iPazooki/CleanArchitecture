using CleanArchitecture.Presentation.Configuration;
using CleanArchitecture.Presentation.Endpoints;
using CleanArchitecture.Presentation.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseHealthChecks("/health");

// API Endpoints
app.MapBookEndpoints();
app.MapOrderEndpoints();
app.MapUserEndpoints();
app.MapMemberEndpoints();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.Run();