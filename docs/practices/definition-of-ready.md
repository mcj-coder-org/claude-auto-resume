---
title: Definition of Ready
summary: Criteria that must be met before work can begin on a ticket
audience: [developer, agent]
topics: [process, workflow, tickets]
prerequisites: []
related:
  [
    definition-of-done.md,
    ticket-lifecycle.md,
    ../adr/0004-contribution-workflow.md,
    ../adr/0003-work-item-management.md,
  ]
last_validated: 2026-01-10
---

# Definition of Ready

Criteria a ticket must meet before development can begin. All criteria verified during Triage
phase before moving to Ready state.

## Required for All Tickets

- [ ] **Clear title** - Describes the outcome, not the task
- [ ] **Acceptance criteria** - Specific, testable conditions for completion
- [ ] **Context provided** - Links to relevant ADRs, designs, or related issues
- [ ] **Design plan linked** - Immutable GitHub blob URL to approved design plan
  - Format: `https://github.com/org/repo/blob/{commit-sha}/docs/plans/{issue#}-{name}-design-plan.md`
  - Status must be `Approved v1` or higher
  - Design plan must be on feature branch, committed and pushed
  - **For Epics:** Plan must be a high-level breakdown at component/deliverable unit level,
    capturing requirements into sub-tickets, how they interrelate, and required skillsets.
  - **For Sub-issues:** Plan must be a detailed breakdown into tasks (1-4 hours each) manageable
    by a single person/role.
- [ ] **No blockers** - Dependencies resolved or clearly documented
- [ ] **Appropriately sized** - Can be completed in a single PR
- [ ] **Priority assigned** - Scrum Master has set priority label (`priority:critical`, `priority:high`,
      `priority:medium`, `priority:low`)
- [ ] **Milestone assigned** - Scrum Master has assigned to milestone
- [ ] **Skill set labeled** - Required skills/personas labeled for pull-based assignment
  - Examples: `skill:dotnet`, `skill:security`, `skill:infrastructure`, `skill:documentation`,
    `skill:testing`

## Additional for Sub-Issues

- [ ] **Parent epic linked** - `Refs: #{parent-issue}` in description
- [ ] **Parent design plan linked** - Immutable link to epic's design plan (for context)
  - Provides complete epic context even if parent design plan updated
- [ ] **Base branch specified** - Clearly states which branch to branch from:
  - **Strategy A:** Base branch is `main` (deploy independently with feature flags)
  - **Strategy B:** Base branch is `feature/{parent-issue#}-{parent-name}` (feature branch strategy)
  - Must be explicit in sub-issue description
- [ ] **Feature flags identified** - Which flags control this sub-issue's behavior (if Strategy A)
  - Example: `ClaudeMonitoring.RateLimitDetection`
  - Flag should be in design plan's feature flag table
- [ ] **All sibling sub-issues in Ready** - For epics, all sub-issues must be Ready before any can start

## Additional for Features

- [ ] **Design reviewed** - Architecture approach agreed (ADR if significant architectural decision)
- [ ] **API contract defined** - For any new public interfaces
  - Public classes, methods, properties documented
  - Expected inputs/outputs specified
  - Error cases identified
- [ ] **Test approach identified** - Unit, integration, E2E, or BDD as appropriate
  - Which test types required
  - Key scenarios to cover
  - Acceptance criteria mapping to tests

## Additional for Bugs

- [ ] **Reproduction steps** - Clear steps to reproduce the issue
  - Step-by-step instructions
  - Minimal repro if possible
- [ ] **Expected vs actual** - What should happen vs what does happen
- [ ] **Environment details** - OS, .NET version, relevant configuration
  - May affect reproduction and fix validation

## Additional for Epics

- [ ] **All sub-issues created** - Complete breakdown into deliverable units
- [ ] **Sub-issues link back** - Each sub-issue has `Refs: #{epic-issue}` and immutable design plan link
- [ ] **Deployment strategy documented** - Explicit in design plan:
  - **Strategy A (Preferred):** Deploy to main independently with feature flags
    - Feature flags table in design plan
    - Flag enablement plan documented
    - Sub-issues can merge to main safely
  - **Strategy B (Fallback):** Feature branch as base for all sub-issues
    - Feature branch name specified: `feature/{epic-issue#}-{epic-name}`
    - Final epic PR will merge feature branch → main
    - Sub-issues merge to feature branch
- [ ] **Epic owner assigned** - Responsible for coordination and epic closure
- [ ] **Feature flag table** (if Strategy A) - In design plan with all flags listed

## Ready Checklist

Before moving a ticket from Triage to Ready:

1. **Read the ticket fully** - Understand the complete scope
2. **Verify all criteria above are met** - Check every box for ticket type
3. **Confirm design plan accessibility** - Immutable link works, status is `Approved v1`
4. **Check for unresolved questions** - All originator questions answered
5. **Verify labels applied** - Priority, milestone, skill set labels present
6. **For epics:** Verify all sub-issues also meet Ready criteria

## Not Ready Indicators

Issue should stay in Triage if:

- Vague requirements ("improve performance" without metrics)
- Missing acceptance criteria or not testable
- Unresolved dependencies or blockers
- Scope unclear or too large for single PR
- No way to verify completion objectively
- Design plan not approved (`Draft` status)
- Missing priority or milestone
- Skill set labels not applied
- Sub-issues not created (for epics)
- Base branch not specified (for sub-issues)

## Pull Model (Kanban)

**No Assignment in Ready State:**

- Issues remain unassigned
- Developers/agents self-select based on:
  - Priority (highest first)
  - Skill set match (labels)
  - Personal availability
  - Interest/expertise

**Self-Assignment Process:**

1. Review Ready column
2. Filter by skill set labels matching your expertise
3. Select highest priority unassigned issue
4. Verify all Ready criteria met
5. Self-assign and move to In Progress
6. Create branch and begin work

**Benefits of Pull Model:**

- No work assigned but not started
- Team members pull when truly ready
- Clear visibility of available work
- Natural load balancing
- Skills-based work distribution

## Design Plan Requirements

Every ticket in Ready must link to an approved design plan:

**Design Plan Location:**

- Path: `docs/plans/{issue#}-{feature-name}-design-plan.md`
- On feature branch: `feature/{issue#}-{feature-name}`
- Committed and pushed (for immutable link generation)

**Design Plan Status:**

- Must be `Approved v1` minimum
- Higher versions acceptable (v1.1, v2.0, etc.) if amended during implementation

**Design Plan Contents:**

- Feature summary
- Key requirements
- Personas involved in refinement
- Testing approach
- Security concerns and mitigations
- Breaking changes highlighted (behavior, API, contracts)
- Expected artifacts (documentation, deployment components)
- Version history table (for tracking amendments)
- Follow-up issues section (for deferred work)

**Immutable Link Format:**

```text
https://github.com/{org}/{repo}/blob/{commit-sha}/docs/plans/{issue#}-{name}-design-plan.md
```

**Why Immutable Links:**

- Design plan may be updated during implementation
- Sub-issues need stable reference to original plan
- Immutable link points to specific commit
- Prevents confusion from plan evolution

## Skill Set Labels Guide

| Label                  | When to Use                              |
| ---------------------- | ---------------------------------------- |
| `skill:dotnet`         | C# code, .NET framework work             |
| `skill:security`       | Authentication, authorization, crypto    |
| `skill:infrastructure` | CI/CD, DevOps, deployment, Azure         |
| `skill:documentation`  | README, ADRs, user guides, API docs      |
| `skill:testing`        | Test frameworks, BDD, test architecture  |
| `skill:frontend`       | UI, UX, client-side work (if applicable) |

Multiple labels acceptable for cross-functional work.

## Priority Guide

| Priority            | Meaning                     | SLA                 |
| ------------------- | --------------------------- | ------------------- |
| `priority:critical` | Blocking release, urgent    | Pick up immediately |
| `priority:high`     | Important for next release  | Within 2 days       |
| `priority:medium`   | Normal priority             | Normal flow         |
| `priority:low`      | Nice to have, when capacity | No SLA              |

Scrum Master assigns priority during refinement based on business value and technical dependencies.

## Example Ready Ticket

```markdown
## Issue #123: Add rate limit detection to Claude monitoring

**Status:** Ready
**Labels:** enhancement, priority:high, skill:dotnet, skill:testing
**Milestone:** v2.0.0
**Design Plan:** https://github.com/org/repo/blob/abc123/docs/plans/123-claude-monitoring-design-plan.md (Approved v1)

### Acceptance Criteria

- [x] Detect rate limit responses from Claude API
- [x] Extract retry-after header value
- [x] Return structured rate limit information
- [x] Handle missing or malformed headers gracefully

### Context

- Parent Epic: Refs: #120
- Related ADR: ADR-0031 (Claude Monitoring Framework)
- Base Branch: `main` (Strategy A with feature flags)
- Feature Flag: `ClaudeMonitoring.RateLimitDetection` (disabled by default)

### Testing Approach

- Unit tests: Rate limit detection logic with various response types
- Integration tests: Mock API responses with rate limit headers
- BDD scenarios: End-to-end rate limit handling workflow

✅ All Definition of Ready criteria met. Ready to be pulled by developer.
```
