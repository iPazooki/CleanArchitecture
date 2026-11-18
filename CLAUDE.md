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
This starts the Aspire AppHost, which orchestrates the API, Next.js admin, PostgreSQL, PgAdmin, and Keycloak in the Development environment. `aspire run` from the repo root does the same (`aspire.config.json` points at the AppHost). Docker must be running.

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
Filter to a single test class or method (xUnit v3 on Microsoft.Testing.Platform, opted in via `global.json`, so use MTP filter flags, not `--filter`):
```bash
dotnet test --project Tests/Domain.UnitTests/Domain.UnitTests.csproj --filter-class "*BookTests"
dotnet test --project Tests/Domain.UnitTests/Domain.UnitTests.csproj --filter-method "*Create*"
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
Use `pnpm` only (the AppHost starts the admin with `.WithPnpm()`). `pnpm generate` needs the API running in Development — OpenAPI and Scalar are mapped only there.

### Database migrations
```bash
dotnet ef migrations add <Name> \
  --project CleanArchitecture.Infrastructure.Persistence \
  --startup-project CleanArchitecture.Presentation/API
```
`dotnet ef` builds the context through `ApplicationDbContextFactory`, which uses a placeholder connection string. That is enough to scaffold a migration, because scaffolding reads the model, not the database. To run `dotnet ef database update` against a real database, set `ConnectionStrings__postgresDatabaseResource` first. Usually you don't need to: the DbMigrator applies pending migrations on every local `aspire run`. In Azure, the DbMigrator is a manually triggered Container Apps Job (`az containerapp job start --name db-migrator ...`).

## Architecture

This is a .NET 10 + Next.js 16 Clean Architecture template orchestrated by .NET Aspire.

### Layer dependencies (strictly enforced by `Tests/Architecture.UnitTests`)
```
Domain ← Application ← Infrastructure / Infrastructure.Persistence ← Presentation
```

- **Domain** — Entities inherit from `Entity` → `AggregateRoot` (or auditable variants). The only package reference is `DomainValidation.NET`; no messaging, persistence or web dependency may leak in (enforced by `Tests/Architecture.UnitTests`). Domain events implement the Domain's own `IDomainEvent` marker, never `INotification`. Only an aggregate may raise its own events (`AddDomainEvent` is `protected`). The dispatch interceptor collects events only from `AggregateRoot` entries, so an event raised on a plain `Entity` is never dispatched. Audit timestamps are `private set` — the persistence layer writes them through EF's change tracker. Errors are `DomainError`, which inherits `DomainValidation.Error` and carries an `ErrorType` used by Presentation to pick a status code. Error messages come from localized RESX files (`Domain/Resources/*.resx`, in en/fa/ar), and the culture is chosen per request by `UseRequestLocalization`. Members of `*Errors` classes such as `BookErrors` are expression-bodied properties on purpose: each access re-reads the resource in the current culture. Turning one into a `static readonly` field would freeze its message in a single language.
- **Application** — CQRS via the Mediator **source generator** (not MediatR). Features follow `Entities/<Aggregate>/Commands/<Verb>/` and `Entities/<Aggregate>/Queries/<Verb>/` (request record, handler, validator), with event handlers in `Entities/<Aggregate>/EventHandlers/`. Commands/queries return `Result<T>`. Handlers implement `IRequestHandler<TRequest, TResponse>`. Pipeline order is `LoggingBehaviour` (outermost) → `ValidationBehaviour`, so validation failures are still logged. FluentValidation validators are `internal sealed`. `AddValidatorsFromAssembly(..., includeInternalTypes: true)` discovers them, and `ValidationBehaviour` runs them, so adding a `*Validator` class is all it takes. Their failures become `DomainError.Validation`, so they surface as 422. Domain events are adapted to mediator notifications here: `DomainEventNotifications` maps each `IDomainEvent` to a concrete `INotification` (Mediator's generator emits a dispatch arm per concrete type, so no open-generic wrapper is possible). `IApplicationDbContext` exposes the aggregate `DbSet`s plus `SaveEntitiesAsync`; it is a `DbContext` facade, deliberately not a Unit of Work.
- **Infrastructure** — External service integrations and authorization policies. `IEmailService` is implemented by `BrevoEmailService`, a typed `HttpClient` over the Brevo transactional API that returns `Result` instead of throwing; it falls back to `NullEmailService` when `Brevo:ApiKey` is unset. Polly resilience — retry, circuit breaker, timeout — is supplied by ServiceDefaults' `ConfigureHttpClientDefaults`, so clients here deliberately add no handler of their own. Authorization policies live in `Security/Policies/`. They are listed explicitly in `PolicyRegistry`, with no assembly scanning, so a new policy needs its file plus one entry there. They are registered by `AddInfrastructureServices` and are referenced from endpoints via `PolicyNames`. Role names in `Security/Roles.cs` (`view`, `create`, `edit`, `delete`) must match the Keycloak realm export and the Entra app roles. Policies that chain `RequireRole` calls need **all** of those roles (`Editor` = view + create + edit), not any one.
- **Infrastructure.Persistence** — EF Core 10 + PostgreSQL. `ApplicationDbContext` implements `IApplicationDbContext`, and its `SaveEntitiesAsync` converts `DbUpdateException`/`DbUpdateConcurrencyException` into a `Result` — the exception is logged, never returned, because the message names tables and constraints. Two SaveChanges interceptors run: `AuditableEntityInterceptor` and `DispatchDomainEventsInterceptor`, which dispatches **before** the commit so handlers share the transaction. Migrations live here; the `CleanArchitecture.DbMigrator` project applies them at startup before the API starts.
- **Presentation/API** — .NET Minimal API, versioned at `/api/v1/...`. Endpoints unwrap `Result<T>` via `ResultExtensions`: success responses carry the payload itself, never the `Result` envelope (which would also publish `Error`'s `[CallerFilePath]` metadata to clients). Status codes come from `DomainError.Type`, not from sniffing error-code strings. When a result holds several errors, one status wins, in this order:
  1. Any `NotFound` error gives 404.
  2. Otherwise, any `Conflict` error gives 409.
  3. Otherwise, 422 only if *every* error is a `Validation` error.
  4. Anything else gives 500. Nothing returns 400.

  Each feature has an endpoint class, such as `Endpoints/BookEndpoints.cs`, that exposes `Map<X>Endpoints(this RouteGroupBuilder)`. A new class has to be wired up in `Program.cs` on the `versionedApi` group. There is no output caching: ASP.NET Core's default policy refuses to cache authenticated responses, and every endpoint requires authorization.
- **Presentation/admin** — Next.js 16 App Router BFF; the .NET API is never exposed publicly. Authentication via NextAuth.js ↔ Keycloak or Entra OIDC (`AUTH_PROVIDER`). The orval mutator `src/lib/orval-fetch.ts` is browser-only and never handles credentials: it calls the app's own proxy route `src/app/api/v1/[...path]`, which strips any client-supplied `Authorization`/`Cookie` and attaches the access token itself before forwarding to `API_BASE_URL`. Server Components must call the .NET API directly with a token from `lib/auth/access-token`, not through orval or the proxy. API client is **orval-generated** from the OpenAPI spec; never edit files under `src/lib/api/` manually. Components may import generated *types* from `@/lib/api/model`. Generated hooks are wrapped instead: queries and query keys in `src/lib/books/book-queries.ts`, mutations in `src/hooks/useBookMutation.ts`. Follow the same pattern for new aggregates so that regenerating the client only affects these wrapper files. `src/proxy.ts` is Next 16's renamed `middleware.ts` (not the API proxy route). It protects page routes only. Handlers under `/api` protect themselves, because redirecting a `fetch()` to the sign-in page would come back as a 200 full of HTML. `src/config/env-vars.ts` validates env vars with Zod, keyed on `AUTH_PROVIDER`: choosing a provider makes its variables required. TanStack Query manages server state (60s stale time). i18n supports `en`, `fa`, `ar` with RTL layout. Next.js 16 has breaking API changes: per `admin/AGENTS.md`, read the relevant guide in `admin/node_modules/next/dist/docs/` before writing Next.js code.

### Authentication
- The API picks its scheme from `Authentication:Provider`: `Entra` uses Microsoft.Identity.Web with the `AzureAd` section; anything else (the default) uses Keycloak JWT bearer from the `Keycloak` section. There is no auth bypass in any environment.
- The Development AppHost always runs Keycloak (realm `clean-api`, imported from `AppHost/Realms/`). The import is `IGNORE_EXISTING` and the container uses a persistent data volume, so edits to `realm-export.json` don't reach an existing realm until that volume is removed.
- The Production AppHost chooses the provider via its `UseKeycloak` setting and sets `Authentication__Provider` / `AUTH_PROVIDER` on the API and admin to match.

### Aspire environments
| Environment | Services |
|---|---|
| Development | API, Next.js admin, Postgres + PgAdmin, Keycloak (realm auto-imported) |
| Production | Azure Container Apps, PostgreSQL Flexible Server, Key Vault, Application Insights |

`AppHost.cs` branches on the environment into `Environments/DevelopmentEnvironmentExtensions.cs` and `Environments/ProductionEnvironmentExtensions.cs`. .NET services are added with `AddDotnetProject(name, path)`, using paths in `AppHostConstants.cs`. This is the Project v2 API, so the AppHost has no `<ProjectReference>`s and a new service needs only a path constant there. The Postgres database resource, and therefore its connection string, is named `postgresDatabaseResource` (`ResourceNames.PostgresDatabase`). In production the API has internal ingress only; the admin is the only public app.

Tests are in-process only — `Tests/Domain.UnitTests`, `Tests/Application.UnitTests`, and `Tests/Architecture.UnitTests`. Nothing boots the Aspire host or a container to run a test, so `dotnet test` needs no Docker daemon.

### Code quality
`Directory.Build.props` enforces `Nullable`, `TreatWarningsAsErrors`, `AnalysisMode=All`, and SonarAnalyzer across all projects. A failing build is a failing lint. The CI pipeline (`.github/workflows/dotnet.yml`) runs `dotnet build` and `dotnet test` in Release mode.

### Key conventions
- All commands/queries return `Result<T>` — never throw for expected business failures.
- Fail with a `DomainError`, never a bare `Error` or a raw string: only `DomainError` carries the `ErrorType` that decides the HTTP status code. An error without one is treated as an unexpected server failure (500).
- Never wrap in-process mediator calls in a retry. Commands are not idempotent — retrying `CreateBook` can insert the book more than once. HTTP-client resilience belongs in ServiceDefaults; database retries belong in the Npgsql connection strategy.
- Adding a domain event means three things: the record implementing `IDomainEvent`, a concrete `INotification` carrying it, and one entry in `DomainEventNotifications`. A missing entry throws at dispatch rather than dropping the event.
- Package versions are centrally managed in `Directory.Packages.props`; add new dependencies there, not directly in `.csproj` files.
- Frontend API calls go through the orval-generated client. Run `pnpm generate` after any OpenAPI spec change.
- The Brevo API key is a secret. Locally: `dotnet user-secrets set "Brevo:ApiKey" <key> --project CleanArchitecture.Presentation/API`. In production it flows from the Aspire `brevoApiKey` parameter into Key Vault, and only when `UseBrevo` is enabled.
- Commits use conventional prefixes: `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:`.
- `.github/copilot-instructions.md` is out of date. It describes `BaseRequestHandler`, `SendWithRetryAsync`, output caching, a unit of work, Zod output from orval, and a server-side branch in `orval-fetch.ts`, none of which exist. Trust the code and this file over it.
