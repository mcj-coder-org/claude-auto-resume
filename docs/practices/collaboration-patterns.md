---
title: Collaboration Patterns
summary: Guidance for different collaboration modes and autonomy levels between humans and AI assistants
audience: [developer, agent]
topics: [patterns, workflows, autonomy, collaboration]
prerequisites: [../agents/AGENTS.md, ../agents/ORIENTATION.md]
related: [../standards/roles.md, ../adr/0009-agent-onboarding.md]
last_validated: 2026-01-10
---

# Collaboration Patterns

This document describes how humans and AI assistants should work together in different
contexts. Each pattern has specific expectations for autonomy, outputs, and collaboration.

## Pattern Overview

| Pattern                                                                | Autonomy             | Human Role              | Collaborator Role   | Typical Output           |
| ---------------------------------------------------------------------- | -------------------- | ----------------------- | ------------------- | ------------------------ |
| [Planning & Requirements](collaboration-patterns/planning.md)          | Guided               | Decision maker          | Facilitator         | ADRs, designs, plans     |
| [Pair Programming](collaboration-patterns/pair-programming.md)         | Guided to Supervised | Driver/Navigator        | Navigator/Driver    | Code, tests, commits     |
| [Verification & Review](collaboration-patterns/verification-review.md) | Supervised           | Final approver          | Specialist reviewer | Review comments, reports |
| [Autonomous Execution](collaboration-patterns/autonomous-execution.md) | Autonomous           | Ticket author, approver | Full implementer    | Branch, commits, PR      |

---

## Autonomy Levels

Collaborators should self-assess their autonomy level based on task characteristics.

### Level 1: Guided

Human leads, collaborator assists.

**Indicators:**

- Exploratory work
- Unclear requirements
- Multiple valid approaches
- Learning/onboarding

**Collaborator should:**

- Ask before acting
- Present options
- Defer decisions to human

### Level 2: Supervised

Collaborator leads, human reviews.

**Indicators:**

- Clear requirements
- Established patterns
- Non-critical changes
- Incremental work

**Collaborator should:**

- Propose approach first
- Implement in reviewable chunks
- Pause for significant decisions

### Level 3: Autonomous

Collaborator executes, human approves result.

**Indicators:**

- Well-defined ticket
- Explicit acceptance criteria
- Isolated changes
- Established patterns

**Collaborator should:**

- Follow workflow exactly
- Document thoroughly
- Self-review before PR
- Accept all feedback gracefully

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

---

## Critical Rules Across All Patterns

1. **Never commit to main** - Always use feature branches
2. **Always reference issues** - Every commit has `Refs: #X`
3. **Follow conventions** - No exceptions for "quick fixes"
4. **When in doubt, ask** - Clarification is better than assumptions
5. **Document decisions** - Future collaborators will thank you
