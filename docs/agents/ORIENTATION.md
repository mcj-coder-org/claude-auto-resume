---
title: Agent Orientation
summary: Project overview, architecture, and key decisions for AI agents
audience: [agent]
topics: [onboarding, architecture, project-overview]
prerequisites: [AGENTS.md]
related: [CONVENTIONS.md, PATTERNS.md]
last_validated: 2026-01-09
---

# Agent Orientation

This document provides detailed context for AI agents working on the
McjCoderOrg.ClaudeAutoResume project.

## Project Purpose

**Claude Auto Resume** is a cross-platform .NET tool that wraps the Claude CLI to provide
automatic session resumption when rate limits are hit.

### Problem Statement

When using the Claude CLI for extended coding sessions, users hit rate limits that
interrupt their workflow. Currently, users must manually monitor output and restart
sessions.

### Solution

This tool:

1. Wraps the Claude CLI process
2. Monitors output for rate limit indicators
3. Detects the reset time
4. Automatically waits and resumes the session
5. Preserves the terminal experience (passthrough mode)

### Key Value Propositions

- **Zero configuration** - Works with default settings
- **Transparent operation** - Acts as a passthrough until needed
- **Cross-platform** - Windows, macOS, Linux
- **No telemetry** - Privacy-first (see ADR-0007)

## Architecture

### High-Level Design

```text
┌─────────────────────────────────────────────────────┐
│                     Terminal                         │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│            claude-auto-resume (this tool)           │
│  ┌──────────────────────────────────────────────┐   │
│  │               ClaudeMonitor                   │   │
│  │  • PTY management                             │   │
│  │  • Output monitoring                          │   │
│  │  • Rate limit detection                       │   │
│  │  • Auto-resume logic                          │   │
│  └──────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────┐   │
│  │               WrapperConfig                   │   │
│  │  • Configuration loading                      │   │
│  │  • CLI argument parsing                       │   │
│  │  • Environment variable handling              │   │
│  └──────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│                   Claude CLI                         │
│               (Anthropic's tool)                     │
└─────────────────────────────────────────────────────┘
```

### Core Components

| Component          | Responsibility                                  |
| ------------------ | ----------------------------------------------- |
| `Program.cs`       | Entry point, CLI parsing, host setup            |
| `ClaudeMonitor.cs` | PTY management, output monitoring, resume logic |
| `WrapperConfig.cs` | Configuration loading from multiple sources     |
| `PlatformInfo.cs`  | Cross-platform environment detection            |
| `ExitCodes.cs`     | Semantic exit codes                             |

### Configuration Hierarchy

Configuration is loaded in priority order (highest to lowest):

1. CLI arguments
2. Environment variables (`CLAUDE_AUTO_RESUME_*`)
3. Project config (`.claude-auto-resume.json`)
4. User config (`~/.config/claude-auto-resume/config.json`)
5. Defaults

## Key Architectural Decisions

These ADRs are essential reading for understanding the project:

| ADR      | Title                 | Impact                                    |
| -------- | --------------------- | ----------------------------------------- |
| ADR-0004 | Contribution Workflow | How to contribute code                    |
| ADR-0007 | Telemetry             | No telemetry - privacy-first              |
| ADR-0012 | Namespace Naming      | `McjCoderOrg.ClaudeAutoResume` convention |
| ADR-0013 | Testing Framework     | xUnit + Reqnroll for BDD                  |
| ADR-0017 | Observability         | Serilog logging patterns                  |
| ADR-0018 | CLI Design            | Command structure and exit codes          |

## Testing Strategy

### Test Project Matrix

| Project        | Type         | Access     | Purpose                          |
| -------------- | ------------ | ---------- | -------------------------------- |
| `.Tests`       | Unit         | Internals  | Isolated component testing       |
| `.SystemTests` | BDD          | Internals  | End-to-end with mocked externals |
| `.E2ETests`    | BDD          | Public API | Production smoke tests           |
| `.ArchTests`   | Architecture | Internals  | Structural rule enforcement      |
| `.Benchmarks`  | Performance  | Internals  | Regression detection             |

### Test Patterns

**Unit Tests:** Use xUnit with AwesomeAssertions and Moq.

```csharp
[Fact]
public void Detect_WhenLimitReached_ReturnsTrue()
{
    var detector = new RateLimitDetector();

    var result = detector.IsRateLimited("Claude AI usage limit reached");

    result.Should().BeTrue();
}
```

**BDD Tests:** Use Reqnroll with Gherkin syntax.

```gherkin
Scenario: Detect rate limit and log reset time
    Given Claude outputs "Claude AI usage limit reached, resets at 3pm"
    When the wrapper processes the output
    Then the wrapper should detect a rate limit
    And the log should contain the reset time
```

## Development Workflow

### Branch Naming

Format: `type/issue#-description`

Examples:

- `feature/42-add-retry-logic`
- `fix/87-handle-null-response`
- `docs/15-update-readme`

### Commit Format

Conventional Commits format:

```text
type(scope): subject

body (optional)

Refs: #123
```

Types: `feat`, `fix`, `docs`, `test`, `refactor`, `perf`, `build`, `ci`, `chore`

### Quality Gates

All commits must pass:

1. **Pre-commit hook**: Formatting, linting, secret scanning
2. **Commit-msg hook**: Conventional commit validation
3. **Pre-push hook**: Branch name validation, build check

## Current State

### What Exists

- Original single-file .NET 8 implementation (being migrated)
- ADRs documenting all architectural decisions
- Design and implementation plans
- Quality gates and hooks (Phase 5 complete)

### What's Being Built

The project is undergoing a .NET 10 migration following the implementation plan:

| Phase | Status      | Description                        |
| ----- | ----------- | ---------------------------------- |
| 1-2   | Complete    | Foundation, security baseline      |
| 3     | In Progress | Documentation foundation           |
| 4-8   | Pending     | Technology setup, CI/CD, migration |

### Migration Approach

**Key Principle:** Establish rules before migrating code.

The infrastructure (quality gates, CI/CD, documentation) is being set up first, then the
existing code will be migrated into the properly configured environment.

## Common Patterns

### Logging

Use Serilog with structured logging and named parameters:

```csharp
Log.Information("Detected rate limit, resets at {ResetTime}", resetTime);
Log.Warning("Retry attempt {Attempt} of {MaxRetries}", attempt, maxRetries);
```

### Error Handling

- Use semantic exit codes (see `ExitCodes.cs`)
- Log errors with full context
- Provide actionable error messages

### Async Patterns

- Prefer `async`/`await` over blocking calls
- Use `ConfigureAwait(false)` in library code
- Pass `CancellationToken` through the call chain

## What Not to Do

1. **Don't add telemetry** - Strict no-telemetry policy (ADR-0007)
2. **Don't commit to main** - Always use feature branches
3. **Don't skip hooks** - Pre-commit validation is mandatory
4. **Don't create placeholders** - All content must be complete
5. **Don't over-engineer** - Keep solutions simple and focused

## Getting Started with a Task

1. Read `AGENTS.md` (entry point)
2. Read this document (context)
3. Read `docs/agents/CONVENTIONS.md` (standards)
4. Read the relevant ADR for your task type
5. Create a properly named branch
6. Make changes following coding standards
7. Commit with conventional commit format
8. Push and create PR

## Questions to Ask Yourself

Before starting work:

- [ ] Do I understand the task requirements?
- [ ] Have I read the relevant ADRs?
- [ ] Is my branch named correctly?
- [ ] Do I know the testing expectations?

Before committing:

- [ ] Does my code follow the coding standards?
- [ ] Have I run the linters locally?
- [ ] Is my commit message in conventional format?
- [ ] Does my commit include the issue reference?

## Next Steps

After reading this document:

1. Read `docs/agents/CONVENTIONS.md` for coding and commit conventions
2. Read `docs/practices/collaboration-patterns.md` to understand working patterns
3. Consult the relevant ADR for your specific task
