# API Reference

Base routes are defined by controller attributes. Authentication is required for list/item endpoints (JWT bearer).

## Auth
- **POST** `/api/auth/login` — Authenticate and return JWT.
- **POST** `/api/auth/register` — Register account and return JWT.
- **GET** `/api/auth/confirm-email` — Confirm email and activate account (query params: `token`, `uid`).

## Lists (JWT required)
- **POST** `/api/lists` — Create a list.
- **GET** `/api/lists` — Get all lists (pagination query params).
- **GET** `/api/lists/{id}` — Get list by id.
- **PUT** `/api/lists/{id}/title` — Update title.
- **PUT** `/api/lists/{id}/description` — Update description.
- **PUT** `/api/lists/{id}/complete` — Mark as completed.
- **PUT** `/api/lists/{id}/incomplete` — Mark as incomplete.
- **DELETE** `/api/lists/{id}` — Delete list.

## Items (JWT required)
- **POST** `/api/lists/{listId}/items` — Create item in list.
- **GET** `/api/lists/{listId}/items` — Get items in list (pagination query params).
- **GET** `/api/lists/{listId}/items/{id}` — Get item by id.
- **PUT** `/api/lists/{listId}/items/{id}/title` — Update item title.
- **PUT** `/api/lists/{listId}/items/{id}/complete` — Mark as completed.
- **PUT** `/api/lists/{listId}/items/{id}/incomplete` — Mark as incomplete.
- **DELETE** `/api/lists/{listId}/items/{id}` — Delete item.
- **PUT** `/api/lists/{listId}/items/{id}/transfer` — Transfer item to another list.
- **PUT** `/api/lists/{listId}/items/{id}/due-date` — Set due date.
- **DELETE** `/api/lists/{listId}/items/{id}/due-date` — Remove due date.
- **GET** `/api/items/due-dates` — List items with due dates for current account.
