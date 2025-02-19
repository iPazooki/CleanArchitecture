![GitHub](https://img.shields.io/github/license/ipazooki/CleanArchitecture)
![GitHub contributors](https://img.shields.io/github/contributors/ipazooki/CleanArchitecture)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/ipazooki/CleanArchitecture/dotnet.yml)

## Clean Architecture

A lightweight **.NET 8** based **Clean Architecture** template leveraging **Minimal API**. This repository helps you quickly set up a maintainable project structure that separates core business rules from infrastructure and presentation concerns.

## Why This Template?

- **Faster Development**: Pre-configured with essential design patterns like **CQRS** (Commands/Queries), value objects, and layered architecture.
- **Domain-Driven Design**: Organize your core business logic with Entities, Value Objects, Specifications, and Domain Services.
- **Flexible Data Access**: Uses **SQLite** by default but supports **SQL Server** and other databases with minimal changes.
- **Simple API**: Minimal API endpoints keep things lightweight and easy to extend.
- **Extensible**: Easily add new modules, microservices, or features without breaking existing code.
- **Scalable**: Clear separation of concerns keeps your code well-organized as your app grows.
- **Robust Testing**: Includes example unit tests (using xUnit and Moq) to encourage a TDD mindset.
- **Polly Integration**: Resilient API calls with built-in retries, circuit breaker patterns, and fallbacks.


## Project Structure

1. **Domain Layer**  
   - Entities, value objects, and domain services.
2. **Application Layer**  
   - Interfaces, DTOs, commands, queries, and validators.
3. **Infrastructure Layer**  
   - Persistence, database repositories, and external service integrations.
4. **Presentation Layer**  
   - Minimal API endpoints, middleware, and request/response handling.


## Getting Started

The following prerequisites are required to build and run the solution:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (LTS)

1. Clone the repository to your local machine.
    - ```git clone https://github.com/iPazooki/CleanArchitecture.git```
    - ```git clone git@github.com:iPazooki/CleanArchitecture.git```
2. Navigate to the project directory.
3. Restore dependencies using:
    - ```dotnet restore```
4. Build and run the application:
    - ```dotnet build```
    - ```dotnet run --project CleanArchitecture.Presentation```
5. Visit the automatically generated Swagger UI (by default at `https://localhost:7281/swagger/index.html`) for interactive documentation.

## Database Configuration

To switch from the default **SQLite** to **SQL Server**, remove the `UseSQLite` constant from the `DefineConstants` property in `CleanArchitecture.Infrastructure.Persistence.csproj` and adjust the connection string in your `appsettings.json`.

```xml
<DefineConstants>UseSQLite</DefineConstants>
```

## Database Migrations
To add migrations and update the database, run the following commands:

```bash
dotnet ef migrations add InitialCreate --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation

dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation
```

## Contributing
Pull requests are welcome! For major changes, please open an [issue](https://github.com/iPazooki/CleanArchitecture/issues) first to discuss proposed modifications. We appreciate your support and feedback. Don’t forget to **star** the project if you find it helpful.

## License
This project is licensed under the [MIT](LICENSE) license. Feel free to use it as a foundation for your own projects!