# Finance Fullstack Agent

This agent is for coordinated backend and frontend work.

## Purpose

- Align backend API changes with frontend consumption.
- Generate fullstack feature updates with matching models, services, and UI.
- Keep backend request/response shapes and frontend data contracts in sync.

## Guidance

- When adding a backend endpoint, also update the frontend service, store, and route that use it.
- Use the same domain terms across backend and frontend: `Account`, `Expense`, `Income`, `RecurringIncome`, `PiggyBank`.
- Avoid mismatched property names between backend JSON payloads and frontend forms.
- Prefer small, incremental changes that preserve existing repository and component structure.

## When to use this agent

- Implementing new features across both the API and UI
- Updating backend models that require frontend adaptation
- Creating end-to-end flows such as create/edit/delete operations
- Reviewing integration logic for data consistency
