---
title: Ticket Lifecycle
summary: Workflow states and transitions for issues from creation to completion
audience: [developer, agent]
topics: [process, workflow, tickets, github]
prerequisites: []
related: [definition-of-ready.md, definition-of-done.md, ../adr/0004-contribution-workflow.md]
last_validated: 2026-01-10
---

# Ticket Lifecycle

How issues move through the development workflow.

## States

```text
┌─────────┐     ┌─────────┐     ┌─────────────┐     ┌───────────┐     ┌──────┐
│ Backlog │ ──► │  Ready  │ ──► │ In Progress │ ──► │ In Review │ ──► │ Done │
└─────────┘     └─────────┘     └─────────────┘     └───────────┘     └──────┘
                     │                 │                   │
                     ▼                 ▼                   ▼
                 ┌───────┐        ┌─────────┐        ┌─────────┐
                 │Blocked│        │ Blocked │        │ Changes │
                 └───────┘        └─────────┘        │Requested│
                                                     └─────────┘
```

## State Definitions

| State             | Description               | Entry Criteria                     |
| ----------------- | ------------------------- | ---------------------------------- |
| Backlog           | Awaiting prioritisation   | Issue created                      |
| Ready             | Ready for development     | Meets Definition of Ready          |
| In Progress       | Active development        | Developer assigned, branch created |
| In Review         | PR open, awaiting review  | PR created, CI passing             |
| Changes Requested | Reviewer feedback pending | Review comments to address         |
| Blocked           | Cannot proceed            | External dependency or blocker     |
| Done              | Complete                  | Meets Definition of Done           |

## Transitions

### Backlog → Ready

- Product owner or tech lead prioritises
- All Definition of Ready criteria met
- Assigned to milestone (optional)

### Ready → In Progress

- Developer self-assigns or is assigned
- Branch created: `feature/{issue#}-description`
- Issue linked to branch

### In Progress → In Review

- PR created with conventional commit title
- CI pipeline passes
- Self-review completed
- Reviewer requested

### In Review → Done

- PR approved
- All CI checks pass
- PR merged (squash)
- Issue auto-closed via `Fixes #N`

### In Review → Changes Requested

- Reviewer requests changes
- Developer addresses feedback
- Re-request review when ready

### Any → Blocked

- Document blocker in comment
- Link to blocking issue if applicable
- Add `blocked` label

## Labels

| Label              | Purpose                    |
| ------------------ | -------------------------- |
| `bug`              | Something isn't working    |
| `enhancement`      | New feature or improvement |
| `documentation`    | Documentation only changes |
| `blocked`          | Cannot proceed             |
| `help wanted`      | Extra attention needed     |
| `good first issue` | Good for newcomers         |

## Time in State

Target durations (not enforced):

| State             | Target     | Action if exceeded             |
| ----------------- | ---------- | ------------------------------ |
| Ready             | < 1 sprint | Re-prioritise or refine        |
| In Progress       | < 3 days   | Check for blockers, offer help |
| In Review         | < 1 day    | Ping reviewers                 |
| Changes Requested | < 1 day    | Follow up with author          |

## GitHub Project Integration

Issues are tracked on the GitHub Project board:

1. New issues land in Backlog
2. Drag to Ready when refined
3. In Progress/Review auto-update via PR status
4. Done when PR merged
