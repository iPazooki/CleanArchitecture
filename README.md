![GitHub](https://img.shields.io/github/license/ipazooki/CleanArchitecture)
![GitHub contributors](https://img.shields.io/github/contributors/ipazooki/CleanArchitecture)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/ipazooki/CleanArchitecture/dotnet.yml)

## Clean Architecture 🏗️

Welcome to the Clean Architecture template with Minimal API! This project serves as a starting point for building robust and maintainable applications using Clean Architecture principles.

### Overview
This template targets .NET 8 and is organized into distinct layers, each with a specific responsibility. It's designed to help developers quickly set up a new project that adheres to best practices in software architecture.

**If you find this project useful, please give it a star. Thanks! ⭐**

### Getting Started

The following prerequisites are required to build and run the solution:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (latest version)

### Layers Explained

#### Domain Layer 🌟
- **Entities**: Core business objects representing data and behavior.
- **Value Objects**: Immutable types that represent a concept through multiple attributes.
- **Specifications**: Encapsulate business rules that are combinable.
- **Domain Services**: Handle complex business logic that doesn't fit within entities.

#### Application Layer 🚀
- **Interfaces**: Abstractions for services used by the application layer.
- **DTOs**: Data Transfer Objects for communication between layers.
- **Commands and Queries**: Implement the CQRS pattern for read and write operations.
- **Validators**: Ensure data meets required rules and constraints.

#### Infrastructure Layer 🛠️
- **Persistence**: Implementation of data access using Entity Framework Core.
- **Repositories**: Classes responsible for data retrieval and storage.
- **Services**: External services like file storage, email senders, etc.

#### Presentation Layer 🎨
- **Minimal API**: Simplified approach to building HTTP APIs with ASP.NET Core.
- **Endpoints**: Define HTTP endpoints for client interactions.
- **Middleware**: Components that handle requests and responses.

### Getting Started 📖
1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Restore dependencies using:
    
```bash
dotnet restore
```
 4.	Run the application using:
```bash
   dotnet build
```

### Database
The template is set to use SQLite by default. If you would like to use SQL Server instead, please open the `CleanArchitecture.Infrastructure.Persistence.csproj` file and remove the `UseSQLite` constant from the `DefineConstants` property. Then, update the connection string in the `appsettings.json` file.

```xml
<DefineConstants>UseSQLite</DefineConstants>
```

### Database Migrations 📂
To add migrations and update the database, run the following commands:

```bash
dotnet ef migrations add InitialCreate --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation

dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation
```

## Support

If you are having problems, please let me know by [raising a new issue](https://github.com/iPazooki/CleanArchitecture/issues).

## License

This project is licensed with the [MIT license](LICENSE).

### Contributing 🤝
Contributions are welcome! If you have ideas for improvements or encounter any bugs, feel free to open an issue or submit a pull request. Let's collaborate to make this project better. 😊