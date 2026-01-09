---
title: Code Style Examples
summary: Detailed examples for braces, var usage, null handling, and pattern matching
parent: ../coding-standards.md
---

# Code Style Examples

## Braces and Indentation

Use Allman style (braces on new lines):

```csharp
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
```

## Expression-Bodied Members

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

// Simple methods - expression body acceptable
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
```

## var Usage

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
```

## Null Handling

```csharp
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
    return _items.FirstOrDefault(i => i.Contains(pattern));
}

// Non-nullable guarantees
public string GetRequiredValue()
{
    return _value ?? throw new InvalidOperationException("Value not set");
}
```

## Pattern Matching

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
```
