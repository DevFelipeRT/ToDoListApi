# Local Setup

## Prerequisites
- .NET 9 SDK (API projects target `net9.0`).【F:src/Api/Api.csproj†L3-L8】【F:src/Application/Application.csproj†L11-L16】【F:src/Infrastructure/Infrastructure.csproj†L15-L20】
- Node.js + npm (frontend scripts defined in `app/package.json`).【F:app/package.json†L5-L10】

## API Configuration
The API relies on configuration values in `src/Api/appsettings.json` and environment variables referenced by those settings:

- **JWT settings** (must be provided): `Jwt:SecretKey`, `Issuer`, `Audience`, `ExpirationMinutes`.【F:src/Api/appsettings.json†L2-L7】
- **SQL Server connection string** uses environment variable placeholders: `DB_HOST`, `DB_DATABASE`, `DB_USER`, `DB_PASSWORD`.【F:src/Api/appsettings.json†L8-L10】
- **Data Protection key ring:** `DP:APP_NAME` and `DP:KEY_RING_PATH` are read at startup with defaults if missing.【F:src/Api/Program.cs†L84-L93】
- **Email outbox:** `EMAIL_OUTBOX_DIR` and `EMAIL_DEFAULT_FROM` configure a disk-based email sender (defaulting to `/data/outbox` and `no-reply@todoapp.local`).【F:src/Infrastructure/DependencyInjection.cs†L90-L94】

## Frontend (Next.js, not implemented)
From the `app/` directory:

```bash
npm run dev
```

This runs the Next.js starter template (not integrated with the API and likely not functional beyond the default page). Status: not implemented.【F:app/app/page.tsx†L1-L103】【F:app/README.md†L1-L36】

## Docker Compose (Recommended for Full Stack)
`docker-compose.yml` defines a development environment with SQL Server, the .NET API (hot reload), and the Next.js frontend (dev server). It uses bind mounts for source code and local data directories for persistence.【F:docker-compose.yml†L1-L103】

### Required Environment Variables
Before running Docker, set these values (they are referenced by the compose file and the API configuration):【F:docker-compose.yml†L24-L61】【F:src/Api/appsettings.json†L2-L10】
- `DB_PASSWORD` (SQL Server `sa` password)
- `JWT_SECRET` (JWT signing key for the API)

### Run
```bash
docker compose up --build
```

### Notes
- SQL Server data is persisted under `./volume_data/sqlserver` and email outbox files under `./volume_data/outbox` as bind mounts.【F:docker-compose.yml†L11-L52】【F:docker-compose.yml†L66-L83】
