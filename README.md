![GitHub](https://img.shields.io/github/license/ipazooki/CleanArchitecture)
![GitHub contributors](https://img.shields.io/github/contributors/ipazooki/CleanArchitecture)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/ipazooki/CleanArchitecture/dotnet.yml)

## Clean Architecture

A lightweight **.NET 10** based **Clean Architecture** template leveraging **Minimal API** and **.NET Aspire**. This repository helps you quickly set up a maintainable project structure that separates core business rules from infrastructure and presentation concerns while taking advantage of Aspire's cloud-native tooling.

## Why This Template?

- **Faster Development**: Pre-configured with essential design patterns like **CQRS** (Commands/Queries), value objects, and layered architecture.
- **Domain-Driven Design**: Organize your core business logic with Entities, Value Objects, Specifications, and Domain Services.
- **Cloud-Ready with .NET Aspire**: Uses an Aspire AppHost to orchestrate services, dependencies, health checks, and observability.
- **Postgres by Default**: Uses **PostgreSQL** as the primary relational database (via EF Core) for local development and deployment.
- **Simple API**: Minimal API endpoints keep things lightweight and easy to extend.
- **Extensible**: Easily add new modules, microservices, or features without breaking existing code.
- **Scalable**: Clear separation of concerns keeps your code well-organized as your app grows.
- **Robust Testing**: Includes example unit tests (using xUnit and Moq) and integration tests.
- **Polly Integration**: Resilient API calls with built-in retries, circuit breaker patterns, and fallbacks.


## Project Structure

1. **Domain Layer**  
   - Entities, value objects, and domain services.
2. **Application Layer**  
   - Interfaces, DTOs, commands, queries, and validators.
3. **Infrastructure Layer**  
   - Persistence, database repositories, and external service integrations.
4. **Infrastructure.Persistence Layer**  
   - EF Core `DbContext`, Postgres configuration, and migrations.
5. **Presentation Layer**  
   - Minimal API endpoints, middleware, and request/response handling.
6. **Aspire AppHost**  
   - `.NET Aspire` AppHost project that wires up the API, Postgres, and shared service defaults.


## Getting Started

The following prerequisites are required to build and run the solution:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/dotnet/aspire/get-started/installation) (for local AppHost support)
- Docker (for running Postgres when orchestrated by Aspire)

1. Clone the repository to your local machine.
    - `git clone https://github.com/iPazooki/CleanArchitecture.git`
    - `git clone git@github.com:iPazooki/CleanArchitecture.git`
2. Navigate to the project directory.
3. Restore dependencies using:
    - `dotnet restore`

### Running with .NET Aspire (recommended)

The solution is configured to run using `.NET Aspire` via the AppHost project.

1. From the solution root, run the AppHost project:
   - `dotnet run --project CleanArchitecture.Aspire/CleanArchitecture.AppHost/CleanArchitecture.AppHost.csproj`
2. Aspire will:
   - Start the `CleanArchitecture.Presentation` minimal API.
   - Provision and start a Postgres container/instance (as configured in the AppHost).
   - Apply service defaults (health checks, telemetry, etc.).
3. Once running, navigate to the API Swagger UI (the port may vary depending on Aspire configuration). By default this is similar to:
   - `https://localhost:7281/swagger/index.html`

### Running the API Project Directly (without Aspire)

If you prefer to run only the API (and manage Postgres yourself):

1. Ensure you have a running Postgres instance and that the connection string in `CleanArchitecture.Presentation/appsettings.json` points to it.
2. Build and run the application:
    - `dotnet build`
    - `dotnet run --project CleanArchitecture.Presentation`
3. Visit the Swagger UI (by default):
    - `https://localhost:7281/swagger/index.html`


## Authentication

This project integrates with Keycloak for authentication. Before running the API with secured endpoints you must provide a client secret for the `scalar` client in Keycloak and store it in the project's user secrets.

Important: for Visual Studio users you can set secrets via the __Manage User Secrets__ UI. The instructions below assume the client in Keycloak is named `scalar`.

### Steps to create and configure a new Client Secret

1. Open your Keycloak admin panel (the instance URL configured for your environment) and select `clean-api` realm.
2. Go to the `Clients` section and select the `scalar` client.
3. Open the `Credentials` (or `Client Secrets`) tab for that client.
4. Create/regenerate a new client secret and copy the secret value.

### Store the secret in Visual Studio (recommended)

1. In Visual Studio, right-click the `CleanArchitecture.Api` project and select __Manage User Secrets__.
2. In the JSON editor that opens, add an entry for the client secret. Example structure:
``
{
  "ScalarApi:ClientSecret": "YourClientSecret"
}
``

### Store the secret in the file system (optionally)

1. Use the `dotnet user-secrets` CLI tool to set the secret from the terminal/command prompt:

```bash
dotnet user-secrets set "ScalarApi:ClientSecret" "<your-client-secret-here>" --project CleanArchitecture.Api
```

## Database Configuration

- **Database**: PostgreSQL
- **EF Core Provider**: `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Configuration**: Connection strings are defined in `CleanArchitecture.Presentation/appsettings.json` and can be overridden in environment-specific files like `CleanArchitecture.Presentation/appsettings.Development.json` or via Aspire resource configuration in `CleanArchitecture.Aspire/CleanArchitecture.AppHost/appsettings*.json`.


## Database Migrations

Migrations are created and applied using EF Core targeting the Postgres-backed persistence project.

To add migrations and update the database, run the following commands from the solution root:

```bash
dotnet ef migrations add InitialCreate --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation

dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation
```

Ensure the configured Postgres instance is reachable when running these commands. When running via Aspire, you may prefer to manage schema changes using migrations applied during startup.


## Contributing
Pull requests are welcome! For major changes, please open an [issue](https://github.com/iPazooki/CleanArchitecture/issues) first to discuss proposed modifications. We appreciate your support and feedback. Don’t forget to **star** the project if you find it helpful.

## License
This project is licensed under the [MIT](LICENSE) license. Feel free to use it as a foundation for your own projects!
