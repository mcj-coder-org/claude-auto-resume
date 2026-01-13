# Playbook: Persona Switching for PR Workflows

## Overview

This playbook documents how to switch between GitHub personas to enforce separation of
duties in PR workflows. This ensures compliance with the InnerSource role model where
Contributors create PRs and Owners/Maintainers review them.

## Why Persona Switching?

GitHub prevents self-approval of PRs by design. When branch protection requires approvals,
the PR author cannot approve their own PR. This creates a deadlock if the same account is
used for both authoring and reviewing.

**Solution:** Use separate GitHub accounts for different roles:

| Role            | Account Purpose        | Actions                    |
| --------------- | ---------------------- | -------------------------- |
| **Contributor** | Create PRs, push code  | `git push`, `gh pr create` |
| **Owner**       | Review, approve, merge | `gh pr review --approve`   |

## Prerequisites

- Two GitHub accounts with repository access
- Both accounts authenticated via `gh auth login`
- GPG keys configured for each account (for signed commits)

## Step 1: Authenticate Multiple Accounts

```bash
# Login with first account (e.g., contributor)
gh auth login

# Login with second account (e.g., owner/reviewer)
gh auth login
```

Verify both accounts are configured:

```bash
gh auth status
```

Expected output:

```text
github.com
  ✓ Logged in to github.com account contributor-account (keyring)
  - Active account: true
  ...

  ✓ Logged in to github.com account owner-account (keyring)
  - Active account: false
  ...
```

## Step 2: Configure Git Identities

Each persona needs a distinct git identity for proper attribution.

### Option A: Repository-Level Config (Recommended)

Create identity configs for each persona:

```bash
# Create contributor identity
git config --global alias.as-contributor '!git config user.name "Contributor Name" && git config user.email "contributor@example.com" && gh auth switch --user contributor-account'

# Create owner identity
git config --global alias.as-owner '!git config user.name "Owner Name" && git config user.email "owner@example.com" && gh auth switch --user owner-account'
```

Usage:

```bash
git as-contributor  # Switch to contributor persona
git as-owner        # Switch to owner persona
```

### Option B: Manual Switching

```bash
# Switch to contributor
git config user.name "Contributor Name"
git config user.email "contributor@example.com"
gh auth switch --user contributor-account

# Switch to owner
git config user.name "Owner Name"
git config user.email "owner@example.com"
gh auth switch --user owner-account
```

## Step 3: GPG Keys per Persona

Each persona should have its own GPG key for signed commits.

```bash
# List keys for current persona
gpg --list-secret-keys --keyid-format=long

# Configure signing key for persona
git config user.signingkey YOUR_PERSONA_KEY_ID
```

See [enable-signed-commits.md](enable-signed-commits.md) for full GPG setup.

## Workflow Example

### Creating a PR (as Contributor)

```bash
# 1. Switch to contributor persona
gh auth switch --user contributor-account
git config user.name "Contributor Name"
git config user.email "contributor@example.com"

# 2. Create branch and make changes
git checkout -b feature/123-new-feature
# ... make changes ...
git add .
git commit -m "feat: add new feature

Refs: #123"

# 3. Push and create PR
git push -u origin feature/123-new-feature
gh pr create --title "feat: add new feature" --body "..."
```

### Reviewing a PR (as Owner)

```bash
# 1. Switch to owner persona
gh auth switch --user owner-account

# 2. Review the PR
gh pr view 123
gh pr diff 123

# 3. Approve (following role-specific format)
gh pr review 123 --approve --body "## Tech Lead Review
...
"
```

### After Merge

```bash
# Switch back to contributor for next task
gh auth switch --user contributor-account
git checkout main
git pull
```

## Verification

Check current persona:

```bash
# GitHub CLI account
gh auth status | grep "Active account: true" -A1

# Git identity
git config user.name
git config user.email
```

## CI Enforcement

A GitHub Actions workflow can warn when PR authors are code owners.
See `.github/workflows/pr-author-check.yml` for implementation.

## Troubleshooting

### "Cannot approve your own pull request"

You're using the same account that created the PR. Switch personas:

```bash
gh auth switch --user reviewer-account
gh pr review 123 --approve
```

### Commits showing wrong author

Git identity wasn't switched before committing:

```bash
# Check current identity
git config user.name
git config user.email

# Fix last commit author
git commit --amend --author="Correct Name <correct@email.com>"
```

### GPG signature shows wrong key

Signing key doesn't match current persona:

```bash
# Check current signing key
git config user.signingkey

# Update to correct key
git config user.signingkey CORRECT_KEY_ID

# Re-sign last commit
git commit --amend --no-edit -S
```

## Best Practices

1. **Always verify persona** before creating commits or PRs
2. **Use aliases** to reduce switching errors
3. **Configure GPG** for each persona to ensure verified commits
4. **Document account mappings** in team onboarding materials

## Role Mapping

| InnerSource Role | GitHub Actions          | Persona Type |
| ---------------- | ----------------------- | ------------ |
| Contributor      | Create PR, push commits | Author       |
| Maintainer       | Review, request changes | Reviewer     |
| Owner            | Approve, merge, admin   | Reviewer     |

## See Also

- [docs/standards/roles.md](../standards/roles.md) - Role definitions
- [enable-signed-commits.md](enable-signed-commits.md) - GPG setup
- [github-setup.md](github-setup.md) - Repository configuration
