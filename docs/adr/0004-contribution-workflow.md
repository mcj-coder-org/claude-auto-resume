---
name: contribution-workflow
description: |
  When contributing code, creating branches, or submitting PRs.
  Applies to all ticket-to-merge workflows including commit format and PR requirements.
decision: Follow GitHub Flow with conventional commits, branch naming conventions, and squash merges.
status: accepted
---

# ADR-0004: Contribution Workflow

## Status

Proposed

## Date

2026-01-09

## Context

We need a documented contribution workflow that:

1. Defines the complete ticket-to-merge process
2. Establishes PR requirements and review process
3. Integrates work items, branches, commits, and CI
4. Provides clear Definition of Done criteria
5. Works for both human and AI contributors

### Requirements

- End-to-end workflow documentation
- Clear PR checklist and review criteria
- CI/CD integration points defined
- Definition of Done for each stage
- Branch strategy and naming conventions

## Decision

**Documented Contribution Workflow** integrating GitHub Flow with our work item management.

### Workflow Overview

```text
┌───────────────────────────────────────────────────────────────────────┐
│                       CONTRIBUTION WORKFLOW                           │
├───────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  1. TICKET          2. BRANCH         3. DEVELOP       4. PR          │
│  ┌─────────┐       ┌─────────┐       ┌─────────┐      ┌─────────┐     │
│  │ Create  │──────▶│ Create  │──────▶│ Commit  │─────▶│ Open    │     │
│  │ Issue   │       │ Branch  │       │ Changes │      │ PR      │     │
│  └─────────┘       └─────────┘       └─────────┘      └─────────┘     │
│       │                 │                 │                │           │
│       ▼                 ▼                 ▼                ▼           │
│  - Describe work   - From main       - Pre-commit     - CI runs       │
│  - Add labels      - Name: type/     - Conventional   - Review        │
│  - Link to epic      {issue#}-desc     commits        - Approval      │
│                                      - Refs: #{issue}                  │
│                                                                        │
│  5. MERGE           6. CLEANUP                                         │
│  ┌─────────┐       ┌─────────┐                                        │
│  │ Squash  │──────▶│ Delete  │                                        │
│  │ Merge   │       │ Branch  │                                        │
│  └─────────┘       └─────────┘                                        │
│       │                 │                                              │
│       ▼                 ▼                                              │
│  - Close issue     - Auto-deleted                                      │
│  - Update ADRs     - Issue closed                                      │
│                                                                        │
└───────────────────────────────────────────────────────────────────────┘
```

### Stage 1: Create Ticket

**Before any work begins, create or identify the work item.**

| Action            | Details                                            |
| ----------------- | -------------------------------------------------- |
| Create Issue      | Use appropriate template (bug, feature, sub-issue) |
| Link to Parent    | If sub-issue, add `Refs: #{parent}`                |
| Add Labels        | `type:*`, `priority:*`, `area:*`                   |
| Add Context Links | Immutable links to design docs (commit SHA)        |

**Definition of Done - Ticket:**

- [ ] Issue has clear description
- [ ] Acceptance criteria defined
- [ ] Labels applied
- [ ] Linked to parent issue (if applicable)
- [ ] Design context linked (for non-trivial work)

### Stage 2: Create Branch

**Create a branch from main following naming conventions.**

| Type     | Pattern                           | Example                      |
| -------- | --------------------------------- | ---------------------------- |
| Feature  | `feature/{issue#}-{description}`  | `feature/123-add-rate-limit` |
| Fix      | `fix/{issue#}-{description}`      | `fix/456-null-check`         |
| Docs     | `docs/{issue#}-{description}`     | `docs/789-update-readme`     |
| Refactor | `refactor/{issue#}-{description}` | `refactor/101-extract-class` |

```bash
git checkout main
git pull origin main
git checkout -b feature/123-add-rate-limit
```

**Definition of Done - Branch:**

- [ ] Branch created from latest main
- [ ] Branch name follows `{type}/{issue#}-{description}` pattern
- [ ] Issue number in branch name matches work item

### Stage 3: Develop

**Make changes following coding standards and commit conventions.**

#### Commit Message Format

```text
type(scope): description

Body explaining what and why (not how).

Refs: #123

Co-Authored-By: Name <email> (if applicable)
```

**Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`

#### Pre-commit Checks (Automated)

| Check           | Tool                    | Failure Action         |
| --------------- | ----------------------- | ---------------------- |
| Commit message  | commitlint              | Commit rejected        |
| Code formatting | dotnet format, prettier | Auto-fixed or rejected |
| Spelling        | cspell                  | Commit rejected        |
| Secrets         | secretlint              | Commit rejected        |
| Markdown        | markdownlint            | Commit rejected        |
| Main branch     | custom hook             | Commit rejected        |

**Definition of Done - Development:**

- [ ] Code follows coding standards
- [ ] All pre-commit hooks pass
- [ ] Unit tests written for new code
- [ ] No analyzer warnings introduced
- [ ] Commits reference issue (`Refs: #123`)

### Stage 4: Pull Request

**Open PR and request review.**

#### PR Title Format

```text
type(scope): description (#123)
```

Must match conventional commit format. Validated by CI.

#### PR Body Template

```markdown
## Summary

Brief description of changes.

## Changes

- Change 1
- Change 2

## Testing

- [ ] Unit tests added/updated
- [ ] Manual testing completed
- [ ] System tests updated (if behaviour change)

## Checklist

- [ ] Code follows project coding standards
- [ ] Self-review completed
- [ ] No new warnings
- [ ] Documentation updated (if applicable)
- [ ] Linked to issue: Closes #123

## Screenshots (if UI changes)

N/A
```

#### CI Checks (Must Pass)

| Check       | Description                                    | Required |
| ----------- | ---------------------------------------------- | -------- |
| lint        | Format, spelling, markdown, secrets            | Yes      |
| build       | All platforms (continue-on-error per platform) | Yes      |
| test-unit   | Unit tests with coverage                       | Yes      |
| test-system | BDD system tests                               | Yes      |
| test-arch   | Architecture tests                             | Yes      |
| codeql      | Security analysis                              | Yes      |
| pr-title    | Conventional commit format                     | Yes      |

#### Review Requirements

| Requirement   | Details                  |
| ------------- | ------------------------ |
| Reviewers     | Minimum 1 approval       |
| Conversations | All resolved             |
| CI Status     | All required checks pass |
| Commits       | Signed                   |

#### Role-Based Task Assignment

Tasks and reviews are assigned based on InnerSource roles. See `docs/standards/roles.md`
for full role definitions.

| InnerSource Role | Task Types                       | PR Actions                      |
| ---------------- | -------------------------------- | ------------------------------- |
| **Owner**        | Repo config, process, direction  | Final approval, merge authority |
| **Maintainer**   | Code review, architecture review | Approve, request changes        |
| **Contributor**  | Implementation, bug fixes, tests | Submit PRs, address feedback    |

**Reviewer Assignment by Change Type:**

| Change Type          | Primary Reviewer Role          | Secondary Reviewer           |
| -------------------- | ------------------------------ | ---------------------------- |
| New feature code     | Senior Developer (Maintainer)  | Domain specialist            |
| Bug fix              | Senior Developer (Maintainer)  | Original author if available |
| Security changes     | Security Reviewer (Maintainer) | Senior Developer             |
| Architecture changes | Tech Lead (Owner)              | Technical Architect          |
| Documentation        | Documentation Specialist       | Domain expert                |
| Test changes         | QA Engineer (Maintainer)       | Senior Developer             |

**Specialist Inheritance:**

Specialist roles (e.g., DotNet Developer) operate at multiple InnerSource levels:

- **As Contributor:** Implements using generic skills + specialist expertise
- **As Maintainer:** Reviews using senior role skills + specialist expertise

Example: DotNet Developer reviewing C# code applies Senior Developer review criteria
plus .NET-specific patterns and idioms.

**Definition of Done - PR:**

- [ ] PR title follows conventional commit format
- [ ] PR body uses template
- [ ] All CI checks pass
- [ ] At least 1 approval received
- [ ] All conversations resolved
- [ ] Branch is up to date with main

### Stage 5: Merge

**Squash merge to maintain linear history.**

#### Merge Commit Message

The squash merge commit message should be the PR title plus a summary:

```text
feat(monitor): add rate limit detection (#123)

Implements automatic detection of Claude rate limit responses
and configurable retry behaviour.

- Add RateLimitDetector class
- Add retry configuration options
- Add unit and system tests

Closes #123
```

#### Post-Merge Actions

| Action        | Automation                       |
| ------------- | -------------------------------- |
| Delete branch | Auto (branch protection setting) |
| Close issue   | Auto (via `Closes #123` in PR)   |
| Update ADRs   | Manual (if completing feature)   |

**Definition of Done - Merge:**

- [ ] Squash merged with descriptive message
- [ ] Branch deleted
- [ ] Issue closed
- [ ] ADRs updated to "Accepted" (if feature complete)

### Stage 6: Verification (Feature Completion)

**When completing a feature (all sub-issues done):**

| Action         | Details                                     |
| -------------- | ------------------------------------------- |
| Update ADRs    | Change Status from "Proposed" to "Accepted" |
| Update docs    | Ensure no placeholder content               |
| Run full suite | Unit, system, E2E, architecture tests       |
| Close epic     | Mark parent issue as complete               |

**Definition of Done - Feature:**

- [ ] All sub-issues completed and merged
- [ ] All related ADRs marked "Accepted"
- [ ] Documentation complete (no placeholders)
- [ ] All test suites pass
- [ ] Epic issue closed

---

## Branch Protection Rules

Applied to `main` branch:

| Rule                            | Setting                                                |
| ------------------------------- | ------------------------------------------------------ |
| Require PR                      | Yes                                                    |
| Required reviewers              | 1                                                      |
| Dismiss stale reviews           | Yes                                                    |
| Require status checks           | lint, build, test-unit, test-system, test-arch, codeql |
| Require branches up to date     | Yes                                                    |
| Require signed commits          | Yes                                                    |
| Require conversation resolution | Yes                                                    |
| Require linear history          | Yes (squash merge only)                                |
| Allow force pushes              | No                                                     |
| Allow deletions                 | No                                                     |
| Auto-delete head branches       | Yes                                                    |

---

## CI/CD Integration Points

```text
┌────────────────────────────────────────────────────────────┐
│                   CI/CD PIPELINE FLOW                      │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  PUSH TO BRANCH            PR OPENED              MERGE    │
│  ┌───────────┐            ┌───────────┐        ┌───────┐   │
│  │ ci.yml    │            │ ci.yml    │        │ ci.yml│   │
│  │ - lint    │            │ - lint    │        │ + docs│   │
│  │ - build   │            │ - build   │        │ + tag │   │
│  │ - test    │            │ - test    │        └───────┘   │
│  └───────────┘            │ - codeql  │                    │
│                           ├───────────┤                    │
│                           │ pr-title  │                    │
│                           │ - validate│                    │
│                           └───────────┘                    │
│                                                            │
│  NIGHTLY                   RELEASE TAG                     │
│  ┌───────────┐            ┌───────────┐                    │
│  │ nightly   │            │ release   │                    │
│  │ - e2e     │            │ - build   │                    │
│  │ - mutate  │            │ - sign    │                    │
│  │ - bench   │            │ - publish │                    │
│  └───────────┘            │ - sbom    │                    │
│                           └───────────┘                    │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

---

## Quick Reference

### Starting Work

```bash
# 1. Ensure you have an issue number
# 2. Create branch
git checkout main && git pull
git checkout -b feature/123-description

# 3. Make changes, commit
git add .
git commit -m "feat(scope): description

Refs: #123"

# 4. Push and create PR
git push -u origin feature/123-description
gh pr create --fill
```

### Commit Types

| Type     | Description      | Changelog Section |
| -------- | ---------------- | ----------------- |
| feat     | New feature      | Features          |
| fix      | Bug fix          | Bug Fixes         |
| docs     | Documentation    | Documentation     |
| style    | Formatting       | (hidden)          |
| refactor | Code restructure | (hidden)          |
| perf     | Performance      | Performance       |
| test     | Tests            | (hidden)          |
| build    | Build system     | (hidden)          |
| ci       | CI/CD            | (hidden)          |
| chore    | Maintenance      | (hidden)          |

### PR Checklist (Copy-Paste)

```markdown
- [ ] PR title follows `type(scope): description` format
- [ ] Linked to issue with `Closes #X` or `Refs: #X`
- [ ] Code follows project standards
- [ ] Tests added/updated
- [ ] Documentation updated (if applicable)
- [ ] Self-review completed
- [ ] No new warnings
```

## Consequences

### Positive

- Complete workflow documented in one place
- Clear expectations for contributors
- Consistent PR quality
- Automated enforcement via CI
- Works for human and AI contributors

### Negative

- Documentation maintenance overhead
- May feel heavyweight for tiny fixes
- Requires discipline to follow

### Mitigations

- Automate as much as possible via hooks and CI
- PR template provides copy-paste checklist
- Tiny fixes can use abbreviated commit scope

## References

- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [ADR-0003: Work Item Management](./0003-work-item-management.md)
- [ADR-0011: Pre-commit Hooks](./0011-pre-commit-hooks.md)
- [ADR-0021: CI/CD Pipeline](./0021-cicd-pipeline.md)
