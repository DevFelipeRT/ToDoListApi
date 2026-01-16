# Troubleshooting

## JWT SecretKey not configured
**Symptom:** Startup fails with `InvalidOperationException: JWT SecretKey not configured.`

**Cause:** `Jwt:SecretKey` is required and missing from configuration.【F:src/Api/Program.cs†L62-L64】【F:src/Api/appsettings.json†L2-L7】

**Fix:** Provide a non-empty `Jwt:SecretKey` value in configuration.

## Database connection issues
**Symptom:** API cannot connect to SQL Server.

**Cause:** The connection string uses environment-variable placeholders for host, database, user, and password.【F:src/Api/appsettings.json†L8-L10】

**Fix:** Export `DB_HOST`, `DB_DATABASE`, `DB_USER`, and `DB_PASSWORD` so the placeholders resolve.

## Invalid activation link or token
**Symptom:** Email confirmation fails with `Invalid activation link` or `Invalid token format`.

**Cause:** Token or UID is missing or malformed; the API expects URL-safe Base64 token and a protected UID.【F:src/Api/Identity/Controllers/AuthController.cs†L139-L176】

**Fix:** Regenerate the activation link from a valid registration flow and retry.
