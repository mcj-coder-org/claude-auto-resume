# GitHub Repository Setup

This playbook documents the manual GitHub configuration required for claude-auto-resume.

## Repository Secrets

Configure these secrets in **Settings > Secrets and variables > Actions**:

### Required Secrets

| Secret Name        | Purpose                                      | How to Obtain                                                                                        |
| ------------------ | -------------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| `NUGET_API_KEY`    | Publish packages to NuGet.org                | [NuGet API Keys](https://www.nuget.org/account/apikeys) - Create with push scope for `McjCoderOrg.*` |
| `MACHINE_USER_PAT` | Automated commits/PRs that trigger workflows | Create a GitHub PAT with `repo` and `workflow` scopes                                                |

### Creating NUGET_API_KEY

1. Go to [NuGet.org API Keys](https://www.nuget.org/account/apikeys)
2. Click "Create"
3. Configure:
   - **Key Name**: `claude-auto-resume-github-actions`
   - **Expiration**: 365 days (set reminder to rotate)
   - **Package owner**: Your NuGet account
   - **Glob pattern**: `McjCoderOrg.*`
   - **Scopes**: Push
4. Copy the generated key
5. Add to GitHub: Settings > Secrets > New repository secret > `NUGET_API_KEY`

### Creating MACHINE_USER_PAT

The machine user PAT allows automated workflows to trigger other workflows (normal `GITHUB_TOKEN` cannot do this).

1. Go to GitHub Settings > Developer settings > Personal access tokens > Fine-grained tokens
2. Click "Generate new token"
3. Configure:
   - **Token name**: `claude-auto-resume-machine-user`
   - **Expiration**: 90 days (set reminder to rotate)
   - **Repository access**: Only select repositories > `claude-auto-resume`
   - **Permissions**:
     - Contents: Read and write
     - Pull requests: Read and write
     - Workflows: Read and write
4. Copy the generated token
5. Add to GitHub: Settings > Secrets > New repository secret > `MACHINE_USER_PAT`

## Branch Protection Rules

Configure in **Settings > Branches > Add branch protection rule**:

### Main Branch (`main`)

**Branch name pattern**: `main`

**Protect matching branches:**

- [x] Require a pull request before merging
  - [x] Require approvals: 1 (or 0 for solo projects)
  - [x] Dismiss stale pull request approvals when new commits are pushed
  - [x] Require review from Code Owners (if CODEOWNERS file exists)
- [x] Require status checks to pass before merging
  - [x] Require branches to be up to date before merging
  - Status checks required:
    - `build (windows-latest)`
    - `build (ubuntu-latest)`
    - `build (macos-latest)`
    - `lint`
    - `test (windows-latest)`
    - `test (ubuntu-latest)`
    - `test (macos-latest)`
- [x] Require conversation resolution before merging
- [x] Require linear history
- [ ] Include administrators (uncheck to allow admins to bypass)
- [x] Restrict who can push to matching branches
  - Allow: Maintainers

**Rules applied to everyone including administrators:**

- [ ] Allow force pushes (keep unchecked)
- [ ] Allow deletions (keep unchecked)

## Repository Settings

### General Settings

Navigate to **Settings > General**:

**Default branch**: `main`

**Features:**

- [x] Issues
- [ ] Wikis (use docs/ instead)
- [ ] Projects (optional)
- [ ] Discussions (optional)

**Pull Requests:**

- [ ] Allow merge commits
- [x] Allow squash merging (default)
- [ ] Allow rebase merging
- [x] Always suggest updating pull request branches
- [x] Automatically delete head branches

### Code Security and Analysis

Navigate to **Settings > Code security and analysis**:

**Security:**

- [x] Dependency graph
- [x] Dependabot alerts
- [x] Dependabot security updates
- [x] Secret scanning
- [x] Push protection

**Code scanning:**

- [x] CodeQL analysis (set up via workflow)

## Environments

Configure in **Settings > Environments** (optional for releases):

### Production Environment

For NuGet publishing with additional protection:

1. Create environment named `nuget`
2. Configure:
   - [x] Required reviewers: Add maintainers
   - Wait timer: 0 minutes
   - Deployment branches: `main` only

## Webhooks (Optional)

If using external services:

- **CI/CD notifications**: Configure webhook to Slack/Discord
- **Package updates**: Configure webhook for monitoring services

## Labels

Required labels for issue/PR management per [ticket-lifecycle.md](../practices/ticket-lifecycle.md):

### Type Labels (GitHub Defaults)

| Label              | Color     | Description                                 |
| ------------------ | --------- | ------------------------------------------- |
| `bug`              | `#d73a4a` | Something isn't working                     |
| `enhancement`      | `#a2eeef` | New feature or request                      |
| `documentation`    | `#0075ca` | Improvements or additions to documentation  |
| `good first issue` | `#7057ff` | Good for newcomers                          |
| `help wanted`      | `#008672` | Extra attention is needed                   |
| `breaking-change`  | `#b60205` | Breaking change                             |
| `dependencies`     | `#0366d6` | Pull requests that update a dependency file |

### Workflow Labels

| Label           | Color     | Description                          |
| --------------- | --------- | ------------------------------------ |
| `status:triage` | `#fbca04` | Issue needs refinement               |
| `blocked`       | `#b60205` | Cannot proceed - see comments        |
| `epic`          | `#5319e7` | Parent issue coordinating sub-issues |

### Priority Labels

| Label               | Color     | Description                         |
| ------------------- | --------- | ----------------------------------- |
| `priority:critical` | `#b60205` | Must fix immediately                |
| `priority:high`     | `#d93f0b` | Important, address soon             |
| `priority:medium`   | `#fbca04` | Normal priority                     |
| `priority:low`      | `#0e8a16` | Nice to have, address when possible |

### Skill Labels

| Label                  | Color     | Description                     |
| ---------------------- | --------- | ------------------------------- |
| `skill:dotnet`         | `#512BD4` | Requires .NET/C# expertise      |
| `skill:security`       | `#d73a4a` | Requires security expertise     |
| `skill:infrastructure` | `#0366d6` | Requires CI/CD/DevOps expertise |
| `skill:documentation`  | `#0075ca` | Requires documentation skills   |
| `skill:testing`        | `#1d76db` | Requires testing expertise      |

### Creating Labels via CLI

```bash
# Workflow labels
gh label create "status:triage" --color "fbca04" --description "Issue needs refinement"
gh label create "blocked" --color "b60205" --description "Cannot proceed - see comments"
gh label create "epic" --color "5319e7" --description "Parent issue coordinating sub-issues"

# Priority labels
gh label create "priority:critical" --color "b60205" --description "Must fix immediately"
gh label create "priority:high" --color "d93f0b" --description "Important, address soon"
gh label create "priority:medium" --color "fbca04" --description "Normal priority"
gh label create "priority:low" --color "0e8a16" --description "Nice to have"

# Skill labels
gh label create "skill:dotnet" --color "512BD4" --description "Requires .NET/C# expertise"
gh label create "skill:security" --color "d73a4a" --description "Requires security expertise"
gh label create "skill:infrastructure" --color "0366d6" --description "Requires CI/CD/DevOps expertise"
gh label create "skill:documentation" --color "0075ca" --description "Requires documentation skills"
gh label create "skill:testing" --color "1d76db" --description "Requires testing expertise"
```

## Verification Checklist

After setup, verify:

- [ ] Push to feature branch triggers CI workflow
- [ ] PR to main requires status checks
- [ ] Merging to main triggers release workflow (if configured)
- [ ] NUGET_API_KEY secret is accessible to workflows
- [ ] MACHINE_USER_PAT can create commits that trigger workflows
- [ ] Branch protection prevents direct push to main
- [ ] Dependabot creates security PRs
- [ ] Required labels exist (run `gh label list` to verify)

## Troubleshooting

### Workflows not triggering on automated commits

If using `GITHUB_TOKEN`, automated commits won't trigger workflows. Use `MACHINE_USER_PAT` instead:

```yaml
- uses: actions/checkout@v4
  with:
    token: ${{ secrets.MACHINE_USER_PAT }}
```

### Status checks not appearing

1. Run the workflow at least once to register check names
2. Verify workflow job names match branch protection settings
3. Check workflow `on:` triggers include `pull_request`

### NuGet publish failing

1. Verify `NUGET_API_KEY` secret is set
2. Check API key hasn't expired
3. Verify package name matches glob pattern
4. Check NuGet.org account has push permissions
