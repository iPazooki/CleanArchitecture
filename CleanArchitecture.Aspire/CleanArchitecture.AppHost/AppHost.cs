var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArchitecture_Presentation>("cleanarchitecture-presentation");

await builder.Build().RunAsync().ConfigureAwait(false);
