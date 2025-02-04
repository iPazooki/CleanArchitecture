## Clean Architecture 🏗️

Welcome to the Clean Architecture template with Minimal API! This project serves as a starting point for building robust and maintainable applications using Clean Architecture principles.

### Overview
This template targets .NET 8 and is organized into distinct layers, each with a specific responsibility. It's designed to help developers quickly set up a new project that adheres to best practices in software architecture.

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

### Database Migrations 📂
To add migrations and update the database, run the following commands:

```bash
dotnet ef migrations add InitialCreate --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation

dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation
```


### Contributing 🤝
Contributions are welcome! If you have ideas for improvements or encounter any bugs, feel free to open an issue or submit a pull request. Let's collaborate to make this project better. 😊