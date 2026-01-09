# Implementation Plan: .NET 10 Migration

## Overview

Migrate McjCoderOrg.ClaudeAutoResume from a single-file .NET 8 project to a best-practice .NET 10 solution.

**Parent Issue:** #1 - Migrate to .NET 10 best practices
**Feature Branch:** `feature/1-dotnet10-migration`
**Design Document:** [2026-01-09-dotnet10-migration-design.md](./2026-01-09-dotnet10-migration-design.md)

---

## Implementation Philosophy

**Establish the rules before migrating the code.**

The implementation follows the ADR tier structure, ensuring foundational constraints, compliance standards, and processes are in place before any technology decisions or code migration. This approach:

1. Prevents retrofitting compliance onto existing code
2. Ensures all code enters a properly configured environment
3. Makes the migration itself a validation of the setup
4. Creates a reusable template for future projects

---

## Work Breakdown Structure

```
#1 Migrate to .NET 10 best practices (Epic)
│
├─ Phase 1: Foundational Constraints (ADRs 0001-0004)
│  └── #2 Repository Foundation
│
├─ Phase 2: Compliance & Standards (ADRs 0005-0007)
│  └── #3 Security & Compliance Baseline
│
├─ Phase 3: Process & Workflow (ADRs 0008-0011)
│  ├── #4 Documentation Foundation
│  └── #5 Quality Gates & Hooks
│
├─ Phase 4: Technology Setup (ADRs 0012-0020)
│  ├── #6 Solution Structure
│  ├── #7 Testing Infrastructure
│  └── #8 Application Framework
│
├─ Phase 5: CI/CD & Release (ADRs 0021-0029)
│  ├── #9 CI/CD Pipeline
│  ├── #10 Versioning & Release
│  └── #11 Developer Environment
│
├─ Phase 6: Infrastructure Verification
│  └── #12 Prove Workflows & Standards
│
├─ Phase 7: Code Migration
│  └── #13 Migrate Code Bundle
│
└─ Phase 8: Final Verification
   └── #14 Release Preparation
```

---

## Phase 1: Foundational Constraints

### Sub-Issue #2: Repository Foundation

**Branch:** `feature/2-repository-foundation`
**ADRs:** 0001 (License), 0002 (GitHub Platform), 0003 (Work Item Management), 0004 (Contribution Workflow)
**Depends on:** None (First phase)

### Context

Establish the legal and organizational foundation before any code or tooling decisions.

- [ADR-0001: License](../adr/0001-license.md)
- [ADR-0002: GitHub Platform](../adr/0002-github-platform.md)
- [ADR-0003: Work Item Management](../adr/0003-work-item-management.md)
- [ADR-0004: Contribution Workflow](../adr/0004-contribution-workflow.md)

### Tasks

- [ ] Create `LICENSE` file (MIT)
- [ ] Create `.github/` directory structure
- [ ] Create issue templates:
  - [ ] `.github/ISSUE_TEMPLATE/bug_report.md`
  - [ ] `.github/ISSUE_TEMPLATE/feature_request.md`
  - [ ] `.github/ISSUE_TEMPLATE/config.yml` (template chooser)
- [ ] Create `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] Create `.github/CODEOWNERS`
- [ ] Document branch protection rules (for manual GitHub setup):
  - [ ] Require PR reviews
  - [ ] Require signed commits
  - [ ] Require status checks
  - [ ] Block direct pushes to main
  - [ ] Auto-delete head branches
- [ ] Create `.gitattributes` for line ending enforcement
- [ ] Update `.gitignore` with comprehensive patterns

### Verification

- [ ] LICENSE file present and correct
- [ ] Issue templates render correctly on GitHub
- [ ] PR template includes checklist
- [ ] Branch protection documentation complete

### Acceptance Criteria

- [ ] Legal framework established (LICENSE)
- [ ] Work item templates ready for use
- [ ] Branch strategy documented
- [ ] Repository configured for collaboration

---

## Phase 2: Compliance & Standards

### Sub-Issue #3: Security & Compliance Baseline

**Branch:** `feature/3-security-compliance`
**ADRs:** 0005 (Security Scanning), 0006 (Accessibility), 0007 (Telemetry)
**Depends on:** #2

### Context

Establish security scanning and compliance standards before any code enters the repository.

- [ADR-0005: Security Scanning](../adr/0005-security-scanning.md)
- [ADR-0006: Accessibility](../adr/0006-accessibility.md)
- [ADR-0007: Telemetry](../adr/0007-telemetry.md)

### Tasks

- [ ] Create `.github/SECURITY.md` (vulnerability reporting policy)
- [ ] Create `.github/dependabot.yml`:
  - [ ] NuGet ecosystem configuration
  - [ ] npm ecosystem configuration
  - [ ] Weekly update schedule
  - [ ] Grouped updates
- [ ] Create `.secretlintrc.json` with recommended rules
- [ ] Install secretlint plugins in package.json (placeholder):
  - [ ] `@secretlint/secretlint-rule-preset-recommend`
  - [ ] `@secretlint/secretlint-rule-aws`
  - [ ] `@secretlint/secretlint-rule-gcp`
  - [ ] `@secretlint/secretlint-rule-privatekey`
- [ ] Create `.github/workflows/codeql.yml` for SAST
- [ ] Document accessibility standards (WCAG 2.1 AA) in `docs/standards/`
- [ ] Document privacy policy (no telemetry) in `docs/standards/`

### Verification

```bash
npx secretlint --version  # After npm install in Phase 3
```

### Acceptance Criteria

- [ ] Security policy documented
- [ ] Secret scanning configured
- [ ] SAST workflow ready
- [ ] Dependabot configured
- [ ] Compliance standards documented

---

## Phase 3: Process & Workflow

### Sub-Issue #4: Documentation Foundation

**Branch:** `feature/4-documentation-foundation`
**ADRs:** 0008 (Documentation Strategy), 0009 (Agent Onboarding)
**Depends on:** #3

### Context

Establish documentation structure and agent onboarding before tooling setup.

- [ADR-0008: Documentation Strategy](../adr/0008-documentation-strategy.md)
- [ADR-0009: Agent Onboarding](../adr/0009-agent-onboarding.md)

### Tasks

- [ ] Create documentation directory structure:
  - [ ] `docs/standards/`
  - [ ] `docs/practices/`
  - [ ] `docs/playbooks/`
  - [ ] `docs/agents/`
- [ ] Create `AGENTS.md` with:
  - [ ] Quick start section
  - [ ] Documentation loading rules (full vs front-matter)
  - [ ] Project structure overview
  - [ ] Common task routing
- [ ] Create `docs/agents/ORIENTATION.md`
- [ ] Create `docs/agents/CONVENTIONS.md`
- [ ] Create `docs/agents/PATTERNS.md` (per ADR-0009)
- [ ] Create `docs/agents/IDE-SETUP.md` (VS Code, Rider, VS)
- [ ] Create `docs/agents/PERSONAS.md`:
  - [ ] Define project-specific roles (subset of full role library)
  - [ ] Include frontmatter: name, description, model tier
  - [ ] Document when to use each persona
  - [ ] Include blocking issues that require escalation
- [ ] Create `docs/agents/TROUBLESHOOTING.md`
- [ ] Create `docs/standards/coding-standards.md`:
  - [ ] C# naming conventions
  - [ ] File organization
  - [ ] Comment and documentation requirements
  - [ ] Error handling patterns
  - [ ] Logging standards
- [ ] Create `docs/practices/code-review.md`:
  - [ ] Review criteria and checklist
  - [ ] Approval requirements
  - [ ] Common issues to watch for
  - [ ] Review etiquette
- [ ] Add front-matter template to documentation guidelines
- [ ] Update `README.md` with project overview

### Verification

- [ ] All documentation files have front-matter
- [ ] AGENTS.md provides clear routing
- [ ] Links between documents work
- [ ] Coding standards document is actionable (not placeholder)
- [ ] Code review practices document is actionable (not placeholder)

### Acceptance Criteria

- [ ] Documentation structure established
- [ ] Agent onboarding path clear
- [ ] All standards documents complete and actionable
- [ ] Project-specific personas defined with selection guidance
- [ ] IDE setup documented for VS Code, Rider, and Visual Studio
- [ ] No placeholder content in any document

---

### Sub-Issue #5: Quality Gates & Hooks

**Branch:** `feature/5-quality-gates`
**ADRs:** 0010 (Code Formatting), 0011 (Pre-commit Hooks)
**Depends on:** #4

### Context

Establish quality gates before any code enters the repository.

- [ADR-0010: Code Formatting](../adr/0010-code-formatting.md)
- [ADR-0011: Pre-commit Hooks](../adr/0011-pre-commit-hooks.md)

### Tasks

- [ ] Create `package.json` with devDependencies:
  - [ ] husky
  - [ ] lint-staged
  - [ ] @commitlint/cli, @commitlint/config-conventional
  - [ ] prettier
  - [ ] secretlint (and plugins from #3)
  - [ ] cspell
  - [ ] markdownlint-cli
- [ ] Create configuration files:
  - [ ] `commitlint.config.js`
  - [ ] `.prettierrc`
  - [ ] `.cspell.json` (en-GB default)
  - [ ] `.markdownlint.json`
- [ ] Create `.editorconfig` with C# and general rules
- [ ] Initialize Husky: `npm install && npx husky init`
- [ ] Create git hooks:
  - [ ] `.husky/pre-commit` (lint-staged, block main commits, verify signed)
  - [ ] `.husky/commit-msg` (commitlint)
  - [ ] `.husky/pre-push` (branch name validation)
- [ ] Configure `lint-staged` in package.json
- [ ] Create `.nvmrc` for Node version (22)
- [ ] Add npm scripts for common tasks

### Verification

```bash
npm install
echo "invalid" | npx commitlint  # Should fail
echo "feat: valid commit

Refs: #1" | npx commitlint  # Should pass
npx secretlint --version
npx cspell --version
npx prettier --version
```

### Acceptance Criteria

- [ ] Pre-commit hooks block invalid commits
- [ ] Commit messages validated
- [ ] Secrets scanned on commit
- [ ] Spelling and formatting checked
- [ ] Direct commits to main blocked

---

## Phase 4: Technology Setup

### Sub-Issue #6: Solution Structure

**Branch:** `feature/6-solution-structure`
**ADRs:** 0012 (Namespace and Project Naming), 0015 (Code Analyzers), 0020 (Breaking Change Detection)
**Depends on:** #5

### Context

Now that governance is in place, create the .NET solution structure.

- [ADR-0012: Namespace and Project Naming](../adr/0012-namespace-and-project-naming.md)
- [ADR-0015: Code Analyzers](../adr/0015-code-analyzers.md)
- [ADR-0020: Breaking Change Detection](../adr/0020-breaking-change-detection.md)

### Tasks

- [ ] Create solution file: `McjCoderOrg.ClaudeAutoResume.sln`
- [ ] Create `src/` directory structure
- [ ] Create main project: `src/McjCoderOrg.ClaudeAutoResume/`
  - [ ] Project file with namespaces and tool configuration
  - [ ] `InternalsVisibleTo` for test projects
  - [ ] Source Link configuration
- [ ] Create `Directory.Build.props` with:
  - [ ] .NET 10 / C# 14 settings
  - [ ] Analyzer references
  - [ ] Strict warnings
- [ ] Create `Directory.Packages.props` for centralized versioning
- [ ] Create empty test project placeholders:
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.Tests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.ArchTests/`
  - [ ] `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/`
- [ ] Create `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt`
- [ ] Add solution to `.gitignore` patterns

### Verification

```bash
dotnet restore
dotnet build
```

### Acceptance Criteria

- [ ] Solution builds with no errors
- [ ] Analyzer warnings treated as errors
- [ ] Namespaces follow convention
- [ ] Public API tracking configured

---

### Sub-Issue #7: Testing Infrastructure

**Branch:** `feature/7-testing-infrastructure`
**ADRs:** 0013 (Testing Framework), 0014 (Test Project Structure), 0016 (Architecture Testing), 0024 (Code Coverage), 0025 (Performance Testing)
**Depends on:** #6

### Context

Configure test projects with proper packages and structure.

- [ADR-0013: Testing Framework](../adr/0013-testing-framework.md)
- [ADR-0014: Test Project Structure](../adr/0014-test-project-structure.md)
- [ADR-0016: Architecture Testing](../adr/0016-architecture-testing.md)

### Tasks

- [ ] Configure unit test project packages:
  - [ ] xunit, xunit.runner.visualstudio, xunit.analyzers
  - [ ] AwesomeAssertions
  - [ ] Moq, Moq.Analyzers
  - [ ] coverlet.collector
- [ ] Configure system test project (BDD):
  - [ ] Reqnroll, Reqnroll.xUnit
  - [ ] Create `Features/`, `StepDefinitions/`, `Support/` directories
- [ ] Configure E2E test project (BDD):
  - [ ] Separate namespace (public API only)
- [ ] Configure architecture test project:
  - [ ] NetArchTest.eNhancedEdition
  - [ ] Create placeholder architecture rules
- [ ] Configure benchmark project:
  - [ ] BenchmarkDotNet
- [ ] Create `coverlet.runsettings` with exclusions
- [ ] Create `GlobalUsings.cs` for each test project

### Verification

```bash
dotnet test --verbosity normal
```

### Acceptance Criteria

- [ ] All test projects build
- [ ] Test framework properly configured
- [ ] Coverage collection works
- [ ] Architecture tests structure ready

---

### Sub-Issue #8: Application Framework

**Branch:** `feature/8-application-framework`
**ADRs:** 0017 (Observability), 0018 (CLI Design), 0019 (Internationalization)
**Depends on:** #6

### Context

Set up application framework patterns before migrating code.

- [ADR-0017: Observability](../adr/0017-observability.md)
- [ADR-0018: CLI Design](../adr/0018-cli-design.md)
- [ADR-0019: Internationalization](../adr/0019-internationalization.md)

### Tasks

- [ ] Add Serilog packages to main project:
  - [ ] Serilog
  - [ ] Serilog.Sinks.File
  - [ ] Serilog.Sinks.Debug
  - [ ] Serilog.Extensions.Hosting
- [ ] Create application framework classes:
  - [ ] `PlatformInfo.cs` - Platform context capture
  - [ ] `ExitCodes.cs` - Semantic exit code constants
  - [ ] `LoggingConfiguration.cs` - Bootstrap and runtime logging
- [ ] Create resource file structure:
  - [ ] `Resources/Strings.resx` (en-GB default)
- [ ] Create `Program.cs` template with:
  - [ ] Host.CreateApplicationBuilder() pattern
  - [ ] Bootstrap logger
  - [ ] Global exception handler
  - [ ] `--help`, `--version`, `--diagnose`, `--verbose` options
- [ ] Create test utilities:
  - [ ] `LogCapture.cs` for test log assertions

### Verification

```bash
dotnet run -- --help
dotnet run -- --version
dotnet run -- --diagnose
```

### Acceptance Criteria

- [ ] CLI framework functional
- [ ] Logging infrastructure ready
- [ ] Platform context captured
- [ ] i18n structure in place

---

## Phase 5: CI/CD & Release

### Sub-Issue #9: CI/CD Pipeline

**Branch:** `feature/9-cicd-pipeline`
**ADRs:** 0002 (GitHub Platform), 0004 (Contribution Workflow), 0021 (CI/CD Pipeline)
**Depends on:** #7, #8

### Context

Implement CI/CD workflows that enforce all previous decisions.

- [ADR-0021: CI/CD Pipeline](../adr/0021-cicd-pipeline.md)

### Tasks

- [ ] Create `.github/workflows/ci.yml`:
  - [ ] Lint (format, prettier, markdownlint, cspell, secretlint)
  - [ ] Build (all OS with continue-on-error)
  - [ ] Test (unit, system, architecture)
  - [ ] Coverage reporting
- [ ] Create `.github/workflows/pr-title.yml`:
  - [ ] Validate PR title against conventional commits
  - [ ] Check for work item reference
- [ ] Create `.github/workflows/nightly.yml`:
  - [ ] E2E tests
  - [ ] Mutation testing
  - [ ] Full benchmark suite
- [ ] Create `.github/workflows/dependabot-automerge.yml`:
  - [ ] Auto-approve patch updates
  - [ ] Use MACHINE_USER_PAT
- [ ] Configure caching strategy for NuGet and npm

### Verification

- Push to feature branch and verify CI runs
- Create test PR and verify checks

### Acceptance Criteria

- [ ] CI runs on all pushes and PRs
- [ ] All quality gates enforced
- [ ] PR validation works
- [ ] Caching reduces build times

---

### Sub-Issue #10: Versioning & Release

**Branch:** `feature/10-versioning-release`
**ADRs:** 0022 (Versioning and Changelog), 0023 (Dependency Management), 0026 (Mutation Testing), 0027 (Release Artifacts)
**Depends on:** #9

### Context

Configure release pipeline with proper versioning and artifacts.

- [ADR-0022: Versioning and Changelog Strategy](../adr/0022-versioning-and-changelog-strategy.md)
- [ADR-0027: Release Artifacts](../adr/0027-release-artifacts.md)

### Tasks

- [ ] Create `GitVersion.yml` configuration
- [ ] Create `cliff.toml` for git-cliff
- [ ] Create `.config/dotnet-tools.json` with local tools:
  - [ ] dotnet-gitversion
  - [ ] dotnet-stryker
  - [ ] dotnet-reportgenerator
- [ ] Create `stryker-config.json`
- [ ] Create `docs/playbooks/mutation-testing.md`
- [ ] Create `.github/workflows/release.yml`:
  - [ ] Version calculation
  - [ ] Multi-platform builds (win-x64, linux-x64, osx-x64, osx-arm64)
  - [ ] SBOM generation
  - [ ] Checksum generation
  - [ ] GitHub Release creation
  - [ ] NuGet publish
  - [ ] Changelog PR (auto-approved)
- [ ] Create initial `CHANGELOG.md`

### Verification

```bash
dotnet tool restore
dotnet gitversion
git cliff --unreleased
```

### Acceptance Criteria

- [ ] Version calculation works
- [ ] Changelog generation works
- [ ] Release workflow configured
- [ ] Mutation testing playbook ready

---

### Sub-Issue #11: Developer Environment

**Branch:** `feature/11-developer-environment`
**ADRs:** 0028 (Documentation Versioning), 0029 (Developer Environment)
**Depends on:** #10

### Context

Create developer onboarding experience and documentation site.

- [ADR-0028: Documentation Versioning](../adr/0028-documentation-versioning.md)
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
  - [ ] Verify build
- [ ] Initialize Docusaurus in `docs/docusaurus/`
- [ ] Configure GitHub Pages deployment
- [ ] Create `.github/workflows/docs.yml`
- [ ] Create initial documentation pages

### Verification

- Test dev container in VS Code/Codespaces
- Run bootstrap scripts
- Build documentation site

### Acceptance Criteria

- [ ] Dev container works
- [ ] Bootstrap scripts work cross-platform
- [ ] Documentation site builds
- [ ] Onboarding path documented

---

## Phase 6: Infrastructure Verification

### Sub-Issue #12: Prove Workflows & Standards

**Branch:** `feature/12-infrastructure-verification`
**ADRs:** All (0001-0029)
**Depends on:** #11

### Context

Before migrating any code, verify all infrastructure, workflows, and standards are functional. This phase proves the system works end-to-end by executing a complete contribution workflow with a test change.

### Tasks

#### Documentation Verification

- [ ] Audit all documentation for placeholder content:
  - [ ] `docs/standards/coding-standards.md` - complete and actionable
  - [ ] `docs/practices/code-review.md` - complete and actionable
  - [ ] `AGENTS.md` - provides clear routing
  - [ ] All documents have proper front-matter
- [ ] Verify all cross-references and links work
- [ ] Ensure no "TODO" or "TBD" markers remain

#### Workflow Verification (Dummy PR)

- [ ] Create test issue: "Test: Infrastructure Verification"
- [ ] Create branch: `feature/{issue#}-infra-test`
- [ ] Make trivial change (add comment to a config file)
- [ ] Commit with conventional message
- [ ] Verify pre-commit hooks execute:
  - [ ] commitlint validates message
  - [ ] prettier/format checks run
  - [ ] secretlint scans content
  - [ ] cspell checks spelling
- [ ] Push branch
- [ ] Verify pre-push hook validates branch name
- [ ] Create PR
- [ ] Verify CI pipeline runs:
  - [ ] lint job passes
  - [ ] build job passes (all platforms)
  - [ ] test jobs pass (unit, system, arch)
  - [ ] codeql job passes
  - [ ] pr-title job validates title
- [ ] Verify PR requirements enforced:
  - [ ] Review required
  - [ ] Status checks required
  - [ ] Conversation resolution required
- [ ] Complete review and merge PR
- [ ] Verify branch auto-deleted
- [ ] Verify issue auto-closed (via `Closes #X`)

#### Security Verification

- [ ] Run secret scan on repository: `npx secretlint .`
- [ ] Verify Dependabot is configured and has run
- [ ] Verify CodeQL workflow is active

#### Tooling Verification

- [ ] Verify dev container launches and builds project
- [ ] Run bootstrap script on clean environment
- [ ] Verify documentation site builds: `npm run build` (in docs/)

#### Agent Verification

Verify that AI agents can follow project processes using only repository documentation (no external skills). Test against all target platforms per ADR-0009.

**Target Platforms:**
- Claude Code (Anthropic) - CLI/terminal
- Codex/Copilot (OpenAI) - VS Code integration
- Junie (JetBrains) - Rider integration

**Test Setup (per platform):**
- Fresh agent session with no pre-loaded skills/context
- Agent has access only to repository files
- Task: "Add a code comment to explain the purpose of ExitCodes.cs"

**Onboarding Verification (all platforms):**
- [ ] Claude Code: reads `AGENTS.md` as first action
- [ ] Codex: reads `AGENTS.md` as first action
- [ ] Junie: reads `AGENTS.md` as first action
- [ ] All agents navigate to relevant documentation via AGENTS.md routing
- [ ] All agents find and read `docs/standards/coding-standards.md`
- [ ] All agents find and read ADR-0004 (Contribution Workflow)

**Process Adherence Verification (all platforms):**
- [ ] Agent creates/identifies appropriate issue (or references existing)
- [ ] Agent creates correctly named branch (`docs/{issue#}-description`)
- [ ] Agent makes change following coding standards
- [ ] Agent commits with valid conventional commit message
- [ ] Agent includes issue reference in commit (`Refs: #X`)

**Output Compliance Verification:**
- [ ] Commit message passes commitlint validation
- [ ] Code change follows documented standards
- [ ] PR description follows template (if PR created)
- [ ] No reliance on external skills or undocumented conventions

**Platform-Specific Notes:**
- [ ] Document any platform-specific behaviour differences
- [ ] Document any documentation that one platform finds unclear
- [ ] Verify sub-agent/persona pattern works (Claude Code)
- [ ] Verify fallback strategies work (Codex, Junie)

**Failure Scenarios to Test:**
- [ ] Agent without AGENTS.md access fails gracefully (requests guidance)
- [ ] Agent presented with ambiguous task asks clarifying questions
- [ ] Agent recognises when documentation is insufficient and flags it

**Documentation Gaps Identified:**
- [ ] Record any points where agent needed guidance not in docs
- [ ] Update documentation to address gaps before proceeding
- [ ] Re-run agent verification on ALL platforms after documentation updates

### Verification

```bash
# All hooks work
echo "test: infra verification" | npx commitlint
npx secretlint .
npx cspell "**/*.md"
npx prettier --check .

# All tooling works
dotnet restore
dotnet build
dotnet test
npm run lint

# Documentation complete
grep -r "placeholder\|TODO\|TBD" docs/ && exit 1 || echo "No placeholders found"
```

### Acceptance Criteria

- [ ] Zero placeholder documentation
- [ ] Pre-commit hooks block invalid commits
- [ ] Pre-push hooks validate branch names
- [ ] CI pipeline runs all required checks
- [ ] PR requirements are enforced
- [ ] Dummy PR successfully merged via full workflow
- [ ] All infrastructure ADRs verified functional
- [ ] Claude Code successfully completes task using only repo documentation
- [ ] Codex successfully completes task using only repo documentation
- [ ] Junie successfully completes task using only repo documentation
- [ ] Sub-agent/persona pattern verified (Claude Code)
- [ ] No documentation gaps identified (or all gaps resolved)
- [ ] Ready for code migration

---

## Phase 7: Code Migration

### Sub-Issue #13: Migrate Code Bundle

**Branch:** `feature/13-code-migration`
**ADRs:** All technology ADRs (0012-0020)
**Depends on:** #12 (Infrastructure verified)

### Context

With all governance, tooling, and infrastructure in place, migrate the original code bundle.

### Tasks

- [ ] Move original files to new structure:
  - [ ] `ClaudeAutoResume.csproj` → Delete (replaced by new project)
  - [ ] `Program.cs` → Merge into new Program.cs
  - [ ] `ClaudeMonitor.cs` → `src/McjCoderOrg.ClaudeAutoResume/`
  - [ ] `WrapperConfig.cs` → `src/McjCoderOrg.ClaudeAutoResume/`
- [ ] Update namespaces to `McjCoderOrg.ClaudeAutoResume`
- [ ] Apply code formatting (`dotnet format`)
- [ ] Fix all analyzer violations
- [ ] Update logging to use Serilog with named parameters
- [ ] Update strings to use resource files
- [ ] Implement layered configuration
- [ ] Write unit tests for existing functionality:
  - [ ] `WrapperConfigTests.cs`
  - [ ] `ClaudeMonitorTests.cs`
- [ ] Write BDD features for core behavior:
  - [ ] Rate limit detection
  - [ ] Auto-resume functionality
- [ ] Write architecture tests:
  - [ ] Dependency rules
  - [ ] Naming conventions
- [ ] Create initial benchmarks

### Verification

```bash
dotnet build -warnaserror
dotnet test
dotnet format --verify-no-changes
npx secretlint .
```

### Acceptance Criteria

- [ ] All code migrated and compiles
- [ ] Zero analyzer warnings
- [ ] All tests pass
- [ ] Code follows all established standards
- [ ] Original functionality preserved

---

## Phase 8: Final Verification

### Sub-Issue #14: Release Preparation

**Branch:** `feature/14-release-preparation`
**ADRs:** All
**Depends on:** #13

### Context

Verify all ADRs are implemented and prepare for initial release.

### Tasks

- [ ] Update all ADRs from "Proposed" to "Accepted"
- [ ] Run full verification suite:
  ```bash
  dotnet restore
  dotnet build -c Release -warnaserror
  dotnet test -c Release
  dotnet format --verify-no-changes
  npm run lint
  npx secretlint .
  dotnet list package --vulnerable
  ```
- [ ] Run mutation testing and review score
- [ ] Run benchmarks and establish baselines
- [ ] Generate CHANGELOG.md from commits
- [ ] Update README.md with:
  - [ ] Installation instructions
  - [ ] Quick start guide
  - [ ] Contributing link
  - [ ] Badge for CI status
- [ ] Create release checklist in `docs/playbooks/release.md`
- [ ] Document manual GitHub configuration:
  - [ ] Branch protection rules
  - [ ] Secrets (MACHINE_USER_PAT, NUGET_API_KEY)
  - [ ] Environment configuration
- [ ] Create PR to merge epic branch to main

### Verification

- All CI checks pass
- Manual review of all components
- Documentation complete

### Acceptance Criteria

- [ ] All 29 ADRs marked as Accepted
- [ ] All tests pass
- [ ] All quality gates pass
- [ ] No security vulnerabilities
- [ ] Documentation complete
- [ ] Ready for v1.0.0 release

---

## Dependency Graph

```
Phase 1: Foundational
#2 Repository Foundation
    │
    ▼
Phase 2: Compliance
#3 Security & Compliance
    │
    ▼
Phase 3: Process
#4 Documentation Foundation
    │
    ▼
#5 Quality Gates & Hooks
    │
    ▼
Phase 4: Technology
#6 Solution Structure ─────┬─────────────────┐
    │                      │                 │
    ▼                      ▼                 │
#7 Testing Infrastructure  #8 App Framework │
    │                      │                 │
    └──────────┬───────────┘                 │
               ▼                             │
Phase 5: CI/CD                               │
#9 CI/CD Pipeline                            │
    │                                        │
    ▼                                        │
#10 Versioning & Release                     │
    │                                        │
    ▼                                        │
#11 Developer Environment                    │
    │                                        │
    ▼                                        │
Phase 6: Verification Gate                   │
#12 Prove Workflows & Standards              │
    │                                        │
    ▼                                        │
Phase 7: Migration                           │
#13 Migrate Code Bundle ◄────────────────────┘
    │
    ▼
Phase 8: Release
#14 Release Preparation
```

---

## Key Principles

1. **Governance Before Code**: Phases 1-3 establish all rules before any .NET code exists

2. **Infrastructure Before Migration**: Phases 4-5 create the target environment before migration

3. **Verify Before Migrating**: Phase 6 proves all workflows work via a real PR before any code migration

4. **Migration is Validation**: Phase 7 validates all setup by applying it to real code

5. **No Placeholders**: All documentation must be complete and actionable before Phase 6

6. **Sequential Dependencies**: Each phase builds on the previous; no skipping ahead

7. **ADR Alignment**: Each sub-issue maps to specific ADR tiers

---

## Notes for Implementers

1. **Immutable Context Links**: Use commit SHA links in sub-issues for stable context

2. **ADR Updates**: Mark ADRs as "Accepted" only when their sub-issue is merged

3. **Branch Strategy**: Each sub-issue gets its own branch from main

4. **PR Workflow**: Squash merge with conventional commit message

5. **No Code Until Phase 7**: The code bundle stays untouched until Phase 7

6. **Verification Gate**: Phase 6 must pass completely before code migration begins
