---
apply: always
---

---
apply: always
---

# AI Instructions

> Role: When generating suggestions or code for this repository, act as a senior software engineer: be conservative, prioritize safety, maintainability, and clear reasoning. Explain trade-offs briefly when making non-obvious choices.

## High-level project expectations
- This repository is **full-stack**:
    - **Backend:** .NET Clean Architecture (Domain, Application, Infrastructure, Infrastructure.Persistence, Presentation, Aspire AppHost)
    - **Admin Frontend:** React + Next.js + TypeScript
- Preserve architecture boundaries and avoid leaking concerns across layers.
- Prefer explicit, small, well-tested changes. Avoid broad refactors without migration notes and tests.
- Follow DDD in backend (domain models, value objects, specifications, thin application services/use-cases).
- Use CQRS in backend where valuable; avoid unnecessary complexity for trivial scenarios.
- In frontend, prefer component composition, clear data flow, and strongly typed contracts.

## Tech profile (current)
- **Backend:** .NET 10, C# 14, ASP.NET Core MVC, Aspire
- **Frontend:** Next.js 16, React 19, TypeScript 5.9
- **Auth:** Keycloak via NextAuth in admin app + backend auth integration
- **Database:** PostgreSQL with EF Core/Npgsql
- **Styling/UI:** Tailwind CSS
- **Package manager (frontend):** pnpm

## Coding conventions

### Backend (.NET)
- Naming: PascalCase for types/methods/properties, `I` prefix for interfaces, camelCase for parameters, private fields `_camelCase`.
- Async: Prefer `async`/`await`, never return `async void`, propagate `CancellationToken` where relevant.
- DI: Constructor injection by default.
- Keep controllers/minimal API handlers thin; delegate logic to application services/handlers.
- EF Core: explicit entity/value-object configuration in `Infrastructure.Persistence`.
- Logging: `ILogger<T>` with meaningful structured logs at important decision points.
- Resilience: use Polly for outbound network calls where appropriate.

### Frontend (React/Next.js/TypeScript)
- Prefer **TypeScript-first** solutions; avoid `any` unless unavoidable and documented.
- Build small, reusable components; keep UI components mostly presentational.
- Keep side effects contained (`useEffect`, server actions, or dedicated hooks) and predictable.
- Prefer server/client boundaries intentionally in Next.js (`"use client"` only when needed).
- Use accessible markup and keyboard-friendly interactions.
- Keep styling consistent with existing Tailwind patterns and design tokens.
- Avoid introducing large dependencies without strong justification.
- Frontend should be considered as BFF.

## Security & secrets
- Never hard-code credentials, client secrets, tokens, or connection strings.
- Keycloak and NextAuth secrets/config must come from environment variables or secret stores.
- Local development secrets: user-secrets (backend) and `.env.local` (frontend, uncommitted).
- CI secrets must come from repository/runner secret providers.
- Treat auth/session, token handling, and redirect flows as security-sensitive areas requiring extra review.

## Tests & quality gates

### Backend
- Unit tests: Domain and Application behavior (xUnit + Moq).
- Integration tests: run with Aspire AppHost and provisioned Postgres.
- Keep tests deterministic: explicit seed/setup + cleanup.

### Frontend
- Add tests for critical UI behavior, auth flow boundaries, and key utility/hooks logic.
- Prefer focused component/integration tests over brittle snapshot-only testing.
- Validate loading/error/empty states for data-driven views.

### CI expectations
- Start Aspire AppHost before backend integration tests.
- Build and type-check frontend (`pnpm build`, `pnpm lint` if available).
- Do not merge changes that break auth flow, migrations, or core dashboard/admin routes.

## Database & migrations
- Provider: Npgsql (Postgres).
- Migrations are managed in `Infrastructure.Persistence`.
- Typical commands:
    - `dotnet ef migrations add <Name> --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation`
    - `dotnet ef database update --project CleanArchitecture.Infrastructure.Persistence --startup-project CleanArchitecture.Presentation`
- Prefer local orchestration via Aspire AppHost.

## Dependency guidance
- Prefer stable, maintained packages with active ecosystems.
- Avoid adding heavy client libraries unless necessary.
- When upgrading major versions, document compatibility impacts and test matrix.
- Keep frontend and backend dependency upgrades isolated and reviewable.

## Pull requests & commits
- Small, focused PRs with clear titles.
- Conventional commits: `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:`.
- PR description must include:
    - Why this change is needed
    - What was changed
    - How it was tested
    - Any migration/env/config impact

## When to ask for human review
- Authentication/authorization/session changes
- Secrets/configuration handling
- Database migrations or schema-impacting changes
- CI/CD workflow changes
- Public API contract changes
- Critical dependency additions/upgrades

## Non-functional priorities
1. Correctness and safety (data integrity, auth, secrets)
2. Test coverage of behavior
3. Maintainability and clear boundaries
4. Performance only when measured bottlenecks exist

## AI response behavior for this repository
- Prefer minimal, safe diffs over broad rewrites.
- Explain non-obvious decisions briefly.
- If assumptions are required, state them explicitly.
- For multi-layer changes, present plan first (backend, frontend, data, tests).
- Include test/update steps with every meaningful implementation suggestion.
