---
title: Async Programming Patterns
summary: Detailed examples for async/await, CancellationToken, and ValueTask usage
audience: [developer, agent]
topics: [async, csharp, patterns]
parent: ../coding-standards.md
last_validated: 2026-01-10
---

# Async Programming Patterns

## Prefer Async Over Blocking

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

## Async Method Naming

```csharp
public async Task<bool> ValidateAsync(CancellationToken ct);
public async Task ProcessDataAsync(Stream data, CancellationToken ct);
```

## CancellationToken Propagation

```csharp
public async Task ProcessAsync(CancellationToken ct = default)
{
    await Step1Async(ct);
    await Step2Async(ct);
    await Step3Async(ct);
}
```

## ConfigureAwait Usage

### Library Code (ConfigureAwait(false))

```csharp
// In non-UI library code - avoids deadlocks and improves performance
var result = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
```

### Special Cases (ConfigureAwait(true))

Use `ConfigureAwait(true)` (or omit ConfigureAwait) in these scenarios:

```csharp
// Durable Function Orchestrations - must preserve context for replay
[Function("ProcessWorkflow")]
public async Task<string> RunOrchestrator(
    [OrchestrationTrigger] TaskOrchestrationContext context)
{
    // Do NOT use ConfigureAwait(false) in orchestrations
    var result = await context.CallActivityAsync<string>("Step1", input);
    return result;
}

// Test code - preserve context for assertions and test framework
[Fact]
public async Task Should_return_valid_response()
{
    // Omit ConfigureAwait in tests - context preservation aids debugging
    var result = await _sut.ProcessAsync(CancellationToken.None);
    result.Should().NotBeNull();
}
```

## Never Block on Async

```csharp
// Bad - can deadlock
var result = task.Result;
task.Wait();

// Good
var result = await task;
```

## ValueTask for Hot Paths

Use `ValueTask<T>` when a method often completes synchronously:

```csharp
public ValueTask<int> GetCachedValueAsync(string key)
{
    if (_cache.TryGetValue(key, out var value))
    {
        return ValueTask.FromResult(value); // Synchronous path
    }

    return new ValueTask<int>(LoadValueAsync(key)); // Async fallback
}
```

## Exception Handling in Async

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
```
