# Agent Orientation

This document is the entry point for AI agents working on the McjCoderOrg.ClaudeAutoResume project.

## Quick Start

**Project:** Claude Auto Resume - A cross-platform .NET tool that automatically
resumes Claude CLI sessions when rate limits are hit.

**Tech Stack:**

- .NET 10 / C# 14
- xUnit + Reqnroll (BDD) for testing
- Serilog for logging
- GitHub Actions for CI/CD

**Key Behaviours:**

1. Read this file completely first
2. Follow the documentation loading rules below
3. Adhere to the contribution workflow (ADR-0004)
4. Use conventional commits for all changes

## Your First Action

After reading this file completely, read `docs/agents/ORIENTATION.md` for detailed project context.

## Documentation Loading Rules

### Always Read (Full Content)

These documents must be read in full before starting work:

- `AGENTS.md` (this file)
- `docs/agents/ORIENTATION.md` - Project overview and architecture
- `docs/agents/CONVENTIONS.md` - Coding and commit conventions

### Read Front-Matter First, Full Content On-Demand

For these documents, read the YAML front-matter to determine relevance, then load full content as needed:

- `docs/standards/*` - Coding standards and guidelines
- `docs/practices/*` - Workflows and processes
- `docs/adr/*` - Architecture Decision Records

### Reference Only When Needed

Load these documents only when specifically relevant to your task:

- `docs/playbooks/*` - Specific procedures and runbooks
- `CHANGELOG.md` - Version history

## Usage Patterns

See `docs/agents/PATTERNS.md` for detailed guidance on:

| Pattern                 | When to Use                          |
| ----------------------- | ------------------------------------ |
| Planning & Requirements | Designing features, writing ADRs     |
| Pair Programming        | Real-time collaboration in IDE       |
| Verification & Review   | Code review, security audit, testing |
| Autonomous Execution    | Implementing well-defined tickets    |

## IDE Integration

See `docs/agents/IDE-SETUP.md` for configuration guidance:

| IDE             | Primary Agent | Setup Guide        |
| --------------- | ------------- | ------------------ |
| VS Code         | Claude Code   | Terminal + Copilot |
| JetBrains Rider | Junie         | AI Assistant       |
| Visual Studio   | Codex/Copilot | Copilot Chat       |

## Common Tasks

| Task                    | Start Here                                             |
| ----------------------- | ------------------------------------------------------ |
| Implement feature       | ADR-0004, `docs/standards/coding-standards.md`         |
| Review code             | `docs/practices/code-review.md`                        |
| Fix bug                 | `docs/agents/TROUBLESHOOTING.md`                       |
| Write tests             | `docs/standards/coding-standards.md` (Testing section) |
| Plan work               | `docs/agents/PATTERNS.md` (Planning section)           |
| Understand architecture | `docs/agents/ORIENTATION.md`, `docs/adr/`              |

## Personas

For specialised tasks, use focused personas defined in `docs/agents/PERSONAS.md`:

| Persona                  | Use For                                 |
| ------------------------ | --------------------------------------- |
| DotNet Developer         | C# implementation, async patterns, LINQ |
| Security Reviewer        | OWASP, input validation, secrets        |
| QA Engineer              | Testing strategy, BDD scenarios         |
| Senior Developer         | Code quality, SOLID, refactoring        |
| Documentation Specialist | XML docs, README, user guides           |

## Project Structure

```text
McjCoderOrg.ClaudeAutoResume/
├── src/
│   └── McjCoderOrg.ClaudeAutoResume/    # Main application
├── tests/
│   ├── McjCoderOrg.ClaudeAutoResume.Tests/       # Unit tests
│   ├── McjCoderOrg.ClaudeAutoResume.SystemTests/ # BDD system tests
│   ├── McjCoderOrg.ClaudeAutoResume.E2ETests/    # BDD E2E tests
│   ├── McjCoderOrg.ClaudeAutoResume.ArchTests/   # Architecture tests
│   └── McjCoderOrg.ClaudeAutoResume.Benchmarks/  # Performance tests
├── docs/
│   ├── standards/    # Coding conventions
│   ├── practices/    # Workflows, processes
│   ├── playbooks/    # Runbooks
│   ├── agents/       # Agent guidance (you are here)
│   ├── adr/          # Architecture decisions
│   └── plans/        # Design documents
├── .github/          # CI/CD, templates
└── scripts/          # Bootstrap scripts
```

## Contribution Quick Reference

| Item          | Format                                                              |
| ------------- | ------------------------------------------------------------------- |
| Branch naming | `type/issue#-description` (e.g., `feature/42-add-retry-logic`)      |
| Commit format | `type(scope): subject` (e.g., `feat(monitor): add retry detection`) |
| PR title      | Same as commit format                                               |
| Work item ref | `Refs: #123` in commit body                                         |

See ADR-0004 for complete contribution workflow.

## Critical Rules

1. **Never commit directly to main** - Always use feature branches
2. **Commits must be signed** - Configure GPG signing
3. **All commits must pass hooks** - Pre-commit validates format
4. **No placeholder content** - All documentation must be complete and actionable
5. **Follow existing patterns** - Check similar files before creating new ones

## Getting Help

- **Stuck on process?** Read ADR-0004 (Contribution Workflow)
- **Code style question?** Read `docs/standards/coding-standards.md`
- **Test question?** Read `docs/standards/coding-standards.md` (Testing section)
- **Unknown error?** Read `docs/agents/TROUBLESHOOTING.md`
