# Overview
This project is designed as a portfolio artifact that showcases API design, domain modeling, and a layered architecture implemented in .NET 9.

## Domain Summary
- **Accounts:** The domain models account identity, activation state, and related events inside an aggregate root (`Account`).
- **To-do lists and items:** Lists are modeled as aggregates with item collections and domain rules for completion and item changes (`ToDoList`).

> Frontend status: not implemented; the current UI appears to be the default Next.js starter page and is not integrated with the API.

## Primary User Flows (API)
- **Authenticate and obtain JWT:** `POST /api/auth/login`.
- **Register and confirm account:** `POST /api/auth/register`, then `GET /api/auth/confirm-email`.
- **Manage lists:** create, read, update title/description, complete/incomplete, delete via `/api/lists` endpoints.
- **Manage items:** create/read/update/complete/incomplete/delete inside `/api/lists/{listId}/items` and related endpoints (transfer, due dates).

## Access Model
- List and item endpoints require authentication and rely on the current account identifier from JWT claims.

## Frontend Status
- Not implemented. The Next.js app appears to be the default starter page and is not integrated with the API; it likely does not function beyond the template UI.
