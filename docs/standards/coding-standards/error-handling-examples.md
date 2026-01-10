---
title: Error Handling Examples
summary: Detailed examples for exception handling, validation, and error logging patterns
audience: [developer, agent]
topics: [error-handling, exceptions, validation]
parent: ../coding-standards.md
last_validated: 2026-01-10
---

# Error Handling Examples

## Specific Exception Catching

```csharp
// Good - catch specific exceptions
try
{
    await ProcessFileAsync(path, ct);
}
catch (FileNotFoundException ex)
{
    _logger.LogWarning(ex, "File not found: {Path}", path);
    return Result.NotFound(path);
}
catch (UnauthorizedAccessException ex)
{
    _logger.LogError(ex, "Access denied to file: {Path}", path);
    throw; // Re-throw security issues
}

// Bad - catching base Exception
try
{
    await ProcessFileAsync(path, ct);
}
catch (Exception ex) // Too broad
{
    _logger.LogError(ex, "Something went wrong");
}
```

## Exception Filters

```csharp
// Use exception filters for conditional handling
try
{
    await httpClient.SendAsync(request, ct);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
{
    _logger.LogWarning("Rate limited, will retry after delay");
    await Task.Delay(retryDelay, ct);
    return await RetryAsync(request, ct);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
{
    _logger.LogWarning("Service unavailable, circuit breaker triggered");
    throw new ServiceUnavailableException("Upstream service is unavailable", ex);
}
```

## Guard Clauses

```csharp
// Use .NET 8+ ArgumentException helpers
public async Task ProcessAsync(string input, int retryCount, CancellationToken ct)
{
    ArgumentException.ThrowIfNullOrEmpty(input);
    ArgumentOutOfRangeException.ThrowIfNegative(retryCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(retryCount, 10);

    // Business logic here
}

// For complex validation
public void Configure(WrapperConfig config)
{
    ArgumentNullException.ThrowIfNull(config);

    if (config.Timeout < TimeSpan.FromSeconds(1))
    {
        throw new ArgumentOutOfRangeException(
            nameof(config),
            config.Timeout,
            "Timeout must be at least 1 second");
    }
}
```

## Error Logging Before Re-throw

```csharp
public async Task<Result> ExecuteAsync(Command command, CancellationToken ct)
{
    try
    {
        return await _handler.HandleAsync(command, ct);
    }
    catch (ValidationException ex)
    {
        // Log and convert to result
        _logger.LogWarning(ex, "Validation failed for command {CommandType}", command.GetType().Name);
        return Result.Invalid(ex.Errors);
    }
    catch (Exception ex)
    {
        // Log with context before re-throwing
        _logger.LogError(
            ex,
            "Unhandled exception processing {CommandType}: {Message}",
            command.GetType().Name,
            ex.Message);
        throw;
    }
}
```

## Result Pattern (Alternative to Exceptions)

```csharp
// For expected failures, use Result pattern instead of exceptions
public readonly record struct Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}

// Usage
public Result<RateLimitInfo> DetectRateLimit(string output)
{
    if (string.IsNullOrEmpty(output))
    {
        return Result<RateLimitInfo>.Failure("Output cannot be empty");
    }

    var match = RateLimitPattern.Match(output);
    if (!match.Success)
    {
        return Result<RateLimitInfo>.Failure("No rate limit detected");
    }

    return Result<RateLimitInfo>.Success(ParseRateLimitInfo(match));
}
```

## Exception Wrapping

```csharp
// Wrap lower-level exceptions with domain-specific ones
public async Task<string> ReadConfigurationAsync(string path, CancellationToken ct)
{
    try
    {
        return await File.ReadAllTextAsync(path, ct);
    }
    catch (IOException ex)
    {
        throw new ConfigurationException(
            $"Failed to read configuration from '{path}'",
            ex);
    }
}

// Domain exception with inner exception preserved
public class ConfigurationException : Exception
{
    public ConfigurationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
```

## Global Exception Handling

```csharp
// In Program.cs - top-level exception handler
try
{
    return await app.RunAsync(args);
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
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
    return ExitCodes.UnhandledException;
}
finally
{
    await Log.CloseAndFlushAsync();
}
```
