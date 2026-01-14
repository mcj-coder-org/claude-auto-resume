---
name: versioning-and-changelog-strategy
description: |
  When implementing version numbering, generating changelogs, or configuring semantic versioning. Apply for feature branch prerelease versions or monorepo versioning decisions.
decision: Use GitVersion for semantic versioning and git-cliff for changelog generation with commitlint enforcement.
status: accepted
type: implementation
implementation_issue: '#49'
---

# ADR-0022: Versioning and Changelog Strategy

## Status

Proposed

## Date

2026-01-09

## Context

We need a versioning and changelog strategy for this .NET project that can scale to
organizational use across monorepos. The strategy must support:

1. **Semantic versioning** based on conventional commits
2. **Independent prerelease versions per feature branch** (e.g., `feature/auth` → `1.0.0-auth.1`)
3. **Monorepo support** with independent versioning per component
4. **Changelog generation** from conventional commits
5. **Consistent tooling** that can be standardized across an organization

### Options Considered

#### Option 1: Release Please

Google's release automation tool that generates release PRs from conventional commits.

**Pros:**

- Single tool for versioning + changelog generation
- Excellent monorepo support with workspace plugins
- Native conventional commits support
- Auto-generated changelogs and release PRs

**Cons:**

- No native support for dynamic branch-based prerelease identifiers
- Feature branch prereleases require workarounds (dynamic config generation)
- Transitioning between prerelease phases (alpha → beta → rc) has known issues

#### Option 2: Nerdbank.GitVersioning

Git height-based versioning with per-project `version.json` files.

**Pros:**

- Excellent monorepo support via path filters
- Simple setup
- Source-generated version info

**Cons:**

- No conventional commits support
- No changelog generation
- Manual version bumps for major/minor

#### Option 3: MinVer

Tag-based versioning with minimal configuration.

**Pros:**

- Simplest setup
- Explicit control via tags

**Cons:**

- No conventional commits support
- No changelog generation
- Manual tagging required
- Limited monorepo support

#### Option 4: GitVersion + git-cliff (Selected)

GitVersion for semantic versioning with git-cliff for changelog generation.

**Pros:**

- Native branch-based prerelease identifiers (`feature/auth` → `1.0.0-auth.1`)
- Conventional commits support via configurable regex patterns
- Excellent monorepo support (`ignore.paths`, `tag-prefix`)
- git-cliff provides changelog generation with matching `--tag-pattern` support
- Both tools are open source (MIT/Apache 2.0)
- Flexible configuration for organizational standards

**Cons:**

- Two tools instead of one
- Requires coordination between GitVersion and git-cliff configuration
- Pre-commit hook needed to enforce conventional commit format

## Decision

We will use **GitVersion** for semantic versioning and **git-cliff** for changelog
generation, with **commitlint** (or similar) for pre-commit enforcement of conventional
commits.

### Configuration Alignment

| Concern              | GitVersion                                     | git-cliff                      |
| -------------------- | ---------------------------------------------- | ------------------------------ |
| Tag prefix           | `tag-prefix: 'ProjectA-v'`                     | `--tag-pattern="ProjectA-v.*"` |
| Path filtering       | `ignore.paths`                                 | `--include-path`               |
| Conventional commits | `major/minor/patch-version-bump-message` regex | Native support                 |

### Version Flow

| Branch         | Version Format                 |
| -------------- | ------------------------------ |
| `main`         | `1.0.0`, `1.1.0`, `2.0.0`      |
| `develop`      | `1.1.0-beta.1`, `1.1.0-beta.2` |
| `feature/auth` | `1.1.0-auth.1`, `1.1.0-auth.2` |
| `feature/ui`   | `1.1.0-ui.1`, `1.1.0-ui.2`     |
| `release/1.2`  | `1.2.0-rc.1`, `1.2.0-rc.2`     |

### Monorepo Strategy

For future monorepo use:

```text
/repo
  /src/ProjectA
    GitVersion.yml    # tag-prefix: 'ProjectA-v', ignore.paths for other projects
    cliff.toml        # tag_pattern: 'ProjectA-v.*'
    CHANGELOG.md
  /src/ProjectB
    GitVersion.yml    # tag-prefix: 'ProjectB-v', ignore.paths for other projects
    cliff.toml        # tag_pattern: 'ProjectB-v.*'
    CHANGELOG.md
```

Tags: `ProjectA-v1.0.0`, `ProjectB-v2.3.0` (independent versioning)

## Consequences

### Positive

- Feature branches get unique prerelease versions without conflicts
- Monorepo components can be versioned and released independently
- Conventional commits provide clear intent and automated version bumping
- Changelogs are generated automatically per component
- Tooling can be standardized across the organization
- Both tools are actively maintained open source projects

### Negative

- Two tools to configure and maintain instead of one
- Team must learn conventional commit format
- Pre-commit hooks add friction to development workflow
- Initial setup is more complex than single-tool solutions

### Risks

- GitVersion and git-cliff configurations must stay synchronized
- Known git-cliff issue: tags on same commit can cause confusion (mitigate by staggering releases)
- Conventional commit enforcement depends on pre-commit hooks (can be bypassed with `--no-verify`)

## References

- [GitVersion Documentation](https://gitversion.net/docs/)
- [GitVersion Configuration](https://gitversion.net/docs/reference/configuration)
- [GitVersion Version Increments](https://gitversion.net/docs/reference/version-increments)
- [git-cliff Documentation](https://git-cliff.org/docs/)
- [git-cliff Monorepos](https://git-cliff.org/docs/usage/monorepos/)
- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [MADR Template](https://adr.github.io/madr/)

## License Verification

| Tool       | License        | Verification Date |
| ---------- | -------------- | ----------------- |
| GitVersion | MIT            | 2026-01-09        |
| git-cliff  | MIT/Apache 2.0 | 2026-01-09        |
