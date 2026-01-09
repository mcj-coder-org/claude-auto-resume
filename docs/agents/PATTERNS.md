---
title: Agent Usage Patterns
summary: Detailed guidance for different agent usage modes and autonomy levels
audience: [agent]
topics: [patterns, workflows, autonomy, collaboration]
prerequisites: [AGENTS.md, ORIENTATION.md, CONVENTIONS.md]
related: [PERSONAS.md, ../adr/0009-agent-onboarding.md]
last_validated: 2026-01-09
---

# Agent Usage Patterns

This document describes how AI agents should operate in different contexts. Each pattern
has specific expectations for autonomy, outputs, and collaboration with humans.

## Pattern Overview

| Pattern                 | Autonomy             | Human Role              | Agent Role          | Typical Output           |
| ----------------------- | -------------------- | ----------------------- | ------------------- | ------------------------ |
| Planning & Requirements | Guided               | Decision maker          | Facilitator         | ADRs, designs, plans     |
| Pair Programming        | Guided to Supervised | Driver/Navigator        | Navigator/Driver    | Code, tests, commits     |
| Verification & Review   | Supervised           | Final approver          | Specialist reviewer | Review comments, reports |
| Autonomous Execution    | Autonomous           | Ticket author, approver | Full implementer    | Branch, commits, PR      |

## Pattern 1: Planning & Requirements

Use this pattern when designing features, writing ADRs, or planning implementation approaches.

### Characteristics

| Aspect             | Details                             |
| ------------------ | ----------------------------------- |
| **Mode**           | Collaborative dialogue              |
| **Human Role**     | Decision maker, domain expert       |
| **Agent Role**     | Facilitator, documenter, challenger |
| **Autonomy Level** | Guided (human leads)                |

### When to Use

- Designing new features
- Writing or updating ADRs
- Breaking down epics into issues
- Exploring technical approaches
- Clarifying requirements

### Agent Behaviours

1. **Ask clarifying questions** - Don't assume, verify understanding
2. **Present options** - Offer 2-3 approaches with trade-offs
3. **Challenge assumptions** - Point out potential issues
4. **Document decisions** - Capture reasoning, not just outcomes
5. **Stay within scope** - Don't implement during planning

### Example Interaction

**Human:** "Help me design the retry mechanism for rate limit recovery."

**Agent should:**

1. Ask about requirements (max retries, backoff strategy, failure handling)
2. Present options (exponential backoff vs fixed delay, immediate vs deferred)
3. Discuss trade-offs (complexity vs user experience)
4. Document the decision in ADR format
5. NOT write any implementation code

### Outputs

- ADRs in `docs/adr/`
- Design documents in `docs/plans/`
- Issue breakdowns with acceptance criteria
- Decision logs with rationale

### Documentation to Reference

- ADR templates in `docs/adr/`
- Existing design documents in `docs/plans/`
- `docs/adr/0008-documentation-strategy.md`

---

## Pattern 2: Pair Programming

Use this pattern when working alongside a human developer in real-time.

### Characteristics

| Aspect             | Details                 |
| ------------------ | ----------------------- |
| **Mode**           | Real-time collaboration |
| **Human Role**     | Driver or navigator     |
| **Agent Role**     | Navigator or driver     |
| **Autonomy Level** | Guided to Supervised    |

### When to Use

- Implementing features together
- Debugging complex issues
- Writing tests
- Refactoring code
- Learning new patterns

### Agent Behaviours (as Navigator)

1. **Suggest approaches** - Offer ideas, let human decide
2. **Spot issues** - Point out potential bugs, style violations
3. **Explain reasoning** - Share thinking, not just answers
4. **Reference standards** - Point to relevant docs
5. **Stay responsive** - Quick, focused responses

### Agent Behaviours (as Driver)

1. **Explain before coding** - State intent before implementation
2. **Write incrementally** - Small, reviewable chunks
3. **Pause for feedback** - Check in regularly
4. **Follow conventions** - Strict adherence to coding standards
5. **Test continuously** - Write tests as you go

### Example Interaction (Agent as Navigator)

**Human:** "I'm implementing the config loading. Should I use a static method or constructor?"

**Agent should:**

1. Consider both approaches
2. Reference project patterns (check existing code)
3. Suggest one approach with reasoning
4. Note any trade-offs

**Response:** "Looking at existing patterns in `ClaudeMonitor.cs`, we use constructor
injection for dependencies. I'd suggest a constructor that takes an
`IConfigurationProvider`. This keeps the class testable and follows our DI conventions.
If you need a convenience method, we could add a static `LoadFromDefault()` factory
method."

### Example Interaction (Agent as Driver)

**Human:** "Write the unit tests for the retry logic."

**Agent should:**

1. State what tests will be written
2. Write one test at a time
3. Pause for review
4. Adjust based on feedback

**Response:** "I'll write tests for the retry logic. Let me start with the happy path
test for successful retry, then cover edge cases. Here's the first test..."

### Outputs

- Code changes (reviewed in real-time)
- Tests
- Commits (with human approval)

### Documentation to Reference

- `docs/agents/CONVENTIONS.md` - Coding standards
- `docs/standards/coding-standards.md` - Full standards
- Existing similar code in the codebase

---

## Pattern 3: Verification & Review

Use this pattern when performing specialised validation from a specific perspective.

### Characteristics

| Aspect             | Details                   |
| ------------------ | ------------------------- |
| **Mode**           | Asynchronous review       |
| **Human Role**     | Requestor, final approver |
| **Agent Role**     | Specialist reviewer       |
| **Autonomy Level** | Supervised                |

### When to Use

- Code review (general or specialised)
- Security audit
- Accessibility review
- Performance analysis
- Documentation review
- Architecture review

### Agent Behaviours

1. **Focus on assigned perspective** - Don't scope creep
2. **Be specific** - Point to exact lines, provide examples
3. **Prioritise findings** - Critical > Major > Minor
4. **Suggest fixes** - Don't just identify, propose solutions
5. **Justify concerns** - Reference standards, explain impact

### Specialist Perspectives

| Persona                | Focus           | Key Concerns                             |
| ---------------------- | --------------- | ---------------------------------------- |
| Security Reviewer      | Vulnerabilities | OWASP top 10, input validation, secrets  |
| Accessibility Auditor  | WCAG compliance | Screen readers, keyboard nav, contrast   |
| Performance Analyst    | Efficiency      | Memory, CPU, async patterns, allocations |
| Architecture Reviewer  | Structure       | Dependencies, coupling, SOLID principles |
| Documentation Reviewer | Clarity         | Accuracy, completeness, examples         |

### Example: Security Review

**Human:** "Review this PR from a security perspective."

**Agent should:**

1. Check input validation
2. Look for injection vulnerabilities
3. Verify authentication/authorisation
4. Check for hardcoded secrets
5. Review error handling for information leakage

**Response format:**

```markdown
## Security Review: PR #123

### Critical Issues

None found.

### Major Issues

1. **Potential command injection** (src/ClaudeMonitor.cs:145)
   - User input is passed to `Process.Start()` without sanitisation
   - Recommend: Use argument array instead of string interpolation

### Minor Issues

1. **Verbose error messages** (src/Program.cs:89)
   - Exception stack trace included in user-facing error
   - Recommend: Log full trace, show generic message to user

### Positive Observations

- Proper use of parameterised queries in config loading
- CancellationToken correctly propagated
```

### Outputs

- Review comments (structured)
- Severity classifications
- Specific fix recommendations
- Summary reports

### Documentation to Reference

- Relevant checklists in `docs/checklists/` (when available)
- `docs/standards/accessibility.md` for accessibility reviews
- `docs/standards/privacy.md` for privacy considerations

---

## Pattern 4: Autonomous Execution

Use this pattern when independently implementing well-defined tickets.

### Characteristics

| Aspect             | Details                    |
| ------------------ | -------------------------- |
| **Mode**           | Independent execution      |
| **Human Role**     | Ticket author, PR approver |
| **Agent Role**     | Full implementer           |
| **Autonomy Level** | Autonomous                 |

### Prerequisites (All Must Be True)

- [ ] Ticket has clear, unambiguous description
- [ ] Acceptance criteria are explicit and testable
- [ ] All referenced documentation is accessible
- [ ] Scope boundaries are defined
- [ ] No blocking dependencies

### When NOT to Use

- Requirements are vague or incomplete
- Architectural decisions needed
- Cross-cutting changes affecting multiple systems
- Security-critical changes requiring human review
- First implementation of a pattern

### Agent Behaviours

1. **Verify prerequisites** - Don't start if ticket is unclear
2. **Follow the workflow exactly** - No shortcuts
3. **Document everything** - Commits tell the story
4. **Test thoroughly** - Meet coverage requirements
5. **Self-review before PR** - Check your own work

### Execution Workflow

```text
1. READ TICKET
   └── Verify acceptance criteria are clear
   └── Identify all referenced docs
   └── If unclear, STOP and ask questions

2. CREATE BRANCH
   └── git checkout -b type/issue#-description

3. IMPLEMENT
   └── Read relevant coding standards
   └── Write code following conventions
   └── Write tests (aim for 80%+ coverage on changes)
   └── Ensure pre-commit hooks pass

4. COMMIT
   └── Conventional commit format
   └── Include Refs: #issue
   └── One logical change per commit

5. SELF-REVIEW
   └── Review own diff
   └── Check against acceptance criteria
   └── Verify tests are meaningful

6. CREATE PR
   └── Use PR template
   └── Link to issue
   └── Describe changes clearly

7. RESPOND TO FEEDBACK
   └── Address all review comments
   └── Push additional commits
   └── Re-request review
```

### Example: Implementing a Ticket

**Ticket #42:** Add configurable retry delay

**Acceptance Criteria:**

- Users can configure retry delay via `--retry-delay` CLI argument
- Default value is 5000ms
- Value is validated (must be positive integer)
- Unit tests cover configuration loading

**Agent execution:**

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

### Outputs

- Feature branch
- Implementation code
- Unit tests
- PR with full description

### Documentation to Reference

- `docs/adr/0004-contribution-workflow.md` - Full workflow
- `docs/agents/CONVENTIONS.md` - Commit format
- `docs/standards/coding-standards.md` - Code standards

---

## Autonomy Levels

Agents should self-assess their autonomy level based on task characteristics.

### Level 1: Guided

Human leads, agent assists.

**Indicators:**

- Exploratory work
- Unclear requirements
- Multiple valid approaches
- Learning/onboarding

**Agent should:**

- Ask before acting
- Present options
- Defer decisions to human

### Level 2: Supervised

Agent leads, human reviews.

**Indicators:**

- Clear requirements
- Established patterns
- Non-critical changes
- Incremental work

**Agent should:**

- Propose approach first
- Implement in reviewable chunks
- Pause for significant decisions

### Level 3: Autonomous

Agent executes, human approves result.

**Indicators:**

- Well-defined ticket
- Explicit acceptance criteria
- Isolated changes
- Established patterns

**Agent should:**

- Follow workflow exactly
- Document thoroughly
- Self-review before PR
- Accept all feedback gracefully

---

## Sub-Agent Delegation

When platform supports it, use sub-agents for isolated tasks.

### When to Delegate

- Different perspectives needed (security vs implementation)
- Parallel independent tasks
- Long-running operations that benefit from fresh context
- Specialised expertise required

### How to Delegate

```text
Main Agent:
  "I need a security review of the authentication changes."
  → Spawn Security Reviewer sub-agent
  → Sub-agent reviews and returns findings
  → Main agent integrates findings

Main Agent:
  "Implement components A, B, and C from ticket #42"
  → Spawn sub-agent for component A
  → Spawn sub-agent for component B
  → Spawn sub-agent for component C
  → Main agent integrates and creates PR
```

### Sub-Agent Context

Each sub-agent should receive:

1. Specific task description
2. Relevant documentation references
3. Persona identifier (if specialised)
4. Return format expectations

---

## Pattern Selection Guide

```text
Is the task about design/planning?
├── Yes → Pattern 1: Planning & Requirements
└── No
    ├── Are you working in real-time with a human?
    │   ├── Yes → Pattern 2: Pair Programming
    │   └── No
    │       ├── Is this a review/validation task?
    │       │   ├── Yes → Pattern 3: Verification & Review
    │       │   └── No
    │       │       ├── Is the ticket clear with explicit acceptance criteria?
    │       │       │   ├── Yes → Pattern 4: Autonomous Execution
    │       │       │   └── No → Ask for clarification, then reassess
```

## Critical Rules Across All Patterns

1. **Never commit to main** - Always use feature branches
2. **Always reference issues** - Every commit has `Refs: #X`
3. **Follow conventions** - No exceptions for "quick fixes"
4. **When in doubt, ask** - Clarification is better than assumptions
5. **Document decisions** - Future agents will thank you
