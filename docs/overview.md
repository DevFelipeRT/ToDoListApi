# Overview
This project is designed as a portfolio artifact that showcases API design, domain modeling, and a layered architecture implemented in .NET 9.【F:src/Api/Program.cs†L20-L185】【F:ToDoList.sln†L6-L15】

## Domain Summary
- **Accounts:** The domain models account identity, activation state, and related events inside an aggregate root (`Account`).【F:src/Domain/Accounts/Entities/Account.cs†L10-L199】
- **To-do lists and items:** Lists are modeled as aggregates with item collections and domain rules for completion and item changes (`ToDoList`).【F:src/Domain/Lists/Entities/ToDoList.cs†L9-L199】

> Frontend status: not implemented; the current UI appears to be the default Next.js starter page and is not integrated with the API.【F:app/app/page.tsx†L1-L103】

## Primary User Flows (API)
- **Authenticate and obtain JWT:** `POST /api/auth/login`.【F:src/Api/Identity/Controllers/AuthController.cs†L49-L86】
- **Register and confirm account:** `POST /api/auth/register`, then `GET /api/auth/confirm-email`.【F:src/Api/Identity/Controllers/AuthController.cs†L88-L203】
- **Manage lists:** create, read, update title/description, complete/incomplete, delete via `/api/lists` endpoints.【F:src/Api/ListManagement/Controllers/ListsController.cs†L32-L287】
- **Manage items:** create/read/update/complete/incomplete/delete inside `/api/lists/{listId}/items` and related endpoints (transfer, due dates).【F:src/Api/ListManagement/Controllers/ItemsController.cs†L38-L406】

## Access Model
- List and item endpoints require authentication and rely on the current account identifier from JWT claims.【F:src/Api/ListManagement/Controllers/ListsController.cs†L17-L20】【F:src/Api/ListManagement/Controllers/ItemsController.cs†L22-L25】【F:src/Api/Common/Controllers/ApiControllerBase.cs†L49-L83】

## Frontend Status
- Not implemented. The Next.js app appears to be the default starter page and is not integrated with the API; it likely does not function beyond the template UI.【F:app/app/page.tsx†L1-L103】【F:app/README.md†L1-L36】
