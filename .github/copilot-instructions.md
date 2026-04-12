---
apply: always
---

# Copilot Instructions

## Build, test, and lint commands

### Backend and solution
- Install the Aspire workload before using the AppHost: `dotnet workload install aspire`
- Restore packages: `dotnet restore`
- Build the solution: `dotnet build --configuration Release`
- Use `dotnet build` as the .NET lint gate. `Directory.Build.props` enables `AnalysisMode=All`, `EnforceCodeStyleInBuild`, `TreatWarningsAsErrors`, and SonarAnalyzer across all `.csproj` files.
- Run all tests: `dotnet test --configuration Release`
- Run one test project: `dotnet test Tests\Domain.UnitTests\Domain.UnitTests.csproj --configuration Release`
- Run a single test class or method: `dotnet test Tests\Domain.UnitTests\Domain.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~Domain.UnitTests.BookTests"`
- Run only the integration suite: `dotnet test Tests\CleanArchitecture.IntegrationTests\CleanArchitecture.IntegrationTests.csproj --configuration Release --filter "FullyQualifiedName~CleanArchitecture.IntegrationTests.BookIntegrationTests"`
- Run the full local stack through Aspire: `dotnet run --project CleanArchitecture.Aspire\CleanArchitecture.AppHost\CleanArchitecture.AppHost.csproj`
- Run only the API: `dotnet run --project CleanArchitecture.Presentation\API\CleanArchitecture.Api.csproj`
- Add a migration: `dotnet ef migrations add <Name> --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation\API`
- Apply migrations manually: `dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation\API`

### Admin app
- Work from `CleanArchitecture.Presentation\admin`
- Install dependencies: `pnpm install`
- Start the admin app: `pnpm dev`
- Build the admin app: `pnpm build`
- Lint the admin app: `pnpm lint` (eslint with `--max-warnings=0`)
- Regenerate API hooks and Zod schemas: `pnpm generate` (runs orval against `http://localhost:5049/openapi/v1.json`)

## High-level architecture

- `CleanArchitecture.sln` is split into Core (`CleanArchitecture.Domain`, `CleanArchitecture.Application`), Infrastructure (`CleanArchitecture.Infrastructure`, `CleanArchitecture.Infrastructure.Persistence`), Presentation (`CleanArchitecture.Presentation\API`, `CleanArchitecture.Presentation\admin`), Aspire (`CleanArchitecture.AppHost`, `CleanArchitecture.ServiceDefaults`), and four test projects.
- `CleanArchitecture.Presentation\API\Program.cs` is the backend composition root. It adds Aspire service defaults first, then wires the Application, Infrastructure, Infrastructure.Persistence, and Presentation layers through their `Add*Services()` extension methods.
- The API is a versioned Minimal API. Endpoints are grouped under `/api/v{version:apiVersion}` and then mapped by feature extension classes such as `BookEndpoints`.
- `CleanArchitecture.Infrastructure.Persistence` owns EF Core concerns: `ApplicationDbContext`, entity configurations, migrations, SaveChanges interceptors (`AuditableEntityInterceptor` for timestamps, `DispatchDomainEventsInterceptor` for publishing domain events after save), and the unit-of-work implementation.
- `CleanArchitecture.Aspire\CleanArchitecture.AppHost\AppHost.cs` is the real local-dev entry point. It has three environment modes: **Development** starts Postgres + PgAdmin, Keycloak with realm import, the API, and the Next.js admin app. **Testing** starts only the API and an ephemeral Postgres database (no Keycloak; `TestAuthHandler` grants all roles). **Production** targets Azure Container Apps, PostgreSQL Flexible Server, Key Vault, and Application Insights.
- `CleanArchitecture.Aspire\CleanArchitecture.ServiceDefaults\Extensions.cs` adds the shared cross-cutting runtime behavior: Serilog, OpenTelemetry, service discovery, resilience handlers, and health endpoints.
- `CleanArchitecture.Presentation\admin` is a Next.js 16 App Router admin app acting as a **Backend for Frontend (BFF)**. Only the frontend and Keycloak admin UI are publicly exposed; the .NET API remains private. The admin uses NextAuth with Keycloak, TanStack Query for server state, and orval-generated API clients/Zod schemas under `src\lib\api` and `src\lib\api\zod`.

## Key conventions

### Architecture and layer boundaries
- Preserve the layer boundaries enforced by `Tests\Architecture.UnitTests\ArchitectureTests.cs`. Domain → no project references. Application → Domain only. Infrastructure → Application (+ Domain). Presentation → Application + Infrastructure.
- Backend use cases follow CQRS with the **Mediator** source-generator package (not MediatR) and FluentValidation auto-registration from `CleanArchitecture.Application\DependencyInjection.cs`. New features should follow the existing `Entities\<Aggregate>\Commands\<Verb>\` and `Entities\<Aggregate>\Queries\<Verb>\` layout, each containing a command/query record, handler, and validator.

### Domain model
- Domain entities inherit from `Entity` → `AggregateRoot` (or their auditable variants `EntityAuditable` → `AggregateRootAuditable` which add `CreatedDate`/`UpdatedDate`). `Entity` provides identity-based equality and a `DomainEvents` collection.
- Aggregates expose static factory methods (e.g., `Book.Create(...)`) that perform domain validation and return `Result<T>`. Use `DomainError` records with `ErrorType` (Validation, NotFound, Conflict, Failure) for domain-level errors, defined in a per-aggregate `*Errors` class.
- Domain events are raised via `AddDomainEvent()` on the entity and dispatched automatically by `DispatchDomainEventsInterceptor` after `SaveChangesAsync`. Handlers live in `Application\Entities\<Aggregate>\EventHandlers\`.

### Application layer patterns
- Command/query handlers extend `BaseRequestHandler<TRequest, TResponse>`, which runs FluentValidation before delegating to the `HandleRequest` override. This means validation is automatic — add a `*Validator.cs` and it's picked up.
- `LoggingBehaviour<,>` is registered as a pipeline behavior. It logs all request start/end and flags slow requests (>1 second) with a warning.
- Application and domain code return `Result` or `Result<T>` (from the `DomainValidation` package). Never throw exceptions for expected failures.

### Minimal API endpoints
- Endpoints are static methods in feature-scoped classes (e.g., `BookEndpoints`). Each class exposes a `Map*Endpoints(this RouteGroupBuilder)` extension.
- Translate `Result<T>` to HTTP via `ResultExtensions`: `ToCreatedResponse`, `ToProblemDetails`, `ToNoContentResponse`. Error types map to: Validation → 422, NotFound → 404, Conflict → 409, other → 400/500. Responses use RFC 9110 ProblemDetails with error codes in `extensions.errors`.
- Write endpoints use `sender.SendWithRetryAsync()` (Polly: retry 3× exponential, circuit breaker, fallback). Read endpoints use `sender.Send()` directly.
- Read endpoints use `CacheOutput` with tag-based invalidation. After a successful write, call `cacheStore.EvictByTagAsync("tag")`.
- Authorization uses named policies: `ViewerPolicy`, `EditorPolicy`, `AdminPolicy`. Permission roles from Keycloak: `view`, `create`, `edit`, `delete`.

### Authentication
- Authentication is environment-sensitive. Normal environments use Keycloak JWT bearer auth via `AddKeycloakJwtBearer`; the `Testing` environment swaps in `TestAuthHandler`, which grants the integration test client all required roles.
- OpenAPI and Scalar are only mapped in Development when the backend user secret `ScalarApi:ClientSecret` is set. This matters for `pnpm generate`, because `orval.config.ts` reads `http://localhost:5049/openapi/v1.json`.

### Database and migrations
- Non-production startup automatically applies EF Core migrations in `WebApplicationExtensions.cs`. The EF CLI works because `ApplicationDbContextFactory.cs` supplies a design-time Npgsql connection when Aspire has not injected `ConnectionStrings:postgresdb`.
- Entity configurations use `IEntityTypeConfiguration<T>` in `Infrastructure.Persistence`, auto-discovered via `ApplyConfigurationsFromAssembly`.

### Testing patterns
- Integration tests use `DistributedApplicationTestingBuilder` to spin up the full Aspire stack with `--environment=Testing`. Tests share a fixture via `[Collection("DistributedApplication collection")]` and create `HttpClient` per class via `App.CreateHttpClient("cleanarchitecture-api")`.
- Unit tests use xUnit + Moq. Domain and Application behavior are tested in `Tests\Domain.UnitTests` and `Tests\Application.UnitTests`.
- CI workflow (`.github\workflows\dotnet.yml`) builds with `--configuration Release`, runs all tests, and sets `ASPIRE_HOSTING_TESTING_DISABLE_DASHBOARD=true`.

### Admin app (Next.js)
- Use `pnpm` exclusively. The repo commits `pnpm-lock.yaml` and the AppHost starts the frontend with `.WithPnpm()`.
- Treat `src\lib\api` and `src\lib\api\zod` as **generated output**. Update the backend contract and rerun `pnpm generate` instead of hand-editing those files.
- The custom fetcher `src\lib\orval-fetch.ts` detects server vs. client context: server-side prepends `API_BASE_URL` and uses `getServerSession()`; client-side sends relative URLs and uses `getSession()`. Both inject `Authorization: Bearer` headers. On 401, it triggers Keycloak re-authentication.
- Routes are organized under `src\app\(admin)\` (protected layout with sidebar) and unauthenticated routes at root (`/signin`). An API proxy at `src\app\api\v1\[...path]\route.ts` forwards browser calls to the backend.
- Frontend has i18n support (en, fa, ar) with RTL layout via `LanguageContext`. Theme supports dark/light mode via `ThemeContext`.
- Permission-based UI: `useUserPermissions()` hook reads roles from the NextAuth session (populated from Keycloak JWT) to conditionally render create/edit/delete actions.
- Forms use React Hook Form + Zod validation + a `fieldErrorMap` pattern that maps API `ProblemDetails` error codes to form field names for inline error display.
- Context provider hierarchy in root layout: `AuthProvider` → `QueryProvider` (TanStack Query, 60s stale time) → `LanguageProvider` → `ThemeProvider` → `SidebarProvider`.
- Local secrets: backend user secrets for `ScalarApi:ClientSecret`, and `CleanArchitecture.Presentation\admin\.env.local` for `API_BASE_URL`, `NEXTAUTH_URL`, `NEXTAUTH_SECRET`, `KEYCLOAK_CLIENT_ID`, `KEYCLOAK_CLIENT_SECRET`, `KEYCLOAK_ISSUER`.

### Commits and PRs
- Use conventional commits: `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:`.
