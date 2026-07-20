## Summary

This is a personal self-hosted finance application.

- `backend` is a C# .NET 10 WebApi server backed by SQL Server via Dapper + stored procedures/views (not Entity Framework).
- `frontend` is a web client written in SvelteKit (Svelte 5 runes) using [shadcn-svelte](https://shadcn-svelte.com/llms.txt) for UI components.
- `backend/Database` is a SQL Server Database Project (`.sqlproj`) — the source of truth for tables, stored procedures, and views.
- `backend/Provisioning` is a standalone console app that provisions the database schema and seeds default data on a fresh environment.

Also available in this repository:

- `.github/copilot-prompt.md` — reusable repo-aware prompt templates.
- `.github/copilot-agents/backend.agent.md` — backend-focused Copilot agent.
- `.github/copilot-agents/frontend.agent.md` — frontend-focused Copilot agent.
- `.github/copilot-agents/fullstack.agent.md` — combined backend/frontend workflow agent.

See `CLAUDE.md` at the repo root for the full architecture reference (commands, repository pattern, frontend structure, naming conventions) — keep these Copilot docs consistent with it.
