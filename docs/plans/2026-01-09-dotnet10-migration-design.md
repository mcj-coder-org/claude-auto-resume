# McjCoderOrg.ClaudeAutoResume .NET 10 Migration Design

## Overview

Migrate ClaudeAutoResume from a single-file .NET 8 project to a best-practice .NET 10 solution that serves as an organizational template. The solution demonstrates professional-grade tooling, testing, documentation, and developer experience patterns.

**Organization Prefix:** `McjCoderOrg` - applied to solution, projects, and packages for brand consistency and NuGet uniqueness.

## Goals

1. Upgrade to .NET 10 with C# 14 language features
2. Establish best-practice solution structure with src/tests separation
3. Comprehensive testing: unit, system (BDD), E2E (BDD), architecture, benchmarks, mutation
4. Security-first: secret scanning, SAST, dependency vulnerability detection
5. Full CI/CD with semantic versioning and automated releases
6. Code quality enforcement through analyzers, formatting, and coverage
7. Professional documentation: GitHub Pages site, versioned docs, agent-friendly structure
8. Excellent developer experience: dev containers, bootstrap scripts, diagnostics
9. CLI best practices: layered configuration, semantic exit codes, i18n-ready

## Non-Goals

- Functional changes to the application logic
- Monorepo structure (single project, but tooling supports future expansion)
- Long-lived release branches (GitHub Flow only)
- Telemetry or usage tracking (privacy-first)

---

## Solution Structure

```
McjCoderOrg.ClaudeAutoResume/
├── src/
│   └── McjCoderOrg.ClaudeAutoResume/
│       ├── McjCoderOrg.ClaudeAutoResume.csproj
│       ├── Program.cs
│       ├── ClaudeMonitor.cs
│       ├── WrapperConfig.cs
│       └── Resources/                    # i18n-ready string resources
├── tests/
│   ├── McjCoderOrg.ClaudeAutoResume.Tests/           # Unit tests
│   ├── McjCoderOrg.ClaudeAutoResume.SystemTests/     # BDD system tests
│   ├── McjCoderOrg.ClaudeAutoResume.E2ETests/        # BDD E2E tests
│   ├── McjCoderOrg.ClaudeAutoResume.ArchTests/       # Architecture tests
│   └── McjCoderOrg.ClaudeAutoResume.Benchmarks/      # Performance benchmarks
├── docs/
│   ├── docusaurus/                       # Documentation website source
│   ├── standards/                        # Coding standards, conventions
│   ├── practices/                        # Workflows, processes
│   ├── playbooks/                        # Runbooks (mutation testing, etc.)
│   ├── agents/                           # Agent-specific documentation
│   ├── adr/                              # Architecture Decision Records
│   └── plans/                            # Design documents
├── scripts/
│   ├── setup.ps1                         # Windows bootstrap
│   └── setup.sh                          # Unix bootstrap
├── .devcontainer/
│   └── devcontainer.json                 # Dev container configuration
├── .github/
│   ├── workflows/                        # CI/CD pipelines
│   ├── ISSUE_TEMPLATE/                   # Issue templates
│   ├── PULL_REQUEST_TEMPLATE.md
│   ├── CODEOWNERS
│   ├── SECURITY.md
│   ├── CONTRIBUTING.md
│   └── dependabot.yml
├── AGENTS.md                             # Agent orientation & routing
├── McjCoderOrg.ClaudeAutoResume.sln
├── Directory.Build.props
├── Directory.Packages.props
├── .editorconfig
├── .gitattributes
├── .prettierrc
├── .cspell.json                          # Spellcheck config (en-GB)
├── .secretlintrc.json                    # Secret scanning config
├── .markdownlint.json                    # Markdown linting config
├── coverlet.runsettings                  # Coverage config
├── stryker-config.json                   # Mutation testing config
├── GitVersion.yml
├── cliff.toml
├── commitlint.config.js
├── package.json
├── LICENSE
├── CHANGELOG.md
└── README.md
```

---

## Project Configuration

### Naming & Namespace Convention

| Item | Convention | Example |
|------|------------|---------|
| Repository | `{org}.{project}` | `McjCoderOrg.ClaudeAutoResume` |
| Solution file | `{org}.{project}.sln` | `McjCoderOrg.ClaudeAutoResume.sln` |
| Main project | `{org}.{project}` | `McjCoderOrg.ClaudeAutoResume` |
| Unit tests | `{org}.{project}.Tests` | `McjCoderOrg.ClaudeAutoResume.Tests` |
| System tests | `{org}.{project}.SystemTests` | `McjCoderOrg.ClaudeAutoResume.SystemTests` |
| E2E tests | `{org}.{project}.E2ETests` | `McjCoderOrg.ClaudeAutoResume.E2ETests` |
| Architecture tests | `{org}.{project}.ArchTests` | `McjCoderOrg.ClaudeAutoResume.ArchTests` |
| Benchmarks | `{org}.{project}.Benchmarks` | `McjCoderOrg.ClaudeAutoResume.Benchmarks` |
| NuGet Package | `{org}.{project}` | `McjCoderOrg.ClaudeAutoResume` |

### Namespace Strategy

Test projects omit their suffix in `RootNamespace`, placing tests in the same namespace as production code:

| Project | RootNamespace | Can Access Internals |
|---------|---------------|---------------------|
| Main | `McjCoderOrg.ClaudeAutoResume` | N/A |
| Tests | `McjCoderOrg.ClaudeAutoResume` | Yes |
| SystemTests | `McjCoderOrg.ClaudeAutoResume` | Yes |
| ArchTests | `McjCoderOrg.ClaudeAutoResume` | Yes |
| Benchmarks | `McjCoderOrg.ClaudeAutoResume` | Yes |
| E2ETests | `McjCoderOrg.ClaudeAutoResume.E2ETests` | No (public API only) |

### Directory.Build.props

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>14.0</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <Deterministic>true</Deterministic>
    <ContinuousIntegrationBuild Condition="'$(CI)' == 'true'">true</ContinuousIntegrationBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Meziantou.Analyzer" PrivateAssets="all" />
    <PackageReference Include="Roslynator.Analyzers" PrivateAssets="all" />
    <PackageReference Include="SonarAnalyzer.CSharp" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.PublicApiAnalyzers" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

### Main Project Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume</AssemblyName>

    <!-- .NET Tool -->
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>claude-auto-resume</ToolCommandName>
    <PackageId>McjCoderOrg.ClaudeAutoResume</PackageId>

    <!-- Source Link -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>

  <ItemGroup>
    <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.Tests" />
    <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.SystemTests" />
    <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.ArchTests" />
    <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.Benchmarks" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.SourceLink.GitHub" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

---

## Testing Strategy

### Test Project Matrix

| Project | Type | Purpose | Framework | Internals Access |
|---------|------|---------|-----------|------------------|
| `.Tests` | Unit | Isolated component tests | xUnit + Moq | Yes |
| `.SystemTests` | System | E2E with mocked externals | xUnit + Reqnroll (BDD) | Yes |
| `.E2ETests` | E2E | Production-safe smoke tests | xUnit + Reqnroll (BDD) | No |
| `.ArchTests` | Architecture | Dependency/structure rules | xUnit + NetArchTest | Yes |
| `.Benchmarks` | Performance | Regression detection | BenchmarkDotNet | Yes |

### Common Test Stack

- **Framework:** xUnit
- **Assertions:** AwesomeAssertions (FluentAssertions OSS fork)
- **Mocking:** Moq + Moq.Analyzers
- **BDD:** Reqnroll (SpecFlow OSS fork)
- **Coverage:** Coverlet
- **Mutation:** Stryker.NET
- **Architecture:** NetArchTest with slice support

### Unit Tests

Fast, isolated tests with mocked dependencies. Log output captured via Serilog test sink for behavior verification:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

public class RateLimitDetectorTests
{
    [Fact]
    public void Detect_WhenLimitReached_LogsExpectedMessage()
    {
        using var logCapture = new LogCapture();
        var detector = new RateLimitDetector();

        detector.Process("Claude AI usage limit reached");

        logCapture.Messages.Should()
            .Contain(m => m.Contains("Detected Session Limit Reached"));
    }
}
```

### System Tests (BDD)

End-to-end tests with mocked external dependencies (PTY, Claude CLI):

```gherkin
Feature: Rate Limit Detection
  As a user running Claude Code for extended sessions
  I want the wrapper to detect rate limits automatically
  So that my work can continue after the limit resets

  Background:
    Given the Claude CLI is mocked
    And the wrapper is configured with default settings

  Scenario: Detect rate limit and log reset time
    Given Claude outputs "Claude AI usage limit reached, resets at 3pm"
    When the wrapper processes the output
    Then the wrapper should detect a rate limit
    And the log should contain "Detected Session Limit Reached, resets at {ResetTime}"
```

### E2E Tests (BDD)

Production-safe smoke tests against real systems (when available):

```gherkin
@production-safe
Feature: CLI Smoke Tests

  Scenario: Wrapper shows help
    When I run "claude-auto-resume --help"
    Then the exit code should be 0
    And the output should contain "claude-auto-resume"

  Scenario: Diagnose command validates environment
    When I run "claude-auto-resume --diagnose"
    Then the exit code should be 0
    And the output should contain ".NET Runtime: OK"
```

### Architecture Tests

Enforce structural rules using NetArchTest with slice support:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

public class ArchitectureTests
{
    private static readonly Assembly MainAssembly =
        typeof(ClaudeMonitor).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        Types.InAssembly(MainAssembly)
            .That().ResideInNamespace("McjCoderOrg.ClaudeAutoResume.Domain")
            .ShouldNot()
            .HaveDependencyOn("McjCoderOrg.ClaudeAutoResume.Infrastructure")
            .GetResult()
            .IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Slices_ShouldNotHaveCrossSliceDependencies()
    {
        SliceRuleDefinition.FromAssembly(MainAssembly)
            .SlicedByNamespace("McjCoderOrg.ClaudeAutoResume.Features.(*)")
            .ShouldNotHaveDependenciesBetweenSlices()
            .GetResult()
            .IsSuccessful.Should().BeTrue();
    }
}
```

### Performance Benchmarks

BenchmarkDotNet for tracking performance regressions:

```csharp
[MemoryDiagnoser]
public class OutputParsingBenchmarks
{
    private readonly RateLimitDetector _detector = new();

    [Benchmark]
    public bool ParseRateLimitMessage()
    {
        return _detector.IsRateLimited("Claude AI usage limit reached");
    }
}
```

Benchmarks run on CI with results stored as artifacts for comparison.

### Mutation Testing

Stryker.NET runs on nightly schedule to validate test effectiveness:

- Mutation score tracked alongside coverage
- Dashboard published to GitHub Pages
- Playbook at `docs/playbooks/mutation-testing.md` for local execution

### Test Execution Strategy

| Test Type | Trigger | Scope |
|-----------|---------|-------|
| Unit | Pre-push hook, PR, main | Affected tests only |
| System | PR, main | Affected tests only |
| E2E | Nightly, manual | Full suite |
| Architecture | PR, main | Full suite |
| Benchmarks | PR (compare), main (baseline) | Full suite |
| Mutation | Nightly | Full suite |

### Code Coverage

- **Threshold:** 80% line, 70% branch on changed code
- **Ratchet:** Coverage can only increase, never decrease
- **Excluded:** Test projects, generated code, `**/obj/**`
- **Tool:** Coverlet with delta coverage reporting on PRs

---

## Security

### Security Scanning Matrix

| Scan Type | Tool | Trigger | Scope |
|-----------|------|---------|-------|
| Secret Detection | secretlint (pre-commit) | Every commit | Staged files |
| Secret Detection | GitHub Secret Scanning | Push to remote | All branches |
| SAST | GitHub CodeQL | PR, main push | Full codebase |
| Dependency Vulnerabilities | Dependabot | Daily | All dependencies |
| Dependency Vulnerabilities | `dotnet list package --vulnerable` | PR | Direct deps |

### Pre-Commit Secret Scanning

secretlint runs as part of pre-commit hook via lint-staged:

**.secretlintrc.json:**
```json
{
  "rules": [
    { "id": "@secretlint/secretlint-rule-preset-recommend" },
    { "id": "@secretlint/secretlint-rule-aws" },
    { "id": "@secretlint/secretlint-rule-gcp" },
    { "id": "@secretlint/secretlint-rule-npm" },
    { "id": "@secretlint/secretlint-rule-privatekey" }
  ]
}
```

### GitHub Native Security

- Secret scanning: Enabled
- Push protection: Enabled (blocks commits containing secrets)
- Dependabot alerts: Enabled
- Dependabot security updates: Enabled
- CodeQL analysis: Enabled on PRs and main

### Security Policy

`.github/SECURITY.md` documents:
- Supported versions
- Vulnerability reporting process (GitHub Security Advisories)
- Expected response times

### Dependency Management

- Dependabot for weekly update PRs
- Auto-merge for patch updates (via org-scoped `MACHINE_USER_PAT`)
- Manual review for minor/major updates
- Grouped updates to reduce PR noise

### Software Bill of Materials (SBOM)

Generated during release using `Microsoft.Sbom.DotNetTool` and attached to GitHub Release.

---

## Code Quality

### Analyzer Stack

| Analyzer | Purpose | Scope |
|----------|---------|-------|
| .NET Analyzers (built-in) | Microsoft CA/IDE rules | All projects |
| Meziantou.Analyzer | Security, performance, best practices | All projects |
| Roslynator.Analyzers | 500+ code quality rules | All projects |
| SonarAnalyzer.CSharp | Security, reliability, maintainability | All projects |
| Microsoft.CodeAnalysis.PublicApiAnalyzers | Breaking change detection | Main project |
| xunit.analyzers | Test-specific rules | Test projects |
| Moq.Analyzers | Mock setup validation | Test projects |

### Public API Tracking

Breaking changes detected via `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt`. Changes must align with conventional commits:
- Additions → `feat:` → minor version bump
- Removals/changes → `BREAKING CHANGE:` → major version bump

### Code Formatting

- **C#:** `dotnet format` (respects `.editorconfig`)
- **Markdown/JSON/YAML:** Prettier
- **Markdown linting:** markdownlint with accessibility rules
- **Spellcheck:** cspell (en-GB default)
- **Line endings:** LF (enforced via `.gitattributes`)

### Linting Integration

**lint-staged configuration:**
```json
{
  "lint-staged": {
    "*": ["secretlint"],
    "*.cs": ["dotnet format --include"],
    "*.md": ["prettier --write", "markdownlint --fix", "cspell --no-must-find-files"],
    "*.{json,yml,yaml}": ["prettier --write"]
  }
}
```

### CI Verification

```yaml
- name: Check C# formatting
  run: dotnet format --verify-no-changes

- name: Check spelling
  run: npx cspell "**/*.md" "**/*.cs" --no-progress

- name: Check Prettier
  run: npx prettier --check "**/*.md" "**/*.json" "**/*.yml"

- name: Check Markdown
  run: npx markdownlint "**/*.md" --ignore node_modules
```

---

## Documentation

### Documentation Architecture

| Location | Purpose | Audience |
|----------|---------|----------|
| `docs/docusaurus/` | Published website source | End users, contributors |
| `docs/standards/` | Coding conventions, style guides | Developers, agents |
| `docs/practices/` | Workflows, processes, decision guides | Developers, agents |
| `docs/playbooks/` | Runbooks for specific tasks | Developers, operators |
| `docs/agents/` | Agent-specific guidance | AI agents |
| `docs/adr/` | Architecture Decision Records | All |
| `AGENTS.md` | Agent orientation & routing | AI agents |

### Agent-Friendly Front-Matter

All documentation files include structured front-matter for progressive loading:

```yaml
---
title: Coding Standards
summary: C# conventions, naming rules, and style guidelines for the project
audience: [developer, agent]
topics: [csharp, conventions, code-style]
prerequisites: [docs/getting-started/development-environment.md]
related: [docs/practices/code-review.md, docs/standards/testing.md]
last_validated: 2026-01-09
---
```

### AGENTS.md (Root)

The agent routing document mandates when to read front-matter vs. full content:

- **Always Read (Full Content):** AGENTS.md, docs/agents/ORIENTATION.md, docs/agents/CONVENTIONS.md
- **Read Front-Matter First:** docs/standards/*, docs/practices/*, docs/playbooks/*
- **Read On-Demand:** docs/adr/*, CHANGELOG.md

### Documentation Website

- **Platform:** Docusaurus on GitHub Pages
- **Versioning:** Aligned with releases using Docusaurus versioning
- **Accessibility:** WCAG 2.1 AA compliance
- **CLI Reference:** Auto-generated from `--help` output

### Playbooks

| Playbook | Purpose |
|----------|---------|
| `mutation-testing.md` | How to run Stryker.NET locally and interpret results |
| `release-hotfix.md` | Emergency hotfix process from release tag |
| `troubleshooting.md` | Common issues and diagnostics |
| `dependency-update.md` | Manual dependency update process |

---

## CI/CD Pipeline

### Pipeline Architecture

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `ci.yml` | Push, PR | Lint, build, test (affected only) |
| `pr-title.yml` | PR open/edit | Validate PR title for squash merge |
| `release.yml` | Push to main | Version, changelog, release, publish |
| `codeql.yml` | PR, main, weekly | Security scanning |
| `docs.yml` | Push to main (docs/**) | Deploy documentation site |
| `nightly.yml` | Scheduled (daily) | E2E tests, mutation testing, benchmarks |
| `dependabot-automerge.yml` | Dependabot PRs | Auto-approve and merge patches |

### Branch Protection

Main branch protection rules:
- Require PR reviews (1 reviewer)
- Require status checks: `lint`, `build` (all OS), `CodeQL`, `pr-title`
- Require signed commits
- Require conversation resolution
- Require linear history (squash merge)
- Do not allow bypassing the above settings
- No direct pushes (enforced by hook and protection)
- Auto-delete head branches after merge

### Pre-commit Hook

Blocks direct commits to main and verifies signed commits:

```bash
#!/bin/sh
branch=$(git rev-parse --abbrev-ref HEAD)
if [ "$branch" = "main" ]; then
  echo "Direct commits to main are not allowed."
  exit 1
fi

if ! git config --get commit.gpgsign | grep -q "true"; then
  echo "Commits must be signed."
  exit 1
fi

npx lint-staged
```

### PR Title Validation

With squash merges, PR title becomes the commit message on main. Validated via workflow:

```yaml
- name: Validate PR title
  run: echo "${{ github.event.pull_request.title }}" | npx commitlint

- name: Check for work item reference
  run: |
    if ! echo "${{ github.event.pull_request.body }}" | grep -qE "Refs: #[0-9]+"; then
      echo "PR body must contain work item reference (Refs: #123)"
      exit 1
    fi
```

### Release Workflow

1. Build and test on all platforms
2. Generate version via GitVersion
3. Build standalone executables (continue-on-error for each OS)
4. Generate SBOM and checksums
5. Create git tag
6. Create GitHub Release with all artifacts
7. Publish to NuGet
8. Create changelog update PR (auto-approved via `MACHINE_USER_PAT`)

### Automated PR Approvals

Org-scoped `MACHINE_USER_PAT` used for:
- Changelog PR auto-approval
- Dependabot patch update auto-approval

### Caching Strategy

| Cache | Key |
|-------|-----|
| NuGet packages | `${{ runner.os }}-nuget-${{ hashFiles('**/Directory.Packages.props') }}` |
| npm packages | `${{ runner.os }}-npm-${{ hashFiles('package-lock.json') }}` |
| .NET build outputs | `${{ runner.os }}-build-${{ hashFiles('**/*.csproj') }}` |

### Artifact Retention

| Artifact Type | Retention |
|---------------|-----------|
| PR artifacts | 7 days |
| Main branch artifacts | 30 days |
| Release artifacts | Permanent (GitHub Release) |
| Mutation reports | 30 days |
| Benchmark history | Permanent (gh-pages) |

---

## Versioning & Releases

### Versioning Strategy

| Tool | Purpose |
|------|---------|
| GitVersion | Semantic version calculation from git history |
| git-cliff | Changelog generation from conventional commits |
| PublicApiAnalyzers | Breaking change detection synced with versioning |

### Conventional Commits

Required format:
```
<type>(<scope>): <subject>

<body>

<footer with Refs: #123>
```

| Type | Description | Version Impact |
|------|-------------|----------------|
| `feat` | New feature | Minor bump |
| `fix` | Bug fix | Patch bump |
| `perf` | Performance improvement | Patch bump |
| `refactor` | Code refactoring | No bump |
| `docs` | Documentation only | No bump |
| `test` | Adding/updating tests | No bump |
| `build` | Build system changes | No bump |
| `ci` | CI configuration | No bump |
| `chore` | Maintenance tasks | No bump |
| `revert` | Revert previous commit | Inherits reverted type |
| `style` | Code style (formatting) | No bump |

Breaking changes: Add `!` after type or `BREAKING CHANGE:` in footer → Major bump

### Branch Naming Convention

Issue-linked pattern: `type/issue#-description`

Examples:
- `feature/123-add-rate-limit-detection`
- `fix/456-handle-empty-config`

Validated via pre-push hook.

### Version Flow

| Branch | Example Version |
|--------|-----------------|
| `main` | `1.2.0` |
| `feature/123-add-auth` | `1.3.0-add-auth.1` |
| `fix/456-null-check` | `1.2.1-null-check.1` |

### Release Artifacts

| Artifact | Description |
|----------|-------------|
| `McjCoderOrg.ClaudeAutoResume.x.y.z.nupkg` | NuGet package (dotnet tool) with Source Link |
| `McjCoderOrg.ClaudeAutoResume.x.y.z.snupkg` | Symbol package |
| `win-x64/claude-auto-resume.exe` | Windows standalone executable |
| `linux-x64/claude-auto-resume` | Linux standalone executable |
| `osx-x64/claude-auto-resume` | macOS Intel standalone executable |
| `osx-arm64/claude-auto-resume` | macOS Apple Silicon standalone executable |
| `checksums.sha256` | SHA256 checksums for all artifacts |
| `manifest.spdx.json` | SBOM (Software Bill of Materials) |

---

## Developer Experience

### Dev Containers

Isolated development environment with all tooling pre-configured:

- .NET 10 SDK
- Node.js 22
- GitHub CLI
- VS Code extensions for C#, EditorConfig, Prettier, cspell, Reqnroll

**Encouraged workflow:** All development and testing within the container for environment isolation.

### Bootstrap Scripts

`scripts/setup.ps1` (Windows) and `scripts/setup.sh` (Unix):
- Check prerequisites (.NET, Node.js)
- Restore .NET and npm dependencies
- Install .NET local tools
- Configure git hooks
- Enable signed commits
- Verify build

### npm Scripts

| Script | Purpose |
|--------|---------|
| `npm run setup` | Run bootstrap script |
| `npm run build` | Build solution |
| `npm run test` | Run all tests |
| `npm run test:unit` | Run unit tests only |
| `npm run lint` | Run all linters |
| `npm run format` | Format all files |
| `npm run docs:dev` | Start docs dev server |

### .NET Local Tools

- `dotnet-gitversion` - Version calculation
- `dotnet-stryker` - Mutation testing
- `dotnet-reportgenerator` - Coverage reports
- `dotnet-affected` - Identify affected tests

### GitHub Templates

- Issue templates: Bug report, feature request
- PR template with automated checks section
- CODEOWNERS for auto-assignment
- SECURITY.md for vulnerability reporting

---

## CLI Design

### Command Structure

```
claude-auto-resume [options] [-- <claude-args>...]

Options:
  -c, --config <path>       Path to configuration file
  -v, --verbose             Enable verbose logging to file
  --diagnose                Run environment diagnostics
  --version                 Show version information
  -h, --help                Show help
```

### Layered Configuration

Priority order (highest to lowest):
1. CLI arguments
2. Environment variables (`CLAUDE_AUTO_RESUME_*`)
3. Project config (`.claude-auto-resume.json`)
4. User config (`~/.config/claude-auto-resume/config.json`)
5. Defaults

### Semantic Exit Codes

| Code | Name | Description |
|------|------|-------------|
| 0 | Success | Normal completion |
| 1 | GeneralError | Unhandled exception |
| 2 | ConfigurationError | Invalid config file or options |
| 3 | DependencyMissing | Claude CLI not found |
| 4 | RateLimitDetected | Exited due to rate limit |
| 5 | UserCancelled | User interrupted (Ctrl+C) |

### Diagnostics Command

`--diagnose` outputs structured environment report including:
- Runtime environment (.NET version, OS, architecture)
- Dependencies (Claude CLI location/version)
- Configuration validity
- Permissions check
- JSON blob for issue reports

### Verbose Logging

`--verbose` enables file logging:
- Windows: `%LOCALAPPDATA%\claude-auto-resume\logs\`
- macOS: `~/Library/Logs/claude-auto-resume/`
- Linux: `~/.local/share/claude-auto-resume/logs/`

### Internationalization

English only initially, but i18n-ready:
- All user-facing strings in resource files
- Named parameters for structured logging: `{ResetTime}`, `{WaitMinutes}`
- Structure allows adding translations without code changes

---

## Observability

### Logging Strategy

| Mode | Console Output | File Logging |
|------|----------------|--------------|
| Default | Errors only | Bootstrap errors |
| `--verbose` | None (passthrough) | Full debug |
| Exception | Error + log path | Full stack trace |

### Platform Context Capture

Anonymous environment details captured at startup:
- .NET version, runtime identifier
- OS description and architecture
- App version
- Command line arguments (sanitized)
- Container/CI detection
- Terminal type
- Locale

Included in error output and `--diagnose` for issue reporting.

### Startup Logging

```csharp
Log.Information("Starting Claude Auto Resume v{AppVersion}", platform.AppVersion);
Log.Information("Command line: {CommandLineArgs}", string.Join(" ", platform.CommandLineArgs));
Log.Information(
    "Platform: {OSDescription} ({OSArchitecture}) | .NET {DotNetVersion}",
    platform.OSDescription, platform.OSArchitecture, platform.DotNetVersion);
```

### Exception Handling

- Bootstrap logger captures startup errors before DI configured
- Global exception handler for unhandled exceptions
- User-friendly error output with log file path
- Full stack trace in log file
- Platform context included for issue reporting

### Test Log Capture

Serilog.Sinks.InMemory for asserting on log messages in tests:

```csharp
logs.AssertLoggedWithProperty(
    "Detected Session Limit Reached",
    "ResetTime",
    "3pm");
```

---

## ADR Summary

| ADR | Title |
|-----|-------|
| 0001 | Versioning and Changelog Strategy |
| 0002 | Testing Framework |
| 0003 | Code Analyzers |
| 0004 | CI/CD Pipeline |
| 0005 | Pre-commit Hooks |
| 0006 | Code Formatting |
| 0007 | Test Project Structure |
| 0008 | Namespace and Project Naming |
| 0009 | GitHub Platform |
| 0010 | Work Item Management |
| 0011 | Security Scanning |
| 0012 | Architecture Testing |
| 0013 | Documentation Strategy |
| 0014 | Agent Onboarding |
| 0015 | Developer Environment |
| 0016 | Observability |
| 0017 | CLI Design |
| 0018 | License |
| 0019 | Accessibility |
| 0020 | Internationalization |
| 0021 | Telemetry |
| 0022 | Breaking Change Detection |
| 0023 | Performance Testing |
| 0024 | Mutation Testing |
| 0025 | Dependency Management |
| 0026 | Code Coverage |
| 0027 | Branch Strategy |
| 0028 | Release Artifacts |
| 0029 | Documentation Versioning |

---

## References

- [ADR-0001: Versioning and Changelog Strategy](../adr/0001-versioning-and-changelog-strategy.md)
- [ADR-0002: Testing Framework](../adr/0002-testing-framework.md)
- [ADR-0003: Code Analyzers](../adr/0003-code-analyzers.md)
- [ADR-0004: CI/CD Pipeline](../adr/0004-cicd-pipeline.md)
- [ADR-0005: Pre-commit Hooks](../adr/0005-pre-commit-hooks.md)
- [ADR-0006: Code Formatting](../adr/0006-code-formatting.md)
- [ADR-0007: Test Project Structure](../adr/0007-test-project-structure.md)
- [ADR-0008: Namespace and Project Naming](../adr/0008-namespace-and-project-naming.md)
- [ADR-0009: GitHub Platform](../adr/0009-github-platform.md)
- [ADR-0010: Work Item Management](../adr/0010-work-item-management.md)
- [ADR-0011: Security Scanning](../adr/0011-security-scanning.md)
- [ADR-0012: Architecture Testing](../adr/0012-architecture-testing.md)
- [ADR-0013: Documentation Strategy](../adr/0013-documentation-strategy.md)
- [ADR-0014: Agent Onboarding](../adr/0014-agent-onboarding.md)
- [ADR-0015: Developer Environment](../adr/0015-developer-environment.md)
- [ADR-0016: Observability](../adr/0016-observability.md)
- [ADR-0017: CLI Design](../adr/0017-cli-design.md)
- [ADR-0018: License](../adr/0018-license.md)
- [ADR-0019: Accessibility](../adr/0019-accessibility.md)
- [ADR-0020: Internationalization](../adr/0020-internationalization.md)
- [ADR-0021: Telemetry](../adr/0021-telemetry.md)
- [ADR-0022: Breaking Change Detection](../adr/0022-breaking-change-detection.md)
- [ADR-0023: Performance Testing](../adr/0023-performance-testing.md)
- [ADR-0024: Mutation Testing](../adr/0024-mutation-testing.md)
- [ADR-0025: Dependency Management](../adr/0025-dependency-management.md)
- [ADR-0026: Code Coverage](../adr/0026-code-coverage.md)
- [ADR-0027: Branch Strategy](../adr/0027-branch-strategy.md)
- [ADR-0028: Release Artifacts](../adr/0028-release-artifacts.md)
- [ADR-0029: Documentation Versioning](../adr/0029-documentation-versioning.md)
