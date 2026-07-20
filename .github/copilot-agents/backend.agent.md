# Finance Backend Agent

This agent is focused on the finance app backend.

## Purpose

- Work with `backend/Core` and `backend/WebApi` (solution `Finance.slnx`).
- Apply .NET 10 WebAPI patterns.
- Respect the SQL Server-backed repository architecture (Dapper + stored procedures/views, not Entity Framework).

## Domain

- Accounts
- Expenses
- Income
- Piggy Banks
- Recurring Actions
- Categories
- Taxes

## Guidance

- Use request models named `CreateXxxRequest` and `UpdateXxxRequest`, defined in `backend/WebApi/Models` (distinct from `backend/Core/Models` domain entities).
- Keep controllers thin — one per domain entity in `backend/WebApi/Controllers`.
- Repositories implement `IRepository<T>` and use Dapper against SQL Server: stored procedures for writes, views/raw SQL for reads.
- Stored procedure names are centralized in `backend/Core/StoredProcedureNames.cs` (`SpNames`); view names in `backend/Core/ViewNames.cs` (`VwNames`). Register new names there instead of hardcoding strings in a repository.
- SQL schema source of truth is `backend/Database` (`.sqlproj`) — adding/changing a repository's SQL shape usually means adding/editing a stored procedure or view file there too.
- `MemoryRepository<T>` still exists as a generic in-memory `IRepository<T>` implementation but is **not** wired up in `Program.cs` — don't assume new repositories should be in-memory.
- Use dependency injection already defined in `backend/WebApi/Program.cs` (`ConfigureServices`/`ConfigureCors`); repositories are registered as singletons holding a shared `IDbConnection`.
- Some repositories expose extra methods beyond `IRepository<T>` (e.g. `ExpenseRepository.ImportAsync`); controllers needing these depend on the concrete repository type instead of the interface.
- Favor clear validation and error handling in request models and controller actions.
- There are currently no backend test projects — don't assume a `dotnet test` target exists.

## When to use this agent

- Adding or updating API endpoints
- Designing backend data models, DTOs, stored procedures, or views
- Refactoring repository logic
- Writing controller actions that consume request models
- Changes to the `backend/Database` schema or `backend/Provisioning` seed data
