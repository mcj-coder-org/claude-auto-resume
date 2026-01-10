---
title: Logging Examples
summary: Detailed examples for structured logging, log levels, and logging patterns
audience: [developer, agent]
topics: [logging, serilog, observability]
parent: ../coding-standards.md
last_validated: 2026-01-10
---

# Logging Examples

## Structured Logging with Named Parameters

```csharp
// Good - structured logging with named PascalCase parameters
_logger.LogInformation(
    "Rate limit detected for {UserId}, resets at {ResetTime}",
    userId,
    resetTime);

_logger.LogWarning(
    "Retry attempt {RetryCount} of {MaxRetries} for operation {OperationName}",
    retryCount,
    maxRetries,
    operationName);

// Bad - string interpolation (loses structure)
_logger.LogInformation($"Rate limit detected for {userId}, resets at {resetTime}");

// Bad - positional parameters (harder to read)
_logger.LogInformation("Rate limit detected for {0}, resets at {1}", userId, resetTime);
```

## Log Level Usage

### Trace - Detailed Diagnostics

```csharp
// Internal implementation details, very verbose
_logger.LogTrace("Entering ParseRateLimitHeader with value: {HeaderValue}", headerValue);
_logger.LogTrace("Regex match groups: {Groups}", string.Join(", ", match.Groups));
```

### Debug - Development Debugging

```csharp
// State information useful during development
_logger.LogDebug("Configuration loaded: {@Config}", config);
_logger.LogDebug("Cache miss for key {CacheKey}, fetching from source", key);
```

### Information - Normal Operations

```csharp
// Business events, normal application flow
_logger.LogInformation("Application started with version {Version}", version);
_logger.LogInformation("Processing completed for {ItemCount} items in {ElapsedMs}ms",
    itemCount, elapsed.TotalMilliseconds);
_logger.LogInformation("User {UserId} authenticated successfully", userId);
```

### Warning - Unexpected but Handled

```csharp
// Anomalies that were handled but should be noted
_logger.LogWarning("Rate limit approaching: {CurrentCount}/{MaxCount}", current, max);
_logger.LogWarning("Retrying operation after transient failure: {ErrorMessage}", ex.Message);
_logger.LogWarning("Configuration value {Key} not found, using default {DefaultValue}",
    key, defaultValue);
```

### Error - Failures Requiring Attention

```csharp
// Failures that need investigation
_logger.LogError(ex, "Failed to process file {FilePath}: {ErrorMessage}",
    filePath, ex.Message);
_logger.LogError("Database connection failed after {MaxRetries} retries", maxRetries);
```

### Critical - Application Cannot Continue

```csharp
// Fatal errors
_logger.LogCritical(ex, "Unrecoverable error during startup");
_logger.LogCritical("Required service {ServiceName} is unavailable", serviceName);
```

## Exception Logging

```csharp
// Always pass exception as first parameter
_logger.LogError(ex, "Operation failed: {Message}", ex.Message);

// Include context
_logger.LogError(
    ex,
    "Failed to process request {RequestId} for user {UserId}",
    requestId,
    userId);

// Don't log and throw (choose one)
// Bad:
_logger.LogError(ex, "Error occurred");
throw; // Now it's logged twice up the stack

// Good - log at the handling point only:
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to complete operation");
    return Result.Failure(ex.Message);
}
```

## Destructuring Objects

```csharp
// Use @ for object destructuring (logs all properties)
_logger.LogDebug("Processing request: {@Request}", request);

// Use $ for ToString() representation
_logger.LogInformation("User: {$User}", user);

// Be careful with sensitive data
_logger.LogDebug("Config loaded: {@Config}", config.Sanitized());
```

## Scoped Logging

```csharp
// Add context that applies to multiple log entries
using (_logger.BeginScope("RequestId: {RequestId}", requestId))
{
    _logger.LogInformation("Starting request processing");
    await ProcessAsync();
    _logger.LogInformation("Request processing completed");
}
// Both log entries include RequestId in their scope
```

## Performance Considerations

```csharp
// Check log level before expensive operations
if (_logger.IsEnabled(LogLevel.Debug))
{
    var debugInfo = GenerateExpensiveDebugInfo();
    _logger.LogDebug("Debug info: {DebugInfo}", debugInfo);
}

// Use source generators for high-performance logging (LoggerMessage)
[LoggerMessage(
    EventId = 1001,
    Level = LogLevel.Information,
    Message = "Rate limit detected for {UserId}, resets at {ResetTime}")]
private static partial void LogRateLimitDetected(
    ILogger logger,
    string userId,
    DateTimeOffset resetTime);

// Usage
LogRateLimitDetected(_logger, userId, resetTime);
```

## Secrets and Sensitive Data

```csharp
// Never log secrets
// Bad:
_logger.LogDebug("Connecting with API key: {ApiKey}", apiKey);

// Good - redact sensitive values:
_logger.LogDebug("Connecting with API key: {ApiKey}", "[REDACTED]");

// Good - log only safe identifiers:
_logger.LogDebug("Connecting with API key ID: {ApiKeyId}", apiKeyId);

// Use enrichers to automatically redact
.Enrich.WithSensitiveDataMasking()
```

## Correlation and Tracing

```csharp
// Include correlation IDs for distributed tracing
_logger.LogInformation(
    "Processing message {MessageId} with correlation {CorrelationId}",
    message.Id,
    message.CorrelationId);

// Use Activity for OpenTelemetry integration
using var activity = ActivitySource.StartActivity("ProcessMessage");
activity?.SetTag("message.id", message.Id);

_logger.LogInformation("Processing started");
```
