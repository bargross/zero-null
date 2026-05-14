# ZeroNull

**Explicit, composable control flow for C#**

 (not yet released)
 
[![NuGet](https://img.shields.io/nuget/v/ZeroNull)](https://www.nuget.org/packages/ZeroNull)

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

## What is this library?

ZeroNull brings **functional control-flow patterns** to C#. It provides lightweight, immutable structs like `Option<T>`, `Result<T, TError>`, and `Either<TLeft, TRight>` that let you model absence, errors, and branching logic explicitly in your type signatures—without exceptions, nulls, or hidden side-effects.

## What is it for?

- **Replace `null`** with `Option<T>` – no more `NullReferenceException`.
- **Replace exceptions** with `Result<T, TError>` – every failure is visible in the return type.
- **Model mutually exclusive choices** with `Either<TLeft, TRight>` – a value can be one of two possible types.
- **Chain operations** safely with `Map`, `Bind`, and LINQ query syntax.
- **Force exhaustive handling** using `Match` – you can’t forget to handle any case.

## Who is it for?

- C# developers who want **more reliable, self‑documenting code**.
- Teams adopting **functional programming** techniques in a pragmatic way.
- Anyone tired of unexpected nulls or hidden exception paths.

## Types & Features

| Type | Purpose | Key methods |
| :--- | :--- | :--- |
| `Option<T>` | A value that may be present (`Some`) or absent (`None`). | `Map`, `Bind`, `Match`, `ValueOr`, LINQ support |
| `Result<T, TError>` | A value that is either successful (`Ok`) or carries an error (`Error`). | `Map`, `Bind`, `MapError`, `Match`, LINQ support |
| `Either<TLeft, TRight>` | A value that is one of two possible types (e.g., `Left` or `Right`). | `MapLeft`, `MapRight`, `BindLeft`, `BindRight`, `Match`, LINQ support |

**Core features:**
- Immutable, `readonly struct` – zero heap allocation in hot paths.
- LINQ query comprehension (`from...select...`).
- Exhaustive pattern matching with `Match`.
- Async extensions (optional – available in a separate package).

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
```

### Using Result<T, TError> - explicit error handling

```csharp
Result<int, string> ParseInt(string input) =>
    int.TryParse(input, out int value)
        ? Result<int, string>.Ok(value)
        : Result<int, string>.Error($"'{input}' is not a valid integer.");

Result<double, string> Compute(string input) =>
    from number in ParseInt(input)
    from reciprocal in number == 0
        ? Result<double, string>.Error("Cannot divide by zero")
        : Result<double, string>.Ok(1.0 / number)
    select reciprocal;

var outcome = Compute("42").Match(
    ok: result => $"Result: {result}",
    error: err => $"Error: {err}"
);
```

### Using Either<TLeft, TRight> – represent a value that can be one of two types

```csharp
// A function returning an Either type. Here, Left is an error message, Right is a valid User.
Either<string, User> GetUser(int id) =>
    _database.ContainsKey(id)
        ? Either<string, User>.Right(_database[id])
        : Either<string, User>.Left($"User {id} not found.");

var message = GetUser(42).Match(
    left: error => $"Error: {error}",
    right: user => $"Success: {user.Name}"
);
```

#### For error handling (using Select)

```csharp
// A simple Either: Left = error message (string), Right = valid integer
Either<string, int> ParseInt(string input) =>
    int.TryParse(input, out int value)
        ? Either<string, int>.Of(value)
        : Either<string, int>.Of($"Invalid number: '{input}'");

// Use Select to transform the Right value
var result = ParseInt("42").Select(x => x * 2);

result.Match(
    onLeft: err => Console.WriteLine($"Error: {err}"),
    onRight: val => Console.WriteLine($"Doubled value: {val}")
);
// Output: Doubled value: 84
```

#### Chaining two operations with SelectMany (LINQ query syntax)

```csharp
Either<string, int> Divide(int numerator, int denominator) =>
    denominator == 0
        ? Either<string, int>.Of("Division by zero")
        : Either<string, int>.Of(numerator / denominator);

var query = from a in ParseInt("10")
            from b in ParseInt("2")
            from result in Divide(a, b)
            select result;

query.Match(
    onLeft: err => Console.WriteLine($"Failed: {err}"),
    onRight: val => Console.WriteLine($"Result: {val}")
);
// Output: Result: 5
```

If any step fails (e.g., ParseInt("abc")), the whole chain short‑circuits to Left.

#### Mixed operations – parsing and validation (using Where explicitly)

Since where clause in query syntax would need a parameterless Where, we use the explicit Where method after the query.

```csharp
// Parse age, then ensure it's between 0 and 120
var ageResult = ParseInt("150")
    .Select(age => age) // identity, just to show flow
    .Where(age => age >= 0 && age <= 120, "Age must be between 0 and 120");

ageResult.Match(
    onLeft: err => Console.WriteLine($"Validation error: {err}"),
    onRight: age => Console.WriteLine($"Valid age: {age}")
);
// Output: Validation error: Age must be between 0 and 120
```

## Roadmap / Future Features

This library is actively evolving. Below are the planned types and capabilities for future releases.

### Phase 1: Core Enhancements (v2.0)

| Feature | Description |
| :--- | :--- |
| **Validation Builder** | Accumulate multiple errors during complex validation (like an applicative `Result`). |
| **`NonEmptyList<T>`** | A compile-time guarantee that a collection has at least one element. |
| **`Try<T>`** | A specialized `Result<T, Exception>` for safely executing exception‑throwing code. |
| **`Unit` type** | Represents a void return, enabling generic monadic code. |
| **Async variants** | `OptionAsync<T>`, `ResultAsync<T,TError>`, `EitherAsync<TLeft,TRight>` with LINQ support. |
| **IEnumerable interop** | `.ToOption()`, `.ToResult()`, `.ToEnumerable()` extensions. |
| **Discriminated Union source generator** | Generate exhaustive matching for sum types via a simple attribute. |

### Phase 2: Advanced Control‑Flow Monads (v3.0)

| Type | Purpose |
| :--- | :--- |
| **`Reader<TEnv, T>`** | Share read‑only environment (config, context) implicitly through a computation. |
| **`Writer<TLog, T>`** | Accumulate a log (strings, metrics, etc.) alongside the main result. |
| **`State<TState, T>`** | Pure, functional state transitions without `ref` or `out` parameters. |
| **Monad Transformers** | Combine effects, e.g., `OptionT<Result<...>>` – computations that can fail, log, or read state simultaneously. |

### Phase 3: Interoperability & Polish (v3.x / v4.0)

| Feature | Description |
| :--- | :--- |
| **Task / ValueTask integration** | Seamless conversion between `Task<T>` and `Result<T, TError>`. |
| **Standard interfaces** | Implement `IEquatable<T>`, `IComparable<T>`, and `System.Text.Json` converters for all types. |
| **High‑performance optimizations** | Ensure all types remain `readonly struct` with zero heap allocation. |
| **Source‑generated matching** | For DUs and sum types, generate exhaustive `Match` and `Switch` methods at compile time. |

### Release Timeline

- **v2.0** – Phase 1 features (Validation, `NonEmptyList`, `Try`, `Unit`, async variants, interop).
- **v3.0** – Phase 2 monads (`Reader`, `Writer`, `State`, transformers).
- **v3.x / v4.0** – Phase 3 interoperability and performance.

Contributions and feedback are welcome! If you have a specific feature request, please open an issue.
