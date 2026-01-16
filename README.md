# ToDoList API + Frontend (Portfolio)

## Overview
This repository is a portfolio-focused .NET 9 Web API that demonstrates layered architecture, domain modeling, and API design around accounts, to-do lists, and to-do items.【F:src/Api/Program.cs†L20-L185】【F:src/Api/ListManagement/Controllers/ListsController.cs†L14-L355】【F:src/Api/ListManagement/Controllers/ItemsController.cs†L19-L431】【F:ToDoList.sln†L6-L15】【F:src/Domain/Accounts/Entities/Account.cs†L10-L199】【F:src/Domain/Lists/Entities/ToDoList.cs†L9-L199】

**Portfolio highlights (evidence-backed):**
- Layered solution structure (Api → Application → Domain → Infrastructure).【F:ToDoList.sln†L6-L15】
- DDD tactical patterns with aggregates and value objects in the Domain layer.【F:src/Domain/Accounts/Entities/Account.cs†L10-L199】【F:src/Domain/Lists/Entities/ToDoList.cs†L9-L199】
- CQRS-style separation via explicit command/query types and MediatR registration.【F:src/Application/Application.csproj†L7-L16】【F:src/Application/Lists/Commands/Lists/CreateToDoListCommand.cs†L1-L37】【F:src/Application/Lists/Queries/Lists/GetAllToDoListsByAccountQuery.cs†L1-L26】
- Authentication and security configuration using JWT and ASP.NET Identity.【F:src/Api/Program.cs†L43-L82】【F:src/Infrastructure/DependencyInjection.cs†L43-L69】
- Docker-based development environment for local reproducibility.【F:docker-compose.yml†L1-L101】

> Frontend status: not implemented; the current UI appears to be the default Next.js starter page and is not wired to the API.【F:app/app/page.tsx†L1-L103】

## Docker-Based Development Environment
This repository includes a `docker-compose.yml` intended for running the API, database, and frontend in development containers with bind-mounted source code and local data persistence.【F:docker-compose.yml†L1-L103】

## Key Features (API)
- Account registration, login, and email confirmation with JWT issuance.【F:src/Api/Identity/Controllers/AuthController.cs†L49-L203】
- CRUD and workflow operations for to-do lists (create, update title/description, complete/incomplete, delete).【F:src/Api/ListManagement/Controllers/ListsController.cs†L32-L287】
- To-do item management inside lists (create, update title, complete/incomplete, delete).【F:src/Api/ListManagement/Controllers/ItemsController.cs†L38-L277】
- Item transfer between lists and due-date management, plus an endpoint for items with due dates.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L280-L406】

## Architecture at a Glance
- **Layered structure:** Api → Application → Domain → Infrastructure projects in the solution file.【F:ToDoList.sln†L6-L15】
- **CQRS-style separation (partial):** explicit command and query types in the Application layer (via MediatR).【F:src/Application/Application.csproj†L7-L16】【F:src/Application/Lists/Commands/Lists/CreateToDoListCommand.cs†L1-L37】【F:src/Application/Lists/Queries/Lists/GetAllToDoListsByAccountQuery.cs†L1-L26】
- **DDD tactical patterns:** domain aggregates and value objects for accounts and lists/items.【F:src/Domain/Accounts/Entities/Account.cs†L10-L199】【F:src/Domain/Lists/Entities/ToDoList.cs†L9-L199】

## Tech Stack and Tooling
- **Backend:** ASP.NET Core (.NET 9), EF Core, ASP.NET Identity, JWT auth, Swagger/OpenAPI, MediatR.【F:src/Api/Api.csproj†L10-L26】【F:src/Application/Application.csproj†L7-L16】【F:src/Infrastructure/Infrastructure.csproj†L7-L12】
- **Frontend (not implemented):** Next.js 15, React 19, ESLint, Tailwind tooling.【F:app/package.json†L11-L33】

## Security Practices (Evidence-Based)
- JWT bearer authentication with issuer/audience validation and zero clock skew.【F:src/Api/Program.cs†L43-L82】
- ASP.NET Identity password complexity rules and lockout configuration.【F:src/Infrastructure/DependencyInjection.cs†L43-L69】
- Data Protection key storage configured for token workflows (email confirmation, etc.).【F:src/Api/Program.cs†L84-L94】
- HTTPS redirection enabled in the HTTP pipeline.【F:src/Api/Program.cs†L177-L179】

## Testing and Quality Signals
- **Tests are not implemented yet.** Planned future work includes automated tests for API behaviors and domain logic.

## Documentation
- Start with `docs/overview.md` and `docs/architecture.md` for a concise view of the system.
- `docs/api.md` provides the endpoint reference for the HTTP API.

## Limitations
- The Next.js frontend is not implemented and is not wired to the API; it likely does not function beyond the default template page.【F:app/app/page.tsx†L1-L103】【F:app/README.md†L1-L36】
