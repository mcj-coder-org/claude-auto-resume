---
title: Definition of Ready
summary: Criteria that must be met before work can begin on a ticket
audience: [developer, agent]
topics: [process, workflow, tickets]
prerequisites: []
related: [definition-of-done.md, ticket-lifecycle.md, ../adr/0004-contribution-workflow.md]
last_validated: 2026-01-10
---

# Definition of Ready

Criteria a ticket must meet before development can begin.

## Required for All Tickets

- [ ] **Clear title** - Describes the outcome, not the task
- [ ] **Acceptance criteria** - Specific, testable conditions for completion
- [ ] **Context provided** - Links to relevant ADRs, designs, or related issues
- [ ] **No blockers** - Dependencies resolved or clearly documented
- [ ] **Appropriately sized** - Can be completed in a single PR

## Additional for Features

- [ ] **Design reviewed** - Architecture approach agreed (ADR if significant)
- [ ] **API contract defined** - For any new public interfaces
- [ ] **Test approach identified** - Unit, integration, or E2E as appropriate

## Additional for Bugs

- [ ] **Reproduction steps** - Clear steps to reproduce the issue
- [ ] **Expected vs actual** - What should happen vs what does happen
- [ ] **Environment details** - OS, .NET version, relevant configuration

## Ready Checklist

Before moving a ticket to "In Progress":

1. Read the ticket fully
2. Verify all criteria above are met
3. Confirm you understand the scope
4. Check for recent related changes in the codebase
5. Identify any questions - ask before starting

## Not Ready Indicators

- Vague requirements ("improve performance")
- Missing acceptance criteria
- Unresolved dependencies
- Scope unclear or too large
- No way to verify completion
