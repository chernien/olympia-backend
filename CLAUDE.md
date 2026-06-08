# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build the project
dotnet build

# Run in development (starts at https://localhost:7166 and http://localhost:5040)
dotnet run --project Olympia/Olympia.csproj

# Publish release build
dotnet publish Olympia/Olympia.csproj -c Release

# EF Core migrations
dotnet ef migrations add <MigrationName> --project Olympia
dotnet ef database update --project Olympia
```

Swagger UI is auto-launched on startup at `/swagger`.

## Architecture

**ASP.NET Core 6 Web API** with a compiled Angular frontend served as static files from `wwwroot/`.

The backend follows a 3-layer pattern wired through DI:

```
Controllers → Interfaces → Repositories → EF Core DbContext → SQL Server
                               ↑
                          Services (cross-cutting, e.g. SmsService)
```

- **Controllers/** — thin API endpoints, route pattern `api/[controller]`
- **Interfaces/** — repository contracts (`IArtRepository`, `IAuthRepository`, etc.)
- **Repository/** — EF Core implementations of those interfaces, all registered as `Scoped`
- **Services/** — `SmsService` (WinSmsPro HTTP client for SMS notifications), registered via `AddHttpClient<SmsService>()`
- **Models/** — EF Core entity classes + DTOs; most entity classes under `Models/` are **auto-generated** by EF Core Power Tools from the SQL Server schema — do not hand-edit them
- **Models/ERP210OLYMPIA_FContext.cs** — the main `DbContext` (82 KB, auto-generated); re-scaffold with EF Power Tools when the DB schema changes

## Key Domain Concepts

- **Art / Article** — product/item catalog; `ArtController` is the most complex controller
- **Tar / Tariff** — pricing rules; `TarWithRemise` adds discount logic on top
- **Commande / CommandeArticle** — orders and their line items
- **CLI** — client records; **VRP** — commercial/sales-rep users; **T2** — another user class
- **Guest** — unauthenticated visitor tracking
- **Authentication** — custom auth table (`Authentication.cs`), not ASP.NET Identity

## Database

- SQL Server at `192.168.1.222\DIVA`, database `OLYMPIA` (credentials in `appsettings.json`)
- Query timeout set to 120 seconds (configured on the DbContext)
- Connection string key: `"DefaultConnection"` in `appsettings.json`

## Configuration

| Key | Location | Purpose |
|-----|----------|---------|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | SQL Server |
| `WinSms:ApiKey` / `WinSms:SenderId` | `appsettings.json` | SMS gateway |
| CORS allowed origins | `Program.cs` | dev (`:4200`), prod (`olympiaclub.olympiacompany.com`, `storeolympia.com`) |

## Frontend

The Angular app is pre-compiled into `wwwroot/`. `FallbackController` catches all non-API routes and serves `wwwroot/index.html` to support Angular client-side routing.

To rebuild the frontend, compile the Angular project separately and copy the output into `wwwroot/`.

## JSON Serialization

`System.Text.Json` is configured with `ReferenceHandler.IgnoreCycles` globally to handle EF navigation-property cycles — avoid circular navigation loading unless needed.
