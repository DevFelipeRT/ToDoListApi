# To-Do List API (Portfolio)

## Overview
This repository is a portfolio-focused .NET 9 Web API that demonstrates layered architecture, domain modeling, and API design around accounts, to-do lists, and to-do items.

**Portfolio highlights (evidence-backed):**
- Layered solution structure (Api → Application → Domain → Infrastructure).
- DDD tactical patterns with aggregates and value objects in the Domain layer.
- CQRS-style separation via explicit command/query types and MediatR registration.
- Authentication and security configuration using JWT and ASP.NET Identity.
- Docker-based development environment for local reproducibility.

> Frontend status: not implemented; the current UI appears to be the default Next.js starter page and is not wired to the API.

## Docker-Based Development Environment
This repository includes a `docker-compose.yml` intended for running the API, database, and frontend in development containers with bind-mounted source code and local data persistence.

## Key Features (API)
- Account registration, login, and email confirmation with JWT issuance.
- CRUD and workflow operations for to-do lists (create, update title/description, complete/incomplete, delete).
- To-do item management inside lists (create, update title, complete/incomplete, delete).
- Item transfer between lists and due-date management, plus an endpoint for items with due dates.

## Architecture at a Glance
- **Layered structure:** Api → Application → Domain → Infrastructure projects in the solution file.
- **CQRS-style separation (partial):** explicit command and query types in the Application layer (via MediatR).
- **DDD tactical patterns:** domain aggregates and value objects for accounts and lists/items.

## Tech Stack and Tooling
- **Backend:** ASP.NET Core (.NET 9), EF Core, ASP.NET Identity, JWT auth, Swagger/OpenAPI, MediatR.
- **Frontend (not implemented):** Next.js 15, React 19, ESLint, Tailwind tooling.

## Security Practices (Evidence-Based)
- JWT bearer authentication with issuer/audience validation and zero clock skew.
- ASP.NET Identity password complexity rules and lockout configuration.
- Data Protection key storage configured for token workflows (email confirmation, etc.).
- HTTPS redirection enabled in the HTTP pipeline.

## Testing and Quality Signals
- **Tests are not implemented yet.** Planned future work includes automated tests for API behaviors and domain logic.

## Documentation
- Start with `docs/overview.md` and `docs/architecture.md` for a concise view of the system.
- `docs/api.md` provides the endpoint reference for the HTTP API.

### Documentation Links
- [Overview](docs/overview.md)
- [Architecture](docs/architecture.md)
- [API Reference](docs/api.md)
- [Local Setup](docs/local-setup.md)
- [Common Tasks](docs/common-tasks.md)
- [Troubleshooting](docs/troubleshooting.md)

## Limitations
- The Next.js frontend is not implemented and is not wired to the API; it likely does not function beyond the default template page.
