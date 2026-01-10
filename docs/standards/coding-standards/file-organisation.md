---
title: File Organisation Examples
summary: Detailed examples for C# file structure and member ordering
audience: [developer, agent]
topics: [csharp, file-structure, conventions]
parent: ../coding-standards.md
last_validated: 2026-01-10
---

# File Organisation Examples

## File Structure

```csharp
// 1. File-scoped namespace (required)
namespace McjCoderOrg.ClaudeAutoResume;

// 2. Using directives (sorted, System first)
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

// 3. Type declaration (one primary type per file)
public sealed class ExampleClass
{
    // Members ordered as below
}
```

## Member Ordering Example

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

    // 7. Methods (public first, then private)
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
```

## Complete Member Order

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

## Accessibility Order (within each group)

1. public
2. internal
3. protected internal
4. protected
5. private protected
6. private
