---
title: Pair Programming Pattern
summary: Real-time collaboration pattern for implementing features, debugging, and writing tests together
audience: [developer, agent]
topics: [patterns, pair-programming, collaboration, coding]
parent: ../collaboration-patterns.md
last_validated: 2026-01-10
---

# Pair Programming Pattern

Use this pattern when working alongside another developer (human or AI) in real-time.

## Characteristics

| Aspect                | Details                 |
| --------------------- | ----------------------- |
| **Mode**              | Real-time collaboration |
| **Human Role**        | Driver or navigator     |
| **Collaborator Role** | Navigator or driver     |
| **Autonomy Level**    | Guided to Supervised    |

## When to Use

- Implementing features together
- Debugging complex issues
- Writing tests
- Refactoring code
- Learning new patterns

## Behaviours as Navigator

1. **Suggest approaches** - Offer ideas, let the driver decide
2. **Spot issues** - Point out potential bugs, style violations
3. **Explain reasoning** - Share thinking, not just answers
4. **Reference standards** - Point to relevant docs
5. **Stay responsive** - Quick, focused responses

## Behaviours as Driver

1. **Explain before coding** - State intent before implementation
2. **Write incrementally** - Small, reviewable chunks
3. **Pause for feedback** - Check in regularly
4. **Follow conventions** - Strict adherence to coding standards
5. **Test continuously** - Write tests as you go

## Example Interaction (Collaborator as Navigator)

**Human:** "I'm implementing the config loading. Should I use a static method or constructor?"

**Collaborator should:**

1. Consider both approaches
2. Reference project patterns (check existing code)
3. Suggest one approach with reasoning
4. Note any trade-offs

**Response:** "Looking at existing patterns in `ClaudeMonitor.cs`, we use constructor
injection for dependencies. I'd suggest a constructor that takes an
`IConfigurationProvider`. This keeps the class testable and follows our DI conventions.
If you need a convenience method, we could add a static `LoadFromDefault()` factory
method."

## Example Interaction (Collaborator as Driver)

**Human:** "Write the unit tests for the retry logic."

**Collaborator should:**

1. State what tests will be written
2. Write one test at a time
3. Pause for review
4. Adjust based on feedback

**Response:** "I'll write tests for the retry logic. Let me start with the happy path
test for successful retry, then cover edge cases. Here's the first test..."

## Outputs

- Code changes (reviewed in real-time)
- Tests
- Commits (with human approval)

## Documentation to Reference

- `docs/agents/CONVENTIONS.md` - Coding standards
- `docs/standards/coding-standards.md` - Full standards
- Existing similar code in the codebase
