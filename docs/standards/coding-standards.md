---
title: Coding Standards
summary: C# conventions, naming rules, patterns, and quality guidelines for the project
audience: [developer, agent]
topics: [csharp, conventions, code-style, testing, patterns]
prerequisites: []
related: [../agents/CONVENTIONS.md, ../practices/code-review.md, ../adr/0010-code-formatting.md]
last_validated: 2026-01-09
---

# Coding Standards

This document defines the coding standards for the McjCoderOrg.ClaudeAutoResume project.
All code must adhere to these standards. Violations will be caught by analyzers
(as errors) or during code review.

## C# Language Version

- **Target:** C# 14 (.NET 10)
- **Nullable reference types:** Enabled
- **Implicit usings:** Enabled

## File Organisation

### File Structure

Each C# file should follow this order:

````csharp
// 1. File-scoped namespace (required for all new files)
namespace McjCoderOrg.ClaudeAutoResume;

// 2. Using directives (sorted, System first)
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

// 3. Type declarations (one primary type per file)
public sealed class ExampleClass
{
    // Members ordered as below
}
```text

### Member Ordering

Within a class, order members as follows:

1. Constants
2. Static readonly fields
3. Static fields
4. Instance readonly fields
5. Instance fields
6. Constructors
7. Finalizers (rare)
8. Delegates (rare)
9. Events (rare)
10. Properties
11. Indexers (rare)
12. Methods
13. Nested types

Within each group, order by accessibility:

1. public
2. internal
3. protected internal
4. protected
5. private protected
6. private

```csharp
public sealed class WrapperConfig
{
    // 1. Constants
    private const int DefaultRetryDelayMs = 5000;
    private const int MaxRetries = 10;

    // 2. Static readonly fields
    private static readonly Regex ConfigPattern = new(@"\.claude-auto-resume\.json$");

    // 3. Instance readonly fields
    private readonly ILogger<WrapperConfig> _logger;

    // 4. Instance fields
    private int _currentRetryCount;

    // 5. Constructors
    public WrapperConfig(ILogger<WrapperConfig> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // 6. Properties
    public int RetryDelayMs { get; init; } = DefaultRetryDelayMs;

    public int MaxRetryAttempts { get; init; } = MaxRetries;

    // 7. Methods
    public bool IsValid()
    {
        return RetryDelayMs > 0 && MaxRetryAttempts > 0;
    }

    private void LogConfiguration()
    {
        _logger.LogDebug("Configuration loaded: {RetryDelay}ms, {MaxRetries} retries",
            RetryDelayMs, MaxRetryAttempts);
    }
}
```text

## Naming Conventions

### General Rules

| Element         | Convention                | Example                               |
| --------------- | ------------------------- | ------------------------------------- |
| Namespaces      | PascalCase, dot-separated | `McjCoderOrg.ClaudeAutoResume`        |
| Classes         | PascalCase, noun          | `RateLimitDetector`                   |
| Interfaces      | IPascalCase               | `IRateLimitDetector`                  |
| Records         | PascalCase                | `RateLimitInfo`                       |
| Structs         | PascalCase                | `TimeRange`                           |
| Enums           | PascalCase                | `ExitCode`                            |
| Enum values     | PascalCase                | `ConfigurationError`                  |
| Methods         | PascalCase, verb          | `DetectRateLimit`                     |
| Async methods   | PascalCase + Async        | `DetectRateLimitAsync`                |
| Properties      | PascalCase                | `MaxRetries`                          |
| Events          | PascalCase                | `RateLimitDetected`                   |
| Public fields   | PascalCase                | `MaxValue` (rare - prefer properties) |
| Private fields  | \_camelCase               | `_retryCount`                         |
| Parameters      | camelCase                 | `retryDelay`                          |
| Local variables | camelCase                 | `isLimited`                           |
| Constants       | PascalCase                | `DefaultTimeout`                      |
| Type parameters | TPascalCase               | `TResult`, `TKey`                     |

### Naming Guidelines

1. **Use meaningful names**

   ```csharp
   // Good
   int retryDelayMilliseconds;
   bool isRateLimited;

   // Bad
   int rd;
   bool flag;
````

2. **Don't use Hungarian notation**

   ```csharp
   // Good
   string name;
   int count;

   // Bad
   string strName;
   int intCount;
   ```

3. **Use consistent terminology**
   - Use project-specific terms consistently
   - `RateLimit` not `ThrottleLimit` or `UsageLimit`
   - `Retry` not `Attempt` or `Try`

4. **Boolean naming**

   ```csharp
   // Good - questions
   bool isEnabled;
   bool hasValue;
   bool canRetry;
   bool shouldContinue;

   // Bad
   bool enabled; // Unclear if state or action
   bool retry;   // Verb, not question
   ```

## Code Style

### Braces and Indentation

- Use Allman style (braces on new lines)
- Use 4 spaces for indentation (no tabs)
- Maximum line length: 120 characters

````csharp
// Correct - Allman style
public void ProcessOutput(string output)
{
    if (string.IsNullOrEmpty(output))
    {
        return;
    }

    foreach (var line in output.Split('\n'))
    {
        ProcessLine(line);
    }
}

// Wrong - K&R style
public void ProcessOutput(string output) {
    if (string.IsNullOrEmpty(output)) {
        return;
    }
}
```text

### Expression-Bodied Members

Use for simple, single-expression members:

```csharp
// Properties - use expression body when simple
public string FullName => $"{FirstName} {LastName}";

// Read-only properties with complex logic - use block body
public bool IsValid
{
    get
    {
        if (string.IsNullOrEmpty(Name)) return false;
        if (RetryDelay <= 0) return false;
        return true;
    }
}

// Simple methods - expression body is acceptable
public bool IsRateLimited(string output) => output.Contains("limit reached");

// Complex methods - always use block body
public async Task<bool> ProcessAsync(CancellationToken ct)
{
    var result = await FetchDataAsync(ct);
    if (result == null)
    {
        return false;
    }
    return ValidateResult(result);
}
```text

### var Usage

Use `var` when the type is obvious from the right side:

```csharp
// Good - type is obvious
var config = new WrapperConfig();
var items = new List<string>();
var result = await GetResultAsync();

// Good - explicit type for clarity
WrapperConfig config = LoadConfig(path);
IEnumerable<string> items = GetItems();

// Bad - type not obvious, be explicit
var result = ProcessData(input); // What's the type?
```text

### Null Handling

Use nullable reference types and modern null handling:

```csharp
// Null checking
public void Process(string? input)
{
    // Pattern matching (preferred)
    if (input is null)
    {
        throw new ArgumentNullException(nameof(input));
    }

    // Null-conditional operator
    var length = input?.Length ?? 0;

    // Null-coalescing assignment
    _cache ??= new Dictionary<string, object>();
}

// Nullable return types
public string? FindMatch(string pattern)
{
    // May return null - caller must handle
    return _items.FirstOrDefault(i => i.Contains(pattern));
}

// Non-nullable guarantees
public string GetRequiredValue()
{
    // Never returns null - use ! only when certain
    return _value ?? throw new InvalidOperationException("Value not set");
}
```text

### Pattern Matching

Use pattern matching for type checks and deconstruction:

```csharp
// Type patterns
if (obj is string text)
{
    Console.WriteLine(text.Length);
}

// Switch expressions
var message = exitCode switch
{
    ExitCode.Success => "Completed successfully",
    ExitCode.ConfigurationError => "Invalid configuration",
    ExitCode.DependencyMissing => "Claude CLI not found",
    _ => "Unknown error"
};

// Property patterns
if (config is { RetryDelay: > 0, MaxRetries: > 0 })
{
    // Valid configuration
}

// List patterns
if (args is [var first, ..])
{
    Console.WriteLine($"First argument: {first}");
}
```text

## Asynchronous Programming

### Async/Await Guidelines

1. **Always prefer async over blocking**

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
````

2. **Suffix async methods with Async**

   ```csharp
   public async Task<bool> ValidateAsync(CancellationToken ct);
   public async Task ProcessDataAsync(Stream data, CancellationToken ct);
   ```

3. **Pass CancellationToken through the call chain**

   ```csharp
   public async Task ProcessAsync(CancellationToken ct = default)
   {
       await Step1Async(ct);
       await Step2Async(ct);
       await Step3Async(ct);
   }
   ```

4. **Use ConfigureAwait(false) in library code**

   ```csharp
   // In non-UI library code
   var result = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
   ```

5. **Never use .Result or .Wait()**

   ```csharp
   // Bad - can deadlock
   var result = task.Result;
   task.Wait();

   // Good
   var result = await task;
   ```

### ValueTask Usage

Use `ValueTask<T>` when a method often completes synchronously:

````csharp
public ValueTask<int> GetCachedValueAsync(string key)
{
    if (_cache.TryGetValue(key, out var value))
    {
        return ValueTask.FromResult(value); // Synchronous
    }

    return new ValueTask<int>(LoadValueAsync(key)); // Async fallback
}
```text

## Error Handling

### Exception Guidelines

1. **Catch specific exceptions**

   ```csharp
   try
   {
       await ProcessAsync(ct);
   }
   catch (OperationCanceledException)
   {
       _logger.LogInformation("Operation cancelled by user");
       return ExitCode.UserCancelled;
   }
   catch (ConfigurationException ex)
   {
       _logger.LogError(ex, "Configuration error: {Message}", ex.Message);
       return ExitCode.ConfigurationError;
   }
````

2. **Don't catch Exception without re-throwing or logging**

   ```csharp
   // Bad - swallowing exceptions
   catch (Exception) { }

   // Good - log and handle appropriately
   catch (Exception ex)
   {
       _logger.LogError(ex, "Unexpected error during processing");
       throw; // Re-throw if can't handle
   }
   ```

3. **Use exception filters when appropriate**

   ```csharp
   catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
   {
       // Handle rate limiting specifically
   }
   ```

### Validation

Validate inputs at boundaries:

````csharp
public void Configure(string path, int retryDelay)
{
    // Guard clauses
    ArgumentException.ThrowIfNullOrEmpty(path);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryDelay);

    // Proceed with valid inputs
    _path = path;
    _retryDelay = retryDelay;
}
```text

## Logging

### Structured Logging

Always use structured logging with named parameters:

```csharp
// Good - structured with named parameters
_logger.LogInformation("Rate limit detected, resets at {ResetTime}", resetTime);
_logger.LogWarning("Retry attempt {Attempt} of {MaxRetries} failed", attempt, maxRetries);
_logger.LogError(ex, "Failed to process {FileName}: {ErrorMessage}", fileName, ex.Message);

// Bad - string interpolation loses structure
_logger.LogInformation($"Rate limit detected, resets at {resetTime}");
```text

### Log Levels

| Level         | Use For                                      |
| ------------- | -------------------------------------------- |
| `Trace`       | Detailed diagnostic info (method entry/exit) |
| `Debug`       | Internal state useful for debugging          |
| `Information` | Normal operation events                      |
| `Warning`     | Unexpected but handled situations            |
| `Error`       | Failures requiring attention                 |
| `Critical`    | Application cannot continue                  |

### Parameter Naming

Use PascalCase for log parameters to ensure consistent output:

```csharp
_logger.LogInformation(
    "Processing file {FileName} ({FileSize} bytes) from {SourceDirectory}",
    fileName,
    fileSize,
    sourceDirectory);
```text

## Testing

### Test Organisation

```text
tests/
├── McjCoderOrg.ClaudeAutoResume.Tests/           # Unit tests
│   ├── ClaudeMonitorTests.cs
│   └── WrapperConfigTests.cs
├── McjCoderOrg.ClaudeAutoResume.SystemTests/     # BDD system tests
│   ├── Features/
│   │   └── RateLimitDetection.feature
│   └── StepDefinitions/
│       └── RateLimitDetectionSteps.cs
└── McjCoderOrg.ClaudeAutoResume.ArchTests/       # Architecture tests
    └── DependencyTests.cs
````

### Test Naming

Format: `MethodName_Scenario_ExpectedBehaviour`

````csharp
[Fact]
public void IsRateLimited_WhenLimitMessagePresent_ReturnsTrue()

[Fact]
public void LoadConfig_WhenFileMissing_ReturnsDefaults()

[Fact]
public async Task ProcessAsync_WhenCancelled_ThrowsOperationCancelledException()
```text

### Test Structure

Use Arrange-Act-Assert (AAA) pattern:

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
```text

### Assertions

Use AwesomeAssertions (FluentAssertions fork):

```csharp
// Boolean
result.Should().BeTrue();
result.Should().BeFalse();

// Equality
value.Should().Be(42);
value.Should().NotBe(0);

// Strings
text.Should().Contain("expected");
text.Should().StartWith("prefix");
text.Should().BeEmpty();

// Collections
list.Should().HaveCount(3);
list.Should().Contain("item");
list.Should().BeEmpty();
list.Should().OnlyContain(x => x > 0);

// Exceptions
action.Should().Throw<ArgumentException>()
    .WithMessage("*invalid*");

action.Should().NotThrow();

// Async
await func.Should().ThrowAsync<InvalidOperationException>();
```text

### BDD Scenarios

Use Reqnroll with Gherkin syntax:

```gherkin
Feature: Rate Limit Detection
    As a user running extended Claude sessions
    I want the wrapper to detect rate limits
    So that my session can resume automatically

    Scenario: Detect rate limit in Claude output
        Given the Claude CLI is running
        When Claude outputs "Claude AI usage limit reached, resets at 3pm"
        Then the wrapper should detect a rate limit
        And the reset time should be "3pm"

    Scenario Outline: Detect various rate limit messages
        Given the Claude CLI is running
        When Claude outputs "<message>"
        Then the wrapper should detect a rate limit

        Examples:
            | message                                        |
            | Claude AI usage limit reached                  |
            | Rate limit exceeded, please wait               |
            | Usage limit reached for this session           |
```text

### Test Coverage

- **Target:** 80% line coverage, 70% branch coverage on changed code
- **Focus:** Critical paths and edge cases
- **Skip:** Trivial code (simple properties, pass-through methods)

## Documentation

### XML Documentation

Required for all public types and members:

```csharp
/// <summary>
/// Detects rate limit messages in Claude CLI output.
/// </summary>
public sealed class RateLimitDetector
{
    /// <summary>
    /// Checks if the output contains a rate limit message.
    /// </summary>
    /// <param name="output">The CLI output to check.</param>
    /// <returns>
    /// <c>true</c> if a rate limit message is detected; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="output"/> is <c>null</c>.
    /// </exception>
    /// <example>
    /// <code>
    /// var detector = new RateLimitDetector();
    /// bool isLimited = detector.IsRateLimited("Claude AI usage limit reached");
    /// </code>
    /// </example>
    public bool IsRateLimited(string output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output.Contains("limit reached", StringComparison.OrdinalIgnoreCase);
    }
}
```text

### When to Skip Documentation

- Private implementation details
- Self-documenting code
- Test methods (use descriptive names)
- Overrides that don't change behaviour

## Performance Considerations

### Avoid Allocations in Hot Paths

```csharp
// Bad - allocates on every call
public bool IsMatch(string input)
{
    var pattern = new Regex(@"limit.*reached");
    return pattern.IsMatch(input);
}

// Good - reuse compiled regex
private static readonly Regex LimitPattern = new(@"limit.*reached", RegexOptions.Compiled);

public bool IsMatch(string input)
{
    return LimitPattern.IsMatch(input);
}
````

### Use `Span<T>` for Slicing

````csharp
// Good - no allocation
ReadOnlySpan<char> span = input.AsSpan();
if (span.StartsWith("prefix"))
{
    ProcessSpan(span[7..]); // Slice without allocation
}
```text

### String Operations

```csharp
// Use StringBuilder for concatenation in loops
var builder = new StringBuilder();
foreach (var item in items)
{
    builder.Append(item);
    builder.Append(separator);
}

// Use string.Create for known-length strings
var result = string.Create(10, seed, (span, state) =>
{
    // Fill span directly
});
```text

## Security Guidelines

### Input Validation

Validate all external input:

```csharp
public void ProcessUserInput(string input)
{
    ArgumentException.ThrowIfNullOrEmpty(input);

    // Validate length
    if (input.Length > MaxInputLength)
    {
        throw new ArgumentException($"Input exceeds maximum length of {MaxInputLength}");
    }

    // Validate content
    if (!IsValidInput(input))
    {
        throw new ArgumentException("Input contains invalid characters");
    }
}
```text

### Process Spawning

Never pass unsanitised input to process commands:

```csharp
// Bad - command injection vulnerability
var process = Process.Start("cmd", $"/c {userInput}");

// Good - use argument array
var startInfo = new ProcessStartInfo
{
    FileName = "claude",
    UseShellExecute = false,
};
startInfo.ArgumentList.Add("--option");
startInfo.ArgumentList.Add(userInput); // Properly escaped

var process = Process.Start(startInfo);
```text

### Secret Handling

Never log or expose secrets:

```csharp
// Bad - logs sensitive data
_logger.LogDebug("API key: {ApiKey}", apiKey);

// Good - mask sensitive data
_logger.LogDebug("API key configured: {HasApiKey}", !string.IsNullOrEmpty(apiKey));
```text

## Code Review Checklist

Before submitting code for review:

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
````
