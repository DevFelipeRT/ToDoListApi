# Architecture

## Layered Structure (API)
The API is organized into explicit layers in the solution: **Api**, **Application**, **Domain**, and **Infrastructure**.

```mermaid
graph TD
  Api[Api (Controllers & HTTP pipeline)] --> Application[Application (Commands/Queries, Services)]
  Application --> Domain[Domain (Aggregates, Value Objects, Events)]
  Application --> Infrastructure[Infrastructure (Persistence, Identity, Email, Links)]
  Api --> Infrastructure
```

## Entry Points
- **HTTP API bootstrap:** `src/Api/Program.cs` configures auth, Swagger, CORS, and controller mapping.
- **Controllers:** list and item endpoints live in `ListsController` and `ItemsController`; auth endpoints in `AuthController`.

## CQRS-Style Segregation (Partial)
Commands and queries are modeled as distinct request types (via MediatR). This reflects a partial CQRS approach: commands mutate, queries read, both routed through MediatR handlers (handlers live in the Application layer).

## DDD Tactical Patterns
- **Aggregates and value objects** encapsulate business rules in the Domain layer (e.g., `Account`, `ToDoList`).

## Supporting Patterns in Use
- **Repository pattern:** aggregate persistence is abstracted behind repositories (`IAccountRepository`, `IToDoListRepository`) with EF Core implementations.
- **Unit of Work:** `EfUnitOfWork` coordinates persistence and dispatches domain events after commits.
- **Mediator:** MediatR routes commands, queries, and notifications between API/controllers and application handlers.
- **Domain events (publish/subscribe):** aggregates raise events that are dispatched to handlers via a domain event dispatcher.
- **Policy/Strategy:** uniqueness rules are expressed as policy interfaces with injectable implementations.

## Infrastructure Concerns
- **Persistence:** EF Core with SQL Server, wired through `ApplicationDbContext`.
- **Identity and auth:** ASP.NET Identity plus JWT bearer authentication configuration in the API. 
- **Email outbox:** Disk-based email sender configured via environment variables. 

## REST and HATEOAS Notes
- The API follows RESTful patterns in its resource-oriented routes and standard HTTP verbs (GET/POST/PUT/DELETE) for lists and items.
- HATEOAS is implemented for list responses via link collections (for example: `self`, `update-title`, `mark-complete`, `items`) and pagination links; item responses do not include HATEOAS links, so the approach is partial.
