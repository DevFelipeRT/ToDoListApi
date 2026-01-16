# Common Tasks

## Explore the API with Swagger (Development)
Swagger UI is enabled in development builds and is registered in the HTTP pipeline when the app environment is Development.【F:src/Api/Program.cs†L170-L175】

## Typical API Actions
- **Register and login:** `POST /api/auth/register`, `POST /api/auth/login`.【F:src/Api/Identity/Controllers/AuthController.cs†L49-L129】
- **Confirm email:** `GET /api/auth/confirm-email`.【F:src/Api/Identity/Controllers/AuthController.cs†L131-L203】
- **Create and list lists:** `POST /api/lists`, `GET /api/lists`.【F:src/Api/ListManagement/Controllers/ListsController.cs†L32-L97】
- **Update list title/description:** `PUT /api/lists/{id}/title`, `PUT /api/lists/{id}/description`.【F:src/Api/ListManagement/Controllers/ListsController.cs†L132-L196】
- **Complete/incomplete/delete list:** `PUT /api/lists/{id}/complete`, `PUT /api/lists/{id}/incomplete`, `DELETE /api/lists/{id}`.【F:src/Api/ListManagement/Controllers/ListsController.cs†L198-L287】
- **Item lifecycle:** create/list/get/update title/complete/incomplete/delete under `/api/lists/{listId}/items`.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L38-L277】
- **Transfer items:** `PUT /api/lists/{listId}/items/{id}/transfer`.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L280-L318】
- **Manage due dates:** `PUT /api/lists/{listId}/items/{id}/due-date`, `DELETE /api/lists/{listId}/items/{id}/due-date`.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L320-L384】
- **Get items with due dates:** `GET /api/items/due-dates`.【F:src/Api/ListManagement/Controllers/ItemsController.cs†L386-L406】
