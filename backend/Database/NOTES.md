# Notes on `SELECT *` vs. explicit columns

The repositories in `backend/Core/Repositories` were switched from `SELECT *` to
explicit column lists (see commit history). This is a correctness/resilience
change, not a performance one — the reasoning is captured here since it's easy
to assume the opposite.

## Why it mostly doesn't help performance, here

SQL Server (outside of columnstore indexes, which this schema doesn't use) is a
**row store**: all of a row's columns live together on the same 8KB page. A
`SELECT` that touches a row — whether it asks for 2 columns or all of them —
already has to pull that page into the buffer pool. Trimming the column list
doesn't reduce how many pages are read or how many rows are scanned; it only
changes how much of the already-fetched row gets copied into the result set
and shipped to the client. For narrow tables like these (5-10 columns, no
LOB data actually being read), that copy/serialize/deserialize cost is
negligible.

Two more reasons the win is close to zero in *this* codebase specifically:

- **The view-backed queries were already narrow.** `vw_GetAllExpenses`,
  `vw_GetAllIncomes`, and `vw_CategoryMonthlySpend` each define their own
  column list in the `CREATE VIEW`. `SELECT * FROM vw_GetAllExpenses` was
  never pulling extra data from `Expenses`/`Categories`/`Accounts` — it was
  already limited to whatever the view projects. Naming the same columns
  explicitly in the repository query changes nothing about what's read.
- **The Dtos already mapped nearly every column.** Most tables here
  (`Categories` being the one exception — see below) only have the columns
  their Dto needs, so `SELECT *` and the explicit list return the same set of
  columns anyway.

## Where explicit columns genuinely would pay off (not the case yet here)

- **Covering indexes.** If a nonclustered index includes exactly the columns
  a query needs, SQL Server can satisfy that query entirely from the index
  (an "index-only" scan/seek) without a key/RID lookup back to the clustered
  index. `SELECT *` can never benefit from this, because it always needs
  every column, which forces the lookup. This schema currently has no
  nonclustered indexes beyond primary keys and the odd `UNIQUE` constraint,
  so this doesn't apply today — but it's the main reason to keep queries
  narrow *before* adding indexes for filtering/sorting. See "next step" below.
- **Wide tables or LOB columns.** If a table carries `NVARCHAR(MAX)`,
  `VARBINARY(MAX)`, or similar large columns that a given query doesn't need,
  `SELECT *` forces reading (and potentially off-row page fetches for) that
  data. `Settings.Value` is `NVARCHAR(MAX)` here, but it's always used, so
  there's no saving. If `Expense.ReceiptImage` ever grows into an actual
  binary blob column instead of a path/string, excluding it from list views
  would matter — it isn't in `ExpenseDto` today, and the view already leaves
  it out.
- **Genuinely wide tables** (many rarely-used columns: audit trails, flags,
  JSON blobs) where a query only needs a handful. None of the tables in this
  schema are wide enough for this to show up.
- **Network hops over a slow link.** All of the above compounds if the app
  and database aren't co-located. For a personal, self-hosted, single-user
  app on a LAN/loopback, this isn't a factor.

## The one real (tiny) exception found

`Categories` has an `IsSystemDefault BIT` column that `CategoryDto` never
mapped. `SELECT *` was pulling that column across the wire for nothing;
the explicit list drops it. One boolean column — not a meaningful saving,
but it's the only place in this pass where the old query was doing
strictly more work than necessary.

## What the change actually buys

- **Schema-change safety.** A column added, renamed, or reordered later no
  longer silently changes what flows into a Dto's mapping, or breaks it in a
  way that's hard to trace back to the migration that caused it. An explicit
  list either keeps working exactly as before or fails loudly at the query
  (missing/renamed column), instead of silently mismapping.
- **Readability/intent.** The query documents exactly what the Dto expects,
  instead of "whatever the table happens to have right now."

## A real next step, if performance is the goal

The stronger lever for this schema is indexing, not column lists. The
budget-alert aggregation added for the notification feature
(`vw_CategoryMonthlySpend`, backed by `CategoryRepository.GetMonthlySpendAsync`)
sums `Expenses.Amount` grouped by `CategoryId` for the current month, scanning
the full `Expenses` table every time an expense is created or updated. As
`Expenses` grows, an index such as:

```sql
CREATE NONCLUSTERED INDEX [IX_Expenses_CategoryId_ActionedAt]
ON [dbo].[Expenses] ([CategoryId], [ActionedAt])
INCLUDE ([Amount]);
```

would let that aggregation (and the `ORDER BY ActionedAt DESC` reads in
`ExpenseRepository.GetAllAsync`) run as an index seek/scan instead of a full
table scan — a real, measurable difference once the table isn't tiny anymore.
This is the point at which explicit column lists and covering indexes start
working together: the aggregation query only needs `CategoryId`, `ActionedAt`,
and `Amount`, and an index covering exactly those three columns would let SQL
Server skip the base table entirely.
