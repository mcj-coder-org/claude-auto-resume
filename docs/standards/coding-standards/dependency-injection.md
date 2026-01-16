# Dependency Injection Patterns

Patterns and examples for dependency injection in the project.

## Constructor Injection

Prefer constructor injection for all dependencies:

```csharp
internal sealed class ClaudeMonitor : IClaudeMonitor
{
    private readonly IConsoleService _console;
    private readonly IEnvironmentService _environment;
    private readonly ILogger _logger;

    public ClaudeMonitor(
        IConsoleService console,
        IEnvironmentService environment,
        ILogger? logger = null)
    {
        _console = console;
        _environment = environment;
        _logger = logger ?? Log.Logger;
    }
}
```

**Benefits:**

- Dependencies are explicit and visible
- Immutable after construction
- Easy to mock in tests
- Compiler enforces required dependencies

## Interface Design

### Design Principles

- One interface per responsibility
- Prefer small, focused interfaces (ISP)
- Name interfaces after what they do, not how

### Example: Console Abstraction

```csharp
internal interface IConsoleService
{
    void Write(string value);
    void WriteLine(string message);
    void WriteErrorLine(string message);
    ConsoleColor ForegroundColor { get; set; }
    void ResetColor();
    bool IsInputRedirected { get; }
    int WindowWidth { get; }
    int WindowHeight { get; }
}

internal sealed class ConsoleService : IConsoleService
{
    public void Write(string value) => Console.Write(value);
    public void WriteLine(string message) => Console.WriteLine(message);
    // ... rest of implementation
}
```

## Service Registration

Register services in Program.cs or dedicated extension methods:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IConsoleService, ConsoleService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<IArgumentParser, ArgumentParser>();
        services.AddSingleton<IApplication, Application>();
    })
    .Build();
```

### Service Lifetimes

| Lifetime  | Registration           | When to Use                       |
| --------- | ---------------------- | --------------------------------- |
| Singleton | `AddSingleton<I, T>()` | Stateless services, shared state  |
| Scoped    | `AddScoped<I, T>()`    | Per-request state (web scenarios) |
| Transient | `AddTransient<I, T>()` | Lightweight, stateful, disposable |

### Choosing Lifetime

- **Singleton**: `IConsoleService`, `IEnvironmentService` - wrap static APIs
- **Transient**: `ClaudeMonitor` - stateful, disposable per use
- **Scoped**: Database contexts, HTTP clients per request

## Testing with Mocked Dependencies

```csharp
public sealed class ClaudeMonitorTests : IDisposable
{
    private readonly Mock<IConsoleService> _mockConsole;
    private readonly Mock<IEnvironmentService> _mockEnvironment;
    private readonly ClaudeMonitor _monitor;

    public ClaudeMonitorTests()
    {
        _mockConsole = new Mock<IConsoleService>();
        _mockEnvironment = new Mock<IEnvironmentService>();

        // Setup default mock behavior
        _mockConsole.Setup(c => c.WindowWidth).Returns(120);
        _mockEnvironment.Setup(e => e.CurrentDirectory).Returns("/test");

        _monitor = new ClaudeMonitor(
            WrapperConfig.Default,
            _mockConsole.Object,
            _mockEnvironment.Object);
    }

    [Fact]
    public void Method_Scenario_ExpectedBehaviour()
    {
        // Arrange - setup specific mock behavior
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH"))
            .Returns("/usr/bin");

        // Act
        var result = _monitor.SomeMethod();

        // Assert
        result.Should().BeExpectedValue();
        _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Once);
    }
}
```

## Anti-Patterns to Avoid

### Static Dependencies

```csharp
// BAD: Hidden static dependency
public class Service
{
    public void DoWork()
    {
        var path = Environment.GetEnvironmentVariable("PATH");  // Static!
        Console.WriteLine("Working...");  // Static!
    }
}

// GOOD: Injected dependencies
public class Service
{
    private readonly IEnvironmentService _environment;
    private readonly IConsoleService _console;

    public Service(IEnvironmentService environment, IConsoleService console)
    {
        _environment = environment;
        _console = console;
    }

    public void DoWork()
    {
        var path = _environment.GetEnvironmentVariable("PATH");
        _console.WriteLine("Working...");
    }
}
```

### Service Locator

```csharp
// BAD: Service locator pattern
public class Service
{
    public void DoWork(IServiceProvider provider)
    {
        var console = provider.GetRequiredService<IConsoleService>();
        console.WriteLine("Working...");
    }
}

// GOOD: Constructor injection
public class Service
{
    private readonly IConsoleService _console;

    public Service(IConsoleService console)
    {
        _console = console;
    }

    public void DoWork()
    {
        _console.WriteLine("Working...");
    }
}
```

### Concrete Dependencies

```csharp
// BAD: Depends on concrete class
public class Application
{
    private readonly ClaudeMonitor _monitor;  // Concrete!

    public Application()
    {
        _monitor = new ClaudeMonitor(config);  // Can't mock!
    }
}

// GOOD: Depends on interface
public class Application
{
    private readonly IClaudeMonitor _monitor;

    public Application(IClaudeMonitor monitor)
    {
        _monitor = monitor;
    }
}
```

## When Static is Acceptable

### Extension Methods

```csharp
public static class StringExtensions
{
    public static string Truncate(this string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
```

### Pure Functions

```csharp
public static class MathUtilities
{
    public static int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}
```

### Bootstrap Code

```csharp
// Before DI container is available
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.File(LoggingConfiguration.GetLogFilePath())
    .CreateBootstrapLogger();
```

## Related

- [Microsoft DI Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [ADR-0022: Dependency Injection Foundation](../adr/0022-dependency-injection-foundation.md)
