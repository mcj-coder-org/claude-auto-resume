---
title: Definition of Done
summary: Criteria that must be met before work is considered complete
audience: [developer, agent]
topics: [process, workflow, quality]
prerequisites: []
related: [definition-of-ready.md, ticket-lifecycle.md, code-review.md, ../adr/0030-feature-flags.md]
last_validated: 2026-01-10
---

# Definition of Done

Criteria that must be satisfied before a ticket can be closed. Applies to all issue types with
additional requirements for epics.

## Code Complete

- [ ] **Implementation complete** - All acceptance criteria met
- [ ] **Code reviewed** - PR approved by at least one reviewer
- [ ] **All conversations resolved** - Reviewer resolved all inline comment threads
- [ ] **No analyzer warnings** - Build passes with zero warnings
- [ ] **Follows coding standards** - Per `docs/standards/coding-standards.md`

## Testing Complete

- [ ] **Unit tests pass** - All existing and new tests green
- [ ] **Coverage maintained** - No reduction in code coverage
- [ ] **Integration tests pass** - If applicable to the change
- [ ] **System tests pass** - BDD scenarios updated and passing if behavior change
- [ ] **E2E tests pass** - If user-facing change
- [ ] **Manual testing done** - For UI or complex behavioral changes
- [ ] **Test evidence posted** - In task completion comments during development

## Documentation Complete

- [ ] **XML docs updated** - For any new/changed public APIs
- [ ] **README updated** - If user-facing behavior changed
- [ ] **Design plan updated** - If implementation deviated from plan (version history)
- [ ] **ADR created/updated** - If architectural decision was made
- [ ] **CHANGELOG entry** - For user-visible changes
- [ ] **Follow-up issues tracked** - In design plan if work deferred

## Deployment Ready

- [ ] **PR merged** - Squash-merged to main (or feature branch for Strategy B sub-issues)
- [ ] **CI pipeline green** - All checks pass on target branch after merge
- [ ] **Main branch clean** - If merged to main, all CI checks passing
- [ ] **No regressions** - Existing functionality unaffected

## Done Checklist

Before closing a ticket:

1. All acceptance criteria verified
2. PR merged to target branch
3. CI pipeline successful on target branch
4. Documentation updated
5. Test evidence posted
6. Design plan updated (if scope changes occurred)
7. Follow-up issues created and tracked (if applicable)
8. Ticket moved to Done column

## Issue Type Specific Requirements

### Single Issue (Standalone Feature/Bug)

**Standard Done:**

- All above criteria met
- PR merged to main
- Issue auto-closed via `Closes #123`
- No additional actions required

### Sub-Issue (Part of Epic)

**Sub-Issue Done:**

- All above criteria met
- PR merged to main (Strategy A) or feature branch (Strategy B)
- Issue auto-closed via `Closes #124`
- **Additional:** Post completion comment on parent epic:

```markdown
✅ Sub-issue #124 completed and merged

**Delivery:**

- Rate limit detection implemented
- Feature behind flag: `ClaudeMonitoring.RateLimitDetection` (disabled)
- All tests passing (23 unit, 5 integration, 2 BDD scenarios)
- PR merged: #PR-link
- Coverage: 85% line, 78% branch

**Parent epic checklist:**

- [x] #124 - Rate limit detection (DONE)
- [ ] #125 - Auto retry logic
- [ ] #126 - Metrics collection
```

**State:**

- Sub-issue: Done
- Parent epic: Remains In Progress until all sub-issues complete

### Epic (Parent Issue)

**Epic owner must complete before closing:**

#### 1. Verify All Sub-Issues Complete

- [ ] All sub-issues closed and merged
- [ ] All sub-issue checklists marked complete on epic
- [ ] All tests passing across all sub-issues

#### 2. Enable Feature Flags (Strategy A Only)

- [ ] Review feature flag enablement plan in design document
- [ ] Update Azure App Configuration (or configured flag provider)
- [ ] Enable parent flag and all sub-feature flags
- [ ] Document enablement date/time in design plan
- [ ] Monitor for stability (minimum 24 hours)
- [ ] Post enablement confirmation comment on epic:

```markdown
✅ Feature flags enabled

**Environment:** Production
**Date:** 2026-01-15 14:00 UTC
**Method:** Azure App Configuration update

**Flags enabled:**

- ClaudeMonitoring = true
- ClaudeMonitoring.RateLimitDetection = true
- ClaudeMonitoring.AutoRetry = true
- ClaudeMonitoring.Metrics = true

**Monitoring (24h):**

- Application Insights: No errors
- Metrics: Detection working (23 rate limits detected)
- Performance: No degradation observed

**Status:** Stable ✓
```

#### 3. Archive Design Plan

- [ ] Update design plan header:
  - Status: `Draft` → `Implemented`
  - Add implementation date
  - Add final version number
- [ ] Add implementation summary section:
  - What was delivered
  - All sub-issues links
  - Feature flag status (if applicable)
  - Next steps (flag removal timeline)
- [ ] Move design plan to archive:
  - From: `docs/plans/{issue#}-{name}-design-plan.md`
  - To: `docs/plans/archive/{issue#}-{name}-design-plan.md`
- [ ] Commit with reference to epic:

```bash
git add docs/plans/archive/123-claude-monitoring-design-plan.md
git commit -m "docs: archive epic 123 design plan

Epic completed and feature flags enabled.
Design plan moved to archive for historical reference.

Refs: #123"
git push
```

#### 4. Update ADRs (Proposed → Accepted)

- [ ] Identify all ADRs related to epic (referenced in design plan)
- [ ] Update each ADR:
  - Status: `Proposed` → `Accepted`
  - Add `Accepted Date: YYYY-MM-DD`
  - Add implementation section with epic reference and design plan archive link
- [ ] Commit ADR updates:

```bash
git add docs/adr/0031-claude-monitoring-framework.md
git commit -m "docs: mark ADR-0031 as accepted

Epic #123 completed. Claude monitoring framework
implemented and enabled in production.

Refs: #123"
git push
```

#### 5. Create Feature Flag Removal Tickets

Per ADR-0030, feature flags must be removed within 2 releases of enablement.

- [ ] Create follow-up issue for flag removal:
  - Title: `Remove {FeatureName} feature flags`
  - Target: 2 releases after enablement
  - Link back to epic and archived design plan
- [ ] Add removal ticket to design plan Follow-up Issues section
- [ ] Update design plan version history with follow-up issue link

**Example Follow-up Issue:**

```markdown
Title: Remove ClaudeMonitoring feature flags

## Context

Epic #123 completed and feature flags enabled on 2026-01-15.
Per ADR-0030, flags should be removed 2 releases after rollout.

## Target Release

v2.1.0 (2 releases after v2.0.0 where flags enabled)

## Tasks

- [ ] Verify flags enabled for 2+ releases in production
- [ ] Remove feature flag checks from code
- [ ] Make behavior default (no conditional checks)
- [ ] Remove flag configuration from Azure App Config
- [ ] Update documentation to remove flag references
- [ ] Update tests to remove flag toggling

## Acceptance Criteria

- [ ] All `ClaudeMonitoring` flag checks removed from codebase
- [ ] Tests updated to test only default behavior
- [ ] Documentation reflects flags no longer exist
- [ ] Clean build with no flag references
- [ ] All tests passing

## Related

- Parent epic: #123
- Design plan: docs/plans/archive/123-claude-monitoring-design-plan.md
- ADR-0030: Feature Flags

Refs: #123
```

#### 6. Update Design Plan Follow-up Issues Section

Ensure all follow-up issues created during implementation/review are tracked:

```markdown
## Follow-up Issues

Issues created during implementation/review for future work:

| Issue | Title                         | Reason                                            | Target Release | Status |
| ----- | ----------------------------- | ------------------------------------------------- | -------------- | ------ |
| #456  | Optimize rate limit detection | Performance improvement deferred during PR review | v2.1.0         | Open   |
| #457  | Add retry telemetry dashboard | Observability enhancement identified              | v2.2.0         | Open   |
| #458  | Remove ClaudeMonitoring flags | Feature flag cleanup per ADR-0030                 | v2.1.0         | Open   |

All follow-up issues link back to this design plan and epic #123.
```

#### 7. Post Epic Closure Summary

- [ ] Post comprehensive closure comment on epic:

```markdown
## Epic Closure Summary

**Epic:** #123 - Claude Monitoring Framework
**Closed:** 2026-01-15
**Owner:** @epic-owner
**Duration:** 2 weeks (2026-01-01 to 2026-01-15)

**Delivered:**
✅ All 3 sub-issues completed and merged
✅ Feature flags enabled in production (2026-01-15)
✅ Design plan archived: `docs/plans/archive/123-claude-monitoring-design-plan.md`
✅ ADR-0031 updated to Accepted
✅ Feature flag removal ticket created: #458

**Sub-Issues Completed:**

- #124 - Rate limit detection ✓
- #125 - Auto retry logic ✓
- #126 - Metrics collection ✓

**Production Status:**

- Enabled: 2026-01-15 14:00 UTC
- Monitoring: Stable, no issues in 24h observation
- Metrics: Detection working as expected (23 detections)
- Performance: No degradation observed

**Follow-up Work:**

- #456 - Performance optimization (target: v2.1.0)
- #457 - Telemetry dashboard (target: v2.2.0)
- #458 - Feature flag removal (target: v2.1.0)

**Next Steps:**

- Monitor for 2 releases
- Remove flags in v2.1.0 per #458
- Consider performance optimization in #456

**Closing epic.**
```

#### 8. Close Epic

- [ ] Manually close epic issue (unless using commit message)
- [ ] Move to Done column on project board
- [ ] Verify all sub-issues also in Done column

## Strategy B (Feature Branch) Epic Closure

For epics using feature branch strategy:

### Final Epic PR to Main

- [ ] Create epic closure PR:
  - Branch: `feature/{issue#}-{name}` → `main`
  - Title: `feat(scope): complete {feature name} epic (#{issue})`
  - Description: Summary of all sub-issues and complete testing results
  - Link: `Closes #{epic-issue}`

- [ ] High-level review (sub-issues already reviewed individually)
  - Integration testing across sub-issues
  - Documentation completeness
  - No conflicts with main

- [ ] Merge to main (squash merge entire feature branch)

- [ ] Epic auto-closed via `Closes` link

- [ ] Complete steps 3-8 above (archive plan, update ADRs, create follow-ups, post summary)

**Note:** No feature flags for Strategy B (code merges all at once)

## Exceptions

Some criteria may be waived with justification:

| Criterion           | Valid Exception                      |
| ------------------- | ------------------------------------ |
| Coverage maintained | Deleting dead code                   |
| README updated      | Internal refactoring                 |
| CHANGELOG entry     | Non-user-facing changes              |
| Manual testing      | Pure refactoring, no behavior change |

Document exceptions in the PR description with rationale.

## Main Branch Health Requirement

**Critical:** Main branch must remain clean after merge.

If CI fails on main after merge:

1. 🚨 Create CRITICAL priority issue immediately
2. Development freeze - no new work starts
3. Owner fixes immediately or reverts
4. Cannot close ticket until main is clean

Owner responsible for post-merge main branch health.
