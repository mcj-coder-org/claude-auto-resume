---
name: observability
description: |
  When adding logging or diagnostics. Apply when configuring structured logging, capturing
  platform context, or implementing test-capturable log behavior.
decision: Use Serilog with file and debug sinks, disabled by default, with bootstrap logger pattern.
status: accepted
type: implementation
implementation_issue: '#39'
---

# ADR-0017: Observability

## Status

Proposed

## Date

2026-01-09

## Context

We need observability for:

1. Debugging issues in production
2. Capturing diagnostic information for bug reports
3. Testing log-based behavior
4. Understanding application state

### Requirements

- Structured logging with named parameters
- File logging (not console, to avoid PTY interference)
- Disabled by default (privacy)
- Test-capturable for behavior verification
- Platform context for diagnostics

### Options Considered

#### Option 1: Serilog (Selected)

Structured logging library with rich sink ecosystem.

**Pros:**

- Structured logging (named parameters)
- File sink for non-console logging
- InMemory sink for test capture
- Bootstrap logger for startup errors
- Active community

**Cons:**

- Additional dependency
- Configuration complexity

#### Option 2: Microsoft.Extensions.Logging

Built-in .NET logging abstraction.

**Pros:**

- No additional dependency
- Familiar API
- DI integration

**Cons:**

- Less powerful sinks
- No bootstrap logger pattern
- Harder to test

## Decision

**Serilog** with file and debug sinks, disabled by default.

### Logging Strategy

| Mode        | Console                | File             |
| ----------- | ---------------------- | ---------------- |
| Default     | Errors only            | Bootstrap errors |
| `--verbose` | None (PTY passthrough) | Full debug       |
| Exception   | Error + log path       | Full stack trace |

### Structured Logging

```csharp
Log.Information("Starting Claude Auto Resume v{AppVersion}", platform.AppVersion);
Log.Warning(Strings.RateLimitDetected, resetTime); // "Detected limit, resets at {ResetTime}"
```

### Platform Context

Captured at startup for diagnostics:

- .NET version, runtime identifier
- OS description, architecture
- Command line arguments (sanitized)
- App version
- Container/CI detection

### Bootstrap Logger

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(GetBootstrapLogPath())
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);
    // Configure full logging
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Console.Error.WriteLine($"Error: {ex.Message}");
    Console.Error.WriteLine($"See log file: {GetLogPath()}");
    return 1;
}
```

### Test Capture

```csharp
[Fact]
public void Detect_WhenLimitReached_LogsExpectedMessage()
{
    using var logCapture = new LogCapture();
    var detector = new RateLimitDetector();

    detector.Process("limit reached");

    logCapture.Messages.Should()
        .Contain(m => m.Contains("Detected Session Limit Reached"));
}
```

## Consequences

### Positive

- Structured, searchable logs
- No console interference
- Test-capturable behavior
- Rich diagnostics for bug reports

### Negative

- Additional dependency
- File I/O overhead
- Log file management

## References

- [Serilog](https://serilog.net/)
- [Serilog.Sinks.File](https://github.com/serilog/serilog-sinks-file)
- [Serilog Bootstrap Logger](https://nblumhardt.com/2020/10/bootstrap-logger/)
