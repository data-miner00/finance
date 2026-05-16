# Copilot Prompts for Finance App

Use these prompts when working in this repository to keep Copilot aligned with the backend/frontend architecture and naming conventions.

## Backend

- "Implement a new C# WebAPI endpoint to create a recurring expense, including request validation, controller action, handler call, and in-memory repository storage."
- "Generate a `CreatePiggyBankRequest` model and the matching `PiggyBankController` POST action using the existing request pattern."
- "Refactor the `MemoryRepository` to support update logic for `Income` and `Expense` entities while preserving in-memory semantics."
- "Add server-side validation for account name uniqueness and positive amounts in `CreateAccountRequest` and `CreateExpenseRequest`."

## Frontend

- "Create a SvelteKit component for adding a transaction, using the existing shadcn-svelte UI pattern and state management."
- "Implement a new `/transactions` route page that fetches expenses and incomes from the backend and renders them in a shared table component."
- "Add a writable store for selected account filters and update the UI to use this store in the dashboard."
- "Create a frontend service wrapper for the backend `RecurringIncome` API and use it in the budget page."

## Fullstack

- "Add a backend API to return account balances and wire it to a frontend dashboard card that shows current totals."
- "Expose a new `GET /api/piggybanks` endpoint and update the frontend service/store so the piggy bank list renders automatically."
- "Update the backend recurring income model and then adjust the frontend create/edit form to use the same field names."

## Generic Templates

- "Generate a new feature for the finance app: [feature description]. Use the existing backend request/response models and frontend component style."
- "Review the current implementation of [filename]. Suggest improvements, remove duplication, and keep the current C# or SvelteKit conventions."
- "Find and fix bugs in [filename]. Explain the root cause and provide the corrected code."

## Tips

- Include the target stack explicitly: `backend`, `frontend`, or `fullstack`.
- Mention existing patterns like `CreateXxxRequest`, `UpdateXxxRequest`, `MemoryRepository`, and `shadcn-svelte`.
- Use short, specific tasks for best results.
