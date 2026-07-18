# Finance

A personal self-hosted finance application for tracking accounts, income, expenses, recurring transactions, piggy banks (savings goals), and taxes.

## Tech stack

| Layer    | Stack                                                                            |
| -------- | -------------------------------------------------------------------------------- |
| Frontend | SvelteKit (Svelte 5), TypeScript, Tailwind CSS, shadcn-svelte                    |
| Backend  | C# .NET 10 WebApi, Dapper                                                        |
| Database | SQL Server, schema managed as a SQL Server Database Project (`backend/Database`) |

## Repository layout

```
backend/
  Core/          domain models, repositories, DTOs (shared library)
  WebApi/        REST API (controllers, request models)
  Database/      SQL Server Database Project — tables, stored procedures, views
  Provisioning/  console app that creates the DB and seeds default data
frontend/        SvelteKit web client
docker-compose.yml       local dev services (SQL Server)
docker-compose-prod.yml  full production stack (SQL Server, Azurite, WebApi, WebApp)
justfile                 shortcuts for the compose files above
```

See [`CLAUDE.md`](./CLAUDE.md) for a deeper architecture walkthrough (repository pattern, DI wiring, frontend state/service conventions, naming conventions).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/) and npm
- Docker (for local SQL Server via `docker-compose`)
- A way to deploy the `backend/Database` SQL project schema (Visual Studio with SQL Server Data Tools, or Azure Data Studio's schema compare) — required once per fresh database

## Getting started (local development)

1. **Configure environment variables**

   ```
   cp .env.example .env
   cp frontend/.env.example frontend/.env
   ```

   Set `MSSQL_SA_PASSWORD` in `.env` to a strong password (SQL Server refuses weak passwords and will keep restarting). `frontend/.env` should point `PUBLIC_API_BASE_URL` at the backend, e.g. `https://localhost:7247`.

2. **Start SQL Server**

   ```
   just dev
   ```

   This runs `docker-compose up -d` and starts a SQL Server container on `localhost:1433`.

3. **Create the database**

   ```
   dotnet run --project backend/Provisioning
   ```

   This creates the `Finance` database (connection settings in `backend/Provisioning/settings.json`) if it doesn't already exist.

4. **Deploy the schema (tables, stored procedures, views)**

   Open `backend/Finance.slnx` in Visual Studio, use SQL Server Object Explorer / Schema Compare to compare the `Database` project against the `Finance` database created in step 3, then publish/update. This step is manual and must be repeated whenever `backend/Database` changes.

5. **Seed default data**

   Run the provisioning app again to seed the default expense categories (defined in `backend/Provisioning/settings.json`) now that the tables exist:

   ```
   dotnet run --project backend/Provisioning
   ```

6. **Run the backend**

   ```
   dotnet run --project backend/WebApi
   ```

   Swagger UI is available at `https://localhost:7247/swagger/index.html`.

7. **Run the frontend**

   ```
   cd frontend
   npm install
   npm run dev
   ```

   The app is served at `http://localhost:5173`.

## Running in production

`docker-compose-prod.yml` builds and runs the full stack (SQL Server, Azurite, WebApi, WebApp) from the `backend/Dockerfile` and `frontend/Dockerfile`:

```
just prod
```

To rebuild the backend image manually:

```
docker build -t finance-webapi:latest ./backend
```

As with local development, the database schema (tables/stored procedures/views) must be deployed manually via schema compare after the `mssql` container is up and the database has been created.

## Frontend commands

Run from `frontend/`:

```
npm run dev       # start dev server
npm run build     # production build
npm run check     # type-check (svelte-check)
npm run lint      # prettier --check . && eslint .
npm run format    # prettier --write .
npm run test      # run unit tests once (vitest --run)
```

## Backend commands

Run from the repo root:

```
dotnet build backend/Finance.slnx
dotnet run --project backend/WebApi
dotnet run --project backend/Provisioning
```
