# Common Tasks

## Explore the API with Swagger (Development)
Swagger UI is enabled in development builds and is registered in the HTTP pipeline when the app environment is Development.

## Typical API Actions
- **Register and login:** `POST /api/auth/register`, `POST /api/auth/login`.
- **Confirm email:** `GET /api/auth/confirm-email`.
- **Create and list lists:** `POST /api/lists`, `GET /api/lists`.
- **Update list title/description:** `PUT /api/lists/{id}/title`, `PUT /api/lists/{id}/description`.
- **Complete/incomplete/delete list:** `PUT /api/lists/{id}/complete`, `PUT /api/lists/{id}/incomplete`, `DELETE /api/lists/{id}`.
- **Item lifecycle:** create/list/get/update title/complete/incomplete/delete under `/api/lists/{listId}/items`.
- **Transfer items:** `PUT /api/lists/{listId}/items/{id}/transfer`.
- **Manage due dates:** `PUT /api/lists/{listId}/items/{id}/due-date`, `DELETE /api/lists/{listId}/items/{id}/due-date`.
- **Get items with due dates:** `GET /api/items/due-dates`.
