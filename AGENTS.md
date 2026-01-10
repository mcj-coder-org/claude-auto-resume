# Agent Orientation

This document is the entry point for AI agents working on the McjCoderOrg.ClaudeAutoResume project.

**Project:** Claude Auto Resume - A cross-platform .NET tool that automatically
resumes Claude CLI sessions when rate limits are hit.

## Critical Rules

1. **Always Clean Build Commits** - Zero Warnings and Errors (Build/Tests/Linting + Everyday Operations)
2. **Evidence based verification** - Record evidence of passing criteria. Passing tests, immutable links to evidence
3. **No Broken Windows** - Fix significant issues ASAP, ensure working directory is rebased often and `main` is always clean
4. **TDD Delivery** - Red, Green, Refactor Always
5. **Never work without a plan** - All work requires a ticket that's been refined and planned. Do not start work without.
6. **Never commit directly to main** - Always use feature branches
7. **Commits must be signed** - Configure GPG signing
8. **All commits must pass hooks** - Pre-commit validates format
9. **NEVER use `--no-verify` without explicit User approval** - Fix root causes of hook failures, do not bypass validation
10. **No placeholder content** - All documentation must be complete and actionable
11. **Follow existing patterns** - Check similar files before creating new ones
12. **Keep docs and tooling in sync** - Standards must match analyzer/linter configuration

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

---

## Document Index

### First Read (Full Content)

Read these documents in full before starting any work:

| Document                     | Purpose                               |
| ---------------------------- | ------------------------------------- |
| `AGENTS.md`                  | This file - routing and loading rules |
| `docs/agents/ORIENTATION.md` | Project overview and architecture     |
| `docs/agents/CONVENTIONS.md` | Coding and commit conventions         |

### Read When Relevant

Load frontmatter first, full content when the task requires it:

| Document                                   | When to Read                                 |
| ------------------------------------------ | -------------------------------------------- |
| `docs/practices/collaboration-patterns.md` | Starting any task - understand working modes |
| `docs/standards/roles.md`                  | Need specialised review or implementation    |
| `docs/practices/code-review.md`            | Reviewing or submitting PRs                  |
| `docs/standards/coding-standards.md`       | Writing or reviewing code                    |
| `docs/playbooks/troubleshooting.md`        | Encountering errors or issues                |
| `docs/agents/IDE-SETUP.md`                 | Configuring IDE for agent use                |
| `docs/adr/*`                               | Understanding architectural decisions        |

### Reference Only

Load only when specifically needed:

| Document                                  | When to Read                               |
| ----------------------------------------- | ------------------------------------------ |
| `docs/playbooks/troubleshooting/*`        | Specific issue categories                  |
| `docs/practices/collaboration-patterns/*` | Deep dive into specific patterns           |
| `docs/standards/roles/*`                  | Individual role details                    |
| `CHANGELOG.md`                            | Version history (created at first release) |

---

## Collaboration Patterns

See `docs/practices/collaboration-patterns.md` for detailed guidance:

| Pattern                                                                               | When to Use                          |
| ------------------------------------------------------------------------------------- | ------------------------------------ |
| [Planning & Requirements](docs/practices/collaboration-patterns/planning.md)          | Designing features, writing ADRs     |
| [Pair Programming](docs/practices/collaboration-patterns/pair-programming.md)         | Real-time collaboration in IDE       |
| [Verification & Review](docs/practices/collaboration-patterns/verification-review.md) | Code review, security audit, testing |
| [Autonomous Execution](docs/practices/collaboration-patterns/autonomous-execution.md) | Implementing well-defined tickets    |

## Roles

For specialised tasks, use focused roles defined in `docs/standards/roles.md`:

| Role                     | Use For                                        |
| ------------------------ | ---------------------------------------------- |
| Tech Lead                | Architecture decisions, cross-cutting concerns |
| DotNet Developer         | C# implementation, .NET 10, async patterns     |
| Senior Developer         | Code quality, SOLID, refactoring               |
| QA Engineer              | Testing strategy, BDD scenarios                |
| Security Reviewer        | OWASP, input validation, secrets               |
| Documentation Specialist | XML docs, README, user guides                  |

See `docs/standards/roles.md` for the full list of 16 roles and selection guidance.

## IDE Integration

See `docs/agents/IDE-SETUP.md` for configuration guidance:

| IDE             | Primary Agent | Setup Guide        |
| --------------- | ------------- | ------------------ |
| VS Code         | Claude Code   | Terminal + Copilot |
| JetBrains Rider | Junie         | AI Assistant       |
| Visual Studio   | Codex/Copilot | Copilot Chat       |

---

## Common Tasks

| Task                    | Start Here                                             |
| ----------------------- | ------------------------------------------------------ |
| Implement feature       | ADR-0004, `docs/standards/coding-standards.md`         |
| Review code             | `docs/practices/code-review.md`                        |
| Fix bug                 | `docs/playbooks/troubleshooting.md`                    |
| Write tests             | `docs/standards/coding-standards.md` (Testing section) |
| Plan work               | `docs/practices/collaboration-patterns/planning.md`    |
| Understand architecture | `docs/agents/ORIENTATION.md`, `docs/adr/`              |

---

## Project Structure

```text
McjCoderOrg.ClaudeAutoResume/
├── src/
│   └── McjCoderOrg.ClaudeAutoResume/    # Main application
├── tests/                                # (planned - Phase 4)
│   ├── McjCoderOrg.ClaudeAutoResume.Tests/       # Unit tests
│   ├── McjCoderOrg.ClaudeAutoResume.SystemTests/ # BDD system tests
│   ├── McjCoderOrg.ClaudeAutoResume.E2ETests/    # BDD E2E tests
│   ├── McjCoderOrg.ClaudeAutoResume.ArchTests/   # Architecture tests
│   └── McjCoderOrg.ClaudeAutoResume.Benchmarks/  # Performance tests
├── docs/
│   ├── standards/    # Coding conventions, roles
│   ├── practices/    # Workflows, collaboration patterns
│   ├── playbooks/    # Runbooks, troubleshooting
│   ├── agents/       # Agent-specific guidance
│   ├── adr/          # Architecture decisions
│   └── plans/        # Design documents
├── .github/          # CI/CD, templates
└── scripts/          # Bootstrap scripts (planned - Phase 5)
```

## Contribution Quick Reference

| Item          | Format                                                              |
| ------------- | ------------------------------------------------------------------- |
| Branch naming | `type/issue#-description` (e.g., `feature/42-add-retry-logic`)      |
| Commit format | `type(scope): subject` (e.g., `feat(monitor): add retry detection`) |
| PR title      | Same as commit format                                               |
| Work item ref | `Refs: #123` in commit body                                         |

See ADR-0004 for complete contribution workflow.

---

## Progressive Document Loading

Agents SHOULD load document frontmatter first, then full content only when needed. This
reduces context consumption and improves selection accuracy.

### Loading Principle

**Load frontmatter first, full content when relevant.** Frontmatter contains summary fields
sufficient for selection and applicability decisions. Only load full document body when
execution or detailed rationale is required.

### Summary Fields by Document Type

| Document Type | Selection Fields      | Execution Fields             |
| ------------- | --------------------- | ---------------------------- |
| Roles         | `name`, `description` | `model` (for tier selection) |
| ADRs          | `name`, `description` | `decision`, `status`         |
| Playbooks     | `title`, `summary`    | Full procedures              |
| Patterns      | `title`, `summary`    | Detailed guidance            |

### Loading Algorithm

1. **Scan frontmatter** - Use `Read` tool with `limit: 20` to capture frontmatter block
2. **Filter** - Match by triggers, descriptions, or status
3. **Select** - Choose most applicable document(s)
4. **Execute** - Use summary fields if sufficient
5. **Load body** - Only if details needed beyond summary

### Writing Style

- **Terse and direct** - No filler words
- **Tables over prose** - Structured data
- **Bullets over paragraphs** - Scannable content
- **Code over description** - Show, don't tell
- **Links over duplication** - Reference, don't repeat

---

## Getting Help

| Problem                 | Solution                                                    |
| ----------------------- | ----------------------------------------------------------- |
| Stuck on process        | Read ADR-0004 (Contribution Workflow)                       |
| Code style question     | Read `docs/standards/coding-standards.md`                   |
| Test question           | Read `docs/standards/coding-standards.md` (Testing section) |
| Unknown error           | Read `docs/playbooks/troubleshooting.md`                    |
| Need specialised review | Read `docs/standards/roles.md`                              |
