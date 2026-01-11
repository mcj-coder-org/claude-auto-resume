---
name: internationalization
description: |
  When adding user-facing strings or planning localization. Apply when creating resource files,
  formatting messages with named parameters, or structuring for future translations.
decision: Use i18n-ready structure with resource files and named parameters, English only initially.
status: accepted
---

# ADR-0019: Internationalization

## Status

Proposed

## Date

2026-01-09

## Context

We need to decide on internationalization (i18n) strategy.

### Requirements

- English as primary language
- Structure that supports future translations
- Minimal overhead for initial release

### Options Considered

#### Option 1: i18n-Ready English Only (Selected)

Structure for i18n but only English strings.

**Pros:**

- No translation overhead
- Ready for future localization
- Named parameters in strings

**Cons:**

- No immediate multi-language support

#### Option 2: Full i18n from Start

Multiple languages from day one.

**Pros:**

- Immediate global reach

**Cons:**

- Translation maintenance burden
- Delays initial release

#### Option 3: No i18n Structure

Hardcoded English strings.

**Pros:**

- Simplest implementation

**Cons:**

- Difficult to add translations later
- Technical debt

## Decision

**i18n-ready structure with English only** initially.

### Implementation

1. All user-facing strings in resource files
2. Named parameters for structured logging: `{ResetTime}`, `{WaitMinutes}`
3. en-GB as default locale
4. Structure allows adding translations without code changes

### Resource Files

```text
src/McjCoderOrg.ClaudeAutoResume/
└── Resources/
    ├── Strings.resx           # Default (en-GB)
    └── Strings.en-US.resx     # US English (future)
```

### String Format

```csharp
// Resources/Strings.resx
RateLimitDetected = "Detected Session Limit Reached, resets at {ResetTime}"
WaitingForReset = "Waiting {WaitMinutes} minutes for rate limit reset"
```

## Consequences

### Positive

- Ready for future localization
- Consistent string management
- Named parameters aid translation

### Negative

- Slight overhead vs hardcoded strings
- Resource file maintenance

## References

- [.NET Globalization](https://docs.microsoft.com/en-us/dotnet/core/extensions/globalization)
- [Resource Files](https://docs.microsoft.com/en-us/dotnet/core/extensions/resources)
