---
title: Testing Examples
summary: Detailed examples for unit tests, BDD scenarios, and assertions
parent: ../coding-standards.md
---

# Testing Examples

## Test Project Structure

```text
tests/
├── McjCoderOrg.ClaudeAutoResume.Tests/           # Unit tests
│   ├── ClaudeMonitorTests.cs
│   └── WrapperConfigTests.cs
├── McjCoderOrg.ClaudeAutoResume.SystemTests/     # BDD system tests
│   ├── Features/
│   │   └── RateLimitDetection.feature
│   └── StepDefinitions/
│       └── RateLimitDetectionSteps.cs
└── McjCoderOrg.ClaudeAutoResume.ArchTests/       # Architecture tests
    └── DependencyTests.cs
```

## Test Naming

Format: `MethodName_Scenario_ExpectedBehaviour`

```csharp
[Fact]
public void IsRateLimited_WhenLimitMessagePresent_ReturnsTrue()

[Fact]
public void LoadConfig_WhenFileMissing_ReturnsDefaults()

[Fact]
public async Task ProcessAsync_WhenCancelled_ThrowsOperationCancelledException()
```

## Arrange-Act-Assert Pattern

```csharp
[Fact]
public void IsRateLimited_WhenLimitMessagePresent_ReturnsTrue()
{
    // Arrange
    var detector = new RateLimitDetector();
    var output = "Claude AI usage limit reached";

    // Act
    var result = detector.IsRateLimited(output);

    // Assert
    result.Should().BeTrue();
}
```

## AwesomeAssertions (FluentAssertions Fork)

```csharp
// Boolean
result.Should().BeTrue();
result.Should().BeFalse();

// Equality
value.Should().Be(42);
value.Should().NotBe(0);

// Strings
text.Should().Contain("expected");
text.Should().StartWith("prefix");
text.Should().BeEmpty();

// Collections
list.Should().HaveCount(3);
list.Should().Contain("item");
list.Should().BeEmpty();
list.Should().OnlyContain(x => x > 0);

// Exceptions
action.Should().Throw<ArgumentException>()
    .WithMessage("*invalid*");

action.Should().NotThrow();

// Async
await func.Should().ThrowAsync<InvalidOperationException>();
```

## BDD with Reqnroll

```gherkin
Feature: Rate Limit Detection
    As a user running extended Claude sessions
    I want the wrapper to detect rate limits
    So that my session can resume automatically

    Scenario: Detect rate limit in Claude output
        Given the Claude CLI is running
        When Claude outputs "Claude AI usage limit reached, resets at 3pm"
        Then the wrapper should detect a rate limit
        And the reset time should be "3pm"

    Scenario Outline: Detect various rate limit messages
        Given the Claude CLI is running
        When Claude outputs "<message>"
        Then the wrapper should detect a rate limit

        Examples:
            | message                                        |
            | Claude AI usage limit reached                  |
            | Rate limit exceeded, please wait               |
            | Usage limit reached for this session           |
```

## Coverage Guidelines

- **Target:** 80% line coverage, 70% branch coverage on changed code
- **Focus:** Critical paths and edge cases
- **Skip:** Trivial code (simple properties, pass-through methods)
