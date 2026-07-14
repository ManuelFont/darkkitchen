# DarkKitchen

DarkKitchen is an Angular frontend backed by an ASP.NET Core 8 Web API and SQL Server.

## Quick start with Docker

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

These credentials are for local/demo use only. Change or remove seeded credentials before deploying to a shared or production environment.

## Local development without Docker

### Backend

Prerequisites:

- .NET SDK 8.0
- SQL Server compatible with the configured SQL Server provider

Create a development configuration from the template:

```bash
cp src/backend/DarkKitchen.WebApi/appsettings.Development.example.json \
   src/backend/DarkKitchen.WebApi/appsettings.Development.json
```

Edit the copied file with the connection string for your local SQL Server. The application applies pending migrations when it starts.

Run the API from the backend directory:

```bash
cd src/backend
dotnet run --project DarkKitchen.WebApi/DarkKitchen.WebApi.csproj
```

The API listens on <http://localhost:5222> with the default development launch profile.

### Frontend

Prerequisites:

- Node.js 22 or a compatible current LTS release
- npm

Install dependencies and start Angular:

```bash
cd src/frontend/darkkitchen
npm ci
npm start
```

Open <http://localhost:4200>. The development frontend is configured to call the API at `http://localhost:5222`.

## Tests

Run backend tests:

```bash
cd src/backend
dotnet test
```

Run frontend tests:

```bash
cd src/frontend/darkkitchen
npm test
```

## Configuration

Docker Compose reads values from `.env`. Available values include:

- `MSSQL_SA_PASSWORD`: SQL Server administrator password; it must satisfy SQL Server password complexity rules.
- `MSSQL_DATABASE`: database name.
- `FRONTEND_PORT`: host port for the frontend.
- `BACKEND_PORT`: host port for the backend.
- `MSSQL_PORT`: host port for SQL Server.

Do not commit `.env` or `appsettings.Development.json`; both may contain local secrets.
