---
title: Naming Convention Examples
summary: Detailed examples and anti-patterns for C# naming conventions
parent: ../coding-standards.md
---

# Naming Convention Examples

## Full Convention Table

| Element         | Convention                | Example                               |
| --------------- | ------------------------- | ------------------------------------- |
| Namespaces      | PascalCase, dot-separated | `McjCoderOrg.ClaudeAutoResume`        |
| Classes         | PascalCase, noun          | `RateLimitDetector`                   |
| Interfaces      | IPascalCase               | `IRateLimitDetector`                  |
| Records         | PascalCase                | `RateLimitInfo`                       |
| Structs         | PascalCase                | `TimeRange`                           |
| Enums           | PascalCase                | `ExitCode`                            |
| Enum values     | PascalCase                | `ConfigurationError`                  |
| Methods         | PascalCase, verb          | `DetectRateLimit`                     |
| Async methods   | PascalCase + Async        | `DetectRateLimitAsync`                |
| Properties      | PascalCase                | `MaxRetries`                          |
| Events          | PascalCase                | `RateLimitDetected`                   |
| Public fields   | PascalCase                | `MaxValue` (rare - prefer properties) |
| Private fields  | \_camelCase               | `_retryCount`                         |
| Parameters      | camelCase                 | `retryDelay`                          |
| Local variables | camelCase                 | `isLimited`                           |
| Constants       | PascalCase                | `DefaultTimeout`                      |
| Type parameters | TPascalCase               | `TResult`, `TKey`                     |

## Meaningful Names

```csharp
// Good
int retryDelayMilliseconds;
bool isRateLimited;

// Bad
int rd;
bool flag;
```

## No Hungarian Notation

```csharp
// Good
string name;
int count;

// Bad
string strName;
int intCount;
```

## Consistent Terminology

Use project-specific terms consistently:

- `RateLimit` not `ThrottleLimit` or `UsageLimit`
- `Retry` not `Attempt` or `Try`

## Boolean Naming

```csharp
// Good - questions
bool isEnabled;
bool hasValue;
bool canRetry;
bool shouldContinue;

// Bad
bool enabled; // Unclear if state or action
bool retry;   // Verb, not question
```
