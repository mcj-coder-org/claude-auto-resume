---
name: Sub-Issue / Task
about: A task that is part of a larger feature or epic
title: ''
labels: 'enhancement'
assignees: ''
---

## Parent Issue

Refs: #<!-- parent issue number -->

## Context

<!-- Link to design documents using immutable URLs (commit SHA or tag) -->

- [Parent Epic Design Plan](<!-- https://github.com/mcj-coder-org/claude-auto-resume/blob/{sha}/docs/plans/{issue#}-{name}-design-plan.md -->)
- [Implementation Section](<!-- link to specific section in design plan if applicable -->)

## Base Branch

<!-- Specify which branch to branch from -->

**Base Branch:** <!-- `main` (Strategy A) OR `feature/{parent-issue#}-{parent-name}` (Strategy B) -->

**Deployment Strategy:** <!-- Strategy A (feature flags) OR Strategy B (feature branch) -->

## Feature Flags (Strategy A Only)

<!-- If using Strategy A, specify which feature flags control this sub-issue -->

**Flag:** <!-- `ParentFeature.SubFeature` (e.g., `ClaudeMonitoring.RateLimitDetection`) -->
**Default State:** <!-- `disabled` (features behind flags until epic complete) -->

## Description

Brief description of this task and what it delivers.

## Tasks

- [ ] Task 1
- [ ] Task 2
- [ ] Task 3

## Acceptance Criteria

- [ ] Feature implemented per design
- [ ] All tests added and passing (unit, integration, system/BDD, E2E as applicable)
- [ ] Test evidence posted in task completion comments
- [ ] No new analyzer warnings
- [ ] XML documentation added for public APIs
- [ ] Design plan updated if implementation deviates
- [ ] Follow-up issues created and linked if work deferred

## Testing Approach

<!-- Specify which test types are required -->

- [ ] Unit tests: <!-- describe key scenarios -->
- [ ] Integration tests: <!-- if applicable -->
- [ ] System/BDD tests: <!-- if behavior change -->
- [ ] E2E tests: <!-- if user-facing -->
- [ ] Manual testing: <!-- if complex behavior -->

## Notes

Any additional notes or context for implementers.

## Definition of Ready Checklist

Before moving to In Progress, verify:

- [ ] All context links accessible (immutable URLs work)
- [ ] Base branch clearly specified
- [ ] Feature flags identified (if Strategy A)
- [ ] Priority and milestone assigned
- [ ] Skill set labels applied
- [ ] Parent epic and all sibling sub-issues in Ready state
