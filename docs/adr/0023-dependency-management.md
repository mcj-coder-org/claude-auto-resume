---
name: dependency-management
description: |
  When configuring automated dependency updates, security patching, or NuGet/npm package management. Apply when setting up Dependabot or auto-merge strategies.
decision: Use Dependabot with grouped updates and auto-merge for patch versions via machine user PAT.
status: accepted
type: implementation
implementation_issue: '#24'
---

# ADR-0023: Dependency Management

## Status

Proposed

## Date

2026-01-09

## Context

We need dependency management for:

1. Keeping dependencies up to date
2. Security vulnerability patching
3. Reducing manual maintenance

### Requirements

- Automated update PRs
- Security-focused updates
- Minimal noise
- Auto-merge for safe updates

## Decision

**Dependabot** with auto-merge for patches.

### Configuration

**.github/dependabot.yml:**

```yaml
version: 2
updates:
  - package-ecosystem: 'nuget'
    directory: '/'
    schedule:
      interval: 'weekly'
    groups:
      development-dependencies:
        patterns:
          - '*Analyzer*'
          - 'coverlet*'
          - 'xunit*'
    open-pull-requests-limit: 10

  - package-ecosystem: 'npm'
    directory: '/'
    schedule:
      interval: 'weekly'
    groups:
      dev-dependencies:
        dependency-type: 'development'
```

### Auto-Merge Strategy

Using org-scoped `MACHINE_USER_PAT`:

- **Patch updates:** Auto-approve and merge if tests pass
- **Minor updates:** Auto-approve, manual merge
- **Major updates:** Manual review required

### Workflow

```yaml
name: Dependabot Auto-Merge
on: pull_request

jobs:
  auto-merge:
    if: github.actor == 'dependabot[bot]'
    runs-on: ubuntu-latest
    steps:
      - name: Auto-approve patches
        if: contains(github.event.pull_request.title, 'patch')
        run: gh pr review --approve "$PR_URL"
        env:
          PR_URL: ${{ github.event.pull_request.html_url }}
          GH_TOKEN: ${{ secrets.MACHINE_USER_PAT }}
```

## Consequences

### Positive

- Automated security updates
- Reduced maintenance burden
- Grouped updates reduce noise

### Negative

- Auto-merge risk (mitigated by tests)
- PR noise for major updates
- Machine user PAT required

## References

- [Dependabot](https://docs.github.com/en/code-security/dependabot)
- [Dependabot Auto-Merge](https://docs.github.com/en/code-security/dependabot/working-with-dependabot/automating-dependabot-with-github-actions)
