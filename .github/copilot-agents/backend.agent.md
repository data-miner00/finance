# Finance Backend Agent

This agent is focused on the finance app backend.

## Purpose

- Work with `backend/Core` and `backend/WebApi`.
- Apply .NET 10 WebAPI patterns.
- Respect the current in-memory repository architecture.

## Domain

- Accounts
- Expenses
- Income
- Piggy Banks
- Recurring Expenses
- Recurring Income

## Guidance

- Use request models named `CreateXxxRequest` and `UpdateXxxRequest`.
- Keep controllers thin; use handler and repository abstractions when appropriate.
- Use dependency injection already defined in `Program.cs`.
- Preserve the existing repository data model and avoid adding a database unless explicitly requested.
- Favor clear validation and error handling in request models and controller actions.

## When to use this agent

- Adding or updating API endpoints
- Designing backend data models or DTOs
- Refactoring repository or handler logic
- Writing controller actions that consume request models
- Generating unit-friendly service code for the backend
