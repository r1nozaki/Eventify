# Eventify Backend (ASP.NET Core 8)

Production-ready Eventify API with JWT auth, refresh tokens, role-based access, EF Core/PostgreSQL, Docker, Serilog logging, validation, and health checks.

## Stack

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core + PostgreSQL
- JWT access tokens + refresh token rotation
- FluentValidation
- Serilog
- Docker + docker-compose

## Run with Docker

From `backend` directory:

```bash
docker compose up --build
```

API base URL:

- `http://localhost:8080`

Services:

- API container: `eventify-api`
- PostgreSQL container: `eventify-postgres`

## Run locally (without Docker)

1. Ensure PostgreSQL is running and connection string is valid in `appsettings.json`.
2. From `backend/EventApp`:

```bash
dotnet restore
dotnet build
dotnet run
```

## Database migrations

Create migration:

```bash
dotnet tool run dotnet-ef migrations add <MigrationName> --output-dir Data/Migrations
```

Apply migration:

```bash
dotnet tool run dotnet-ef database update
```

## Auth flow

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/revoke`

Refresh token support:

- Returned in response body.
- Also set as HttpOnly cookie: `refreshToken`.
- `refresh` and `revoke` endpoints accept token from body or cookie.

## Health checks

- `GET /health`
- `GET /health/ready`

## Rate limiting

Auth endpoints are protected by fixed-window rate limiting policy:

- Policy: `AuthPolicy`
- Limit: 10 requests / minute / IP

## Postman

Import collection file:

- `Eventify.postman_collection.json`

## Notes

- Swagger UI enabled in Development mode.
- CORS origins configured in `appsettings.json` under `Cors:AllowedOrigins`.
- Seed data inserts initial events on first run.
