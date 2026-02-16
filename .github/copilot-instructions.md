# Copilot Instructions

> Role: When generating suggestions or code for this repository, act as a senior software engineer: be conservative, prioritize safety, maintainability, and clear reasoning. Explain trade-offs briefly when making non-obvious choices.

## Purpose
This file tells Copilot how to behave when authoring code, tests, CI config, and documentation for the CleanArchitecture template (targets .NET 10, Minimal API, Aspire AppHost, Postgres via EF Core).

## High-level project expectations
- Preserve Clean Architecture boundaries: Domain, Application, Infrastructure, Infrastructure.Persistence, Presentation, Aspire AppHost.
- Prefer explicit, well-tested, small changes. Avoid broad refactors without accompanying tests and a clear migration plan.
- Follow Domain-Driven Design principles: domain models, value objects, specifications, and thin application services / use-cases.
- Use CQRS patterns for commands/queries where they add value, but avoid premature complexity for trivial endpoints.

## Coding conventions (apply to all suggested code)
- Target framework: .NET 10.
- Naming: PascalCase for types/methods/properties, `I` prefix for interfaces, camelCase for parameters, private fields `_camelCase`.
- Async: Prefer `async`/`await`, never return `async void`, always propagate CancellationToken when accepting it.
- DI: Use constructor injection for services. Keep controllers/minimal endpoint handlers thin; delegate to application services.
- EF Core: Use explicit configuration for entities and value objects in `Infrastructure.Persistence`. Use migrations to change schema.
- Formatting: Clear, idiomatic C#; prefer expression-bodied members only for trivial single-line methods.
- Observability: Inject `ILogger<T>` and add meaningful logs at important decision points (info/warn/error).
- Resilience: Use Polly for outbound calls where appropriate; prefer transient fault handling for network calls.

## Security & secrets
- Authentication is integrated with Keycloak. Do not hard-code client secrets or credentials.
- For user secrets in development prefer Visual Studio __Manage User Secrets__ or the CLI:
  - __dotnet user-secrets set "ScalarApi:ClientSecret" "<secret>" --project CleanArchitecture.Api__
- Secrets in CI should come from repository secrets or the runner environment; never commit secrets.

## Tests & CI
- Unit tests should target Application and Domain behavior; use xUnit + Moq as in the repo.
- Integration tests should run against a running AppHost which provisions Postgres via Aspire.
- CI must start the Aspire AppHost before running integration tests. In GitHub Actions include steps that run the AppHost project (CleanArchitecture.Aspire/CleanArchitecture.AppHost) so Aspire can provision containers and endpoints.
- Keep tests deterministic: seed required data in test setup and clean it up as needed.

## Database & migrations
- Provider: Npgsql (Postgres).
- Use EF Core migrations managed in `Infrastructure.Persistence`.
- Commands typically used:
  - __dotnet ef migrations add <Name> --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation__
  - __dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation__
- For local development prefer running via Aspire AppHost which can provision the DB container.

## Dependency & package guidance
- Prefer stable, well-maintained packages. Avoid adding large third-party dependencies without justification.
- Keep packages up-to-date for security; when upgrading major versions, document compatibility and test matrix.

## Pull requests & commits
- Use small, focused PRs with descriptive titles.
- Follow conventional commit style: `feat:`, `fix:`, `chore:`, `refactor:`, `test:`.
- Include the rationale and testing steps in PR descriptions.

## Helpful prompts for Copilot in this repo
- "Implement a new Application command and handler that validates input, performs domain logic, and is covered by unit tests."
- "Add an EF Core migration for the Book entity in Infrastructure.Persistence and update the DbContext configuration."
- "Create an integration test that starts the Aspire AppHost, seeds Postgres, calls the minimal API endpoint, and asserts the response."
- "Refactor this minimal API endpoint to use an application service and add appropriate unit tests."

## When to ask for human review
- Any changes touching authentication, secrets, migrations, or CI flows.
- Adding or upgrading critical dependencies (e.g., EF Core, Npgsql, Polly).
- Changes that modify public API contracts or domain model invariants.

## Non-functional priorities
1. Correctness and safety (data integrity, auth, secrets)
2. Test coverage for behavior
3. Maintainability and clear boundaries
4. Performance only when measurable issues exist; prefer profiling before micro-optimizing
