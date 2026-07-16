# 🍽️ DarkKitchen

DarkKitchen is an Angular frontend backed by an ASP.NET Core Web API and SQL Server.

## Setup with Docker

### Prerequisites

- Docker Desktop with Docker Compose
- Ports `8080`, `5222`, and `1433` available, or configure different ports in `.env`

### Start the application

From the repository root:

```bash
cp .env.example .env
docker compose up --build
```

Once the containers are ready, open:

- Frontend: <http://localhost:8080>
- Backend: <http://localhost:5222>

The backend automatically applies the committed Entity Framework migrations when it starts. The SQL Server data is stored in the `sqlserver-data` Docker volume.

Stop the application with `Ctrl+C`, or run:

```bash
docker compose down
```

To remove the database and recreate the seeded data on the next start:

```bash
docker compose down -v
docker compose up --build
```

### Default seeded users

The database is seeded with these accounts:

| Role | Email | Password |
| --- | --- | --- |
| Administrator | `admin@darkkitchen.com` | `D@rkK!tchen#2026` |
| Dispatcher | `dispatcher@darkkitchen.com` | `D@rkK!tchen#2026` |
| Customer | `customer@darkkitchen.com` | `D@rkK!tchen#2026` |

## Configuration

Docker Compose reads values from `.env`. Available values include:

- `MSSQL_SA_PASSWORD`: SQL Server administrator password; it must satisfy SQL Server password complexity rules.
- `MSSQL_DATABASE`: database name.
- `FRONTEND_PORT`: host port for the frontend.
- `BACKEND_PORT`: host port for the backend.
- `MSSQL_PORT`: host port for SQL Server.
