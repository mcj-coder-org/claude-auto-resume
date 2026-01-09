---
title: Async Programming Patterns
summary: Detailed examples for async/await, CancellationToken, and ValueTask usage
parent: ../coding-standards.md
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

## ConfigureAwait in Library Code

```csharp
// In non-UI library code
var result = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
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
