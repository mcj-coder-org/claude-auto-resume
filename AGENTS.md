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
- `CHANGELOG.md` - Version history (created at first release)

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
│   ├── standards/    # Coding conventions
│   ├── practices/    # Workflows, processes
│   ├── playbooks/    # Runbooks
│   ├── agents/       # Agent guidance (you are here)
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

## Documentation Standards

When creating or updating documentation, follow these principles:

## Progressive Document Loading

Agents SHOULD load document frontmatter first, then full content only when needed. This
reduces context consumption and improves selection accuracy.

### Progressive Loading Principle

**Load frontmatter first, full content when relevant.** Frontmatter contains summary fields
sufficient for selection and applicability decisions. Only load full document body when
execution or detailed rationale is required.

### Summary Fields by Document Type

| Document Type | Selection Fields      | Execution Fields             |
| ------------- | --------------------- | ---------------------------- |
| Roles         | `name`, `description` | `model` (for tier selection) |
| ADRs          | `name`, `description` | `decision`, `status`         |
| Playbooks     | `name`, `triggers`    | `description`, `summary`     |

For complete field definitions and validation rules, see the respective README files.

### When Frontmatter Suffices

Use frontmatter only when:

- **Selecting** which document applies to current context
- **Checking applicability** of a role, decision, or playbook
- **Building lists** of relevant documents for a task
- **Quick reference** to a decision or trigger condition

### When Full Document Needed

Load the full document body when:

- **Executing** a playbook requires details beyond the summary
- **Understanding rationale** for why a decision was made
- **Following step-by-step** procedures with nested steps or decision points
- **Reviewing alternatives** that were considered (ADRs)
- **Learning capabilities** of a role beyond its description

### Loading Algorithm

1. **Scan frontmatter** of all documents in the relevant directory
   - Use `Read` tool with `limit: 20` to capture frontmatter block
   - Frontmatter ends at closing `---` delimiter (typically lines 1-15)
2. **Filter** by matching triggers, descriptions, or status (for ADRs)
3. **Select** the most applicable document(s) using conflict resolution rules
4. **Execute** using summary fields if sufficient
5. **Load body** only if summary references details not provided
   - Use `Read` without limit for full document content

### Writing Style

- **Terse and direct** - No filler words, no verbose explanations
- **Tables over prose** - Structured data is faster to parse
- **Bullets over paragraphs** - Scannable content
- **Code over description** - Show, don't tell
- **Links over duplication** - Reference, don't repeat

### Documentation-Tooling Sync

Standards documented must be enforced by tooling where possible:

| Documentation      | Enforcement                          |
| ------------------ | ------------------------------------ |
| Naming conventions | `.editorconfig` naming rules         |
| Code style         | `.editorconfig` + `dotnet format`    |
| Member ordering    | StyleCop analyzers (SA1201-SA1214)   |
| Security patterns  | Analyzer rules (CA2100, S2068, etc.) |
| Commit format      | commitlint via husky                 |
| Markdown style     | markdownlint + prettier              |

When adding new standards, update both documentation AND tooling configuration.

## Getting Help

- **Stuck on process?** Read ADR-0004 (Contribution Workflow)
- **Code style question?** Read `docs/standards/coding-standards.md`
- **Test question?** Read `docs/standards/coding-standards.md` (Testing section)
- **Unknown error?** Read `docs/agents/TROUBLESHOOTING.md`
