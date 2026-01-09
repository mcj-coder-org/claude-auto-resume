# ADR-0014: Test Project Structure and BDD Framework

## Status

Proposed

## Date

2026-01-09

## Context

We need a comprehensive testing strategy that covers:

1. **Unit tests** - Fast, isolated tests for individual components
2. **System tests** - End-to-end tests with mocked/stubbed external dependencies
3. **E2E tests** - Production-safe smoke tests against real systems

System and E2E tests should be written in BDD (Behavior-Driven Development) style for:

- Readable specifications that serve as documentation
- Clear Given/When/Then structure
- Collaboration between technical and non-technical stakeholders

### Options Considered for BDD Framework

#### Option 1: Reqnroll (Selected)

Community fork of SpecFlow after licensing concerns.

**Pros:**

- Full Gherkin syntax support (.feature files)
- Active OSS development (Apache 2.0)
- SpecFlow-compatible API (easy migration)
- IDE support (VS, Rider plugins)
- Living documentation generation
- Parallel test execution support

**Cons:**

- Newer than SpecFlow (less battle-tested)
- Requires feature file management

#### Option 2: SpecFlow

Original Gherkin-based BDD framework.

**Pros:**

- Most widely adopted in .NET
- Extensive documentation
- Mature ecosystem

**Cons:**

- Licensing concerns (custom license)
- Commercial features gated
- Not fully open source

#### Option 3: LightBDD

Code-first BDD without Gherkin files.

**Pros:**

- No feature file management
- Refactoring-friendly
- Lighter weight

**Cons:**

- No Gherkin syntax
- Less readable for non-developers
- Smaller community

#### Option 4: xBehave.net

xUnit extension for BDD.

**Pros:**

- Lightweight
- xUnit integration
- Code-based scenarios

**Cons:**

- Less structured than Gherkin
- No living documentation
- Limited adoption

## Decision

We will use **Reqnroll** for BDD-style tests and organize tests into three projects:

### Test Project Structure

```text
tests/
├── ClaudeAutoResume.Tests/           # Unit tests
│   ├── Unit/
│   └── ClaudeAutoResume.Tests.csproj
├── ClaudeAutoResume.SystemTests/     # System tests (mocked dependencies)
│   ├── Features/
│   ├── StepDefinitions/
│   ├── Hooks/
│   ├── Support/
│   └── ClaudeAutoResume.SystemTests.csproj
└── ClaudeAutoResume.E2ETests/        # E2E smoke tests (real dependencies)
    ├── Features/
    ├── StepDefinitions/
    ├── Hooks/
    ├── Support/
    └── ClaudeAutoResume.E2ETests.csproj
```

### Test Categories

| Project          | Purpose                          | Dependencies                   | Speed              | CI Frequency   |
| ---------------- | -------------------------------- | ------------------------------ | ------------------ | -------------- |
| **Unit Tests**   | Isolated component testing       | None (mocks only)              | Fast (<1s each)    | Every commit   |
| **System Tests** | End-to-end with mocked externals | Mocked PTY, stubbed processes  | Medium (<10s each) | Every commit   |
| **E2E Tests**    | Production-safe smoke tests      | Real Claude CLI (if available) | Slow (<60s each)   | Nightly/manual |

### Package Configuration

**Directory.Packages.props (additions):**

```xml
<!-- BDD Testing -->
<PackageVersion Include="Reqnroll" Version="2.0.3" />
<PackageVersion Include="Reqnroll.xUnit" Version="2.0.3" />

<!-- Mocking -->
<PackageVersion Include="Moq" Version="4.20.72" />
<PackageVersion Include="Moq.Analyzers" Version="0.0.9" />
```

### Unit Test Project

**ClaudeAutoResume.Tests.csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="xunit.analyzers" PrivateAssets="all" />
    <PackageReference Include="AwesomeAssertions" />
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="Moq" />
    <PackageReference Include="Moq.Analyzers" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\ClaudeAutoResume\ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

### System Test Project

**ClaudeAutoResume.SystemTests.csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="Reqnroll" />
    <PackageReference Include="Reqnroll.xUnit" />
    <PackageReference Include="AwesomeAssertions" />
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="Moq" />
    <PackageReference Include="Moq.Analyzers" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\ClaudeAutoResume\ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

### E2E Test Project

**ClaudeAutoResume.E2ETests.csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="Reqnroll" />
    <PackageReference Include="Reqnroll.xUnit" />
    <PackageReference Include="AwesomeAssertions" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\ClaudeAutoResume\ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

### Example Feature File (System Test)

**Features/RateLimitDetection.feature:**

```gherkin
Feature: Rate Limit Detection
  As a user running Claude Code for extended sessions
  I want the wrapper to detect rate limits automatically
  So that my work can continue after the limit resets

  Background:
    Given the Claude CLI is mocked
    And the wrapper is configured with default settings

  Scenario: Detect rate limit message in output
    Given Claude outputs "Claude AI usage limit reached"
    When the wrapper processes the output
    Then the wrapper should detect a rate limit
    And the wrapper should start waiting

  Scenario: Continue after rate limit wait
    Given a rate limit has been detected
    And the wait period has elapsed
    When the wrapper resumes
    Then the wrapper should send a continue command
    And normal operation should resume

  Scenario Outline: Detect various rate limit patterns
    Given Claude outputs "<pattern>"
    When the wrapper processes the output
    Then the wrapper should detect a rate limit

    Examples:
      | pattern                        |
      | Claude AI usage limit reached  |
      | rate limit exceeded            |
      | too many requests              |
      | quota exceeded                 |
```

### Example Feature File (E2E Test)

**Features/SmokeTests.feature:**

```gherkin
@production-safe
Feature: Production Smoke Tests
  As an operator
  I want to verify the wrapper starts correctly
  So that I can confirm deployments are healthy

  @requires-claude-cli
  Scenario: Wrapper starts and shows version
    Given the Claude CLI is installed
    When I run the wrapper with "--help"
    Then the exit code should be 0
    And the output should contain "claude-auto-resume"

  @skip-if-no-claude
  Scenario: Wrapper connects to Claude
    Given the Claude CLI is installed
    When I start the wrapper in headless mode with a simple prompt
    Then the wrapper should start without errors
    And Claude should respond within 30 seconds
```

### Step Definition Example

**StepDefinitions/RateLimitSteps.cs:**

```csharp
namespace ClaudeAutoResume.SystemTests.StepDefinitions;

[Binding]
public class RateLimitSteps
{
    private readonly MockPtyConnection _mockPty;
    private readonly ClaudeMonitor _monitor;
    private bool _rateLimitDetected;

    public RateLimitSteps()
    {
        _mockPty = new MockPtyConnection();
        _monitor = new ClaudeMonitor(WrapperConfig.Default);
    }

    [Given(@"the Claude CLI is mocked")]
    public void GivenTheClaudeCliIsMocked()
    {
        // Setup mock PTY
    }

    [Given(@"Claude outputs ""(.*)""")]
    public void GivenClaudeOutputs(string output)
    {
        _mockPty.SimulateOutput(output);
    }

    [When(@"the wrapper processes the output")]
    public async Task WhenTheWrapperProcessesTheOutput()
    {
        await _monitor.ProcessOutputAsync(_mockPty.GetOutput());
    }

    [Then(@"the wrapper should detect a rate limit")]
    public void ThenTheWrapperShouldDetectARateLimit()
    {
        _rateLimitDetected.Should().BeTrue();
    }
}
```

### Test Execution Strategy

**CI Pipeline:**

```yaml
# Run unit and system tests on every commit
- name: Run Unit Tests
  run: dotnet test tests/ClaudeAutoResume.Tests --filter "Category!=Integration"

- name: Run System Tests
  run: dotnet test tests/ClaudeAutoResume.SystemTests

# Run E2E tests nightly or manually
- name: Run E2E Tests
  if: github.event_name == 'schedule' || github.event_name == 'workflow_dispatch'
  run: dotnet test tests/ClaudeAutoResume.E2ETests
```

**Pre-push hook (fast tests only):**

```bash
dotnet test --filter "Category!=Integration & Category!=E2E"
```

## Consequences

### Positive

- Clear separation of test concerns
- BDD features serve as living documentation
- System tests provide confidence without external dependencies
- E2E tests validate production behavior safely
- Reqnroll's Gherkin syntax is readable by non-developers
- Moq provides familiar mocking syntax widely used in .NET

### Negative

- Three test projects to maintain
- Feature files require synchronization with step definitions
- Reqnroll has learning curve for team
- E2E tests may be flaky if Claude CLI changes

### Risks

- Feature file drift from implementation
- Mock fidelity may not match real behavior
- E2E tests depend on Claude CLI availability

## References

- [Reqnroll Documentation](https://docs.reqnroll.net/)
- [Reqnroll GitHub](https://github.com/reqnroll/Reqnroll)
- [Moq Documentation](https://github.com/moq/moq)
- [Gherkin Syntax Reference](https://cucumber.io/docs/gherkin/reference/)

## License Verification

| Package        | License    | Verification Date |
| -------------- | ---------- | ----------------- |
| Reqnroll       | Apache 2.0 | 2026-01-09        |
| Reqnroll.xUnit | Apache 2.0 | 2026-01-09        |
| Moq            | BSD-3      | 2026-01-09        |
| Moq.Analyzers  | MIT        | 2026-01-09        |
