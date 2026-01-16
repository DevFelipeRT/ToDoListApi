# Architecture

## Layered Structure (API)
The API is organized into explicit layers in the solution: **Api**, **Application**, **Domain**, and **Infrastructure**.【F:ToDoList.sln†L6-L15】

```mermaid
graph TD
  Api[Api (Controllers & HTTP pipeline)] --> Application[Application (Commands/Queries, Services)]
  Application --> Domain[Domain (Aggregates, Value Objects, Events)]
  Application --> Infrastructure[Infrastructure (Persistence, Identity, Email, Links)]
  Api --> Infrastructure
```

## Entry Points
- **HTTP API bootstrap:** `src/Api/Program.cs` configures auth, Swagger, CORS, and controller mapping.【F:src/Api/Program.cs†L20-L185】
- **Controllers:** list and item endpoints live in `ListsController` and `ItemsController`; auth endpoints in `AuthController`.【F:src/Api/ListManagement/Controllers/ListsController.cs†L14-L355】【F:src/Api/ListManagement/Controllers/ItemsController.cs†L19-L431】【F:src/Api/Identity/Controllers/AuthController.cs†L21-L203】

## CQRS-Style Segregation (Partial)
Commands and queries are modeled as distinct request types (via MediatR). This reflects a partial CQRS approach: commands mutate, queries read, both routed through MediatR handlers (handlers live in the Application layer).【F:src/Application/Application.csproj†L7-L16】【F:src/Application/Lists/Commands/Lists/CreateToDoListCommand.cs†L1-L37】【F:src/Application/Lists/Queries/Lists/GetAllToDoListsByAccountQuery.cs†L1-L26】

## DDD Tactical Patterns
- **Aggregates and value objects** encapsulate business rules in the Domain layer (e.g., `Account`, `ToDoList`).【F:src/Domain/Accounts/Entities/Account.cs†L10-L199】【F:src/Domain/Lists/Entities/ToDoList.cs†L9-L199】

## Infrastructure Concerns
- **Persistence:** EF Core with SQL Server, wired through `ApplicationDbContext`.【F:src/Infrastructure/DependencyInjection.cs†L37-L42】【F:src/Infrastructure/Persistence/ApplicationDbContext.cs†L10-L47】
- **Identity and auth:** ASP.NET Identity plus JWT bearer authentication configuration in the API. 【F:src/Infrastructure/DependencyInjection.cs†L43-L64】【F:src/Api/Program.cs†L43-L82】
- **Email outbox:** Disk-based email sender configured via environment variables. 【F:src/Infrastructure/DependencyInjection.cs†L90-L94】
