---
title: Dependency Injection Foundation Design
issue: '#116'
author: Claude
created: 2026-01-16
status: approved
---

# Dependency Injection Foundation Design

## Overview

Implement dependency injection using `Microsoft.Extensions.Hosting` to enable proper testability and reduce coupling in the application.

## Goals

- Reduce `Program.cs` from ~560 lines to ~30 lines
- Enable unit testing of application logic by mocking dependencies
- Establish DI patterns for future service extraction
- Maintain backward compatibility with CLI arguments

## Architecture

### Host Setup (Program.cs)

Minimal entry point with host configuration:

```csharp
internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IConsoleService, ConsoleService>();
                    services.AddSingleton<IEnvironmentService, EnvironmentService>();
                    services.AddSingleton<IArgumentParser, ArgumentParser>();
                    services.AddTransient<IClaudeMonitor, ClaudeMonitor>();
                    services.AddSingleton<IApplication, Application>();
                })
                .UseSerilog((context, config) =>
                {
                    // Configure Serilog from host
                })
                .Build();

            var app = host.Services.GetRequiredService<IApplication>();
            return await app.RunAsync(args);
        }
        catch (Exception ex)
        {
            // Bootstrap error handling
            return ExitCodes.GeneralError;
        }
    }
}
```

### IApplication Interface

Main application entry point with injected dependencies:

```csharp
public interface IApplication
{
    Task<int> RunAsync(string[] args);
}

internal sealed class Application : IApplication
{
    private readonly IArgumentParser _argumentParser;
    private readonly IConsoleService _console;
    private readonly IClaudeMonitor _monitor;
    private readonly ILogger<Application> _logger;

    public Application(
        IArgumentParser argumentParser,
        IConsoleService console,
        IClaudeMonitor monitor,
        ILogger<Application> logger)
    {
        _argumentParser = argumentParser;
        _console = console;
        _monitor = monitor;
        _logger = logger;
    }

    public async Task<int> RunAsync(string[] args)
    {
        var parseResult = _argumentParser.Parse(args);

        if (parseResult.ShowHelp)
        {
            _console.PrintHelp();
            return ExitCodes.Success;
        }

        // ... validation, config building, monitor execution

        var config = BuildConfig(parseResult);
        var success = await _monitor.RunAsync(config, parseResult.ClaudeArgs);
        return success ? ExitCodes.Success : ExitCodes.DependencyMissing;
    }
}
```

### IConsoleService Interface

Abstracts all console I/O for testability:

```csharp
public interface IConsoleService
{
    // Output
    void WriteLine(string message);
    void Write(string message);
    void WriteErrorLine(string message);

    // Formatting
    void SetForegroundColor(ConsoleColor color);
    void ResetColor();

    // Input
    bool IsInputRedirected { get; }
    bool KeyAvailable { get; }
    ConsoleKeyInfo ReadKey(bool intercept);
    TextReader In { get; }

    // Window
    int WindowWidth { get; }
    int WindowHeight { get; }

    // High-level helpers
    void PrintHelp();
    void PrintVersion();
    void PrintDiagnostics();
    void PrintStartupInfo(WrapperConfig config, bool headless, bool dangerous);
    void WriteRateLimitDetected(string pattern, int waitMinutes);
}
```

### IEnvironmentService Interface

Abstracts environment and filesystem access:

```csharp
public interface IEnvironmentService
{
    string? GetEnvironmentVariable(string name);
    string CurrentDirectory { get; }
    string UserProfile { get; }
    IDictionary<string, string> GetEnvironmentVariables();
    bool FileExists(string path);
    void CreateDirectory(string path);
    bool IsWindows { get; }
}
```

### IArgumentParser Interface

Extracted argument parsing:

```csharp
public interface IArgumentParser
{
    ParseResult Parse(string[] args);
}
```

### IClaudeMonitor Interface

Extract interface from existing class:

```csharp
public interface IClaudeMonitor : IDisposable
{
    Task<bool> RunAsync(IReadOnlyList<string> additionalArgs);
}
```

## File Structure

```
src/McjCoderOrg.ClaudeAutoResume/
├── Program.cs                    # Minimal host setup (~30 lines)
├── Application.cs                # IApplication implementation
├── Services/
│   ├── IConsoleService.cs
│   ├── ConsoleService.cs
│   ├── IEnvironmentService.cs
│   ├── EnvironmentService.cs
│   ├── IArgumentParser.cs
│   └── ArgumentParser.cs
├── ClaudeMonitor.cs              # Updated with DI
├── IClaudeMonitor.cs             # New interface
├── WrapperConfig.cs              # Unchanged
├── ParseResult.cs                # Extracted, made public
├── ExitCodes.cs                  # Unchanged
├── PlatformInfo.cs               # Unchanged
└── LoggingConfiguration.cs       # Unchanged
```

## Service Lifetimes

| Service               | Lifetime  | Rationale                           |
| --------------------- | --------- | ----------------------------------- |
| `IConsoleService`     | Singleton | Stateless, wraps static Console     |
| `IEnvironmentService` | Singleton | Stateless, wraps static Environment |
| `IArgumentParser`     | Singleton | Stateless                           |
| `IClaudeMonitor`      | Transient | Stateful, disposable                |
| `IApplication`        | Singleton | Single run per process              |

## Implementation Sequence

1. Add `Microsoft.Extensions.Hosting` package reference
2. Create `Services/` folder with interfaces and implementations
3. Extract `ParseResult` to separate file, make public
4. Create `IClaudeMonitor` interface
5. Create `IApplication` and `Application` class
6. Update `ClaudeMonitor` to use injected services
7. Refactor `Program.cs` to host setup only
8. Add unit tests for `Application` class
9. Verify all existing tests pass

## Testing Strategy

With DI in place, tests can:

```csharp
[Fact]
public async Task RunAsync_WithHelpFlag_PrintsHelpAndReturnsSuccess()
{
    var mockConsole = new Mock<IConsoleService>();
    var mockParser = new Mock<IArgumentParser>();
    mockParser.Setup(p => p.Parse(It.IsAny<string[]>()))
        .Returns(new ParseResult { ShowHelp = true });

    var app = new Application(mockParser.Object, mockConsole.Object, ...);

    var result = await app.RunAsync(["--help"]);

    result.Should().Be(ExitCodes.Success);
    mockConsole.Verify(c => c.PrintHelp(), Times.Once);
}
```

## Related Issues

- #117: Extract interfaces for core dependencies (builds on this)
- #118: Convert static classes to injectable services (builds on this)
- #119: Update coding standards for DI patterns
