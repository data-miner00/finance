# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Summary

Personal self-hosted finance application with two independently deployed halves:

- `backend` — C# .NET 10 WebApi, backed by SQL Server via Dapper + stored procedures/views (not Entity Framework).
- `frontend` — SvelteKit (Svelte 5 runes) using shadcn-svelte for UI components.

Also in this repo:

- `backend/Database` — a SQL Server Database Project (`.sqlproj`) holding the actual schema (tables, stored procedures, views).
- `backend/Provisioning` — a standalone console app (Autofac DI) that provisions the database and seeds default data (e.g. expense categories) on a fresh environment. Entry point `backend/Provisioning/Program.cs`, config in `backend/Provisioning/settings.json`.
- `.github/copilot-agents/*.agent.md` and `.github/copilot-prompt.md` — Copilot agent/prompt docs for backend/frontend/fullstack work. Their domain-terms guidance (see below) still applies.

## Commands

### Local dev environment

```
just dev      # docker-compose up -d — starts SQL Server (mssql) for local dev
just prod     # docker-compose -f docker-compose-prod.yml up -d
```

`MSSQL_SA_PASSWORD` must be set in `.env` (see `.env.example`). After the DB container is up, provision the schema by running the `Provisioning` project (see `backend/README`/`README.md` for the SSMS schema-compare based flow), or run `dotnet run --project backend/Provisioning`.

### Backend (`backend/`, solution `Finance.slnx`)

```
dotnet build backend/Finance.slnx
dotnet run --project backend/WebApi
dotnet run --project backend/Provisioning   # provision DB schema + seed data
```

There are currently no backend test projects — don't assume a `dotnet test` target exists.

Backend connects to SQL Server using the `ConnectionStrings:SQLServer` setting in `backend/WebApi/appsettings.json` (dev password lives in-repo for local-only use — `sa`/`MyStrong!Password123` against `localhost,1433`).

### Frontend (`frontend/`)

```
npm run dev             # vite dev server
npm run build            # production build
npm run check             # svelte-kit sync + svelte-check (type checking)
npm run lint               # prettier --check . && eslint .
npm run format               # prettier --write .
npm run test                  # vitest --run (single run)
npm run test:unit                # vitest (watch mode)
```

To run a single frontend test file: `npx vitest run path/to/file.spec.ts`.

## Architecture

### Backend: Core / WebApi split

- `backend/Core` holds domain `Models` (plain entities, e.g. `Expense`, `Account`, `Income`, `PiggyBank`, `RecurringAction`, `Category`, `Tax` — all deriving from `Entity` with `Id`/`CreatedAt`/`UpdatedAt`), `Dtos` (internal Dapper row-mapping types with a `ToModel()` method converting DTO → domain model), and `Repositories`.
- `backend/WebApi` holds `Controllers` (thin, one per domain entity) and request `Models` (`CreateXxxRequest` / `UpdateXxxRequest`, distinct from `Core.Models` domain types).

### Repository pattern

- `IRepository<T>` (`backend/Core/Repositories/IRepository.cs`) defines the standard CRUD contract (`GetAllAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteByIdAsync`).
- Concrete repositories (`ExpenseRepository`, `AccountRepository`, `IncomeRepository`, etc.) implement it against SQL Server using Dapper, calling stored procedures for writes and views/raw SQL for reads. Stored procedure names are centralized in `backend/Core/StoredProcedureNames.cs` (aliased as `SpNames` via a global `Using`), and view names in `backend/Core/ViewNames.cs` (aliased `VwNames`). When adding a new stored procedure/view, register its name there rather than hardcoding strings in a repository.
- `MemoryRepository<T>` still exists as a generic in-memory `IRepository<T>` implementation but is **not** wired up in `Program.cs` anymore — DI now registers the concrete SQL-backed repositories directly as singletons holding a shared `IDbConnection`. Don't assume new repositories should be in-memory.
- Some repositories (e.g. `ExpenseRepository`) expose extra methods beyond `IRepository<T>` (like `ImportAsync`); controllers that need these depend on the concrete repository type instead of the interface.
- SQL schema source of truth is `backend/Database` (`.sqlproj`) — tables, stored procedures, and views live there as individual `.sql` files under `dbo/`. Changing a repository's SQL shape usually means adding/editing a stored procedure or view file there too.

### Backend wiring

All DI registration happens in `backend/WebApi/Program.cs` (`ConfigureServices`/`ConfigureCors` extension methods) — no separate startup class. CORS policy (`FinanceCorsPolicy`) is configured from the `Cors` section of `appsettings.json`; when adding a new frontend origin, update `AllowedOrigins` there.

### Frontend structure

- `src/routes/(app)/*` — authenticated app pages (dashboard, `account`, `expense`, `income`, `piggy-bank`, `recurring`, `tax`, `settings`), each typically with a `+page.svelte` plus a `data-table/` subfolder (`column.ts` for TanStack column defs, `data-table-actions.svelte` for row actions) and sometimes a `charts/` subfolder.
- `src/routes/(auth)/*` — unauthenticated routes (`login`).
- `src/lib/services/*Service.ts` — one file per backend domain entity (`expenseService`, `accountService`, `incomeService`, `piggyBankService`, `recurringActionService`, `taxService`), all built on the shared low-level helpers in `src/lib/services/api.ts` (`apiGet`/`apiPost`/`apiPut`/`apiDelete`/`apiDownloadFile`/`apiUploadFile`). New backend integrations should follow this same thin-wrapper pattern rather than calling `fetch` directly from components.
- `src/lib/states.svelte.ts` — single global `appState` object (Svelte 5 `$state` rune) holding all shared app data (accounts, expenses, incomes, recurring actions, piggy banks, taxes, UI flags, settings). This is the app's de facto client-side store; there's no separate Redux/Zustand-style state layer.
- `src/lib/components/ui/*` — shadcn-svelte generated primitives; treat as vendored, prefer composing over hand-editing.
- `src/lib/components/custom/*` and other top-level `src/lib/components/*.svelte` — app-specific components (dialogs, nav, charts, data tables).
- API base URL comes from the `PUBLIC_API_BASE_URL` env var (`$env/static/public`), combined with `/api` in `src/lib/services/api.ts`.

### Cross-cutting naming conventions

- Keep domain terms consistent across backend and frontend: `Account`, `Expense`, `Income`, `RecurringAction`, `PiggyBank`, `Category`, `Tax`.
- Backend request models follow `CreateXxxRequest` / `UpdateXxxRequest`; keep frontend `types.ts` request/response shapes in sync with these (matching JSON property names).
- When adding a full-stack feature, backend and frontend changes are expected to land together: controller + repository (+ stored procedure/view if needed) on the backend, service wrapper + state + UI on the frontend.
