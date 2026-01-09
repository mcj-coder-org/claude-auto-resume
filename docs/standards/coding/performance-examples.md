---
title: Performance Examples
summary: Detailed examples for avoiding allocations and optimising hot paths
parent: ../coding-standards.md
---

# Performance Examples

## Avoid Allocations in Hot Paths

```csharp
// Bad - allocates on every call
public bool IsMatch(string input)
{
    var pattern = new Regex(@"limit.*reached");
    return pattern.IsMatch(input);
}

// Good - reuse compiled regex
private static readonly Regex LimitPattern = new(@"limit.*reached", RegexOptions.Compiled);

public bool IsMatch(string input)
{
    return LimitPattern.IsMatch(input);
}
```

## Use Span for Slicing

```csharp
// Good - no allocation
ReadOnlySpan<char> span = input.AsSpan();
if (span.StartsWith("prefix"))
{
    ProcessSpan(span[7..]); // Slice without allocation
}
```

## String Operations

```csharp
// Use StringBuilder for concatenation in loops
var builder = new StringBuilder();
foreach (var item in items)
{
    builder.Append(item);
    builder.Append(separator);
}

// Use string.Create for known-length strings
var result = string.Create(10, seed, (span, state) =>
{
    // Fill span directly
});
```

## Collection Initialisation

```csharp
// Good - specify capacity when known
var list = new List<string>(expectedCount);
var dict = new Dictionary<string, int>(expectedCount);

// Good - use collection expressions
int[] numbers = [1, 2, 3, 4, 5];
List<string> items = ["a", "b", "c"];
```

## Avoid LINQ in Hot Paths

```csharp
// Slower - LINQ overhead
var first = items.FirstOrDefault(x => x.Id == id);

// Faster for hot paths
Item? first = null;
foreach (var item in items)
{
    if (item.Id == id)
    {
        first = item;
        break;
    }
}
```

## Object Pooling

```csharp
// Use ArrayPool for temporary arrays
var pool = ArrayPool<byte>.Shared;
var buffer = pool.Rent(1024);
try
{
    // Use buffer
}
finally
{
    pool.Return(buffer);
}
```
