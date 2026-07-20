# Copilot Prompts for Finance App

Use these prompts when working in this repository to keep Copilot aligned with the backend/frontend architecture and naming conventions.

## Backend

- "Implement a new C# WebAPI endpoint to create a recurring action, including request validation, controller action, repository call, and the backing stored procedure in `backend/Database`."
- "Generate a `CreatePiggyBankRequest` model and the matching `PiggyBankController` POST action using the existing request pattern."
- "Add an `UpdateAsync` stored procedure and repository method for `Tax` following the pattern used by `ExpenseRepository`."
- "Add server-side validation for account name uniqueness and positive amounts in `CreateAccountRequest` and `CreateExpenseRequest`."

## Frontend

- "Create a SvelteKit component for adding a transaction, using the existing shadcn-svelte UI pattern and `appState`."
- "Implement a new `/transactions` route page that fetches expenses and incomes via their service wrappers and renders them in a shared data table."
- "Add filtered account selection to the dashboard using `appState` in `src/lib/states.svelte.ts`."
- "Create a frontend service wrapper (`recurringActionService`) for the backend `RecurringAction` API and use it in the recurring page."

## Fullstack

- "Add a backend API to return account balances and wire it to a frontend dashboard card that shows current totals."
- "Expose a new `GET /api/piggybanks` endpoint and update the frontend service/state so the piggy bank list renders automatically."
- "Update the backend `RecurringAction` model and then adjust the frontend create/edit form to use the same field names."

## Generic Templates

- "Generate a new feature for the finance app: [feature description]. Use the existing backend request/response models, stored procedures, and frontend component style."
- "Review the current implementation of [filename]. Suggest improvements, remove duplication, and keep the current C# or SvelteKit conventions."
- "Find and fix bugs in [filename]. Explain the root cause and provide the corrected code."

## Tips

- Include the target stack explicitly: `backend`, `frontend`, or `fullstack`.
- Mention existing patterns like `CreateXxxRequest`, `UpdateXxxRequest`, `SpNames`/`VwNames`, and `shadcn-svelte`.
- Use short, specific tasks for best results.
