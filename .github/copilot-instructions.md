---
apply: always
---

# Copilot Instructions

## Build, test, and lint commands

### Backend and solution
- Install the Aspire workload before using the AppHost: `dotnet workload install aspire`
- Restore packages: `dotnet restore`
- Build the solution: `dotnet build --configuration Release`
- Use `dotnet build` as the .NET lint gate. `Directory.Build.props` enables analyzers, enforces code style in build, and treats warnings as errors across all `.csproj` files.
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
- Lint the admin app: `pnpm lint`
- Regenerate API hooks and Zod schemas: `pnpm generate`

## High-level architecture

- `CleanArchitecture.sln` is split into Core (`CleanArchitecture.Domain`, `CleanArchitecture.Application`), Infrastructure (`CleanArchitecture.Infrastructure`, `CleanArchitecture.Infrastructure.Persistence`), Presentation (`CleanArchitecture.Presentation\API`, `CleanArchitecture.Presentation\admin`), Aspire (`CleanArchitecture.AppHost`, `CleanArchitecture.ServiceDefaults`), and four test projects.
- `CleanArchitecture.Presentation\API\Program.cs` is the backend composition root. It adds Aspire service defaults first, then wires the Application, Infrastructure, Infrastructure.Persistence, and Presentation layers through their `Add*Services()` extension methods.
- The API is a versioned Minimal API. Endpoints are grouped under `/api/v{version:apiVersion}` and then mapped by feature extension classes such as `BookEndpoints`.
- `CleanArchitecture.Infrastructure.Persistence` owns EF Core concerns: `ApplicationDbContext`, entity configurations, migrations, SaveChanges interceptors, and the unit-of-work implementation.
- `CleanArchitecture.Aspire\CleanArchitecture.AppHost\AppHost.cs` is the real local-dev entry point. In Development it starts Postgres + PgAdmin, Keycloak with realm import, the API, and the Next.js admin app. In Testing it starts only the API and an ephemeral Postgres database, which is why integration tests do not need a separately managed database or Keycloak instance.
- `CleanArchitecture.Aspire\CleanArchitecture.ServiceDefaults\Extensions.cs` adds the shared cross-cutting runtime behavior: Serilog, OpenTelemetry, service discovery, resilience handlers, and health endpoints.
- `CleanArchitecture.Presentation\admin` is a Next.js 16 App Router admin app. It uses NextAuth with Keycloak, TanStack Query for API hooks, and generated API clients/Zod schemas under `src\lib\api` and `src\lib\api\zod`.

## Key conventions

- Preserve the layer boundaries enforced by `Tests\Architecture.UnitTests\ArchitectureTests.cs`. Domain must not reference Application, Infrastructure, or Presentation; Application must not reference Infrastructure or Presentation; Infrastructure projects must not reference Presentation.
- Backend use cases follow CQRS with Mediator source generation and FluentValidation auto-registration from `CleanArchitecture.Application\DependencyInjection.cs`. New features should follow the existing `Entities\<Aggregate>\Commands` and `Entities\<Aggregate>\Queries` layout.
- Application and domain code return `Result` or `Result<T>`. Minimal API endpoints translate those results to HTTP responses through `CleanArchitecture.Presentation\API\Extensions\ResultExtensions.cs` instead of building ad hoc response shapes.
- Write endpoints use `SendWithRetryAsync()` from `CleanArchitecture.Presentation\API\Configuration\MediatorPollyExtensions.cs`, which wraps mediator calls with Polly retry, circuit-breaker, and fallback behavior. Read endpoints call `sender.Send()` directly.
- Non-production startup automatically applies EF Core migrations in `CleanArchitecture.Presentation\API\Configuration\WebApplicationExtensions.cs`. The EF CLI works because `CleanArchitecture.Infrastructure.Persistence\Data\ApplicationDbContextFactory.cs` supplies a design-time Npgsql connection when Aspire has not injected `ConnectionStrings:postgresdb`.
- Authentication is environment-sensitive. Normal environments use Keycloak JWT bearer auth with named policies from `CleanArchitecture.Infrastructure\Security`; the `Testing` environment swaps in `TestAuthHandler`, which grants the integration test client all required roles.
- OpenAPI and Scalar are only mapped in Development when the backend user secret `ScalarApi:ClientSecret` is set. This matters for `pnpm generate`, because `orval.config.ts` reads `http://localhost:5049/openapi/v1.json`.
- Treat `CleanArchitecture.Presentation\admin\src\lib\api` and `CleanArchitecture.Presentation\admin\src\lib\api\zod` as generated output. Update the backend contract and rerun `pnpm generate` instead of hand-editing those files. The repo also relies on `scripts\fix-zod-schemas.js` after generation.
- Use `pnpm` for the admin app. The repo commits `pnpm-lock.yaml`, the AppHost starts the frontend with `.WithPnpm()`, and `.aiassistant\rules\instructions.md` assumes pnpm-based frontend workflows.
- The admin app behaves like a thin BFF. `next.config.ts` rewrites browser calls from `/api/v1/*` to `API_BASE_URL`, while `src\lib\orval-fetch.ts` attaches the NextAuth access token for both server-side and client-side requests.
- Local secrets live in two places: backend user secrets for `ScalarApi:ClientSecret`, and `CleanArchitecture.Presentation\admin\.env.local` for `API_BASE_URL`, `NEXTAUTH_*`, and `KEYCLOAK_*`.
