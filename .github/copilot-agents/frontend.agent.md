# Finance Frontend Agent

This agent is focused on the finance app frontend.

## Purpose

- Work with `frontend/src`.
- Use SvelteKit (Svelte 5 runes) and `shadcn-svelte` UI patterns.
- Respect the app's component and route structure.

## Domain

- Accounts, Expenses, Income, Recurring Actions, Piggy Banks, Categories, Taxes
- Dashboard and charts
- Tables and lists (TanStack table via `data-table/` subfolders)
- Global state and services
- API integration with backend endpoints

## Guidance

- Routes live under `src/routes/(app)/*` (authenticated: dashboard, `account`, `expense`, `income`, `piggy-bank`, `recurring`, `tax`, `settings`) and `src/routes/(auth)/*` (unauthenticated: `login`). Each app route typically has a `+page.svelte`, plus `data-table/` (`column.ts`, `data-table-actions.svelte`) and sometimes `charts/`.
- Never call `fetch` directly from components. Use `src/lib/services/*Service.ts` (one file per backend domain entity, e.g. `expenseService`, `accountService`), all built on the shared helpers in `src/lib/services/api.ts` (`apiGet`/`apiPost`/`apiPut`/`apiDelete`/`apiDownloadFile`/`apiUploadFile`). New backend integrations should follow this thin-wrapper pattern.
- API base URL comes from the `PUBLIC_API_BASE_URL` env var (`$env/static/public`), combined with `/api` in `api.ts` — don't hardcode URLs in services or components.
- Use `src/lib/states.svelte.ts` (`appState`, a Svelte 5 `$state` rune) for shared application state — there's no separate Redux/Zustand-style store layer.
- `src/lib/components/ui/*` are shadcn-svelte generated primitives — treat as vendored, prefer composing over hand-editing.
- App-specific components live in `src/lib/components/custom/*` and other top-level `src/lib/components/*.svelte`.
- Keep frontend `types.ts` request/response shapes in sync with backend `CreateXxxRequest`/`UpdateXxxRequest` models (matching JSON property names).
- Respect client-side validation for forms and handle API errors gracefully.

## When to use this agent

- Building or updating UI components
- Creating new SvelteKit routes
- Integrating backend API calls into the frontend via service wrappers
- Refactoring frontend state management (`appState`)
- Adding responsive design and accessibility improvements
