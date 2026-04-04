# Clean Architecture Project Overview

This project is a modern, cloud-native template based on **.NET 10**, **Clean Architecture**, **Minimal APIs**, and **.NET Aspire**. It features a C# backend and a Next.js admin portal acting as a **Backend-for-Frontend (BFF)**, with a focus on maintainability, scalability, and robust dev-ops.

## Core Technologies

### Backend (.NET 10)
- **Framework:** .NET 10 with Minimal APIs.
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, Persistence, Presentation).
- **Patterns:** CQRS using `Mediator` (Source Generator), Repository/Unit of Work.
- **Database:** PostgreSQL with Entity Framework Core (Npgsql).
- **Authentication:** Keycloak (OIDC/JWT) via `Aspire.Keycloak.Authentication`.
- **Validation:** `FluentValidation` for application requests and `DomainValidation.NET` for domain rules.
- **API Documentation:** `Scalar` for interactive OpenAPI specs.
- **Resilience:** `Polly` for HTTP retries and circuit breakers.
- **Logging:** `Serilog` with sinks for Console, File, OpenTelemetry, and Application Insights.

### Frontend (Next.js - BFF Pattern)
- **Framework:** Next.js 16 (React 19).
- **BFF Pattern:** The Next.js server acts as a **Backend-for-Frontend**, handling authentication, session management, and proxying requests to the backend API to keep sensitive tokens out of the browser.
- **Styling:** Tailwind CSS 4.
- **State Management:** `@tanstack/react-query` for server state.
- **Authentication:** `next-auth` (NextAuth.js) integrated with Keycloak on the server side.
- **Forms:** `react-hook-form` with `zod` schema validation.
- **API Client:** Generated via `orval` from the backend OpenAPI spec.

---

## Project Structure

```text
├── CleanArchitecture.Aspire/         # .NET Aspire AppHost and Service Defaults
├── CleanArchitecture.Domain/         # Core business logic: Entities, Value Objects, Domain Events
├── CleanArchitecture.Application/    # Use Cases: Commands, Queries, DTOs, Handlers, Validators
├── CleanArchitecture.Infrastructure/ # External service integrations (Email, etc.)
├── CleanArchitecture.Infrastructure.Persistence/ # EF Core DbContext, Migrations, Repositories
├── CleanArchitecture.Presentation/   # Delivery Mechanisms
│   ├── API/                          # .NET Minimal API (C#)
│   └── admin/                        # Next.js Admin Portal (BFF)
├── CleanArchitecture.DbMigrator/     # Standalone project for applying DB migrations
└── Tests/                            # Unit, Integration, and Architecture tests
```

---

## Key Development Commands

### Running the Application
- **Run the entire stack (Aspire):**
  ```bash
  dotnet run --project CleanArchitecture.Aspire/CleanArchitecture.AppHost/CleanArchitecture.AppHost.csproj
  ```
  *This starts the API, Next.js Admin (BFF), PostgreSQL, and Keycloak.*

- **Run only the API:**
  ```bash
  dotnet run --project CleanArchitecture.Presentation/API/CleanArchitecture.Api.csproj
  ```

- **Run the Admin Portal separately:**
  ```bash
  cd CleanArchitecture.Presentation/admin
  pnpm dev
  ```

### Database & Migrations
- **Add a new migration:**
  ```bash
  dotnet ef migrations add [MigrationName] --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation/API
  ```
- **Update database manually:**
  ```bash
  dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation/API
  ```

### Testing
- **Run all tests:**
  ```bash
  dotnet test
  ```

### Deployment (Azure)
- **Provision and deploy to Azure:**
  ```bash
  azd up
  ```

---

## Development Conventions

1.  **BFF Pattern:** The UI must interact with the API through the Next.js server (BFF). NextAuth handles the OIDC flow with Keycloak on the server, ensuring that access tokens are managed securely.
2.  **Surgical Updates:** When modifying entities or use cases, ensure corresponding tests in the `Tests/` directory are updated or created.
3.  **CQRS:** All API operations must go through a Command or Query handler. Avoid putting business logic directly in Minimal API endpoints.
4.  **Domain Events:** Use `Entity.AddDomainEvent()` to trigger side effects. Events are dispatched via an EF Core interceptor after a successful `SaveChangesAsync`.
5.  **Validation:** Use `FluentValidation` for input validation in the Application layer and `DomainValidation.NET` for invariant checks in the Domain layer.
6.  **Clean Architecture Layers:**
    *   **Domain:** No dependencies on other projects.
    *   **Application:** Depends only on Domain.
    *   **Infrastructure:** Depends on Application and Domain.
    *   **Presentation:** Depends on Application and Infrastructure.
7.  **API Versioning:** The API uses URL-based versioning (e.g., `/api/v1/...`). Define versioned route groups in `Program.cs`.
8.  **Admin Portal:** Use `orval` to regenerate the API client after any backend API changes (`pnpm generate`).

---

## Configuration

- **User Secrets:** Store sensitive local config (like Keycloak client secrets) using `dotnet user-secrets`.
- **Environment Variables:**
    - Backend: `appsettings.json`, `appsettings.Development.json`.
    - Admin: `.env.local` (see `README.md` for required variables).
