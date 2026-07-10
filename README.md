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
  │      🧱 CLEAN ARCHITECTURE & MICROSOFT ENTRA ID 🧱      │
  │         🚀 Orchestrated by .NET Aspire 10 & 13         │
  └────────────────────────────────────────────────────────┘
```

A production-ready, cloud-native template built on **.NET 10** and **Next.js 16 (React 19)**, orchestrated by **.NET Aspire** and configured for instant Azure containerized deployment. 

It features a C# Minimal API backend, a Next.js Admin Portal acting as a secure **Backend-for-Frontend (BFF)**, integrated authentication (Microsoft Entra ID / Keycloak), and a PostgreSQL database.

---

## ✨ Highlights & Features

- 🧱 **Clean Architecture** — Strict layer boundaries enforced via automated architecture unit tests.
- ⚡ **.NET 10 Minimal API** — Type-safe versioned endpoints (`/api/v1/...`), result pattern, and OpenAPI integration.
- 🧩 **CQRS via Source Generator** — Powered by the [Mediator](https://github.com/martinothamar/Mediator) source generator, eliminating runtime reflection cost.
- 🔐 **Secure BFF Auth** — Next.js server handles authentication (NextAuth.js) and token exchanges, keeping tokens safe from the browser.
- 🌐 **Next.js 16 Admin Panel** — Styled with **Tailwind CSS 4**, full i18n support (English, Persian, Arabic with RTL direction), and responsive layouts.
- ⚡ **Orval-Generated Client** — Auto-generated TypeScript types and TanStack Query hooks from the backend OpenAPI spec.
- 🔑 **Pluggable Identity** — Supports **Microsoft Entra ID** (default CIAM/AD) and **Keycloak** (OIDC/JWT) switchable via configuration.
- 🐘 **PostgreSQL & EF Core 10** — Robust persistence, change-tracked auditing, and a dedicated startup migrator project (`DbMigrator`).
- 📧 **Brevo Email Service** — Out-of-the-box transactional email client with automatic fallback to a null service.
- ☁️ **.NET Aspire Orchestration** — Streamlined developer experience, local service discovery, and zero-effort Azure provisioning via `azd`.
- 🛡️ **Polly Resilience** — HTTP retries, timeouts, and circuit breakers automatically configured via Service Defaults.
- 📜 **Scalar API Docs** — Sleek, interactive API reference interface with built-in OAuth authentication support.

---

## 🏗️ Architectural Overview

```text
       ┌────────────────────────┐
       │   Presentation (API)   │ ──┐
       └────────────────────────┘   │
                    │               │
                    ▼               │
       ┌────────────────────────┐   │
       │      Application       │   │
       └────────────────────────┘   │
                    │               │
                    ▼               ▼
       ┌────────────────────────┐ ┌──────────────────────────────────────┐
       │         Domain         │ │ Infrastructure / Persistence (EF Core)│
       └────────────────────────┘ └──────────────────────────────────────┘
```

| Layer | Responsibility & Details |
|---|---|
| 🌌 **Domain** | Entities, Value Objects, Domain Events, validations (`DomainValidation.NET`). Free of dependencies. |
| ⚙️ **Application** | CQRS Commands/Queries, validators (`FluentValidation`), and `Result<T>` handlers. |
| 🛡️ **Infrastructure** | Integrations like Brevo Email Service, policies, and outbound Polly resilience. |
| 🐘 **Persistence** | EF Core 10 DbContext, auditing interceptors, migrations, and PostgreSQL schemas. |
| 🔵 **Presentation (API)** | Minimal API, versioned route groups, Result unwrapping, and Scalar UI docs. |
| 🟣 **Presentation (Admin)** | Next.js admin dashboard (NextAuth.js BFF, Orval-generated query hooks). |
| 🚀 **Aspire AppHost** | Orchestrates services locally and generates infrastructure manifests for Azure. |

---

## 🛠️ Prerequisites

Make sure you have the following installed on your machine:
* 📦 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* 🐳 [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Postgres database)
* 🟢 [Node.js 20+](https://nodejs.org/) & [pnpm](https://pnpm.io/)
* 🌩️ *Optional:* [Azure Developer CLI (`azd`)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) for cloud deployments.

---

## 🚀 Quick Start (Local Run)

Get up and running in minutes:

```bash
# 1. Clone the repository
git clone https://github.com/iPazooki/CleanArchitecture.git
cd CleanArchitecture

# 2. Restore .NET dependencies
dotnet restore

# 3. Install admin portal node modules
cd CleanArchitecture.Presentation/admin
pnpm install
cd ../..

# 4. Run the entire stack with .NET Aspire
dotnet run --project CleanArchitecture.Aspire/CleanArchitecture.AppHost/CleanArchitecture.AppHost.csproj
```

### 🛰️ Orchestrated Services
Once Aspire starts, open the **Aspire Dashboard** link in your terminal to monitor and access all services:
* 🔵 **API Service** — C# Backend Minimal API
* 🟣 **Admin Portal** — Next.js Dashboard
* 🐘 **Postgres + PgAdmin** — Database and management UI
* 📜 **Scalar UI** — Live API reference docs

---

## 🔐 Authentication Config

The project is designed with a pluggable authentication setup. You can switch between **Microsoft Entra ID** and **Keycloak**. 

### ⚙️ AppHost Configuration
In `CleanArchitecture.Aspire/CleanArchitecture.AppHost/appsettings.json` (or your local environment secrets):
```json
{
  "UseKeycloak": false // Set to true to use Keycloak; false uses Entra ID
}
```

### ⚙️ Admin Portal Config (`.env.local`)
Create a `.env.local` inside `CleanArchitecture.Presentation/admin/` to authenticate the Next.js server side:

```env
API_BASE_URL=http://localhost:5049/
NEXTAUTH_URL=http://localhost:65499
# Generate a secret: node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"
NEXTAUTH_SECRET=your-random-32-byte-base64-secret

AUTH_PROVIDER=Entra # Set to "Entra" or "Keycloak"

# Microsoft Entra ID Config (CIAM/AD)
ENTRA_CLIENT_ID=<Your-Entra-Client-ID>
ENTRA_CLIENT_SECRET=<Your-Entra-Client-Secret>
ENTRA_TENANT_ID=<Your-Entra-Tenant-ID>
ENTRA_SCOPES=openid profile email offline_access api://<Your-Audience-ID>/permissions
ENTRA_OPENID_CONNECT=https://<Your-Domain>.ciamlogin.com/<Your-Tenant-ID>/v2.0/.well-known/openid-configuration

# Keycloak Config (Optional)
KEYCLOAK_CLIENT_ID=mrpanel
KEYCLOAK_CLIENT_SECRET=<Your-Keycloak-Client-Secret>
KEYCLOAK_ISSUER=http://localhost:8080/realms/clean-api
KEYCLOAK_SCOPES=openid profile email permissions
```

---

## 🖥️ Next.js 16 Admin Portal

The UI resides in `CleanArchitecture.Presentation/admin` and connects to the backend through the Next.js API route BFF.

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
> The TypeScript API client and queries located in `src/lib/api/` are **automatically generated by Orval** from the OpenAPI document. **Do not modify these files manually.** Run `pnpm generate` whenever you alter endpoints or backend DTOs.

---

## 🗄️ Database & Migrations

Database operations use **EF Core 10** with **Npgsql** targeting **PostgreSQL**.
A dedicated container project (`CleanArchitecture.DbMigrator`) runs at startup to automatically apply pending migrations.

### Add a New EF Migration
If you modify your DB context or entities in persistence, generate a migration:
```bash
dotnet ef migrations add <MigrationName> \
  --project CleanArchitecture.Infrastructure.Persistence \
  --startup-project CleanArchitecture.Presentation/API
```

---

## 📧 Transactional Emails (Brevo)

The template includes integration with **Brevo Email Service** for transaction emails.
* **Fallback Behavior**: If `Brevo:ApiKey` is not configured, the application falls back to `NullEmailService` rather than crashing, ensuring a seamless local experience.
* **Secrets Configuration**: Set your Brevo key locally:
  ```bash
  dotnet user-secrets set "Brevo:ApiKey" "<your-key>" --project CleanArchitecture.Presentation/API
  ```

---

## 🧪 Testing Strategy

Run all tests from the repository root:
```bash
dotnet test --configuration Release
```

* **Domain Unit Tests** — Validates invariants, aggregates, and value object behaviors.
* **Architecture Tests** — Enforces clean layer dependency flow (Domain has zero dependencies, Application depends only on Domain, etc.). If a forbidden dependency leaks into a layer, the build fails.

---

## ☁️ Deploy to Azure with `azd`

Thanks to .NET Aspire integration, deploying the entire stack to Azure is fully automated. The deployment provisions:
* 🚢 **Azure Container Apps** — Hosts the API and Admin Next.js portal.
* 🐘 **Azure Database for PostgreSQL (Flexible Server)**.
* 🔒 **Azure Key Vault** — Stores database credentials, secrets, and auth provider tokens.
* 📊 **Azure Application Insights & Log Analytics** — Log, trace, and metric collection.

### Deploy Command
```bash
# 1. Initialize config and secrets
aspire publish

# 2. Deploy infrastructure and code
aspire deploy
```

---

## 🤝 Contributing

Contributions are welcome! Please open an [issue](https://github.com/iPazooki/CleanArchitecture/issues) first to discuss proposed changes.

⭐ If you find this template helpful, please **star the repo** — it helps others find it!

---

## 📄 License

This project is licensed under the [MIT License](LICENSE) - free for personal and commercial use.
