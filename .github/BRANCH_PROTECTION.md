# Branch Protection Rules

This document describes the branch protection rules that should be applied to the `main` branch.
These rules must be configured manually in GitHub repository settings.

## Configuration Steps

1. Go to **Settings** > **Branches** > **Add branch protection rule**
2. Set **Branch name pattern** to `main`
3. Configure the following settings:

## Required Settings

| Rule                                          | Setting  | Notes                          |
| --------------------------------------------- | -------- | ------------------------------ |
| **Require a pull request before merging**     | Enabled  |                                |
| └─ Required approving reviews                 | 1        | At least one approval required |
| └─ Dismiss stale pull request approvals       | Enabled  | New commits require re-review  |
| └─ Require review from Code Owners            | Enabled  | CODEOWNERS must approve        |
| **Require status checks to pass**             | Enabled  |                                |
| └─ Require branches to be up to date          | Enabled  | Must merge/rebase before merge |
| └─ Status checks (add when CI is configured): |          |                                |
| &nbsp;&nbsp;&nbsp;&nbsp;• `lint`              | Required | Format, spelling, markdown     |
| &nbsp;&nbsp;&nbsp;&nbsp;• `build`             | Required | Multi-platform build           |
| &nbsp;&nbsp;&nbsp;&nbsp;• `test-unit`         | Required | Unit tests with coverage       |
| &nbsp;&nbsp;&nbsp;&nbsp;• `test-system`       | Required | BDD system tests               |
| &nbsp;&nbsp;&nbsp;&nbsp;• `test-arch`         | Required | Architecture tests             |
| &nbsp;&nbsp;&nbsp;&nbsp;• `codeql`            | Required | Security analysis              |
| **Require conversation resolution**           | Enabled  | All review comments resolved   |
| **Require signed commits**                    | Enabled  | GPG or SSH signed commits      |
| **Require linear history**                    | Enabled  | Squash merge only              |
| **Do not allow bypassing**                    | Enabled  | Applies to admins too          |

## Merge Settings

Configure in **Settings** > **General** > **Pull Requests**:

| Setting                            | Value                              |
| ---------------------------------- | ---------------------------------- |
| Allow merge commits                | Disabled                           |
| Allow squash merging               | Enabled (default)                  |
| Allow rebase merging               | Disabled                           |
| Default commit message             | Pull request title and description |
| Automatically delete head branches | Enabled                            |

## Notes

- Status checks will be added as CI workflows are implemented in Phase 5
- Until CI is configured, only enable the checks that exist
- The `Require signed commits` setting requires contributors to configure GPG or SSH signing

## References

- [ADR-0004: Contribution Workflow](../docs/adr/0004-contribution-workflow.md)
- [GitHub Branch Protection Documentation](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches)
