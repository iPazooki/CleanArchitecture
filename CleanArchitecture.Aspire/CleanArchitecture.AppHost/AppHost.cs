using CleanArchitecture.AppHost;
using CleanArchitecture.AppHost.Environments;
using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment(AppHostConstants.TestingEnvironment))
{
    builder.ConfigureTestingEnvironment();
}
else if (builder.Environment.IsDevelopment() && builder.ExecutionContext.IsRunMode)
{
    builder.ConfigureDevelopmentEnvironment();
}
else
{
    builder.ConfigureProductionEnvironment();
}

await builder.Build().RunAsync().ConfigureAwait(false);
