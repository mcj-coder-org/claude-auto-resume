# Implementation Plan: .NET 10 Migration

## Overview

Migrate McjCoderOrg.ClaudeAutoResume from a single-file .NET 8 project to a best-practice .NET 10 solution.

**Parent Issue:** #1 - Migrate to .NET 10 best practices
**Feature Branch:** `feature/1-dotnet10-migration`
**Design Document:** [2026-01-09-dotnet10-migration-design.md](./2026-01-09-dotnet10-migration-design.md)

---

## Work Breakdown Structure

```
#1 Migrate to .NET 10 best practices (Epic)
├── #2 Solution Structure & Build Configuration
├── #3 Code Quality & Formatting
├── #4 npm Tooling & Pre-commit Hooks
├── #5 Testing Infrastructure
├── #6 CI/CD Workflows
├── #7 Versioning & Release Pipeline
├── #8 Documentation Foundation
├── #9 Documentation Website
├── #10 Developer Environment
├── #11 Application Updates (Observability & CLI)
└── #12 Final Verification & Release Preparation
```

---

## Sub-Issue #2: Solution Structure & Build Configuration

**Branch:** `feature/2-solution-structure`
**ADRs:** 0012 (Namespace and Project Naming)
**Depends on:** None (Foundation)

### Context

- [Design Document - Solution Structure](./2026-01-09-dotnet10-migration-design.md#solution-structure)
- [Design Document - Project Configuration](./2026-01-09-dotnet10-migration-design.md#project-configuration)
- [ADR-0012: Namespace and Project Naming](../adr/0012-namespace-and-project-naming.md)

### Tasks

- [ ] Create solution file: `McjCoderOrg.ClaudeAutoResume.sln`
- [ ] Create `src/` directory structure
- [ ] Move and rename main project to `src/McjCoderOrg.ClaudeAutoResume/`
- [ ] Update project file with `McjCoderOrg` namespace and tool configuration
- [ ] Create `Directory.Build.props` with .NET 10 settings
- [ ] Create `Directory.Packages.props` for centralized package management
- [ ] Create placeholder test projects (structure only):
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.Tests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.ArchTests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/`
- [ ] Configure `InternalsVisibleTo` for test projects
- [ ] Add Source Link configuration
- [ ] Update `.gitignore` for new structure

### Verification

```bash
dotnet restore
dotnet build
```

### Acceptance Criteria

- [ ] Solution builds without errors
- [ ] All projects in correct locations
- [ ] Namespaces follow `McjCoderOrg.ClaudeAutoResume` convention
- [ ] Source Link configured

---

## Sub-Issue #3: Code Quality & Formatting

**Branch:** `feature/3-code-quality`
**ADRs:** 0010 (Code Formatting), 0015 (Code Analyzers), 0020 (Breaking Change Detection)
**Depends on:** #2

### Context

- [Design Document - Code Quality](./2026-01-09-dotnet10-migration-design.md#code-quality)
- [ADR-0010: Code Formatting](../adr/0010-code-formatting.md)
- [ADR-0015: Code Analyzers](../adr/0015-code-analyzers.md)
- [ADR-0020: Breaking Change Detection](../adr/0020-breaking-change-detection.md)

### Tasks

- [ ] Create `.editorconfig` with C# style rules
- [ ] Create `.gitattributes` for line ending enforcement
- [ ] Add analyzer packages to `Directory.Build.props`:
  - [ ] Meziantou.Analyzer
  - [ ] Roslynator.Analyzers
  - [ ] SonarAnalyzer.CSharp
  - [ ] Microsoft.CodeAnalysis.PublicApiAnalyzers
- [ ] Create `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt`
- [ ] Normalize line endings: `git add --renormalize .`
- [ ] Fix all analyzer violations in existing code
- [ ] Document justified suppressions in `.editorconfig`

### Verification

```bash
dotnet build -warnaserror
dotnet format --verify-no-changes
```

### Acceptance Criteria

- [ ] Zero analyzer warnings
- [ ] Code formatting consistent
- [ ] Line endings normalized to LF
- [ ] Public API tracked

---

## Sub-Issue #4: npm Tooling & Pre-commit Hooks

**Branch:** `feature/4-npm-tooling`
**ADRs:** 0005 (Security Scanning), 0011 (Pre-commit Hooks)
**Depends on:** #3

### Context

- [Design Document - Security](./2026-01-09-dotnet10-migration-design.md#security)
- [Design Document - Code Quality](./2026-01-09-dotnet10-migration-design.md#code-quality)
- [ADR-0005: Security Scanning](../adr/0005-security-scanning.md)
- [ADR-0011: Pre-commit Hooks](../adr/0011-pre-commit-hooks.md)

### Tasks

- [ ] Create `package.json` with devDependencies:
  - [ ] husky
  - [ ] lint-staged
  - [ ] @commitlint/cli, @commitlint/config-conventional
  - [ ] prettier
  - [ ] secretlint and plugins
  - [ ] cspell
  - [ ] markdownlint-cli
- [ ] Create configuration files:
  - [ ] `commitlint.config.js`
  - [ ] `.prettierrc`
  - [ ] `.secretlintrc.json`
  - [ ] `.cspell.json` (en-GB default)
  - [ ] `.markdownlint.json`
- [ ] Initialize Husky: `npm install && npx husky init`
- [ ] Create git hooks:
  - [ ] `.husky/pre-commit` (lint-staged, block main commits)
  - [ ] `.husky/commit-msg` (commitlint)
  - [ ] `.husky/pre-push` (branch name validation, tests)
- [ ] Configure `lint-staged` in package.json
- [ ] Add npm scripts for common tasks
- [ ] Create `.nvmrc` for Node version

### Verification

```bash
npm install
echo "invalid" | npx commitlint  # Should fail
echo "feat: test" | npx commitlint  # Should pass
npx secretlint --version
npx cspell --version
```

### Acceptance Criteria

- [ ] Pre-commit hook blocks direct commits to main
- [ ] Commit messages validated against conventional commits
- [ ] Secrets scanned on commit
- [ ] Spelling checked on markdown files

---

## Sub-Issue #5: Testing Infrastructure

**Branch:** `feature/5-testing-infrastructure`
**ADRs:** 0013 (Testing Framework), 0014 (Test Project Structure), 0016 (Architecture Testing), 0024 (Code Coverage), 0025 (Performance Testing)
**Depends on:** #2

### Context

- [Design Document - Testing Strategy](./2026-01-09-dotnet10-migration-design.md#testing-strategy)
- [ADR-0013: Testing Framework](../adr/0013-testing-framework.md)
- [ADR-0014: Test Project Structure](../adr/0014-test-project-structure.md)

### Tasks

- [ ] Configure unit test project with packages:
  - [ ] xunit, xunit.runner.visualstudio, xunit.analyzers
  - [ ] AwesomeAssertions
  - [ ] Moq, Moq.Analyzers
  - [ ] coverlet.collector
- [ ] Configure system test project (BDD):
  - [ ] Reqnroll, Reqnroll.xUnit
  - [ ] Create `Features/`, `StepDefinitions/`, `Support/` directories
- [ ] Configure E2E test project (BDD):
  - [ ] Separate namespace (public API only)
  - [ ] Create feature and step definition structure
- [ ] Configure architecture test project:
  - [ ] NetArchTest.eNhancedEdition
  - [ ] Create initial architecture rules
- [ ] Configure benchmark project:
  - [ ] BenchmarkDotNet
  - [ ] Create initial benchmarks
- [ ] Create `coverlet.runsettings` with exclusions
- [ ] Create `GlobalUsings.cs` for each test project
- [ ] Write initial unit tests for existing code
- [ ] Write initial BDD features for rate limit detection

### Verification

```bash
dotnet test --verbosity normal
dotnet test --collect:"XPlat Code Coverage"
```

### Acceptance Criteria

- [ ] All test projects build and run
- [ ] Unit tests pass
- [ ] BDD features execute
- [ ] Architecture rules pass
- [ ] Code coverage generated

---

## Sub-Issue #6: CI/CD Workflows

**Branch:** `feature/6-cicd-workflows`
**ADRs:** 0002 (GitHub Platform), 0004 (Branch Strategy), 0021 (CI/CD Pipeline)
**Depends on:** #4, #5

### Context

- [Design Document - CI/CD Pipeline](./2026-01-09-dotnet10-migration-design.md#cicd-pipeline)
- [ADR-0021: CI/CD Pipeline](../adr/0021-cicd-pipeline.md)

### Tasks

- [ ] Create `.github/workflows/ci.yml`:
  - [ ] Lint (format, prettier, markdownlint, cspell)
  - [ ] Build (all OS with continue-on-error)
  - [ ] Test (unit, system, architecture)
  - [ ] Coverage reporting
- [ ] Create `.github/workflows/pr-title.yml`:
  - [ ] Validate PR title against conventional commits
  - [ ] Check for work item reference in body
- [ ] Create `.github/workflows/codeql.yml`:
  - [ ] CodeQL analysis for C#
  - [ ] Run on PR and main push
- [ ] Create `.github/workflows/nightly.yml`:
  - [ ] E2E tests
  - [ ] Mutation testing (Stryker)
  - [ ] Full benchmark suite
- [ ] Create `.github/workflows/dependabot-automerge.yml`:
  - [ ] Auto-approve patch updates
  - [ ] Use MACHINE_USER_PAT
- [ ] Create `.github/dependabot.yml`:
  - [ ] NuGet weekly updates
  - [ ] npm weekly updates
  - [ ] Grouped dependencies
- [ ] Configure branch protection rules (document for manual setup)
- [ ] Configure caching strategy for NuGet and npm

### Verification

- Push to feature branch
- Create test PR
- Verify all workflows trigger correctly

### Acceptance Criteria

- [ ] CI workflow runs on push and PR
- [ ] PR title validation works
- [ ] CodeQL analysis runs
- [ ] Dependabot configured

---

## Sub-Issue #7: Versioning & Release Pipeline

**Branch:** `feature/7-versioning-release`
**ADRs:** 0022 (Versioning and Changelog Strategy), 0027 (Release Artifacts)
**Depends on:** #6

### Context

- [Design Document - Versioning & Releases](./2026-01-09-dotnet10-migration-design.md#versioning--releases)
- [ADR-0022: Versioning and Changelog Strategy](../adr/0022-versioning-and-changelog-strategy.md)
- [ADR-0027: Release Artifacts](../adr/0027-release-artifacts.md)

### Tasks

- [ ] Create `GitVersion.yml` configuration
- [ ] Create `cliff.toml` for git-cliff changelog
- [ ] Install GitVersion as local tool
- [ ] Create initial `CHANGELOG.md`
- [ ] Create `.github/workflows/release.yml`:
  - [ ] Version calculation
  - [ ] Build standalone executables (all platforms)
  - [ ] Generate SBOM
  - [ ] Generate checksums
  - [ ] Create GitHub Release
  - [ ] Publish to NuGet
  - [ ] Create changelog PR (auto-approved via MACHINE_USER_PAT)
- [ ] Create `stryker-config.json` for mutation testing
- [ ] Document release process in playbook

### Verification

```bash
dotnet tool restore
dotnet gitversion
git cliff --unreleased
```

### Acceptance Criteria

- [ ] GitVersion calculates correct version
- [ ] git-cliff generates changelog
- [ ] Release workflow configured (manual trigger to test)

---

## Sub-Issue #8: Documentation Foundation

**Branch:** `feature/8-documentation-foundation`
**ADRs:** 0001 (License), 0008 (Documentation Strategy), 0009 (Agent Onboarding)
**Depends on:** #2

### Context

- [Design Document - Documentation](./2026-01-09-dotnet10-migration-design.md#documentation)
- [ADR-0001: License](../adr/0001-license.md)
- [ADR-0008: Documentation Strategy](../adr/0008-documentation-strategy.md)
- [ADR-0009: Agent Onboarding](../adr/0009-agent-onboarding.md)

### Tasks

- [ ] Create documentation directory structure:
  - [ ] `docs/standards/`
  - [ ] `docs/practices/`
  - [ ] `docs/playbooks/`
  - [ ] `docs/agents/`
- [ ] Create `AGENTS.md` with agent routing rules
- [ ] Create `docs/agents/ORIENTATION.md`
- [ ] Create `docs/agents/CONVENTIONS.md`
- [ ] Create `docs/standards/coding-standards.md`
- [ ] Create `docs/practices/code-review.md`
- [ ] Create `docs/playbooks/mutation-testing.md`
- [ ] Create `LICENSE` file (MIT)
- [ ] Update `README.md` with new structure
- [ ] Create GitHub templates:
  - [ ] `.github/ISSUE_TEMPLATE/bug_report.md`
  - [ ] `.github/ISSUE_TEMPLATE/feature_request.md`
  - [ ] `.github/PULL_REQUEST_TEMPLATE.md`
  - [ ] `.github/CODEOWNERS`
  - [ ] `.github/SECURITY.md`
  - [ ] `.github/CONTRIBUTING.md`
- [ ] Add front-matter to all documentation files

### Verification

- Review all documentation renders correctly
- Verify links work

### Acceptance Criteria

- [ ] AGENTS.md provides clear routing
- [ ] All documentation has front-matter
- [ ] GitHub templates configured
- [ ] LICENSE file present

---

## Sub-Issue #9: Documentation Website

**Branch:** `feature/9-documentation-website`
**ADRs:** 0006 (Accessibility), 0008 (Documentation Strategy), 0028 (Documentation Versioning)
**Depends on:** #8

### Context

- [Design Document - Documentation](./2026-01-09-dotnet10-migration-design.md#documentation)
- [ADR-0006: Accessibility](../adr/0006-accessibility.md)
- [ADR-0028: Documentation Versioning](../adr/0028-documentation-versioning.md)

### Tasks

- [ ] Initialize Docusaurus in `docs/docusaurus/`:
  ```bash
  npx create-docusaurus@latest docusaurus classic --typescript
  ```
- [ ] Configure Docusaurus for GitHub Pages
- [ ] Set up versioning configuration
- [ ] Create initial documentation pages:
  - [ ] Getting Started
  - [ ] Installation
  - [ ] Configuration
  - [ ] CLI Reference (from --help)
  - [ ] Troubleshooting
- [ ] Configure accessibility features (WCAG 2.1 AA)
- [ ] Create `.github/workflows/docs.yml` for deployment
- [ ] Add Lighthouse CI for accessibility audits

### Verification

```bash
cd docs/docusaurus
npm install
npm run build
npm run serve
```

### Acceptance Criteria

- [ ] Documentation site builds
- [ ] Versioning configured
- [ ] Accessibility audit passes
- [ ] GitHub Pages deployment works

---

## Sub-Issue #10: Developer Environment

**Branch:** `feature/10-developer-environment`
**ADRs:** 0029 (Developer Environment)
**Depends on:** #4

### Context

- [Design Document - Developer Experience](./2026-01-09-dotnet10-migration-design.md#developer-experience)
- [ADR-0029: Developer Environment](../adr/0029-developer-environment.md)

### Tasks

- [ ] Create `.devcontainer/devcontainer.json`:
  - [ ] .NET 10 SDK
  - [ ] Node.js 22
  - [ ] GitHub CLI
  - [ ] VS Code extensions
  - [ ] Post-create command
- [ ] Create bootstrap scripts:
  - [ ] `scripts/setup.ps1` (Windows)
  - [ ] `scripts/setup.sh` (Unix)
- [ ] Scripts should:
  - [ ] Check prerequisites
  - [ ] Restore dependencies
  - [ ] Configure git hooks
  - [ ] Enable signed commits
  - [ ] Verify build
- [ ] Add npm scripts for common tasks
- [ ] Create `.NET local tools manifest` (`.config/dotnet-tools.json`)

### Verification

- Test dev container in VS Code
- Run bootstrap scripts on clean machine

### Acceptance Criteria

- [ ] Dev container starts and builds project
- [ ] Bootstrap scripts work on Windows and Unix
- [ ] All tooling accessible after setup

---

## Sub-Issue #11: Application Updates (Observability & CLI)

**Branch:** `feature/11-application-updates`
**ADRs:** 0017 (Observability), 0018 (CLI Design), 0019 (Internationalization)
**Depends on:** #2

### Context

- [Design Document - Observability](./2026-01-09-dotnet10-migration-design.md#observability)
- [Design Document - CLI Design](./2026-01-09-dotnet10-migration-design.md#cli-design)
- [ADR-0017: Observability](../adr/0017-observability.md)
- [ADR-0018: CLI Design](../adr/0018-cli-design.md)

### Tasks

- [ ] Add Serilog packages:
  - [ ] Serilog
  - [ ] Serilog.Sinks.File
  - [ ] Serilog.Sinks.Debug
  - [ ] Serilog.Extensions.Hosting
- [ ] Implement bootstrap logger for startup errors
- [ ] Implement platform context capture:
  - [ ] .NET version
  - [ ] OS description
  - [ ] Command line arguments
  - [ ] App version
- [ ] Create resource files for i18n-ready strings:
  - [ ] `Resources/Strings.resx` (en-GB default)
- [ ] Refactor to use modern Host.CreateApplicationBuilder() pattern
- [ ] Implement `--verbose` flag for file logging
- [ ] Implement `--diagnose` command for diagnostics
- [ ] Implement layered configuration loading
- [ ] Define semantic exit codes
- [ ] Add global exception handler with log path output
- [ ] Create test log capture utility for unit tests
- [ ] Update logging to use named parameters

### Verification

```bash
dotnet run -- --help
dotnet run -- --version
dotnet run -- --diagnose
dotnet run -- --verbose
```

### Acceptance Criteria

- [ ] `--help` shows all options
- [ ] `--diagnose` outputs environment info
- [ ] `--verbose` creates log file
- [ ] Startup errors logged with path shown to user
- [ ] Log messages use named parameters

---

## Sub-Issue #12: Final Verification & Release Preparation

**Branch:** `feature/12-final-verification`
**ADRs:** All remaining
**Depends on:** All previous sub-issues

### Context

- All design documents and ADRs

### Tasks

- [ ] Verify all ADRs are complete and accurate
- [ ] Update all ADRs from "Proposed" to "Accepted"
- [ ] Run full test suite:
  ```bash
  dotnet test --configuration Release
  ```
- [ ] Run full lint suite:
  ```bash
  npm run lint
  dotnet format --verify-no-changes
  ```
- [ ] Run security scans:
  ```bash
  npx secretlint .
  dotnet list package --vulnerable
  ```
- [ ] Run architecture tests
- [ ] Run benchmarks and establish baselines
- [ ] Generate initial CHANGELOG.md
- [ ] Create release checklist document
- [ ] Update README with final instructions
- [ ] Create PR to merge feature branch to main
- [ ] Document branch protection setup (manual GitHub configuration)

### Verification

- All CI checks pass
- Manual review of all components

### Acceptance Criteria

- [ ] All ADRs marked as Accepted
- [ ] All tests pass
- [ ] All linting passes
- [ ] No security vulnerabilities
- [ ] Documentation complete
- [ ] Ready for initial release

---

## Dependency Graph

```
#2 Solution Structure
├──► #3 Code Quality
│    └──► #4 npm Tooling
│         ├──► #6 CI/CD Workflows
│         │    └──► #7 Versioning & Release
│         └──► #10 Developer Environment
├──► #5 Testing Infrastructure
│    └──► #6 CI/CD Workflows
├──► #8 Documentation Foundation
│    └──► #9 Documentation Website
└──► #11 Application Updates

All ──► #12 Final Verification
```

---

## Parallel Execution Opportunities

The following can be worked on in parallel once #2 is complete:

**Track A (Testing & CI):**
- #3 Code Quality → #4 npm Tooling → #6 CI/CD → #7 Versioning

**Track B (Testing):**
- #5 Testing Infrastructure → #6 CI/CD

**Track C (Documentation):**
- #8 Documentation Foundation → #9 Documentation Website

**Track D (Application):**
- #11 Application Updates

**Track E (Dev Experience):**
- #10 Developer Environment (after #4)

---

## Notes for Implementers

1. **Immutable Context Links**: When creating sub-issues, use commit SHA links to this plan and design document to ensure context stability.

2. **ADR Updates**: Each sub-issue lists which ADRs it validates. Mark ADRs as "Accepted" only when the sub-issue is complete and merged.

3. **Branch Strategy**: Each sub-issue gets its own branch from main (not from the epic branch). The epic branch contains planning artifacts only.

4. **PR Workflow**: Each sub-issue branch creates a PR to main. Use squash merge with conventional commit message.

5. **Testing Each Merge**: After merging each sub-issue, verify main still builds and all existing tests pass before starting next sub-issue.
