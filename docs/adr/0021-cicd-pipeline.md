---
name: cicd-pipeline
description: |
  When configuring CI/CD workflows, setting up build automation, or implementing release pipelines. Apply when deciding on pipeline tools, branching strategies, or quality gates.
decision: Use GitHub Actions with GitHub Flow branching for CI/CD with automated releases to NuGet.
status: accepted
---

# ADR-0021: CI/CD Pipeline

## Status

Proposed

## Date

2026-01-09

## Context

We need a CI/CD pipeline that:

1. Builds and tests on multiple platforms (Windows, Linux, macOS)
2. Enforces code quality and commit message standards
3. Automates releases with semantic versioning
4. Publishes packages to NuGet
5. Generates changelogs and GitHub releases

### Options Considered

#### Option 1: GitHub Actions (Selected)

**Pros:**

- Native GitHub integration
- Free for public repos, generous limits for private
- Excellent .NET support
- Matrix builds for cross-platform testing
- Built-in secrets management
- Large marketplace of actions

**Cons:**

- Vendor lock-in to GitHub
- YAML configuration can be complex

#### Option 2: Azure DevOps Pipelines

**Pros:**

- Microsoft ecosystem integration
- Powerful pipeline features
- Good .NET support

**Cons:**

- Separate service from GitHub
- More complex setup for GitHub repos
- Less community action ecosystem

#### Option 3: GitLab CI

**Pros:**

- GitLab-native
- Powerful pipeline features

**Cons:**

- Requires GitLab (we use GitHub)
- Migration overhead

## Decision

We will use **GitHub Actions** with a GitHub Flow branching strategy (main branch only, feature branches for development).

### Branching Strategy

**GitHub Flow (Simplified):**

- `main`: Production-ready, every merge triggers release
- `feature/*`: Development branches, merge via PR

No long-lived branches. Hotfixes branch from release tags and merge to main.

### Pipeline Architecture

**Two workflows:**

1. **CI (`ci.yml`)**: Runs on all pushes and PRs
   - Lint commits and code formatting
   - Build on all platforms
   - Run tests with coverage

2. **Release (`release.yml`)**: Runs on merge to main
   - Build and test
   - Generate version (GitVersion)
   - Create git tag
   - Generate changelog (git-cliff)
   - Create GitHub release
   - Publish to NuGet

### CI Workflow

```yaml
name: CI

on:
  push:
    branches: [main, 'feature/**']
  pull_request:
    branches: [main]

jobs:
  lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0
      - uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'
      - run: npm ci
      - uses: wagoid/commitlint-github-action@v6
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - run: dotnet format --verify-no-changes
      - run: npx prettier --check "**/*.md" "**/*.json" "**/*.yml"

  build:
    needs: lint
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
    runs-on: ${{ matrix.os }}
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - uses: gittools/actions/gitversion/setup@v3
        with:
          versionSpec: '6.x'
      - uses: gittools/actions/gitversion/execute@v3
        id: gitversion
      - run: dotnet build -c Release -p:Version=${{ steps.gitversion.outputs.semVer }}
      - run: dotnet test -c Release --no-build --collect:"XPlat Code Coverage"
      - uses: codecov/codecov-action@v4
        if: matrix.os == 'ubuntu-latest'
```

### Release Workflow

```yaml
name: Release

on:
  push:
    branches: [main]

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - uses: gittools/actions/gitversion/setup@v3
        with:
          versionSpec: '6.x'
      - uses: gittools/actions/gitversion/execute@v3
        id: gitversion
      - run: dotnet build -c Release -p:Version=${{ steps.gitversion.outputs.semVer }}
      - run: dotnet test -c Release --no-build
      - run: dotnet pack -c Release --no-build -p:PackageVersion=${{ steps.gitversion.outputs.semVer }} -o ./artifacts
      - run: git cliff --latest --strip header -o RELEASE_NOTES.md
      - name: Create Git Tag
        run: |
          git config user.name "github-actions[bot]"
          git config user.email "github-actions[bot]@users.noreply.github.com"
          git tag -a "v${{ steps.gitversion.outputs.semVer }}" -m "Release v${{ steps.gitversion.outputs.semVer }}"
          git push origin "v${{ steps.gitversion.outputs.semVer }}"
      - uses: softprops/action-gh-release@v2
        with:
          tag_name: v${{ steps.gitversion.outputs.semVer }}
          body_path: RELEASE_NOTES.md
          files: ./artifacts/*.nupkg
      - run: dotnet nuget push ./artifacts/*.nupkg -k ${{ secrets.NUGET_API_KEY }} -s https://api.nuget.org/v3/index.json
        if: env.NUGET_API_KEY != ''
        env:
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
```

### Quality Gates

| Gate                  | Enforcement                         |
| --------------------- | ----------------------------------- |
| Commit message format | commitlint in CI                    |
| Code formatting       | `dotnet format --verify-no-changes` |
| Build success         | All platforms must pass             |
| Tests pass            | All platforms must pass             |
| PR approval           | Required for merge to main          |

## Consequences

### Positive

- Automated releases reduce manual effort and errors
- Cross-platform testing catches platform-specific issues
- Semantic versioning provides clear version progression
- Changelog generation documents changes automatically
- Quality gates prevent regression

### Negative

- GitHub vendor lock-in
- YAML configuration complexity
- Secrets management required for NuGet publishing

### Risks

- GitHub Actions outages affect releases
- GitVersion misconfiguration can produce wrong versions
- NuGet API key exposure risk (mitigated by secrets)

## References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [GitVersion GitHub Action](https://github.com/GitTools/actions)
- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)
