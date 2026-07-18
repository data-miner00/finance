# TODO

## Core Features

- [x] Transaction Tracking
  - Fields: amount, category, date/time, notes
  - [x] Add income & expenses
  - [x] Recurring transactions (salary, rent, subscriptions)
- [x] Categories & Tags
  - [x] Predefined categories (Food, Transport, Rent, Utilities, Entertainment)
  - [x] Custom categories
  - [x] Tags for flexibility (e.g. "trip", "work", "urgent")
- [x] Account Management
  - Account types: Cash, Bank, Credit card, E-wallets
  - [x] Multiple accounts
  - [x] Transfers between accounts
- [x] Dashboard / Overview
  - [x] Total balance
  - [x] Monthly income vs expenses
  - [x] Breakdown by category
- [x] Basic Reports
  - [x] Monthly summary
  - [x] Category pie chart
  - [x] Spending trends over time

## Advanced Features

- [ ] Budgeting System
  - [ ] Set monthly budgets per category
  - [ ] Track usage vs limit
  - [ ] Alerts when nearing/exceeding
- [ ] Recurring & Subscription Detection
  - [ ] Automatically detect known subscriptions (Netflix, Spotify, etc.)
  - [ ] Show "You spend RM X/month on subscriptions"
- [ ] Smart Insights — give meaning, not just data, e.g.:
  - "Your food spending increased 25% this month"
  - "You spend more on weekends"
  - "Top 3 categories this month"
- [ ] Credit card usage / annual spend tracking (e.g. track progress toward a spend target needed to waive an annual fee)

## Open Questions

- [ ] Decide whether to keep `Category` normalized as its own table, or denormalize the category value directly onto `Expense`

## Backlog

- [ ] Topups
- [ ] Recurrings
- [ ] Agent (trace who used the money on behalf of you)
- [ ] Upload files/image
- [ ] OAuth/OIDC/ApiKey
- [ ] Email Notification
- [ ] Event sourcing (for audit/history)
- [ ] Sync layer later (optional)
- [ ] Advanced filters, exports, reports
- [ ] Local encryption
- [ ] Quick add — e.g. "12 lunch" auto-parses into an expense
- [ ] Auto-categorization using rules — e.g. "Grab → Transport"
- [ ] Search
- [ ] Filter by date range / category / account
- [ ] Add example-style tags (e.g. "Travel 2026")
- [ ] Export: generate PDF report
