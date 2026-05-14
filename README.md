# YourLibrary

**Explicit, composable control flow for C#**

[![NuGet](https://img.shields.io/nuget/v/YourLibrary)](https://www.nuget.org/packages/YourLibrary)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

## What is this library?

YourLibrary brings **functional control‑flow patterns** to C#. It provides lightweight, immutable structs like `Option<T>` and `Result<T, TError>` that let you model absence, errors, and branching logic explicitly in your type signatures – without exceptions, nulls, or hidden side‑effects.

## What is it for?

- **Replace `null`** with `Option<T>` – no more `NullReferenceException`.
- **Replace exceptions** with `Result<T, TError>` – every failure is visible in the return type.
- **Chain operations** safely with `Map`, `Bind`, and LINQ query syntax.
- **Force exhaustive handling** using `Match` – you can’t forget to handle the `None` or `Error` case.

## Who is it for?

- C# developers who want **more reliable, self‑documenting code**.
- Teams adopting **functional programming** techniques in a pragmatic way.
- Anyone tired of unexpected nulls or hidden exception paths.

## Types & Features

| Type | Purpose | Key methods |
|------|---------|--------------|
| `Option<T>` | A value that may be present (`Some`) or absent (`None`). | `Map`, `Bind`, `Match`, `ValueOr`, LINQ support |
| `Result<T, TError>` | A value that is either successful (`Ok`) or carries an error (`Error`). | `Map`, `Bind`, `MapError`, `Match`, LINQ support |

**Core features:**
- Immutable, `readonly struct` – zero heap allocation in hot paths.
- LINQ query comprehension (`from...select...`).
- Exhaustive pattern matching with `Match`.
- Async extensions (optional – available in separate package).

## Examples

### Using `Option<T>` – avoid null

```csharp
Option<User> FindUser(int id) => 
    _database.ContainsKey(id) 
        ? Option<User>.Some(_database[id]) 
        : Option<User>.None();

// Chain safely without null checks
string greeting = FindUser(42)
    .Map(user => $"Hello, {user.Name}!")
    .Match(
        some: msg => msg,
        none: () => "User not found."
    );
