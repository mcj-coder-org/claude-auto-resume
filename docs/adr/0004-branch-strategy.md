# ADR-0004: Branch Strategy

## Status

Proposed

## Date

2026-01-09

## Context

We need a branch strategy for:
1. Feature development
2. Release management
3. Collaboration

### Requirements

- Simple workflow
- No long-lived branches
- Issue traceability
- Protected main branch

## Decision

**GitHub Flow** with issue-linked branch naming.

### Branch Types

| Type | Pattern | Example |
|------|---------|---------|
| Feature | `feature/{issue#}-{description}` | `feature/123-add-rate-limit` |
| Fix | `fix/{issue#}-{description}` | `fix/456-null-check` |
| Docs | `docs/{issue#}-{description}` | `docs/789-update-readme` |
| Refactor | `refactor/{issue#}-{description}` | `refactor/101-extract-class` |

### Workflow

1. Create issue for work item
2. Create branch from main: `feature/{issue#}-{description}`
3. Develop and commit (conventional commits)
4. Create PR targeting main
5. Squash merge after approval
6. Delete branch

### Branch Protection (main)

- Require PR reviews (1 reviewer)
- Require status checks (lint, build, test, CodeQL)
- Require signed commits
- Require conversation resolution
- Require linear history (squash merge)
- No direct pushes
- Auto-delete head branches

### Pre-push Hook

Validates branch naming:
```bash
branch=$(git rev-parse --abbrev-ref HEAD)
if [[ "$branch" == "main" ]]; then
  echo "Cannot push directly to main"
  exit 1
fi

if ! [[ "$branch" =~ ^(feature|fix|docs|refactor)/[0-9]+-[a-z0-9-]+$ ]]; then
  echo "Branch name must follow pattern: type/{issue#}-{description}"
  exit 1
fi
```

## Consequences

### Positive
- Simple, proven workflow
- Issue traceability
- Clean git history (squash)
- Protected main branch

### Negative
- Branch naming enforcement overhead
- Squash loses granular history

## References

- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)
