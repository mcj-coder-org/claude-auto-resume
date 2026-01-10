---
name: work-item-management
description: |
  When creating issues, tracking features, or organizing work breakdown.
  Applies when structuring epics, sub-issues, or linking work to branches and PRs.
decision: Use GitHub Issues with sub-issues and task lists for hierarchical work tracking.
status: accepted
---

# ADR-0003: Work Item Management

## Status

Proposed

## Date

2026-01-09

## Context

We need a work item management system to:

1. Track features, bugs, and technical debt
2. Support hierarchical work breakdown (epic → story → task)
3. Link work items to branches, PRs, and commits
4. Enable progressive context loading for AI agents
5. Integrate with our chosen development platform (GitHub)

### Requirements

- Hierarchical structure: Parent issues with sub-issues and task lists
- Traceability: Work items linked to branches, PRs, commits, and ADRs
- Agent-friendly: Immutable links to context documents for progressive loading
- Low ceremony: Minimal overhead for small tasks
- Visibility: Clear status and progress tracking

### Options Considered

#### Option 1: GitHub Issues with Sub-Issues and Task Lists (Selected)

Native GitHub issue tracking with hierarchical support.

**Pros:**

- Native integration with GitHub (ADR-0009)
- Sub-issues for hierarchical breakdown
- Task lists with progress tracking
- Automatic linking via branch naming (`#123`)
- No additional tooling or cost
- API access for automation
- Markdown support for rich descriptions

**Cons:**

- Less sophisticated than dedicated project management tools
- Sub-issues are relatively new feature
- Limited custom fields without GitHub Projects

#### Option 2: GitHub Projects (Classic or New)

GitHub's project management layer on top of Issues.

**Pros:**

- Board/table views
- Custom fields and workflows
- Roadmap views
- Automation rules

**Cons:**

- Additional complexity layer
- Overkill for single-project repos
- Learning curve for contributors

#### Option 3: Linear

Modern issue tracking designed for software teams.

**Pros:**

- Excellent UX
- Powerful automation
- Great keyboard shortcuts
- Cycles and roadmaps

**Cons:**

- External tool (context switching)
- Paid for teams
- Requires GitHub integration setup
- Duplicate source of truth

#### Option 4: Jira

Enterprise project management platform.

**Pros:**

- Industry standard for enterprises
- Highly configurable
- Rich reporting
- Extensive integrations

**Cons:**

- Heavy and complex
- Expensive
- Overkill for open source
- Poor developer experience
- External to GitHub

## Decision

We will use **GitHub Issues with Sub-Issues and Task Lists** for work item management.

### Work Item Hierarchy

```text
Epic (Parent Issue)
├── Feature Branch: feature/{issue#}-{description}
├── Design Document: docs/plans/{date}-{topic}-design.md
├── ADRs: docs/adr/{number}-{topic}.md (Status: Proposed)
│
├── Sub-Issue 1: Implement {feature-part-1}
│   ├── Task List: [ ] Step 1, [ ] Step 2, ...
│   └── Branch: feature/{sub-issue#}-{description}
│
├── Sub-Issue 2: Implement {feature-part-2}
│   ├── Task List: [ ] Step 1, [ ] Step 2, ...
│   └── Branch: feature/{sub-issue#}-{description}
│
└── Sub-Issue N: Verify and close
    └── Task List: [ ] Update ADRs to Accepted, [ ] Merge to main
```

### Issue Templates

**Epic Template:**

```markdown
## Summary

{Brief description of the epic}

## Design Documents

- [ ] [Design Document](link-to-design-doc)
- [ ] ADRs: {list of related ADRs}

## Sub-Issues

- [ ] #{sub-issue-1}
- [ ] #{sub-issue-2}

## Acceptance Criteria

- [ ] All sub-issues completed
- [ ] All ADRs marked as Accepted
- [ ] Documentation updated
- [ ] Tests passing
```

**Sub-Issue Template:**

```markdown
## Parent Issue

Refs: #{parent-issue}

## Context

- [Design Document](immutable-link-to-design-doc)
- [Implementation Plan](immutable-link-to-plan)

## Tasks

- [ ] Task 1
- [ ] Task 2

## Acceptance Criteria

- [ ] Feature implemented per design
- [ ] Tests added and passing
- [ ] No new analyzer warnings
```

### Branch Naming Convention

As per [ADR-0004: Contribution Workflow](./0004-contribution-workflow.md):

- `feature/{issue#}-{description}` - New features
- `fix/{issue#}-{description}` - Bug fixes
- `docs/{issue#}-{description}` - Documentation only
- `refactor/{issue#}-{description}` - Code refactoring

### Linking Strategy

| Artifact           | Link Format          | Example                       |
| ------------------ | -------------------- | ----------------------------- |
| Issue → Branch     | Automatic via naming | `feature/123-add-auth`        |
| Commit → Issue     | Footer reference     | `Refs: #123`                  |
| PR → Issue         | Body reference       | `Closes #123` or `Refs: #123` |
| Sub-issue → Parent | Body reference       | `Refs: #100`                  |
| Issue → Design Doc | Markdown link        | `[Design](../plans/...)`      |
| Issue → ADR        | Markdown link        | `[ADR-0010](../adr/0010-...)` |

### Immutable Links for Agent Context

Sub-issues reference design documents using immutable links (commit SHA or tag):

```markdown
## Context

- [Design Document](https://github.com/org/repo/blob/{sha}/docs/plans/design.md)
- [Implementation Plan](https://github.com/org/repo/blob/{sha}/docs/plans/plan.md)
```

This ensures agents can progressively load context without risk of document drift.

### Workflow

1. **Planning Phase**
   - Create parent issue (epic)
   - Create feature branch from issue
   - Write design document on feature branch
   - Write ADRs (Status: Proposed) on feature branch
   - Commit and push planning artifacts

2. **Breakdown Phase**
   - Create sub-issues with task lists
   - Link sub-issues to parent
   - Add immutable links to design docs
   - Assign sub-issues if team project

3. **Implementation Phase**
   - Work on sub-issues independently
   - Each sub-issue may have its own branch (or share parent branch)
   - PRs reference sub-issues
   - Task lists track progress

4. **Verification Phase**
   - All sub-issues completed
   - Update ADRs to Status: Accepted
   - Final PR to merge feature branch
   - Close parent issue

## Consequences

### Positive

- Single platform for code and work tracking
- Native GitHub integration (branches, PRs, commits)
- Hierarchical structure supports complex features
- Task lists provide granular progress tracking
- Immutable links enable agent context loading
- No additional tooling cost
- Familiar to most developers

### Negative

- Less sophisticated than dedicated PM tools
- Limited reporting/analytics
- Sub-issues feature still maturing
- No built-in time tracking

### Risks

- Sub-issues feature may change
- Complex epics may need external planning tools
- GitHub Issues UI can become cluttered

### Mitigations

- Keep issue descriptions concise with links to detailed docs
- Archive completed epics
- Use labels for categorization and filtering
- Consider GitHub Projects if complexity grows

## References

- [GitHub Issues Documentation](https://docs.github.com/en/issues)
- [GitHub Sub-Issues](https://docs.github.com/en/issues/tracking-your-work-with-issues/adding-sub-issues)
- [Task Lists in GitHub](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/about-task-lists)
- [Linking PRs to Issues](https://docs.github.com/en/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue)
