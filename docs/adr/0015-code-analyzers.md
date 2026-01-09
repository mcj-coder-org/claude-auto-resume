# ADR-0015: Code Analyzers

## Status

Proposed

## Date

2026-01-09

## Context

We need static analysis tooling to enforce code quality, security best practices, and consistent coding standards across the codebase. The analyzers should:

1. Catch bugs and security issues at compile time
2. Enforce consistent code style
3. Integrate with CI/CD pipeline
4. Support .NET 10 and C# 14
5. Be open source

### Options Considered

#### Option 1: Built-in .NET Analyzers Only

Using `AnalysisLevel=latest-all` enables Microsoft's built-in analyzers.

**Pros:**
- No additional dependencies
- Microsoft-supported
- Covers CA (code analysis) and IDE rules

**Cons:**
- Limited coverage compared to third-party analyzers
- Some important patterns not covered

#### Option 2: Comprehensive Analyzer Stack (Selected)

Combine built-in analyzers with third-party analyzers for comprehensive coverage.

**Selected Analyzers:**

| Analyzer | Focus Area | Rules |
|----------|------------|-------|
| Built-in (.NET) | Microsoft best practices | CA*, IDE* |
| Meziantou.Analyzer | Security, perf, practices | MA* |
| Roslynator.Analyzers | Code quality, refactoring | RCS* |
| SonarAnalyzer.CSharp | Security, reliability | S* |
| xunit.analyzers | Test patterns | xUnit* |

**Pros:**
- Comprehensive coverage across multiple domains
- Catches issues that individual analyzers miss
- Active maintenance on all packages
- All open source

**Cons:**
- More dependencies
- Potential rule conflicts (mitigated by configuration)
- Longer build times (minimal impact)

#### Option 3: SonarQube/SonarCloud Integration

External SonarQube server or SonarCloud service.

**Pros:**
- Centralized quality dashboard
- Historical trends
- Quality gates

**Cons:**
- External service dependency
- Additional infrastructure (self-hosted)
- Cost (SonarCloud for private repos)
- Overkill for single project

## Decision

We will use a **comprehensive analyzer stack** combining built-in .NET analyzers with Meziantou.Analyzer, Roslynator.Analyzers, and SonarAnalyzer.CSharp. Test projects additionally include xunit.analyzers.

### Configuration

**Directory.Build.props:**

```xml
<Project>
  <PropertyGroup>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Meziantou.Analyzer" PrivateAssets="all" />
    <PackageReference Include="Roslynator.Analyzers" PrivateAssets="all" />
    <PackageReference Include="SonarAnalyzer.CSharp" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

**Directory.Packages.props:**

```xml
<PackageVersion Include="Meziantou.Analyzer" Version="2.0.182" />
<PackageVersion Include="Roslynator.Analyzers" Version="4.12.9" />
<PackageVersion Include="SonarAnalyzer.CSharp" Version="10.4.0.108396" />
<PackageVersion Include="xunit.analyzers" Version="1.18.0" />
```

### Severity Configuration

Configure in `.editorconfig`:

```ini
[*.cs]
# Disable rules that conflict with project conventions
dotnet_diagnostic.CA1062.severity = none  # Nullable handles this
dotnet_diagnostic.CA2007.severity = none  # ConfigureAwait not needed in apps

# Elevate important rules to errors
dotnet_diagnostic.CA2100.severity = error  # SQL injection
dotnet_diagnostic.CA2300.severity = error  # Insecure deserialization
```

### Analyzer Coverage

| Category | Analyzers | Examples |
|----------|-----------|----------|
| Security | CA, S, MA | SQL injection, XSS, insecure crypto |
| Performance | CA, RCS, MA | Allocations, async patterns |
| Reliability | S, CA | Null handling, disposal, exceptions |
| Maintainability | RCS, IDE | Complexity, naming, dead code |
| Style | IDE, RCS | Formatting, modern syntax |
| Testing | xUnit | Test patterns, assertion usage |

## Consequences

### Positive

- Comprehensive bug and security detection at compile time
- Consistent code quality across the codebase
- Educational: developers learn best practices from analyzer messages
- CI enforcement: warnings-as-errors prevents quality degradation
- IDE integration provides real-time feedback

### Negative

- Initial cleanup effort to fix existing violations
- Some false positives require suppression with justification
- Build time slightly increased (typically <5%)
- Team must understand when suppressions are appropriate

### Risks

- Over-suppression can negate analyzer benefits
- Rule conflicts between analyzers (rare, configure as needed)
- Major analyzer updates may introduce new violations

## References

- [.NET Code Analysis](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview)
- [Meziantou.Analyzer GitHub](https://github.com/meziantou/Meziantou.Analyzer)
- [Roslynator GitHub](https://github.com/dotnet/roslynator)
- [SonarAnalyzer GitHub](https://github.com/SonarSource/sonar-dotnet)

## License Verification

| Package | License | Verification Date |
|---------|---------|-------------------|
| Meziantou.Analyzer | MIT | 2026-01-09 |
| Roslynator.Analyzers | Apache 2.0 | 2026-01-09 |
| SonarAnalyzer.CSharp | LGPL 3.0 | 2026-01-09 |
| xunit.analyzers | Apache 2.0 | 2026-01-09 |
