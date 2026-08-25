# `[Range(double, double)]` on a `decimal` property risks an overflow at validation time

## Context

While adding request validation for `RecurringAction` (guarding `IntervalValue`,
`DayOfMonth`, and `Amount` on `CreateRecurringActionRequest` /
`UpdateRecurringActionRequest` in `backend/WebApi/Models/`), the natural first
attempt for `Amount` was:

```csharp
[Range(0.01, double.MaxValue)]
public decimal Amount { get; set; }
```

## The problem

Two separate issues collide here:

- **`decimal` isn't a valid attribute argument type.** C# attribute constructor
  arguments must be compile-time constants of a fixed set of types (`bool`, numeric
  primitives, `string`, `enum`, etc.). `decimal`, despite being a compile-time
  constant expression in ordinary code, is not on that list — its bit layout isn't
  one CLR metadata can encode directly as an attribute argument. So `RangeAttribute`
  only exposes numeric bounds as `int`/`double`, plus a separate
  `Range(Type type, string minimum, string maximum)` overload that parses the bounds
  into the target type at runtime instead.
- **`double.MaxValue` doesn't fit in a `decimal`.** `double.MaxValue` is
  ~1.8×10³⁰⁸; `decimal.MaxValue` is ~7.9×10²⁸ — vastly smaller. When
  `RangeAttribute` validates a `decimal` property against `double` bounds, it
  converts the bound into the property's type for comparison. Converting
  `double.MaxValue` into `decimal` overflows, throwing an exception during
  validation instead of returning a clean `400`.

## Fix

Use the string-based overload with `decimal.MaxValue` (`79228162514264337593543950335`,
i.e. `2^96 - 1`) written out as a literal string, so the bound is parsed straight into
`decimal` with no `double` conversion in the middle:

```csharp
[Range(typeof(decimal), "0.01", "79228162514264337593543950335",
    ErrorMessage = "Amount must be greater than 0.")]
public decimal Amount { get; set; }
```

Applied in `backend/WebApi/Models/CreateRecurringActionRequest.cs` and
`UpdateRecurringActionRequest.cs`.

## Takeaway

Never use the `Range(double, double)` overload on a `decimal` property. Use
`Range(typeof(decimal), "min", "max")` with string bounds, and use
`decimal.MaxValue`'s literal digits (not `double.MaxValue`) as the upper bound when
you just mean "no real upper limit." No other `Amount`-style `decimal` field in this
codebase (`CreateExpenseRequest`, `CreateIncomeRequest`, `CreateTaxRequest`,
`CreatePiggyBankRequest`, etc.) has `[Range]` validation yet — apply the same pattern
if/when that's added.
