# Shared `SqlConnection` singleton caused a startup race condition

## Context

While implementing `RecurrentActionService` (a second `BackgroundService`, alongside
`DummyService`), the app started crashing on launch with an `AggregateException` /
`InvalidCastException` / `TaskCanceledException` coming out of Dapper calls in
`ServiceMetadataRepository`.

## Root cause

`Program.cs` registered a single `SqlConnection` instance as an `IDbConnection`
singleton, shared by every repository in the app:

```csharp
var connection = new SqlConnection(builder.Configuration.GetConnectionString("SQLServer"));
builder.Services.AddSingleton<IDbConnection>(connection);
```

This worked while only one background service (`DummyService`) touched the DB. Once
`RecurrentActionService` also ran as a hosted service, both loops called the DB at
nearly the same instant on startup — two threads calling `OpenAsync`/`ExecuteAsync` on
the *same* `SqlConnection` object concurrently. `SqlConnection` is not thread-safe for
concurrent operations on one instance; it has internal mutable state (connection state
machine, current command/transaction) that gets corrupted by concurrent access. The
same race is equally possible between a controller request and a background service —
this wasn't unique to the two hosted services, they just made it show up immediately.

## The misconception

It's easy to conflate two different things under "share the connection":

- **Reusing a connection string / pool — correct, and already automatic.** ADO.NET
  pools physical TCP connections under the hood, keyed by connection string.
  `new SqlConnection(connStr)` + dispose doesn't actually open/close a socket each
  time; disposal returns the physical connection to the pool for the next caller.
  Creating many short-lived `SqlConnection` *objects* is cheap and is exactly what the
  pool is designed for (Microsoft's own guidance: "open late, close early").
- **Reusing one connection *object* instance across concurrent callers — the
  anti-pattern.** That's what the original singleton registration did.

This differs from clients like `HttpClient` or a Redis multiplexer, which *are*
designed to be long-lived singletons because they manage concurrency internally.
`SqlConnection` isn't built that way.

## Fix

Replaced the singleton connection with a factory, so each repository call opens (and
disposes) its own connection, relying on ADO.NET pooling underneath:

- `Core/Repositories/IDbConnectionFactory.cs` + `SqlConnectionFactory.cs` — new,
  `CreateConnection()` returns a fresh `SqlConnection`.
- All 10 repositories (`Account`, `Category`, `PiggyBank`, `Tax`, `Profile`, `Expense`,
  `Income`, `Settings`, `ServiceMetadata`, `RecurringAction`) now take
  `IDbConnectionFactory` and do `using var connection = this.connectionFactory.CreateConnection();`
  per call (or once per logical unit of work where a method makes multiple related
  calls, e.g. `ProfileRepository.SaveAsync`'s read-then-write, `ExpenseRepository.ImportAsync`'s
  batch loop).
- `WebApi/Program.cs` — registers `IDbConnectionFactory` instead of a singleton
  `IDbConnection`.
- `Provisioning/ContainerConfig.cs` — the console provisioning tool shares the same
  `Core.Repositories` classes, so it needed the same factory registration. The raw
  `SqlConnection` singleton used by `ProvisionDatabaseActivity` for one-off DDL
  (`CREATE DATABASE`) was left as-is — that's a single-threaded, one-shot console flow
  with no concurrent access, so it doesn't have this failure mode.

## Takeaway

Don't register a `SqlConnection` (or any ADO.NET `IDbConnection`) as a DI singleton in
an app with concurrent access (web requests, multiple background services, etc.).
Register a factory/connection-string instead, and open a connection per unit of work.
The pooling that makes this cheap is already there — you don't need to hand-roll it by
keeping one connection open for the app's lifetime.
