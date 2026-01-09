---
title: Agent Conventions
summary: Coding standards, commit conventions, and formatting rules summary
audience: [agent]
topics: [conventions, coding-standards, commits, formatting]
prerequisites: [AGENTS.md, ORIENTATION.md]
related: [../standards/coding-standards.md, ../adr/0004-contribution-workflow.md]
last_validated: 2026-01-09
---

# Agent Conventions

This document provides a quick reference for conventions agents must follow.
For detailed standards, see `docs/standards/coding-standards.md`.

## Commit Conventions

### Message Format

```text
type(scope): subject

body (optional - explain what and why)

Refs: #123
```

### Commit Types

| Type       | Use For                                | Version Impact |
| ---------- | -------------------------------------- | -------------- |
| `feat`     | New feature                            | Minor bump     |
| `fix`      | Bug fix                                | Patch bump     |
| `docs`     | Documentation only                     | None           |
| `style`    | Formatting, whitespace                 | None           |
| `refactor` | Code restructure (no behaviour change) | None           |
| `perf`     | Performance improvement                | Patch bump     |
| `test`     | Adding or updating tests               | None           |
| `build`    | Build system, dependencies             | None           |
| `ci`       | CI/CD configuration                    | None           |
| `chore`    | Maintenance tasks                      | None           |

### Breaking Changes

Add `!` after type or include `BREAKING CHANGE:` in footer:

```text
feat!: remove deprecated configuration option

BREAKING CHANGE: The `--legacy-mode` flag has been removed.

Refs: #456
```

### Scope Guidelines

| Scope     | Description                          |
| --------- | ------------------------------------ |
| `monitor` | ClaudeMonitor, rate limit detection  |
| `config`  | WrapperConfig, configuration loading |
| `cli`     | CLI parsing, argument handling       |
| `logging` | Serilog configuration, log output    |
| `docs`    | Documentation                        |
| `test`    | Test infrastructure                  |
| `build`   | Build configuration                  |

### Examples

**Good commits:**

```text
feat(monitor): add retry delay configuration

Allow users to configure the delay between retry attempts
when rate limits are detected.

Refs: #42
```

```text
fix(config): handle missing config file gracefully

Return default configuration when config file doesn't exist
instead of throwing an exception.

Refs: #87
```

```text
docs: update installation instructions

Refs: #15
```

**Bad commits:**

```text
fixed bug          # No type, no scope, no reference
update code        # Meaningless subject
feat: stuff        # Non-descriptive subject
```

## Branch Naming

### Format

```text
type/issue#-description
```

### Types

| Type       | Use For               |
| ---------- | --------------------- |
| `feature`  | New functionality     |
| `fix`      | Bug fixes             |
| `docs`     | Documentation changes |
| `refactor` | Code restructuring    |
| `test`     | Test improvements     |

### Examples

- `feature/42-add-retry-logic`
- `fix/87-handle-null-response`
- `docs/15-update-readme`
- `refactor/103-extract-config-class`

### Rules

1. Always include issue number
2. Use lowercase with hyphens
3. Keep description concise (2-4 words)

## Code Formatting

### C# Style

| Element         | Convention        | Example                        |
| --------------- | ----------------- | ------------------------------ |
| Namespaces      | PascalCase        | `McjCoderOrg.ClaudeAutoResume` |
| Classes         | PascalCase        | `RateLimitDetector`            |
| Interfaces      | IPascalCase       | `IRateLimitDetector`           |
| Methods         | PascalCase        | `DetectRateLimit`              |
| Properties      | PascalCase        | `MaxRetries`                   |
| Private fields  | \_camelCase       | `_retryCount`                  |
| Parameters      | camelCase         | `retryDelay`                   |
| Local variables | camelCase         | `isLimited`                    |
| Constants       | PascalCase        | `DefaultTimeout`               |
| Async methods   | Suffix with Async | `ProcessAsync`                 |

### File Organisation

```csharp
// 1. File-scoped namespace
namespace McjCoderOrg.ClaudeAutoResume;

// 2. Using statements (inside namespace for file-scoped)
using System.Text.RegularExpressions;

// 3. Class declaration
public sealed class RateLimitDetector
{
    // 4. Constants
    private const int DefaultTimeout = 5000;

    // 5. Static fields
    private static readonly Regex Pattern = new("...");

    // 6. Instance fields
    private readonly ILogger _logger;

    // 7. Constructor
    public RateLimitDetector(ILogger logger)
    {
        _logger = logger;
    }

    // 8. Properties
    public int MaxRetries { get; init; } = 3;

    // 9. Public methods
    public bool IsRateLimited(string output) { ... }

    // 10. Private methods
    private void LogDetection(string message) { ... }
}
```

### Formatting Rules

1. **Indentation:** 4 spaces (no tabs)
2. **Braces:** Allman style (new line)
3. **Line length:** 120 characters max
4. **Blank lines:** One between members
5. **Trailing whitespace:** None
6. **Final newline:** Required

### Automated Formatting

Pre-commit hooks run these formatters:

```bash
dotnet format              # C# code
npx prettier --write       # Markdown, JSON, YAML
npx markdownlint --fix     # Markdown structure
```

## Logging Conventions

### Use Structured Logging

```csharp
// Good - structured with named parameters
Log.Information("Rate limit detected, resets at {ResetTime}", resetTime);
Log.Warning("Retry attempt {Attempt} of {MaxRetries}", attempt, maxRetries);

// Bad - string interpolation
Log.Information($"Rate limit detected, resets at {resetTime}");
```

### Log Levels

| Level         | Use For                              |
| ------------- | ------------------------------------ |
| `Verbose`     | Detailed diagnostic info             |
| `Debug`       | Internal state, useful for debugging |
| `Information` | Normal operation events              |
| `Warning`     | Unexpected but handled situations    |
| `Error`       | Failures requiring attention         |
| `Fatal`       | Application cannot continue          |

### Parameter Naming

Use descriptive PascalCase for log parameters:

```csharp
Log.Information("Processing {FileName} ({FileSize} bytes)", fileName, fileSize);
Log.Error(ex, "Failed to connect to {ServiceName} at {Endpoint}", serviceName, endpoint);
```

## Testing Conventions

### Test Naming

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehaviour()
{
    // Arrange
    // Act
    // Assert
}
```

Examples:

- `IsRateLimited_WhenLimitMessagePresent_ReturnsTrue`
- `LoadConfig_WhenFileMissing_ReturnsDefaults`
- `ProcessAsync_WhenCancelled_ThrowsOperationCancelled`

### Test Structure

```csharp
[Fact]
public void IsRateLimited_WhenLimitMessagePresent_ReturnsTrue()
{
    // Arrange
    var detector = new RateLimitDetector();
    var output = "Claude AI usage limit reached";

    // Act
    var result = detector.IsRateLimited(output);

    // Assert
    result.Should().BeTrue();
}
```

### Assertions

Use AwesomeAssertions (FluentAssertions fork):

```csharp
// Good
result.Should().BeTrue();
result.Should().Be(42);
list.Should().HaveCount(3);
action.Should().Throw<ArgumentException>();

// Avoid
Assert.True(result);
Assert.Equal(42, result);
```

### BDD Scenarios (Reqnroll)

```gherkin
Feature: Rate Limit Detection

  Scenario: Detect rate limit in output
    Given the wrapper is monitoring Claude output
    When Claude outputs "Claude AI usage limit reached"
    Then the wrapper should detect a rate limit
```

## Error Handling

### Exit Codes

| Code | Constant             | Meaning                   |
| ---- | -------------------- | ------------------------- |
| 0    | `Success`            | Normal completion         |
| 1    | `GeneralError`       | Unhandled exception       |
| 2    | `ConfigurationError` | Invalid config            |
| 3    | `DependencyMissing`  | Claude CLI not found      |
| 4    | `RateLimitDetected`  | Rate limit triggered exit |
| 5    | `UserCancelled`      | Ctrl+C                    |

### Exception Handling

```csharp
// Catch specific exceptions
try
{
    await ProcessAsync(cancellationToken);
}
catch (OperationCanceledException)
{
    Log.Information("Operation cancelled by user");
    return ExitCodes.UserCancelled;
}
catch (ConfigurationException ex)
{
    Log.Error(ex, "Configuration error: {Message}", ex.Message);
    return ExitCodes.ConfigurationError;
}
```

### Never Swallow Exceptions Silently

```csharp
// Bad - silently swallowed
catch (Exception) { }

// Good - at minimum, log it
catch (Exception ex)
{
    Log.Warning(ex, "Non-critical error occurred, continuing");
}
```

## Async Patterns

### Always Use Async/Await

```csharp
// Good
public async Task<string> ReadFileAsync(string path, CancellationToken ct)
{
    return await File.ReadAllTextAsync(path, ct);
}

// Bad - blocking
public string ReadFile(string path)
{
    return File.ReadAllText(path);
}
```

### Pass CancellationToken

```csharp
public async Task ProcessAsync(CancellationToken cancellationToken = default)
{
    await DoWorkAsync(cancellationToken);
    await DoMoreWorkAsync(cancellationToken);
}
```

### Use ConfigureAwait(false) in Library Code

```csharp
// In library/non-UI code
var result = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
```

## Documentation Comments

### When Required

- All public types and members
- Complex internal logic

### Format

```csharp
/// <summary>
/// Detects rate limit messages in Claude CLI output.
/// </summary>
/// <param name="output">The output string to check.</param>
/// <returns>True if a rate limit message is detected; otherwise, false.</returns>
/// <exception cref="ArgumentNullException">Thrown when output is null.</exception>
public bool IsRateLimited(string output)
```

### When to Skip

- Private implementation details
- Self-documenting code
- Test methods (use descriptive names instead)

## Quick Checklist

Before committing:

- [ ] Branch named `type/issue#-description`
- [ ] Commit message follows `type(scope): subject` format
- [ ] Commit body includes `Refs: #issue`
- [ ] Code formatted with `dotnet format`
- [ ] No linting errors
- [ ] Tests follow naming convention
- [ ] Structured logging used
- [ ] Async methods have `Async` suffix
- [ ] CancellationToken passed where appropriate
