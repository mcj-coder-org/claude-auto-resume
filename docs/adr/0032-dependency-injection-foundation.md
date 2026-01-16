---
number: '0032'
title: 'Dependency Injection Foundation'
status: accepted
date: 2026-01-16
decision-makers:
  - '@mcj-coder'
  - '@martincjarvis'
context: Architecture
---

# ADR-0032: Dependency Injection Foundation

## Status

Accepted (2026-01-16)

## Context

The codebase had grown to include significant business logic in Program.cs (~560 lines) with direct
dependencies on static classes and the Console/Environment APIs. This made unit testing difficult:

- Program.cs contained argument parsing, validation, help output, diagnostics
- ClaudeMonitor depended directly on Console, Environment, and File static APIs
- Static classes like LoggingConfiguration couldn't be mocked
- Test coverage was limited to integration-level testing

We needed a testable architecture that:

1. Enables unit testing of business logic in isolation
2. Reduces coupling between components
3. Establishes patterns for future development
4. Maintains backward compatibility with existing CLI behavior

## Decision

Implement dependency injection using `Microsoft.Extensions.Hosting` with the following structure:

### Core Interfaces

| Interface             | Purpose                           |
| --------------------- | --------------------------------- |
| `IApplication`        | Main application entry point      |
| `IConsoleService`     | Console I/O abstraction           |
| `IEnvironmentService` | Environment and filesystem access |
| `IArgumentParser`     | CLI argument parsing              |
| `IClaudeMonitor`      | Claude process monitoring         |

### Service Lifetimes

| Service               | Lifetime  | Rationale                           |
| --------------------- | --------- | ----------------------------------- |
| `IConsoleService`     | Singleton | Stateless, wraps static Console     |
| `IEnvironmentService` | Singleton | Stateless, wraps static Environment |
| `IArgumentParser`     | Singleton | Stateless                           |
| `IApplication`        | Singleton | Single run per process              |
| `ClaudeMonitor`       | Transient | Stateful, disposable                |

### Program.cs Structure

Minimal host setup (~70 lines):

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IConsoleService, ConsoleService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<IArgumentParser, ArgumentParser>();
        services.AddSingleton<IApplication, Application>();
    })
    .UseSerilog()
    .Build();

var app = host.Services.GetRequiredService<IApplication>();
return await app.RunAsync(args);
```

## Consequences

### Positive

- **Testability**: All business logic can be unit tested with mocked dependencies
- **Separation of Concerns**: Clear boundaries between components
- **Maintainability**: Smaller, focused classes (Program.cs reduced from ~560 to ~70 lines)
- **Extensibility**: Easy to add new services or swap implementations
- **Consistency**: Standard Microsoft.Extensions.Hosting patterns

### Negative

- **Complexity**: More files and indirection than static approach
- **Learning Curve**: Contributors need to understand DI patterns
- **Bootstrap Limitation**: Some code (logging setup) must run before DI is available

### Neutral

- **Performance**: Negligible overhead for CLI application
- **Migration**: Existing tests updated to use mock services

## Alternatives Considered

### 1. Manual DI (No Framework)

Pass dependencies through constructors without a DI container.

**Rejected**: Missing lifetime management, service resolution, and host integration benefits.

### 2. Third-Party DI (Autofac, etc.)

Use a third-party DI container instead of Microsoft's.

**Rejected**: Microsoft.Extensions.Hosting is sufficient and already bundled with Serilog.Extensions.Hosting.

### 3. Static Factory Pattern

Keep static classes but add factory methods returning interfaces.

**Rejected**: Doesn't solve the fundamental testability problem.

## Related

- [Coding Standards: Dependency Injection](../standards/coding-standards/dependency-injection.md)
- [Design Document](../plans/2026-01-16-dependency-injection-foundation-design.md)
- Issue #116, #117, #118
