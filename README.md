![GitHub](https://img.shields.io/github/license/ipazooki/CleanArchitecture)
![GitHub contributors](https://img.shields.io/github/contributors/ipazooki/CleanArchitecture)
[![.NET Aspire CI](https://github.com/iPazooki/CleanArchitecture/actions/workflows/dotnet.yml/badge.svg)](https://github.com/iPazooki/CleanArchitecture/actions/workflows/dotnet.yml)

```text
  ┌────────────────────────────────────────────────────────┐
  │  ______ _                  ___           _             │
  │ / ____/ /__  ____ _____   /   |  _______/ /_           │
  │/ /   / / _ \/ __ `/ __ \ / /| | / ___/ __  /           │
  │/ /___/ /  __/ /_/ / / / // ___ |/ /  / /_/ /           │
  │\____/_/\___/\__,_/_/ /_//_/  |_/_/   \__,_/            │
  │                                                        │
  │      🧱  CLEAN ARCHITECTURE & KEYCLOAK/ENTRA ID 🧱      │
  │         🚀 Orchestrated by .NET 10 & Aspire 13         │
  └────────────────────────────────────────────────────────┘
```

A production-ready, cloud-native template built on **.NET 10** and **Next.js 16 (React 19)**, orchestrated by **Aspire 13** and ready to deploy to **Azure Container Apps**.

It features a C# Minimal API backend, a Next.js Admin Portal acting as a secure **Backend-for-Frontend (BFF)**, pluggable authentication (**Keycloak** or **Microsoft Entra ID**), and a PostgreSQL database.

---

## ✨ Highlights & Features

- 🧱 **Clean Architecture** — Layer boundaries enforced by automated architecture tests.
- ⚡ **.NET 10 Minimal API** — Versioned endpoints (`/api/v1/...`), the result pattern, and errors returned as `ProblemDetails` with status codes driven by the domain error type.
- 🧩 **CQRS via Source Generator** — Powered by the [Mediator](https://github.com/martinothamar/Mediator) source generator (not MediatR), with no runtime reflection cost. Logging and FluentValidation pipeline behaviours included.
- 🔐 **Secure BFF Auth** — The Next.js server handles sign-in (NextAuth.js) and forwards API calls through its own proxy route, attaching the access token server-side. Tokens never reach the browser, and the .NET API is never exposed publicly.
- 🔑 **Pluggable Identity** — **Keycloak** runs locally with its realm auto-imported; production uses **Microsoft Entra ID** by default, or Keycloak if you turn it on.
- 🛂 **Role-Based Authorization** — `Viewer`, `Editor` and `Admin` policies built on the `view`, `create`, `edit` and `delete` roles.
- 🌐 **Next.js 16 Admin Panel** — **Tailwind CSS 4**, i18n in English, Persian and Arabic (RTL), and responsive layouts.
- ⚡ **Orval-Generated Client** — TypeScript types and TanStack Query hooks generated from the backend OpenAPI spec.
- 🐘 **PostgreSQL & EF Core 10** — Change-tracked auditing, domain events dispatched inside the save transaction, and a dedicated migrator project (`DbMigrator`).
- 📧 **Brevo Email Service** — Transactional email client that falls back to a null service when no API key is configured.
- ☁️ **Aspire Orchestration** — One command starts the whole stack locally; `aspire deploy` provisions and deploys it to Azure.
- 📊 **Observability** — Serilog and OpenTelemetry feed the Aspire dashboard locally and Application Insights in Azure.
- 🛡️ **HTTP Resilience** — Standard retry, timeout and circuit-breaker handlers on every `HttpClient`, configured once in Service Defaults.
- 📜 **Scalar API Docs** — Interactive API reference with OAuth sign-in through Keycloak (Development only).

---

## 🏗️ Architectural Overview

Arrows point from a layer to the layer it depends on.

```text
   ┌──────────────────────────────┐
   │      Presentation (API)      │   composition root
   └──────────────────────────────┘
          │                │
          ▼                ▼
   ┌─────────────┐   ┌──────────────────────────────┐
   │ Application │◀──│ Infrastructure / Persistence │
   └─────────────┘   └──────────────────────────────┘
          │
          ▼
   ┌─────────────┐
   │   Domain    │
   └─────────────┘
```

| Layer | Responsibility & Details |
|---|---|
| 🌌 **Domain** | Aggregates, value objects (`Genre`), domain events and validation rules. Its only package is `DomainValidation.NET`; nothing from persistence, messaging or the web is allowed in. |
| ⚙️ **Application** | CQRS commands/queries returning `Result<T>`, `FluentValidation` validators, logging/validation pipeline, and domain-event notification handlers. |
| 🛡️ **Infrastructure** | Brevo email service, current-user access, and the authorization policies. |
| 🐘 **Persistence** | EF Core 10 `DbContext`, auditing and domain-event interceptors, and migrations for PostgreSQL. |
| 🗃️ **DbMigrator** | Applies pending migrations before the API starts. |
| 🔵 **Presentation (API)** | Minimal API endpoints, versioned route groups, `Result` → HTTP response mapping, and Scalar docs. |
| 🟣 **Presentation (Admin)** | Next.js admin dashboard: NextAuth.js BFF, API proxy route, and Orval-generated query hooks. |
| 🧰 **ServiceDefaults** | OpenTelemetry, health checks, service discovery, and HTTP resilience shared by every .NET service. |
| 🚀 **Aspire AppHost** | Orchestrates everything locally and defines the Azure resources for deployment. |

---

## 🛠️ Prerequisites

* 📦 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* 🌟 [Aspire CLI](https://aspire.dev/) — `curl -sSL https://aspire.dev/install.sh | bash`
* 🐳 [Docker Desktop](https://www.docker.com/products/docker-desktop/) — runs the PostgreSQL, PgAdmin and Keycloak containers
* 🟢 [Node.js 22.13+](https://nodejs.org/) & [pnpm 11](https://pnpm.io/) — `corepack enable` picks up the version pinned in `package.json`
* 🔒 A trusted HTTPS development certificate — `dotnet dev-certs https --trust`
* 🌩️ *Optional, for deployment:* an Azure subscription and the [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli), signed in with `az login`

---

## 🚀 Quick Start (Local Run)

```bash
# 1. Clone the repository
git clone https://github.com/iPazooki/CleanArchitecture.git
cd CleanArchitecture

# 2. Restore .NET dependencies
dotnet restore

# 3. Install admin portal node modules
cd CleanArchitecture.Presentation/admin
pnpm install

# 4. Create .env.local for the admin portal (see "Admin Portal Config" below)

# 5. Run the entire stack with Aspire from the repository root
cd ../..
aspire run
```

`aspire run` uses `aspire.config.json` to find the AppHost. Running `dotnet run --project CleanArchitecture.Aspire/CleanArchitecture.AppHost` does the same thing.

### 🛰️ Orchestrated Services

Open the **Aspire Dashboard** link printed in your terminal to monitor every service:

| Service | Local address |
|---|---|
| 🟣 Admin Portal (Next.js) | http://localhost:65499 |
| 🔵 API | https://localhost:7281 · http://localhost:5049 |
| 📜 Scalar API reference | https://localhost:7281/scalar |
| 🔑 Keycloak (realm `clean-api`) | https://localhost:8080 — admin console login `admin` / `admin` |
| 🐘 PostgreSQL + PgAdmin | Linked from the Aspire dashboard |
| 🗃️ DbMigrator | Runs once, before the API starts |

### 👤 Create Your First User

The imported realm has no users. Before you sign in to the admin portal:

1. Open the Keycloak admin console at https://localhost:8080 and switch to the **`clean-api`** realm.
2. Create a user and set a (non-temporary) password.
3. Under **Role mapping**, assign the realm roles the user needs:

| Action | Policy | Roles required |
|---|---|---|
| List / view books | `Viewer` | **any one** of `view`, `create`, `edit`, `delete` |
| Create / edit books | `Editor` | **all** of `view`, `create`, `edit` |
| Delete books | `Admin` | **all four** roles |

> [!NOTE]
> Keycloak keeps its data in a persistent volume, and the realm import skips existing realms. Changes to `Realms/realm-export.json` only take effect after you remove the Keycloak data volume.

---

## 🔐 Authentication Config

### 🧪 Local Development (Keycloak)

The Development AppHost **always** runs Keycloak and imports the `clean-api` realm. It has two clients: `mrpanel` (the admin portal) and `scalar` (the API reference). The API's `appsettings.Development.json` already points at it, so the API needs no extra setup.

The AppHost generates the `mrpanel` client secret once, saves it in the AppHost's user secrets, and injects it into Keycloak and the admin portal as `KEYCLOAK_CLIENT_SECRET`.

### ⚙️ Admin Portal Config (`.env.local`)

Create `CleanArchitecture.Presentation/admin/.env.local`:

```env
API_BASE_URL=http://localhost:5049/
NEXTAUTH_URL=http://localhost:65499
# Generate a secret: node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"
NEXTAUTH_SECRET=your-random-32-byte-base64-secret

AUTH_PROVIDER=Keycloak # "Keycloak" or "Entra" (case-insensitive)

KEYCLOAK_CLIENT_ID=mrpanel
KEYCLOAK_ISSUER=https://localhost:8080/realms/clean-api
KEYCLOAK_SCOPES=openid profile email permissions
# KEYCLOAK_CLIENT_SECRET is injected by the AppHost; a value already in the
# environment takes precedence over .env.local.

# Only when AUTH_PROVIDER=Entra:
# ENTRA_CLIENT_ID=<Your-Entra-Client-ID>
# ENTRA_CLIENT_SECRET=<Your-Entra-Client-Secret>
# ENTRA_TENANT_ID=<Your-Entra-Tenant-ID>
# ENTRA_SCOPES=openid profile email offline_access api://<Your-Audience-ID>/permissions
# ENTRA_OPENID_CONNECT=https://<Your-Domain>.ciamlogin.com/<Your-Tenant-ID>/v2.0/.well-known/openid-configuration
```

The admin validates these variables at startup. Whichever provider you select, its variables become required.

### ☁️ Production (Entra ID or Keycloak)

The Production AppHost reads two settings, from `CleanArchitecture.Aspire/CleanArchitecture.AppHost/appsettings.json` or environment variables:

| Setting | Default | Effect |
|---|---|---|
| `UseKeycloak` | `false` | `false` → Microsoft Entra ID; `true` → a Keycloak container app backed by the PostgreSQL server |
| `UseBrevo` | `false` | `true` → wires the Brevo API key and sender into the API |

The AppHost sets `Authentication__Provider` on the API and `AUTH_PROVIDER` on the admin portal to match. Each mode needs these AppHost parameters, which `aspire deploy` prompts for:

* **Always:** `nextAuthSecret`, `PostgresUsername`, `PostgresPassword`
* **Entra ID:** `EntraTenantId`, `EntraAPIInstance`, `EntraAPIPrimaryDomain`, `EntraAPIClientId`, `EntraAPIAudienceId`, `EntraAdminClientId`, `EntraAdminClientSecret`, `EntraAdminScope`, `EntraAdminOpenIdURL`
* **Keycloak:** `keycloakClientId`, `keycloakClientSecret`, `keycloakRealm`, `keycloakAdminUsername`, `keycloakAdminPassword`, `keycloakDbUsername`, `keycloakDbPassword`
* **Brevo:** `brevoApiKey`, `brevoSenderName`, `brevoSenderEmail`

Secrets are stored in Azure Key Vault. Entra app roles (or realm roles in a production Keycloak) must use the same names as `Security/Roles.cs`: `view`, `create`, `edit` and `delete`. Production Keycloak doesn't import the realm, so you configure it yourself.

---

## 🖥️ Next.js 16 Admin Portal

The UI lives in `CleanArchitecture.Presentation/admin`. Browser code calls the app's own proxy route (`src/app/api/v1/[...path]`), which strips any client-supplied credentials, attaches the user's access token, and forwards the request to `API_BASE_URL`.

### Useful Commands
```bash
# Start Next.js development server manually
pnpm dev

# Produce a production build
pnpm build

# Run linting with warnings treated as errors
pnpm lint

# Regenerate API client and TanStack Query hooks from the OpenAPI spec
pnpm generate
```

> [!IMPORTANT]
> The TypeScript API client and hooks in `src/lib/api/` are **generated by Orval** from `http://localhost:5049/openapi/v1.json`. **Do not edit these files by hand.** Run `pnpm generate` after changing endpoints or DTOs. The API must be running in Development, the only environment where OpenAPI and Scalar are mapped.

---

## 🗄️ Database & Migrations

Database operations use **EF Core 10** with **Npgsql** targeting **PostgreSQL**. The `CleanArchitecture.DbMigrator` project applies pending migrations:

* **Locally:** the migrator runs on every `aspire run`, and the API waits for it to finish.
* **In Azure:** it is deployed as a **manually triggered Container Apps Job** (`db-migrator`). Start it after a deployment that adds migrations:
  ```bash
  az containerapp job start --name db-migrator --resource-group <your-resource-group>
  ```

### Add a New EF Migration
Requires the EF Core CLI (`dotnet tool install --global dotnet-ef`):
```bash
dotnet ef migrations add <MigrationName> \
  --project CleanArchitecture.Infrastructure.Persistence \
  --startup-project CleanArchitecture.Presentation/API
```

---

## 📧 Transactional Emails (Brevo)

The template integrates the **Brevo** transactional email API.
* **Fallback behavior:** If `Brevo:ApiKey` is not configured, the application uses `NullEmailService`, so local runs work without a key.
* **Local secret:**
  ```bash
  dotnet user-secrets set "Brevo:ApiKey" "<your-key>" --project CleanArchitecture.Presentation/API
  ```
* **Production:** set `UseBrevo` to `true` in the AppHost; the key flows from the `brevoApiKey` parameter into Key Vault.

---

## 🧪 Testing Strategy

Run all tests from the repository root:
```bash
dotnet test --configuration Release
```

All tests run in-process, so neither Docker nor the Aspire host needs to be running.

* **Domain Unit Tests** — Aggregate invariants, domain events, and the `Genre` value object.
* **Application Unit Tests** — Persistence behaviour such as the auditable-entity interceptor.
* **Architecture Tests** — Enforce the layer dependency rules: Domain depends on nothing, Application only on Domain, and Infrastructure/Persistence only on Application. A forbidden dependency fails the test run.

The tests use xUnit v3 on Microsoft.Testing.Platform, so filter with MTP flags instead of `--filter`:
```bash
dotnet test --project Tests/Domain.UnitTests/Domain.UnitTests.csproj --filter-class "*BookTests"
dotnet test --project Tests/Domain.UnitTests/Domain.UnitTests.csproj --filter-method "*Create*"
```

### Code Quality
`Directory.Build.props` turns on nullable reference types, `TreatWarningsAsErrors`, `AnalysisMode=All` and SonarAnalyzer for every project, so an analyzer warning breaks the build. CI (`.github/workflows/dotnet.yml`) builds and tests in Release mode on every push and pull request to `main` or `aspire`.

---

## ☁️ Deploy to Azure

The Production AppHost deploys the entire stack to Azure, provisioning:
* 🚢 **Azure Container Apps** — The admin portal (public), the API (internal ingress only), the DbMigrator job, and, when `UseKeycloak` is on, Keycloak.
* 🐘 **Azure Database for PostgreSQL (Flexible Server)** — Burstable `Standard_B1ms` tier, with 7-day backups.
* 🔒 **Azure Key Vault** — Database, auth provider, NextAuth and Brevo secrets.
* 📊 **Application Insights** — Logs, traces and metrics.

### Deploy Command
```bash
# Provision Azure resources and deploy the code (prompts for missing settings and parameters)
aspire deploy

# Optional: generate the deployment artifacts without deploying
aspire publish
```

Then start the `db-migrator` job (see [Database & Migrations](#-database--migrations)). The repository also includes an `azure.yaml`, so `azd up` works too.

---

## 🤝 Contributing

Contributions are welcome! Please open an [issue](https://github.com/iPazooki/CleanArchitecture/issues) first to discuss proposed changes.

⭐ If you find this template helpful, please **star the repo** — it helps others find it!

---

## 📄 License

This project is licensed under the [MIT License](LICENSE) - free for personal and commercial use.
