---
title: Definition of Done
summary: Criteria that must be met before work is considered complete
audience: [developer, agent]
topics: [process, workflow, quality]
prerequisites: []
related: [definition-of-ready.md, ticket-lifecycle.md, code-review.md]
last_validated: 2026-01-10
---

# Definition of Done

Criteria that must be satisfied before a ticket can be closed.

## Code Complete

- [ ] **Implementation complete** - All acceptance criteria met
- [ ] **Code reviewed** - PR approved by at least one reviewer
- [ ] **No analyzer warnings** - Build passes with zero warnings
- [ ] **Follows coding standards** - Per `docs/standards/coding-standards.md`

## Testing Complete

- [ ] **Unit tests pass** - All existing and new tests green
- [ ] **Coverage maintained** - No reduction in code coverage
- [ ] **Integration tests pass** - If applicable to the change
- [ ] **Manual testing done** - For UI or complex behavioural changes

## Documentation Complete

- [ ] **XML docs updated** - For any new/changed public APIs
- [ ] **README updated** - If user-facing behaviour changed
- [ ] **ADR created** - If architectural decision was made
- [ ] **CHANGELOG entry** - For user-visible changes

## Deployment Ready

- [ ] **PR merged** - Squash-merged to main branch
- [ ] **CI pipeline green** - All checks pass on main
- [ ] **No regressions** - Existing functionality unaffected

## Done Checklist

Before closing a ticket:

1. All acceptance criteria verified
2. PR merged to main
3. CI pipeline successful
4. Documentation updated
5. Ticket moved to Done column

## Exceptions

Some criteria may be waived with justification:

| Criterion           | Valid Exception         |
| ------------------- | ----------------------- |
| Coverage maintained | Deleting dead code      |
| README updated      | Internal refactoring    |
| CHANGELOG entry     | Non-user-facing changes |

Document exceptions in the PR description.
