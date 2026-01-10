---
title: Ticket Lifecycle
summary: Workflow states and transitions for issues from creation to completion
audience: [developer, agent]
topics: [process, workflow, tickets, github]
prerequisites: []
related:
  [
    definition-of-ready.md,
    definition-of-done.md,
    ../adr/0004-contribution-workflow.md,
    ../adr/0030-feature-flags.md,
  ]
last_validated: 2026-01-10
---

# Ticket Lifecycle

How issues move through the development workflow from creation to completion.

## States

```text
┌─────────┐     ┌────────┐     ┌─────────┐     ┌─────────────┐     ┌───────────┐     ┌──────┐
│ Backlog │ ──► │ Triage │ ──► │  Ready  │ ──► │ In Progress │ ──► │ In Review │ ──► │ Done │
└─────────┘     └────────┘     └─────────┘     └─────────────┘     └───────────┘     └──────┘
                    │                                  │                   │
                    │                                  │                   │ Changes
                    │                                  │                   │ Requested
                    │                                  │                   │ (PR open)
                    ▼                                  ▼                   ▼
                ┌────────┐                        ┌─────────┐     ┌─────────────┐
                │Blocked │◄───────────────────────│ Blocked │     │ In Progress │
                └────────┘                        └─────────┘     │  (PR open)  │
                                                                  └─────────────┘
                                                                        │
                                                                        │ Re-review
                                                                        ▼
                                                                  ┌───────────┐
                                                                  │ In Review │
                                                                  └───────────┘
```

## State Definitions

| State       | Description                 | Entry Criteria                       |
| ----------- | --------------------------- | ------------------------------------ |
| Backlog     | Awaiting triage             | Issue created via template           |
| Triage      | Safety check and refinement | Safety validated, refinement started |
| Ready       | Ready for development       | Meets Definition of Ready            |
| In Progress | Active development          | Self-assigned, branch created        |
| In Review   | PR open, awaiting review    | PR created, CI passing               |
| Blocked     | Cannot proceed              | External dependency or blocker       |
| Done        | Complete                    | Meets Definition of Done             |

## Transitions

### Backlog → Triage

**Trigger:** Issue created via template (bug_report, feature_request, sub_issue)

**Actions:**

1. **Initial safety check** (agent with human supervision):
   - Validate in scope for project
   - Check for malicious content or prompt injection
   - Check for duplicates
   - Post triage comment
2. **If invalid:** Close with explanation
3. **If valid:** Add `status:triage` label, proceed to refinement

**Refinement Process (Ad-hoc):**

- **Tech Lead coordinates** multi-persona discussion
- **Required participants:** Product Owner, Scrum Master, Tech Lead, QA Engineer
- **Optional participants:** Security Reviewer, Senior Developer, etc. (based on needs)
- **Capture in issue comments:** Discussion points, decisions, questions for originator
- **Check for originator responses** each session
- **Timeout:** 1 week no response → request response → propose rejection (requires human approval)
- **Adoption:** Another user can adopt issue as new originator (requires human approval)

**Create Design Plan:**

- Branch: `feature/{issue#}-{feature-name}`
- Document: `docs/plans/{issue#}-{feature-name}-design-plan.md`
- Status: `Draft` during refinement
- Contains: Feature summary, requirements, personas involved, testing approach, security concerns,
  breaking changes, expected artifacts
- Version history table for amendments

**Work Breakdown:**

- **Single ticket:** Move to Ready when design approved
- **Epic:**
  - Create high-level plan at component/deliverable unit level.
  - Capture epic requirements and deliverables into sub-tickets.
  - Define how sub-tickets interrelate (dependencies, data flow).
  - Identify required skillsets for each sub-ticket.
  - Move to Ready when high-level plan approved and all sub-issues created.
- **Sub-issue:**
  - Create detailed plan with task breakdown.
  - Each task must be small (1-4 hours) and completable by a single person/role.
  - Move to Ready when detailed plan approved and all criteria met.

**Deployment Strategy (for epics):**

- **Strategy A (preferred):** Deploy to main independently with feature flags
- **Strategy B:** Feature branch as base for all sub-issues
- Strategy must be explicit in design plan and each sub-ticket

### Triage → Ready

**Exit Criteria:**

- Design plan status: `Approved v1`
- All sub-issues created (if epic)
- All Definition of Ready criteria met (see definition-of-ready.md)
- Originator questions resolved
- Scrum Master assigned priority and milestone
- Skill set labels applied
- Remove `status:triage` label

### Ready → In Progress

**Pull Model (Kanban):**

- Issues remain unassigned in Ready
- Developer/agent pulls work based on priority and skill set match
- No assignment until work starts

**Actions:**

1. Self-assign issue
2. Verify all Definition of Ready criteria met
3. Review design plan thoroughly
4. Create branch following naming convention:
   - `feature/{issue#}-description` - New features
   - `fix/{issue#}-description` - Bug fixes
   - `docs/{issue#}-description` - Documentation
   - `refactor/{issue#}-description` - Refactoring
5. **Base branch:**
   - Standard: Branch from `main`
   - Sub-issue Strategy A: Branch from `main`
   - Sub-issue Strategy B: Branch from `feature/{parent-issue#}-{parent-name}`
6. Post starting comment with branch name and estimated tasks
7. Move to In Progress

**Branch Maintenance (Strategy B):**

- **Owner responsibility:** Keep feature branch rebased with base branch (main)
- **Sub-issue owners:** Keep branch rebased with feature branch
- Rebase schedule: Before starting work, daily if active, before PR
- Resolve conflicts and re-run tests after rebase

### In Progress → In Review

**Prerequisites:**

- All implementation tasks completed
- **All tests complete:**
  - Unit tests (all passing)
  - Integration tests (if applicable)
  - System tests / BDD scenarios (if behavior change)
  - E2E tests (if user-facing change)
  - Manual testing with evidence
- Test evidence posted in task completion comments
- Self-review completed
- No analyzer warnings
- Documentation complete

**Actions:**

1. Create PR with conventional commit title
2. Use PR template, link issue with `Closes #123`
3. Reference task completion comments for reviewer context
4. CI must be passing (or failures being actively fixed)
5. Move to In Review

**Task-Based Development:**

- Break work into small tasks (1-4 hours each)
- Post completion comment after each task:

  ```markdown
  ✅ Task completed: [Brief description]

  **Summary:**

  - Added X class
  - Added Y tests

  **Key artifacts:**

  - path/to/file.cs - Description

  **Test results:**

  - All tests passing ✓
  - Coverage: X% line, Y% branch
  ```

**WIP Expectations:**

- No enforced limit, expected ~1 per developer
- If picking up additional work (e.g., blocking bug), post comment explaining

### In Review Process

**Tech Lead Assigns Reviewers:**

- Reviews PR scope and assigns appropriate persona reviewers
- DotNet Developer / Senior Developer for code
- QA Engineer for tests
- Security Reviewer for security-sensitive changes
- Documentation Specialist for documentation
- Posts assignment comment referencing persona checklists

**Review Format:**

- **Prefer inline comments:** File/line-based comments with conversation threads
- **Use comment prefixes:** `blocker:`, `issue:`, `suggestion:`, `question:`, `nit:`, `praise:`
- **Conversation resolution:** Reviewer resolves conversations when satisfied (not author)
- **All conversations must be resolved** before merge (enforced by branch protection)

**Reviewer Scope:**

- Code: Standards, correctness, patterns, performance
- Tests: Coverage, quality, meaningful assertions
- Documentation: XML docs, README, design plan updates, ADR updates

### In Review → In Progress (Changes Requested)

**Trigger:** Reviewer requests changes (blockers, issues, questions)

**Critical:** PR remains open throughout

**Process:**

1. Reviewer uses "Request Changes" status
2. Author moves issue back to In Progress (PR stays open)
3. Author addresses feedback:
   - Replies in conversation threads
   - Makes code changes
   - Re-runs all tests (unit, integration, system, E2E)
   - Posts new test evidence
   - Updates PR checklist
4. Author does NOT resolve conversations (reviewer's responsibility)
5. Re-request review when complete
6. Move back to In Review

**Review Cycles:**

- No limit on iterations
- Must re-review after every code change
- New test evidence required each round
- PR checklist kept updated

**Scope Changes During Review:**

- If approved in-scope: Update design plan version history, implement, link back
- If out of scope: Create follow-up issue, link in design plan, continue original scope
- May trigger additional reviewer personas

**Follow-up Issues:**

- Create for deferred enhancements, optimizations
- Link in design plan Follow-up Issues section
- Update PR description with follow-up links
- Track in design plan table

**CI Failures:**

- Must be fixed within PR scope (no deferral)
- Owner diagnoses, fixes, posts evidence
- Re-run CI and verify passing

### In Review → Done

**Approval Criteria:**

- All reviewers approved
- All conversations resolved
- All CI checks passing
- PR checklist complete
- Test evidence posted

**Merge:**

- Squash merge to main (or feature branch for Strategy B)
- Issue auto-closed via `Closes #123`
- Branch auto-deleted
- Move to Done

**Epic Sub-Issue Completion:**

- Post completion comment on parent epic
- Update parent epic checklist
- Feature behind flag (Strategy A) or in feature branch (Strategy B)

### Any → Blocked

**When to Block:**

- External dependency unavailable
- Blocking bug in another component
- Awaiting stakeholder decision
- Technical/infrastructure blocker

**Process:**

1. Document the block in comment:
   - What's blocking
   - Link to blocking issue
   - Impact
   - Actions taken
   - Workaround plan (if any)
2. Add `blocked` label
3. Move to Blocked state (or stay in current state if making progress)

**Owner Responsibility:**

- Actively chase blocking issue
- Check if blocker is in your skill set → consider fixing
- Coordinate with blocker owner
- Implement workarounds when possible
- Keep stakeholders updated

**Self-Unblock Options:**

- Fix the blocker yourself (if in skill set)
- Implement temporary workaround
- Work on non-blocked tasks
- Help review blocking PR to expedite

**Unblocking:**

- When blocker resolved, post unblock comment
- Remove `blocked` label
- Return to previous state (In Progress or In Review)

## Epic Lifecycle

**Epic Closure Process:**

**Pre-Closure (Epic Owner Responsibility):**

1. Verify all sub-issues closed and merged
2. Enable feature flags (if Strategy A) per enablement plan in design
3. Monitor for stability (24 hours minimum)
4. Archive design plan:
   - Update status to `Implemented`
   - Move to `docs/plans/archive/`
   - Commit with reference to epic
5. Update ADRs from `Proposed` to `Accepted` with implementation date
6. Create feature flag removal tickets (target: 2 releases after enablement)
7. Update design plan Follow-up Issues section with all follow-ups
8. Post epic closure summary
9. Close epic issue

**Strategy B Final Merge:**

- Create final epic PR merging feature branch → main
- High-level review (sub-issues already reviewed)
- Merge and follow closure process above (no feature flags)

## Labels

| Label                  | Purpose                              |
| ---------------------- | ------------------------------------ |
| `status:triage`        | In triage/refinement                 |
| `bug`                  | Something isn't working              |
| `enhancement`          | New feature or improvement           |
| `documentation`        | Documentation only changes           |
| `epic`                 | Parent issue coordinating sub-issues |
| `blocked`              | Cannot proceed                       |
| `priority:critical`    | Immediate action required            |
| `priority:high`        | Important, next release              |
| `priority:medium`      | Normal priority                      |
| `priority:low`         | Nice to have                         |
| `skill:dotnet`         | Requires .NET/C# expertise           |
| `skill:security`       | Requires security expertise          |
| `skill:infrastructure` | Requires DevOps/infrastructure work  |
| `skill:documentation`  | Primarily documentation work         |
| `skill:testing`        | Primarily test development           |

## Time in State

Target durations (not enforced):

| State       | Target     | Action if exceeded                    |
| ----------- | ---------- | ------------------------------------- |
| Triage      | < 5 days   | Check originator response, may reject |
| Ready       | < 1 sprint | Re-prioritise or refine               |
| In Progress | < 3 days   | Check for blockers, offer help        |
| In Review   | < 1 day    | Ping reviewers                        |

## Main Branch Health

**Critical Rule:** `main` must always be clean

- All CI checks passing
- Build succeeds on all platforms
- All tests passing
- No analyzer warnings

**If Main Breaks:**

1. 🚨 Create CRITICAL priority issue immediately
2. Stop all new tickets from moving to In Progress
3. Halt all rebase operations from main
4. Owner of breaking merge fixes immediately or reverts
5. Verify CI passes on main
6. Resume normal development

Development freeze until main is restored.

## GitHub Project Integration

Issues tracked on GitHub Project board:

1. New issues → Backlog
2. Safety validated → Triage (with `status:triage` label)
3. Design approved → Ready
4. Self-assigned → In Progress
5. PR created → In Review
6. Changes requested → In Progress (PR open)
7. Approved and merged → Done

Board columns match states for visibility.
