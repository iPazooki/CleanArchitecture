# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Documentation

Always use Context7 MCP (`npx ctx7@latest`) proactively when working with any library, framework, SDK, API, or CLI tool in this project — including .NET, EF Core, Aspire, Next.js, NextAuth, TanStack Query, orval, FluentValidation, Keycloak, Polly, Serilog, and OpenTelemetry. Do this automatically without waiting to be asked, whenever a task involves library-specific syntax, configuration, version migration, or setup steps.

Steps:
1. `npx ctx7@latest library <name> "<question>"` — find the library ID
2. `npx ctx7@latest docs <id> "<question>"` — fetch current docs

## Commands

### Running the full stack
```bash
dotnet run --project CleanArchitecture.Aspire/CleanArchitecture.AppHost/CleanArchitecture.AppHost.csproj
```
This starts the Aspire AppHost, which orchestrates the API, Next.js admin, PostgreSQL, PgAdmin, and Keycloak in the Development environment.

### Backend
```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```
Run a specific test project:
```bash
dotnet test Tests/Domain.UnitTests/Domain.UnitTests.csproj --configuration Release
```
Filter to a single test class or method:
```bash
dotnet test Tests/Domain.UnitTests/Domain.UnitTests.csproj --filter "FullyQualifiedName~BookTests"
```

### Frontend (Next.js admin)
```bash
cd CleanArchitecture.Presentation/admin
pnpm install
pnpm dev        # dev server
pnpm build      # production build
pnpm lint       # ESLint with --max-warnings=0
pnpm generate   # regenerate orval API client from http://localhost:5049/openapi/v1.json
```

### Database migrations
```bash
dotnet ef migrations add <Name> \
  --project CleanArchitecture.Infrastructure.Persistence \
  --startup-project CleanArchitecture.Presentation/API
```

## Architecture

This is a .NET 10 + Next.js 16 Clean Architecture template orchestrated by .NET Aspire.

### Layer dependencies (strictly enforced by `Tests/Architecture.UnitTests`)
```
Domain ← Application ← Infrastructure / Infrastructure.Persistence ← Presentation
```

- **Domain** — Entities inherit from `Entity` → `AggregateRoot` (or auditable variants). No external dependencies. Domain events and Value Objects live here.
- **Application** — CQRS via the Mediator **source generator** (not MediatR). Commands/queries return `Result<T>`. Handlers extend `BaseRequestHandler<TRequest, TResponse>`. FluentValidation validators are auto-registered as behaviors.
- **Infrastructure** — External service integrations (email, etc.) with Polly resilience policies.
- **Infrastructure.Persistence** — EF Core 10 + PostgreSQL. `ApplicationDbContext` uses two SaveChanges interceptors: `AuditableEntityInterceptor` and `DispatchDomainEventsInterceptor`. Migrations live here; the `CleanArchitecture.DbMigrator` project applies them at startup before the API starts.
- **Presentation/API** — .NET Minimal API, versioned at `/api/v1/...`. Endpoints translate `Result<T>` to HTTP via `ResultExtensions`. Output caching uses tag-based invalidation (e.g., the `"books"` tag).
- **Presentation/admin** — Next.js 16 App Router BFF. Authentication via NextAuth.js ↔ Keycloak OIDC. API client is **orval-generated** from the OpenAPI spec; never edit files under `src/lib/api/` manually. TanStack Query manages server state (60s stale time). i18n supports `en`, `fa`, `ar` with RTL layout.

### Aspire environments
| Environment | Services |
|---|---|
| Development | API, Next.js admin, Postgres + PgAdmin, Keycloak (realm auto-imported) |
| Testing | API + ephemeral Postgres only. `TestAuthHandler` grants all roles — no Keycloak needed. |
| Production | Azure Container Apps, PostgreSQL Flexible Server, Key Vault, Application Insights |

Integration tests use `DistributedApplicationTestingBuilder` and share the Aspire fixture via `[Collection("DistributedApplication collection")]`.

### Code quality
`Directory.Build.props` enforces `Nullable`, `TreatWarningsAsErrors`, `AnalysisMode=All`, and SonarAnalyzer across all projects. A failing build is a failing lint. The CI pipeline (`.github/workflows/dotnet.yml`) runs `dotnet build` and `dotnet test` in Release mode.

### Key conventions
- All commands/queries return `Result<T>` — never throw for expected business failures.
- Use `ISpecification<T>` for reusable query predicates in the Domain layer.
- Package versions are centrally managed in `Directory.Packages.props`; add new dependencies there, not directly in `.csproj` files.
- Frontend API calls go through the orval-generated client. Run `pnpm generate` after any OpenAPI spec change.
