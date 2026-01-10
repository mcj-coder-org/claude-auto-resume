---
title: Planning & Requirements Pattern
summary: Collaborative dialogue pattern for designing features, writing ADRs, and planning implementation approaches
audience: [developer, agent]
topics: [patterns, planning, requirements, collaboration]
parent: ../collaboration-patterns.md
last_validated: 2026-01-10
---

# Planning & Requirements Pattern

Use this pattern when designing features, writing ADRs, or planning implementation approaches.

## Characteristics

| Aspect                | Details                             |
| --------------------- | ----------------------------------- |
| **Mode**              | Collaborative dialogue              |
| **Human Role**        | Decision maker, domain expert       |
| **Collaborator Role** | Facilitator, documenter, challenger |
| **Autonomy Level**    | Guided (human leads)                |

## When to Use

- Designing new features
- Writing or updating ADRs
- Breaking down epics into issues
- Exploring technical approaches
- Clarifying requirements

## Collaborator Behaviours

1. **Ask clarifying questions** - Don't assume, verify understanding
2. **Present options** - Offer 2-3 approaches with trade-offs
3. **Challenge assumptions** - Point out potential issues
4. **Document decisions** - Capture reasoning, not just outcomes
5. **Stay within scope** - Don't implement during planning

## Example Interaction

**Human:** "Help me design the retry mechanism for rate limit recovery."

**Collaborator should:**

1. Ask about requirements (max retries, backoff strategy, failure handling)
2. Present options (exponential backoff vs fixed delay, immediate vs deferred)
3. Discuss trade-offs (complexity vs user experience)
4. Document the decision in ADR format
5. NOT write any implementation code

## Outputs

- ADRs in `docs/adr/`
- Design documents in `docs/plans/`
- Issue breakdowns with acceptance criteria
- Decision logs with rationale

## Documentation to Reference

- ADR templates in `docs/adr/`
- Existing design documents in `docs/plans/`
- `docs/adr/0008-documentation-strategy.md`
