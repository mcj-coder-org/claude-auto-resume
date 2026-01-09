---
title: Security Examples
summary: Detailed examples for input validation, process spawning, and secret handling
parent: ../coding-standards.md
---

# Security Examples

## Input Validation

```csharp
public void ProcessUserInput(string input)
{
    ArgumentException.ThrowIfNullOrEmpty(input);

    // Validate length
    if (input.Length > MaxInputLength)
    {
        throw new ArgumentException($"Input exceeds maximum length of {MaxInputLength}");
    }

    // Validate content
    if (!IsValidInput(input))
    {
        throw new ArgumentException("Input contains invalid characters");
    }
}
```

## Safe Process Spawning

```csharp
// Bad - command injection vulnerability
var process = Process.Start("cmd", $"/c {userInput}");

// Good - use argument array (auto-escapes)
var startInfo = new ProcessStartInfo
{
    FileName = "claude",
    UseShellExecute = false,
};
startInfo.ArgumentList.Add("--option");
startInfo.ArgumentList.Add(userInput); // Properly escaped

var process = Process.Start(startInfo);
```

## Secret Handling

```csharp
// Bad - logs sensitive data
_logger.LogDebug("API key: {ApiKey}", apiKey);

// Good - mask sensitive data
_logger.LogDebug("API key configured: {HasApiKey}", !string.IsNullOrEmpty(apiKey));
```

## Path Validation

```csharp
public void ProcessFile(string userPath)
{
    // Resolve to absolute path
    var fullPath = Path.GetFullPath(userPath);

    // Ensure within allowed directory
    var allowedRoot = Path.GetFullPath(_configDirectory);
    if (!fullPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
    {
        throw new UnauthorizedAccessException("Access to path denied");
    }
}
```

## Exception Messages

```csharp
// Bad - exposes internal details
throw new Exception($"Database error: {sqlException.Message}");

// Good - generic message, log details internally
_logger.LogError(sqlException, "Database operation failed");
throw new InvalidOperationException("Operation failed. Please try again.");
```
