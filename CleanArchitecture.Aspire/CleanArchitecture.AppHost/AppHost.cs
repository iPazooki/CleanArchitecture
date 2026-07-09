using CleanArchitecture.AppHost.Environments;
using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.ConfigureDevelopmentEnvironment();
}
else
{
    builder.ConfigureProductionEnvironment();
}

await builder.Build().RunAsync().ConfigureAwait(false);
