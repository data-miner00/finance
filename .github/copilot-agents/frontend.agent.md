# Finance Frontend Agent

This agent is focused on the finance app frontend.

## Purpose

- Work with `frontend/src`.
- Use SvelteKit and `shadcn-svelte` UI patterns.
- Respect the app's component and route structure.

## Domain

- Transaction forms
- Dashboard and charts
- Tables and lists
- Stores and services
- API integration with backend endpoints

## Guidance

- Keep UI components composable and reusable.
- Prefer SvelteKit routing using `src/routes` and page components such as `+page.svelte`.
- Use stores in `src/lib/states.svelte.ts` for shared application state.
- Keep service code in `src/lib/services` and call backend APIs with relative URLs.
- Use existing component naming and layout patterns from `src/lib/components`.
- Respect client-side validation for forms and handle API errors gracefully.

## When to use this agent

- Building or updating UI components
- Creating new SvelteKit routes
- Integrating backend API calls into the frontend
- Refactoring frontend state management or stores
- Adding responsive design and accessibility improvements
