---
title: Coding Standards
summary: C# conventions, naming rules, patterns, and quality guidelines for the project
audience: [developer, agent]
topics: [csharp, conventions, code-style, testing, patterns]
prerequisites: []
related: [../agents/CONVENTIONS.md, ../practices/code-review.md, ../adr/0010-code-formatting.md]
last_validated: 2026-01-10
---

# Coding Standards

C# coding standards for McjCoderOrg.ClaudeAutoResume. Violations are caught by analyzers, linting or code review.

- Automated Enforcement/Fixing >> Manual Inspection
- SOLID, DRY, YAGNI - less code >> more code
- TDD First - Red, Green, Refactor - every bug should start with a failing test, every userjourney a failing BDD

## Standards Priorities

| Priority | Category                 | Examples                             |
| -------- | ------------------------ | ------------------------------------ |
| P0       | Safety & Integrity       | Security, traceability, issue-driven |
| P1       | Quality & Correctness    | Clean builds, test validity          |
| P2       | Consistency & Governance | Conventions, versioning              |
| P3       | Delivery & Flow          | Incremental execution, DX            |

## Language Settings

- **C# 14** (.NET 10)
- **Nullable reference types:** Enabled
- **Implicit usings:** Enabled

## File Organisation

- One primary type per file
- File-scoped namespaces required
- Usings sorted, `System` first

**Member ordering:** Constants → Static fields → Instance fields → Constructors → Properties → Methods → Nested types

Within groups, order by accessibility: public → internal → protected → private

See [file-organisation.md](coding-standards/file-organisation.md) for examples.

## Naming Conventions

| Element        | Convention      | Example                        |
| -------------- | --------------- | ------------------------------ |
| Namespaces     | PascalCase      | `McjCoderOrg.ClaudeAutoResume` |
| Classes        | PascalCase noun | `RateLimitDetector`            |
| Interfaces     | IPascalCase     | `IRateLimitDetector`           |
| Methods        | PascalCase verb | `DetectRateLimit`              |
| Async methods  | +Async suffix   | `DetectRateLimitAsync`         |
| Properties     | PascalCase      | `MaxRetries`                   |
| Private fields | \_camelCase     | `_retryCount`                  |
| Parameters     | camelCase       | `retryDelay`                   |
| Constants      | PascalCase      | `DefaultTimeout`               |

**Key rules:**

- Use meaningful names (not `rd`, `flag`)
- No Hungarian notation
- Boolean names should be questions: `isEnabled`, `hasValue`, `canRetry`
- Consistent terminology: `RateLimit` not `ThrottleLimit`

See [naming-examples.md](coding-standards/naming-examples.md) for detailed guidance.

## Code Style

- **Braces:** Allman style (new line)
- **Indentation:** 4 spaces
- **Line length:** 120 characters max
- **Expression bodies:** For simple single-expression members only
- **var:** When type is obvious from right side

**Null handling:**

- Use `is null` pattern matching for null checks
- Use `?.` and `??` operators
- Annotate nullable return types

**Pattern matching:** Prefer switch expressions, type patterns, property patterns.

See [code-style-examples.md](coding-standards/code-style-examples.md) for examples.

## Asynchronous Programming

- Prefer async over blocking calls
- Suffix async methods with `Async`
- Pass `CancellationToken` through call chain
- Use `ConfigureAwait(false)` in library code
- Use `ConfigureAwait(true)` in Durable Function Orchestrations and Test Libraries
- Never use `.Result` or `.Wait()`
- Use `ValueTask<T>` when often completing synchronously

See [async-patterns.md](coding-standards/async-patterns.md) for examples.

## Error Handling

- Catch specific exceptions, not `Exception`
- Use exception filters: `catch (Ex ex) when (condition)`
- Validate inputs at boundaries with guard clauses
- Log errors with context before re-throwing

```csharp
public void Configure(string path, int retryDelay)
{
    ArgumentException.ThrowIfNullOrEmpty(path);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryDelay);
}
```

See [error-handling-examples.md](coding-standards/error-handling-examples.md) for patterns.

## Logging

Use structured logging with named PascalCase parameters:

```csharp
_logger.LogInformation("Rate limit detected, resets at {ResetTime}", resetTime);
```

| Level         | Use For                           |
| ------------- | --------------------------------- |
| `Trace`       | Detailed diagnostics              |
| `Debug`       | Internal state for debugging      |
| `Information` | Normal operation events           |
| `Warning`     | Unexpected but handled situations |
| `Error`       | Failures requiring attention      |
| `Critical`    | Application cannot continue       |

See [logging-examples.md](coding-standards/logging-examples.md) for patterns.

## Testing

**Test naming:** `MethodName_Scenario_ExpectedBehaviour`

**Test structure:** Arrange-Act-Assert (AAA)

**Test types:**

| Project        | Purpose            | Framework                              | Scope            |
| -------------- | ------------------ | -------------------------------------- | ---------------- |
| `.Tests`       | Unit tests         | xUnit, Moq                             | Project          |
| `.SystemTests` | BDD system tests   | Reqnroll, xUnit                        | Project          |
| `.E2ETests`    | BDD E2E Tests      | Reqnroll, xUnit, TestContainers/Aspire | Project/Solution |
| `.ArchTests`   | Architecture tests | ArchUnitNET                            | Solution         |
| `.Benchmarks`  | Performance tests  | BenchmarkDotNet                        | Project/Solution |

- Test project root namespaces should omit the `*.Tests` or `.Benchmarks` suffix
- Project internals should be exposed to the Arch, Unit and System Test Projects

**Coverage target:** 80% line, 70% branch on changed code

See [testing-examples.md](coding-standards/testing-examples.md) for patterns and BDD guidance.

## Documentation

XML docs required for all public types and members:

```csharp
/// <summary>
/// Detects rate limit messages in Claude CLI output.
/// </summary>
/// <param name="output">The CLI output to check.</param>
/// <returns><c>true</c> if rate limited; otherwise <c>false</c>.</returns>
public bool IsRateLimited(string output)
```

Skip docs for: private members, self-documenting code, test methods, unchanged overrides.

## Performance

- Avoid allocations in hot paths
- Reuse compiled `Regex` instances
- Use `Span<T>` for slicing without allocation
- Use `StringBuilder` for concatenation in loops

See [performance-examples.md](coding-standards/performance-examples.md) for patterns.

## Security

- Validate all external input
- Never pass unsanitised input to process commands
- Never log secrets (apply OWASP Logging Standards)
- Use `ArgumentList` for process arguments (auto-escapes)

See [security-examples.md](coding-standards/security-examples.md) for examples.

## Code Review Checklist

- [ ] Follows naming conventions
- [ ] Uses file-scoped namespaces
- [ ] Members ordered correctly
- [ ] Async methods suffixed with `Async`
- [ ] `CancellationToken` passed through
- [ ] Uses structured logging
- [ ] Has appropriate tests
- [ ] XML docs on public members
- [ ] No analyzer warnings
- [ ] No hardcoded secrets
- [ ] Input validation at boundaries
