---
name: github-platform
description: |
  When setting up repository infrastructure, CI/CD, or integrations.
  Applies when choosing tools for code hosting, issues, packages, or security scanning.
decision: Use GitHub as unified development platform for source, CI/CD, packages, and security features.
status: accepted
type: process
---

# ADR-0002: GitHub as Development Platform

## Status

Proposed

## Date

2026-01-09

## Context

We need a unified platform for:

1. Source code hosting
2. Issue and project tracking
3. Code review (pull requests)
4. CI/CD pipelines
5. Package publishing
6. Documentation hosting
7. Security scanning

### Requirements

- Open source friendly with free tier for public repos
- Integrated CI/CD with cross-platform support
- Native package registry for NuGet
- Developer familiarity and ecosystem
- API access for automation

### Options Considered

#### Option 1: GitHub (Selected)

Full-stack development platform with integrated CI/CD, packages, and security.

**Pros:**

- Industry standard for open source
- GitHub Actions for CI/CD (covered in ADR-0004)
- Integrated package registry (GitHub Packages + NuGet.org)
- Native security features (Dependabot, secret scanning, code scanning)
- Excellent .NET support and Microsoft backing
- Large community and marketplace
- Free for public repositories
- GitHub Copilot and AI integrations
- GitHub Projects for issue tracking

**Cons:**

- Vendor lock-in (Microsoft/GitHub)
- Some advanced features require paid plans
- GitHub-specific workflow syntax

#### Option 2: GitLab

Self-hosted or cloud DevOps platform.

**Pros:**

- Complete DevOps platform in one tool
- Self-hosting option for control
- Built-in container registry
- Strong CI/CD features

**Cons:**

- Less adoption than GitHub for open source
- Migration cost from GitHub ecosystem
- Smaller community action ecosystem
- Self-hosting adds operational overhead

#### Option 3: Azure DevOps

Microsoft's enterprise DevOps platform.

**Pros:**

- Deep Azure integration
- Enterprise-grade features
- Good .NET tooling support

**Cons:**

- Less open source friendly
- Separate from code hosting (uses GitHub)
- More complex for simple projects
- Less community/marketplace

#### Option 4: Gitea/Forgejo + External CI

Self-hosted Git with external CI (Drone, Jenkins, etc.).

**Pros:**

- Full control
- No vendor lock-in
- Lightweight

**Cons:**

- Multiple tools to integrate
- Operational overhead
- Smaller ecosystem
- No integrated security scanning

## Decision

We will use **GitHub** as our unified development platform, leveraging:

| Feature            | GitHub Service              |
| ------------------ | --------------------------- |
| Source hosting     | GitHub Repositories         |
| Code review        | Pull Requests               |
| CI/CD              | GitHub Actions (ADR-0004)   |
| Package publishing | GitHub Packages + NuGet.org |
| Issue tracking     | GitHub Issues               |
| Documentation      | GitHub Wiki/Pages           |
| Security           | Dependabot, Secret Scanning |

### Platform Configuration

**Repository Settings:**

- Default branch: `main`
- Branch protection on `main`:
  - Require PR reviews
  - Require status checks to pass
  - Require conversation resolution
  - Require linear history
- Auto-delete head branches after merge
- Squash merge as default

**Security Features:**

- Dependabot alerts: Enabled
- Dependabot security updates: Enabled
- Secret scanning: Enabled
- Code scanning (CodeQL): Enabled for public repos

**GitHub Actions Permissions:**

- Workflow permissions: Read repository contents
- Allow GitHub Actions to create PRs: Disabled
- Allow GitHub Actions to approve PRs: Disabled

### Integration Points

```text
GitHub Repository
    │
    ├── Push Event ──► GitHub Actions (CI)
    │                      │
    │                      ├── Build
    │                      ├── Test
    │                      └── Lint
    │
    ├── Merge to Main ──► GitHub Actions (Release)
    │                          │
    │                          ├── Version (GitVersion)
    │                          ├── Changelog (git-cliff)
    │                          ├── GitHub Release
    │                          └── NuGet Publish
    │
    ├── Dependabot ──► Security PRs
    │
    └── CodeQL ──► Security Alerts
```

### Package Publishing Strategy

| Package Type        | Primary Registry | Backup/Mirror   |
| ------------------- | ---------------- | --------------- |
| NuGet (.NET)        | NuGet.org        | GitHub Packages |
| npm (hooks tooling) | Not published    | -               |

## Consequences

### Positive

- Single platform for all development activities
- Native integrations reduce configuration complexity
- Large ecosystem of Actions and tooling
- Free for open source projects
- Security scanning included at no cost
- Familiar to most .NET developers

### Negative

- Full vendor lock-in to GitHub/Microsoft
- Repository migration would be significant effort
- Some features (advanced security) require Enterprise
- API rate limits may affect heavy automation

### Risks

- GitHub outages affect all development activities
- Microsoft/GitHub policy changes
- Pricing changes for advanced features

### Mitigations

- Keep git history clean for potential migration
- Avoid GitHub-specific features where standard alternatives exist
- Document all integrations for potential migration
- Use standard formats (conventional commits, CHANGELOG.md)

## References

- [GitHub Docs](https://docs.github.com/)
- [GitHub Actions](https://docs.github.com/en/actions)
- [GitHub Packages](https://docs.github.com/en/packages)
- [GitHub Security Features](https://docs.github.com/en/code-security)
- [Branch Protection Rules](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches)
