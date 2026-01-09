# ADR-0002: Testing Framework

## Status

Proposed

## Date

2026-01-09

## Context

We need a testing framework for the ClaudeAutoResume project that supports:

1. Unit testing with clear, readable assertions
2. Integration with .NET 10
3. Good IDE and CI tooling support
4. Open source licensing
5. Active maintenance and community support

### Options Considered

#### Option 1: xUnit + AwesomeAssertions (Selected)

**xUnit:**
- Most popular .NET testing framework
- Built-in parallel test execution
- Extensible with traits and custom attributes
- First-class .NET support

**AwesomeAssertions:**
- Community fork of FluentAssertions (after licensing concerns)
- Fluent assertion API for readable tests
- Extensive assertion methods for collections, exceptions, etc.
- Open source (Apache 2.0)

**Pros:**
- Industry standard combination
- Excellent tooling support (VS, Rider, CLI)
- xunit.analyzers enforces best practices
- AwesomeAssertions provides readable assertions
- Both actively maintained OSS projects

**Cons:**
- Two packages instead of built-in assertions
- Learning curve for fluent assertion syntax

#### Option 2: NUnit + AwesomeAssertions

**Pros:**
- Mature framework with long history
- Rich attribute-based configuration
- Good constraint-based assertions built-in

**Cons:**
- Less popular in modern .NET projects
- Slightly more verbose setup
- Parallel execution requires more configuration

#### Option 3: MSTest + AwesomeAssertions

**Pros:**
- Microsoft's official test framework
- Tight Visual Studio integration

**Cons:**
- Less popular in OSS community
- Fewer community extensions
- Less flexible than xUnit

#### Option 4: xUnit with Built-in Assertions Only

**Pros:**
- No additional dependencies
- Simpler setup

**Cons:**
- Less readable assertions
- Verbose failure messages
- Limited collection assertions

## Decision

We will use **xUnit** as the test framework with **AwesomeAssertions** for fluent assertions and **xunit.analyzers** for test best practices enforcement.

### Package Configuration

```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageVersion Include="xunit" Version="2.9.0" />
<PackageVersion Include="xunit.runner.visualstudio" Version="2.8.0" />
<PackageVersion Include="xunit.analyzers" Version="1.18.0" />
<PackageVersion Include="AwesomeAssertions" Version="8.0.0" />
<PackageVersion Include="coverlet.collector" Version="6.0.2" />
```

### Test Project Structure

```
tests/
└── ClaudeAutoResume.Tests/
    ├── ClaudeAutoResume.Tests.csproj
    ├── GlobalUsings.cs
    ├── Unit/
    │   ├── WrapperConfigTests.cs
    │   └── ClaudeMonitorTests.cs
    └── Integration/
        └── (future integration tests)
```

### Example Test

```csharp
public class WrapperConfigTests
{
    [Fact]
    public void Default_ShouldHaveExpectedRateLimitPatterns()
    {
        // Arrange & Act
        var config = WrapperConfig.Default;

        // Assert
        config.RateLimitPatterns
            .Should().Contain("limit reached")
            .And.Contain("rate limit");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WaitMinutes_ShouldRejectInvalidValues(int invalidMinutes)
    {
        // Arrange & Act
        var act = () => WrapperConfig.Default with { WaitMinutes = invalidMinutes };

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

## Consequences

### Positive

- Readable, maintainable tests with fluent assertions
- xunit.analyzers catches common test mistakes at compile time
- Excellent CI integration with standard `dotnet test`
- Code coverage via Coverlet integrates with CI
- Industry-standard tooling knowledge transfers across projects

### Negative

- Additional dependency on AwesomeAssertions
- Team must learn fluent assertion patterns
- xunit.analyzers may flag existing patterns as issues

### Risks

- AwesomeAssertions is a fork; monitor for divergence from FluentAssertions ecosystem
- xUnit major version updates may require test refactoring

## References

- [xUnit Documentation](https://xunit.net/)
- [AwesomeAssertions GitHub](https://github.com/AwesomeAssertions/AwesomeAssertions)
- [xunit.analyzers Rules](https://xunit.net/xunit.analyzers/rules/)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)

## License Verification

| Package | License | Verification Date |
|---------|---------|-------------------|
| xunit | Apache 2.0 | 2026-01-09 |
| xunit.analyzers | Apache 2.0 | 2026-01-09 |
| AwesomeAssertions | Apache 2.0 | 2026-01-09 |
| coverlet | MIT | 2026-01-09 |
