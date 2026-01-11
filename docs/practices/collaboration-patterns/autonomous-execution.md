---
title: Autonomous Execution Pattern
summary: Independent execution pattern for implementing well-defined tickets with minimal supervision
audience: [developer, agent]
topics: [patterns, autonomous, execution, workflow]
parent: ../collaboration-patterns.md
last_validated: 2026-01-10
---

# Autonomous Execution Pattern

Use this pattern when independently implementing well-defined tickets.

## Characteristics

| Aspect                | Details                    |
| --------------------- | -------------------------- |
| **Mode**              | Independent execution      |
| **Human Role**        | Ticket author, PR approver |
| **Collaborator Role** | Full implementer           |
| **Autonomy Level**    | Autonomous                 |

## Prerequisites (All Must Be True)

- [ ] Ticket has clear, unambiguous description
- [ ] Acceptance criteria are explicit and testable
- [ ] All referenced documentation is accessible
- [ ] Scope boundaries are defined
- [ ] No blocking dependencies

## When NOT to Use

- Requirements are vague or incomplete
- Architectural decisions needed
- Cross-cutting changes affecting multiple systems
- Security-critical changes requiring human review
- First implementation of a pattern

## Collaborator Behaviours

1. **Verify prerequisites** - Don't start if ticket is unclear
2. **Follow the workflow exactly** - No shortcuts
3. **Document everything** - Commits tell the story
4. **Test thoroughly** - Meet coverage requirements
5. **Self-review before PR** - Check your own work

## Execution Workflow

```text
1. READ TICKET
   |-- Verify acceptance criteria are clear
   |-- Identify all referenced docs
   |-- If unclear, STOP and ask questions

2. CREATE BRANCH
   |-- git checkout -b type/issue#-description

3. IMPLEMENT
   |-- Read relevant coding standards
   |-- Write code following conventions
   |-- Write tests (aim for 80%+ coverage on changes)
   |-- Ensure pre-commit hooks pass

4. COMMIT
   |-- Conventional commit format
   |-- Include Refs: #issue
   |-- One logical change per commit

5. SELF-REVIEW
   |-- Review own diff
   |-- Check against acceptance criteria
   |-- Verify tests are meaningful

6. CREATE PR
   |-- Use PR template
   |-- Link to issue
   |-- Describe changes clearly

7. RESPOND TO FEEDBACK
   |-- Address all review comments
   |-- Push additional commits
   |-- Re-request review
```

## Example: Implementing a Ticket

**Ticket #42:** Add configurable retry delay

**Acceptance Criteria:**

- Users can configure retry delay via `--retry-delay` CLI argument
- Default value is 5000ms
- Value is validated (must be positive integer)
- Unit tests cover configuration loading

**Collaborator execution:**

1. Create branch: `feature/42-add-retry-delay`
2. Read `docs/standards/coding-standards.md`
3. Implement in `WrapperConfig.cs`:
   - Add `RetryDelayMs` property
   - Add CLI argument parsing
   - Add validation
4. Write tests in `WrapperConfigTests.cs`
5. Commit: `feat(config): add configurable retry delay\n\nRefs: #42`
6. Create PR with template
7. Wait for review

## Outputs

- Feature branch
- Implementation code
- Unit tests
- PR with full description

---

## Sub-Agent Delegation

When the platform supports it, AI assistants can use sub-agents for isolated tasks.

### When to Delegate

- Different perspectives needed (security vs implementation)
- Parallel independent tasks
- Long-running operations that benefit from fresh context
- Specialised expertise required

### How to Delegate

```text
Main Collaborator:
  "I need a security review of the authentication changes."
  -> Spawn Security Reviewer sub-agent
  -> Sub-agent reviews and returns findings
  -> Main collaborator integrates findings

Main Collaborator:
  "Implement components A, B, and C from ticket #42"
  -> Spawn sub-agent for component A
  -> Spawn sub-agent for component B
  -> Spawn sub-agent for component C
  -> Main collaborator integrates and creates PR
```

### Sub-Agent Context

Each sub-agent should receive:

1. Specific task description
2. Relevant documentation references
3. Role identifier (if specialised) - see `docs/standards/roles.md`
4. Return format expectations

---

## Documentation to Reference

- `docs/adr/0004-contribution-workflow.md` - Full workflow
- `docs/agents/CONVENTIONS.md` - Commit format
- `docs/standards/coding-standards.md` - Code standards
