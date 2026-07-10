![GitHub](https://img.shields.io/github/license/ipazooki/CleanArchitecture)
![GitHub contributors](https://img.shields.io/github/contributors/ipazooki/CleanArchitecture)
[![.NET Aspire CI](https://github.com/iPazooki/CleanArchitecture/actions/workflows/dotnet.yml/badge.svg)](https://github.com/iPazooki/CleanArchitecture/actions/workflows/dotnet.yml)

# 🧱 Clean Architecture

A production-ready **.NET 10 + Next.js 16** Clean Architecture template, orchestrated by **.NET Aspire** and ready to deploy to Azure with `azd`. It ships with a Minimal API, an admin dashboard, dual authentication providers, and PostgreSQL — all wired up out of the box.

---

## ✨ Highlights

- 🎯 **Clean Architecture** — strict layer boundaries enforced by architecture tests
- ⚡ **.NET 10 Minimal API** — versioned endpoints, output caching, `Result<T>` pattern
- 🧩 **CQRS** via the [Mediator](https://github.com/martinothamar/Mediator) source generator (no MediatR runtime cost)
- 🌐 **Next.js 16 Admin Panel** — App Router BFF with i18n (en / fa / ar + RTL)
- 🔐 **Pluggable Authentication** — switch between **Keycloak** and **Microsoft Entra ID** via config
- 📜 **Scalar API Reference** — interactive OpenAPI UI with implicit OAuth flow (no client secret needed)
- 🛠️ **Orval-generated API client** — typed React hooks regenerated from the OpenAPI spec
- 🐘 **PostgreSQL** via EF Core 10 with auto-applied migrations
- ☁️ **.NET Aspire** — orchestrates all services locally and provisions Azure infrastructure
- 🧪 **Robust Testing** — Domain unit tests and architecture tests
- 🛡️ **Polly** — retries, circuit breakers, fallbacks for outbound calls

---

## 🏗️ Architecture

```
Domain ← Application ← Infrastructure / Infrastructure.Persistence ← Presentation
```

| Layer | Responsibility |
|---|---|
| **Domain** | Entities, Value Objects, Specifications, Domain Events |
| **Application** | CQRS commands/queries, validators, `Result<T>` |
| **Infrastructure** | External integrations (email, etc.) with Polly resilience |
| **Infrastructure.Persistence** | EF Core `DbContext`, Postgres, migrations, interceptors |
| **Presentation/API** | Minimal API, versioned at `/api/v1/...`, Scalar UI |
| **Presentation/admin** | Next.js admin dashboard (BFF + NextAuth.js) |
| **Aspire AppHost** | Orchestrates the full stack and Azure deployment |

---

## ✅ Prerequisites

- 📦 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- 🐳 Docker Desktop (for Postgres, Keycloak, PgAdmin containers)
- 🟢 [Node.js 20+](https://nodejs.org/) and [pnpm](https://pnpm.io/installation)
- 🌩️ (Optional) [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) + [Azure Developer CLI (`azd`)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) for cloud deployment

---

## 🚀 Quick Start

```bash
# 1. Clone
git clone https://github.com/iPazooki/CleanArchitecture.git
cd CleanArchitecture

# 2. Restore .NET dependencies
dotnet restore

# 3. Install admin panel dependencies
cd CleanArchitecture.Presentation/admin && pnpm install && cd ../..

# 4. Run the full stack with Aspire
dotnet run --project CleanArchitecture.Aspire/CleanArchitecture.AppHost/CleanArchitecture.AppHost.csproj
```

Aspire will spin up everything you need:

| Service | What it does |
|---|---|
| 🔵 **API** | .NET Minimal API |
| 🟣 **Admin** | Next.js dashboard |
| 🐘 **Postgres** + **PgAdmin** | Database & UI |
| 🔑 **Keycloak** | Local identity provider (realm auto-imported) |

The Aspire dashboard prints the URLs for every resource (API, Scalar UI, admin, Keycloak, PgAdmin).

---

## 🔐 Authentication

The app supports **two providers** and switches between them via configuration.

| Environment | Default Provider | Why |
|---|---|---|
| 🛠️ Development | **Keycloak** | Runs as a local container, realm auto-imported, no cloud setup needed |
| ☁️ Production | **Microsoft Entra ID** | Managed identity, no secrets to rotate for the API |

### Switching providers

In `CleanArchitecture.Aspire/CleanArchitecture.AppHost/appsettings.*.json`:

```jsonc
{
  "UseKeycloak": true   // false → Entra
}
```

The AppHost wires the right environment variables (`Authentication__Provider`, issuer URLs, audiences, NextAuth client config) into the API and admin projects automatically.

> 📘 Scalar uses the **OAuth 2.0 implicit flow**, so no client secret is needed for the API reference UI. Just sign in from the Scalar page.

---

## 📜 API Reference (Scalar)

Once running, open the **Scalar** UI from the Aspire dashboard's API resource. It renders the OpenAPI spec at `/openapi/v1.json` and lets you authenticate and call endpoints interactively.

---

## 🖥️ Admin Panel

The admin dashboard lives in `CleanArchitecture.Presentation/admin` (Next.js 16, App Router, Tailwind, NextAuth.js, TanStack Query).

```bash
cd CleanArchitecture.Presentation/admin
pnpm dev        # dev server (Aspire normally runs this for you)
pnpm build      # production build
pnpm lint       # ESLint with --max-warnings=0
pnpm generate   # regenerate the typed API client from the live OpenAPI spec
```

### ⚙️ Required Configuration (.env.local)

Create a `.env.local` file in the `CleanArchitecture.Presentation/admin` directory with the following required configuration:

```env
API_BASE_URL=http://localhost:5049/

NEXTAUTH_URL=http://localhost:65499
NEXTAUTH_SECRET=<Secret> you can use: node -e "console.log(require('crypto').randomBytes(32).toString('base64'))" to generate a secret code

AUTH_PROVIDER=Entra //or Keycloak

KEYCLOAK_CLIENT_ID=<Client ID>
KEYCLOAK_CLIENT_SECRET=<Client Secret>
KEYCLOAK_ISSUER=<Issuer URL>
KEYCLOAK_SCOPES=openid profile email permissions

ENTRA_CLIENT_ID=<Client ID>
ENTRA_CLIENT_SECRET=<Client Secret>
ENTRA_TENANT_ID=<Tenant ID>
ENTRA_SCOPES=openid profile email <Scopes>
ENTRA_OPENID_CONNECT=<OpenID Connect URL>
```

### 🤖 Orval — generated API client

The React Query hooks and TypeScript DTOs under `src/lib/api/` are **generated by Orval** from the API's OpenAPI document. **Never edit them by hand.** After changing an endpoint or DTO, run:

```bash
pnpm generate
```

(Make sure the API is running on `http://localhost:5049` so Orval can fetch the spec.)

---

## 🗄️ Database

- **Engine**: PostgreSQL
- **ORM**: EF Core 10 (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Migrations**: live in `CleanArchitecture.Infrastructure.Persistence`
- **Auto-apply**: the dedicated `CleanArchitecture.DbMigrator` project runs migrations at startup before the API boots

### Add a migration

```bash
dotnet ef migrations add <MigrationName> \
  --project CleanArchitecture.Infrastructure.Persistence \
  --startup-project CleanArchitecture.Presentation/API
```

> 💡 An `ApplicationDbContextFactory` provides a design-time connection string so EF tools work without Aspire running.

---

## ☁️ Deploy to Azure with `azd`

The Aspire AppHost provisions everything for you:

- 🚢 **Azure Container Apps** — API, admin, (optionally Keycloak)
- 🐘 **Azure Database for PostgreSQL Flexible Server**
- 🔒 **Azure Key Vault** — all secrets stored here automatically
- 📊 **Azure Application Insights**

### Deploy

```bash
aspire publish
```

and then after entering you desired values, run:
```bash
aspire deploy
```

> 🔐 All secret parameters (`*Secret`, `*Password`, `nextAuthSecret`) are written to **Azure Key Vault** automatically — they never live in plain text in your container apps.

---

## 🧪 Testing

```bash
dotnet test --configuration Release
```

- 🧬 **Domain unit tests** — `Tests/Domain.UnitTests`
- 🏛️ **Architecture tests** — enforce layer dependencies

Every test runs in-process, so `dotnet test` needs no Docker daemon and no running services.

---

## 🤝 Contributing

Pull requests are welcome! For major changes, please open an [issue](https://github.com/iPazooki/CleanArchitecture/issues) first to discuss what you'd like to change.

⭐ If you find this template useful, please **star the repo** — it really helps!

---

## 📄 License

[MIT](LICENSE) — use it freely as a foundation for your own projects.
