# API Reference

Base routes are defined by controller attributes. Authentication is required for list/item endpoints (JWT bearer).【F:src/Api/ListManagement/Controllers/ListsController.cs†L17-L20】【F:src/Api/ListManagement/Controllers/ItemsController.cs†L22-L25】

## Auth
- **POST** `/api/auth/login` — Authenticate and return JWT.【F:src/Api/Identity/Controllers/AuthController.cs†L49-L86】
- **POST** `/api/auth/register` — Register account and return JWT.【F:src/Api/Identity/Controllers/AuthController.cs†L88-L129】
- **GET** `/api/auth/confirm-email` — Confirm email and activate account (query params: `token`, `uid`).【F:src/Api/Identity/Controllers/AuthController.cs†L131-L203】

## Lists (JWT required)
- **POST** `/api/lists` — Create a list.【F:src/Api/ListManagement/Controllers/ListsController.cs†L32-L58】
- **GET** `/api/lists` — Get all lists (pagination query params).【F:src/Api/ListManagement/Controllers/ListsController.cs†L60-L97】
- **GET** `/api/lists/{id}` — Get list by id.【F:src/Api/ListManagement/Controllers/ListsController.cs†L99-L130】
- **PUT** `/api/lists/{id}/title` — Update title.【F:src/Api/ListManagement/Controllers/ListsController.cs†L132-L163】
- **PUT** `/api/lists/{id}/description` — Update description.【F:src/Api/ListManagement/Controllers/ListsController.cs†L165-L196】
- **PUT** `/api/lists/{id}/complete` — Mark as completed.【F:src/Api/ListManagement/Controllers/ListsController.cs†L198-L228】
- **PUT** `/api/lists/{id}/incomplete` — Mark as incomplete.【F:src/Api/ListManagement/Controllers/ListsController.cs†L230-L260】
- **DELETE** `/api/lists/{id}` — Delete list.【F:src/Api/ListManagement/Controllers/ListsController.cs†L262-L292】

## Items (JWT required)
- **POST** `/api/lists/{listId}/items` — Create item in list.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L38-L77】
- **GET** `/api/lists/{listId}/items` — Get items in list (pagination query params).【F:src/Api/ListManagement/Controllers/ItemsController.cs†L79-L134】
- **GET** `/api/lists/{listId}/items/{id}` — Get item by id.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L136-L173】
- **PUT** `/api/lists/{listId}/items/{id}/title` — Update item title.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L175-L203】
- **PUT** `/api/lists/{listId}/items/{id}/complete` — Mark as completed.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L205-L228】
- **PUT** `/api/lists/{listId}/items/{id}/incomplete` — Mark as incomplete.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L230-L253】
- **DELETE** `/api/lists/{listId}/items/{id}` — Delete item.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L255-L278】
- **PUT** `/api/lists/{listId}/items/{id}/transfer` — Transfer item to another list.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L280-L318】
- **PUT** `/api/lists/{listId}/items/{id}/due-date` — Set due date.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L320-L354】
- **DELETE** `/api/lists/{listId}/items/{id}/due-date` — Remove due date.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L356-L384】
- **GET** `/api/items/due-dates` — List items with due dates for current account.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L386-L406】
