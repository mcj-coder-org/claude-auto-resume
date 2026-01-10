# Security & Compliance Baseline Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Establish security scanning and compliance standards before any application code enters the repository.

**Architecture:** Multi-layered security approach with pre-commit secret scanning (secretlint), push-time protection (GitHub Secret Scanning), SAST (CodeQL), and dependency vulnerability scanning (Dependabot). Privacy-first policy with no telemetry. WCAG 2.1 AA accessibility standards for documentation.

**Tech Stack:** secretlint (npm), GitHub CodeQL, Dependabot, Markdown documentation

**Issue:** #4 - Phase 2: Security & Compliance Baseline
**Branch:** `feature/4-security-compliance`
**ADRs:** 0005 (Security Scanning), 0006 (Accessibility), 0007 (Telemetry)

---

## Task 1: Create Security Policy

**Files:**

- Create: `.github/SECURITY.md`

**Step 1: Create the security policy file**

```markdown
# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security issue, please report it responsibly.

### How to Report

1. **Do NOT** create a public GitHub issue for security vulnerabilities
2. Use [GitHub Security Advisories](https://github.com/mcj-coder-org/claude-auto-resume/security/advisories/new) to report privately
3. Include as much detail as possible:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

### What to Expect

- **Acknowledgement:** Within 48 hours of your report
- **Initial Assessment:** Within 7 days
- **Resolution Timeline:** Depends on severity
  - Critical: 7 days
  - High: 14 days
  - Medium: 30 days
  - Low: 90 days

### Disclosure Policy

- We follow [coordinated disclosure](https://en.wikipedia.org/wiki/Coordinated_vulnerability_disclosure)
- We will credit reporters in release notes (unless anonymity is requested)
- We request 90 days before public disclosure to allow time for a fix

## Security Measures

This project implements multiple layers of security:

- **Pre-commit:** Secret scanning with secretlint
- **Push protection:** GitHub Secret Scanning
- **SAST:** GitHub CodeQL analysis
- **Dependencies:** Dependabot alerts and updates
- **CI:** Vulnerability scanning in pull requests

## Scope

The following are in scope for security reports:

- The `claude-auto-resume` CLI tool
- Build and release pipelines
- Documentation website

The following are out of scope:

- Third-party dependencies (report to maintainers directly)
- Social engineering attacks
- Denial of service attacks
```

**Step 2: Verify the file exists and has correct content**

Run: `head -20 .github/SECURITY.md`
Expected: Shows "# Security Policy" header and version table

**Step 3: Commit the security policy**

```bash
git add .github/SECURITY.md
git commit -m "docs: add security vulnerability reporting policy

Establishes security policy with:
- Supported versions table
- Private reporting via GitHub Security Advisories
- Response timeline expectations (48h ack, 7-90 day resolution)
- Coordinated disclosure policy
- Security measures overview

Refs: #4"
```

---

## Task 2: Create Dependabot Configuration

**Files:**

- Create: `.github/dependabot.yml`

**Step 1: Create the Dependabot configuration file**

```yaml
# Dependabot configuration
# https://docs.github.com/en/code-security/dependabot/dependabot-version-updates/configuration-options-for-the-dependabot.yml-file

version: 2
updates:
  # NuGet dependencies (.NET)
  - package-ecosystem: 'nuget'
    directory: '/'
    schedule:
      interval: 'weekly'
      day: 'monday'
      time: '06:00'
      timezone: 'UTC'
    open-pull-requests-limit: 10
    commit-message:
      prefix: 'deps'
      include: 'scope'
    groups:
      # Group all patch updates together
      nuget-patch:
        applies-to: version-updates
        update-types:
          - 'patch'
      # Group Microsoft packages
      microsoft:
        applies-to: version-updates
        patterns:
          - 'Microsoft.*'
          - 'System.*'
      # Group test packages
      testing:
        applies-to: version-updates
        patterns:
          - 'xunit*'
          - 'Moq*'
          - 'coverlet*'
          - 'AwesomeAssertions'
          - 'NetArchTest*'
          - 'BenchmarkDotNet*'
          - 'Reqnroll*'
      # Group analyzers
      analyzers:
        applies-to: version-updates
        patterns:
          - '*Analyzer*'
          - 'Roslynator*'
          - 'Meziantou*'
          - 'SonarAnalyzer*'
    labels:
      - 'dependencies'
      - 'nuget'

  # npm dependencies (tooling: prettier, husky, secretlint, etc.)
  - package-ecosystem: 'npm'
    directory: '/'
    schedule:
      interval: 'weekly'
      day: 'monday'
      time: '06:00'
      timezone: 'UTC'
    open-pull-requests-limit: 5
    commit-message:
      prefix: 'deps'
      include: 'scope'
    groups:
      # Group all npm patch updates
      npm-patch:
        applies-to: version-updates
        update-types:
          - 'patch'
      # Group linting tools
      linting:
        applies-to: version-updates
        patterns:
          - 'prettier'
          - 'eslint*'
          - 'markdownlint*'
          - 'cspell'
      # Group git hooks
      git-hooks:
        applies-to: version-updates
        patterns:
          - 'husky'
          - 'lint-staged'
          - '@commitlint/*'
      # Group security scanning
      security:
        applies-to: version-updates
        patterns:
          - 'secretlint'
          - '@secretlint/*'
    labels:
      - 'dependencies'
      - 'npm'

  # GitHub Actions
  - package-ecosystem: 'github-actions'
    directory: '/'
    schedule:
      interval: 'weekly'
      day: 'monday'
      time: '06:00'
      timezone: 'UTC'
    open-pull-requests-limit: 5
    commit-message:
      prefix: 'ci'
      include: 'scope'
    groups:
      github-actions:
        applies-to: version-updates
        patterns:
          - '*'
    labels:
      - 'dependencies'
      - 'github-actions'
```

**Step 2: Validate YAML syntax**

Run: `python -c "import yaml; yaml.safe_load(open('.github/dependabot.yml'))"`
Expected: No output (valid YAML)

Alternative if Python not available:
Run: `cat .github/dependabot.yml | head -30`
Expected: Shows valid YAML structure

**Step 3: Commit Dependabot configuration**

```bash
git add .github/dependabot.yml
git commit -m "ci: configure Dependabot for dependency updates

Configures automated dependency updates for:
- NuGet packages (weekly, Monday 06:00 UTC)
- npm packages (weekly, Monday 06:00 UTC)
- GitHub Actions (weekly, Monday 06:00 UTC)

Features:
- Grouped updates to reduce PR noise
- Semantic commit prefixes (deps:, ci:)
- Separate groups for testing, analyzers, security tools
- Labels for easy filtering

Refs: #4"
```

---

## Task 3: Create secretlint Configuration

**Files:**

- Create: `.secretlintrc.json`

**Step 1: Create the secretlint configuration file**

```json
{
  "rules": [
    {
      "id": "@secretlint/secretlint-rule-preset-recommend"
    },
    {
      "id": "@secretlint/secretlint-rule-aws"
    },
    {
      "id": "@secretlint/secretlint-rule-gcp"
    },
    {
      "id": "@secretlint/secretlint-rule-npm"
    },
    {
      "id": "@secretlint/secretlint-rule-privatekey"
    }
  ]
}
```

**Step 2: Validate JSON syntax**

Run: `python -c "import json; json.load(open('.secretlintrc.json'))"`
Expected: No output (valid JSON)

Alternative:
Run: `cat .secretlintrc.json`
Expected: Shows valid JSON structure

**Step 3: Commit secretlint configuration**

```bash
git add .secretlintrc.json
git commit -m "build: add secretlint configuration for secret scanning

Configures pre-commit secret detection with rules for:
- Recommended preset (generic secrets, tokens)
- AWS credentials and keys
- GCP service account keys
- npm tokens
- Private keys (RSA, DSA, EC, etc.)

This enables the pre-commit hook (Phase 3) to scan for secrets.

Refs: #4"
```

---

## Task 4: Create package.json with secretlint Dependencies

**Files:**

- Create: `package.json`

**Step 1: Create package.json with secretlint plugins**

```json
{
  "name": "mcjcoderorg-claudeautoresume-tooling",
  "version": "0.0.0",
  "private": true,
  "description": "Development tooling for McjCoderOrg.ClaudeAutoResume",
  "scripts": {
    "secretlint": "secretlint \"**/*\"",
    "secretlint:fix": "secretlint \"**/*\" --fix"
  },
  "devDependencies": {
    "secretlint": "^8.0.0",
    "@secretlint/secretlint-rule-preset-recommend": "^8.0.0",
    "@secretlint/secretlint-rule-aws": "^8.0.0",
    "@secretlint/secretlint-rule-gcp": "^8.0.0",
    "@secretlint/secretlint-rule-npm": "^8.0.0",
    "@secretlint/secretlint-rule-privatekey": "^8.0.0"
  },
  "engines": {
    "node": ">=22.0.0"
  }
}
```

**Step 2: Validate JSON syntax**

Run: `python -c "import json; json.load(open('package.json'))"`
Expected: No output (valid JSON)

**Step 3: Install dependencies and verify secretlint works**

Run: `npm install`
Expected: Dependencies installed successfully

Run: `npx secretlint --version`
Expected: Shows secretlint version (8.x.x)

**Step 4: Run secretlint to verify configuration**

Run: `npx secretlint "**/*" --no-terminalLink`
Expected: No secrets found (or shows scan completed)

**Step 5: Commit package.json and lock file**

```bash
git add package.json package-lock.json
git commit -m "build: add secretlint dependencies for secret scanning

Installs secretlint and plugins:
- @secretlint/secretlint-rule-preset-recommend
- @secretlint/secretlint-rule-aws
- @secretlint/secretlint-rule-gcp
- @secretlint/secretlint-rule-npm
- @secretlint/secretlint-rule-privatekey

Adds npm scripts:
- secretlint: scan all files
- secretlint:fix: auto-fix where possible

Refs: #4"
```

---

## Task 5: Create CodeQL Workflow

**Files:**

- Create: `.github/workflows/codeql.yml`

**Step 1: Create the CodeQL workflow file**

```yaml
name: 'CodeQL'

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  schedule:
    # Run weekly on Sundays at 00:00 UTC
    - cron: '0 0 * * 0'

jobs:
  analyze:
    name: Analyze
    runs-on: ubuntu-latest
    permissions:
      actions: read
      contents: read
      security-events: write

    strategy:
      fail-fast: false
      matrix:
        language: ['csharp']

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Initialize CodeQL
        uses: github/codeql-action/init@v3
        with:
          languages: ${{ matrix.language }}
          # Use default queries plus security-extended
          queries: security-extended

      # For C#, CodeQL needs to see the build
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Perform CodeQL Analysis
        uses: github/codeql-action/analyze@v3
        with:
          category: '/language:${{ matrix.language }}'
```

**Step 2: Validate YAML syntax**

Run: `python -c "import yaml; yaml.safe_load(open('.github/workflows/codeql.yml'))"`
Expected: No output (valid YAML)

**Step 3: Commit CodeQL workflow**

```bash
git add .github/workflows/codeql.yml
git commit -m "ci: add CodeQL workflow for SAST scanning

Configures GitHub CodeQL for static analysis:
- Triggers: push to main, PRs, weekly schedule
- Language: C# (will auto-detect when code exists)
- Queries: security-extended for comprehensive coverage
- Builds project to enable accurate analysis

Refs: #4"
```

---

## Task 6: Create Accessibility Standards Document

**Files:**

- Create: `docs/standards/accessibility.md`

**Step 1: Create the docs/standards directory**

Run: `mkdir -p docs/standards`
Expected: Directory created (or already exists)

**Step 2: Create the accessibility standards document**

```markdown
---
title: Accessibility Standards
summary: WCAG 2.1 AA compliance requirements for documentation and CLI output
audience: [developer, agent]
topics: [accessibility, wcag, documentation, cli]
prerequisites: []
related: [docs/adr/0006-accessibility.md]
last_validated: 2026-01-09
---

# Accessibility Standards

This document defines accessibility requirements for the McjCoderOrg.ClaudeAutoResume project.

## Compliance Target

**WCAG 2.1 Level AA** for all documentation and user-facing output.

## Documentation Website

The documentation website (Docusaurus) must meet these requirements:

### Perceivable

1. **Text Alternatives (1.1.1)**
   - All images must have meaningful `alt` text
   - Decorative images use `alt=""`
   - Complex diagrams include text descriptions

2. **Captions (1.2.2)**
   - Videos include captions
   - Audio content includes transcripts

3. **Adaptable (1.3.x)**
   - Use semantic HTML (`<nav>`, `<main>`, `<article>`, `<aside>`)
   - Headings follow logical hierarchy (h1 > h2 > h3)
   - Lists use proper `<ul>`, `<ol>`, `<dl>` elements
   - Tables include headers and scope attributes

4. **Distinguishable (1.4.x)**
   - Minimum contrast ratio: 4.5:1 for normal text, 3:1 for large text
   - Text resizable to 200% without loss of functionality
   - No information conveyed by colour alone
   - Focus indicators visible

### Operable

1. **Keyboard Accessible (2.1.x)**
   - All functionality available via keyboard
   - No keyboard traps
   - Skip navigation link provided

2. **Enough Time (2.2.x)**
   - No time limits on reading content
   - Auto-updating content can be paused

3. **Seizures (2.3.x)**
   - No flashing content more than 3 times per second

4. **Navigable (2.4.x)**
   - Clear page titles
   - Logical focus order
   - Descriptive link text (no "click here")
   - Multiple navigation methods (menu, search, sitemap)
   - Visible focus indicators

### Understandable

1. **Readable (3.1.x)**
   - Page language declared (`lang="en-GB"`)
   - Abbreviations explained on first use

2. **Predictable (3.2.x)**
   - Consistent navigation across pages
   - Consistent component identification

3. **Input Assistance (3.3.x)**
   - Error messages are descriptive
   - Labels provided for form inputs

### Robust

1. **Compatible (4.1.x)**
   - Valid HTML
   - ARIA attributes used correctly
   - Custom components have appropriate roles

## CLI Output

Command-line interface output must be accessible:

### Requirements

1. **No Colour-Only Information**
   - Status must not rely solely on colour
   - Use text indicators alongside colour (e.g., `[OK]`, `[FAIL]`)

2. **Clear Formatting**
   - Consistent indentation
   - Logical grouping of information
   - Machine-parseable output option (e.g., `--json`)

3. **Exit Codes**
   - Semantic exit codes for automation
   - Error messages include actionable guidance

4. **Screen Reader Compatibility**
   - Avoid excessive use of special characters
   - Progress indicators work with screen readers

### Example: Good CLI Output
```

Checking environment...
.NET Runtime: [OK] 10.0.0
Claude CLI: [OK] Found at /usr/local/bin/claude
Configuration: [OK] Valid

Ready to start.

```

### Example: Bad CLI Output

```

Checking environment...
.NET Runtime: ✓
Claude CLI: ✓
Configuration: ✓

Ready!

````

(Bad: relies on colour and symbols that may not render correctly)

## Validation

### Automated Testing

Run these checks in CI:

```bash
# Lighthouse accessibility audit (documentation site)
npx lighthouse https://your-docs-site.com --only-categories=accessibility --output=json

# axe-core for automated testing
npx @axe-core/cli https://your-docs-site.com
````

### Manual Testing

Perform quarterly:

1. **Keyboard-only navigation** - Navigate entire site without mouse
2. **Screen reader testing** - Test with NVDA, VoiceOver, or JAWS
3. **High contrast mode** - Verify readability in Windows High Contrast
4. **Zoom testing** - Verify usability at 200% zoom

## Resources

- [WCAG 2.1 Quick Reference](https://www.w3.org/WAI/WCAG21/quickref/)
- [Docusaurus Accessibility](https://docusaurus.io/docs/accessibility)
- [axe-core](https://github.com/dequelabs/axe-core)
- [ADR-0006: Accessibility](../adr/0006-accessibility.md)

````

**Step 3: Verify the file was created**

Run: `head -20 docs/standards/accessibility.md`
Expected: Shows front-matter and title

**Step 4: Commit accessibility standards**

```bash
git add docs/standards/accessibility.md
git commit -m "docs: add WCAG 2.1 AA accessibility standards

Documents accessibility requirements for:
- Documentation website (Docusaurus)
- CLI output

Covers WCAG 2.1 AA criteria:
- Perceivable: alt text, semantic HTML, contrast
- Operable: keyboard access, navigation
- Understandable: clear language, consistent UI
- Robust: valid HTML, ARIA

Includes validation approach with Lighthouse and axe-core.

Refs: #4"
````

---

## Task 7: Create Privacy Policy Document

**Files:**

- Create: `docs/standards/privacy.md`

**Step 1: Create the privacy policy document**

```markdown
---
title: Privacy Policy
summary: No telemetry policy and privacy-first principles for the project
audience: [developer, agent, user]
topics: [privacy, telemetry, data-collection]
prerequisites: []
related: [docs/adr/0007-telemetry.md]
last_validated: 2026-01-09
---

# Privacy Policy

This document defines the privacy policy for the McjCoderOrg.ClaudeAutoResume project.

## Core Principle

**This tool collects no telemetry, analytics, or usage data.**

We believe in privacy-first software. Your usage of this tool is entirely private.

## What We Do NOT Collect

- Usage statistics
- Error reports or crash data
- Feature usage metrics
- Session information
- IP addresses
- Any personally identifiable information (PII)
- Any anonymous or pseudonymous identifiers

## What Stays on Your Machine

All data remains local:

- Configuration files (`~/.config/claude-auto-resume/`)
- Log files (when `--verbose` is used)
- Session state

## Network Connections

This tool only makes network connections to:

1. **Claude CLI** - The tool wraps the Claude CLI, which has its own privacy policy managed by Anthropic
2. **No other connections** - The wrapper itself makes no network requests

## Third-Party Services

When you use this tool:

- **Claude CLI**: Subject to [Anthropic's Privacy Policy](https://www.anthropic.com/privacy)
- **GitHub** (for updates): Only if you choose to check for updates manually

## Diagnostics

The `--diagnose` command outputs environment information:

- .NET version
- Operating system
- Configuration validity
- Claude CLI location

This information is:

- Displayed locally only
- Never transmitted anywhere
- Intended for you to include in bug reports (your choice)

## Bug Reports

If you choose to submit a bug report:

1. You decide what information to include
2. We recommend using `--diagnose` output
3. Submissions go to public GitHub Issues (your choice to submit)

## Updates to This Policy

- Changes will be documented in the CHANGELOG
- Major changes will be announced in release notes
- This policy is versioned with the software

## Your Rights

You have complete control:

- **Access**: All your data is on your machine
- **Deletion**: Delete config/log directories anytime
- **Portability**: Files are plain text, easily transferable

## Contact

Questions about privacy:

- Open a [GitHub Discussion](https://github.com/mcj-coder-org/claude-auto-resume/discussions)
- Review our [source code](https://github.com/mcj-coder-org/claude-auto-resume) - it's open source

## References

- [ADR-0007: Telemetry](../adr/0007-telemetry.md) - Decision record for no telemetry
- [Anthropic Privacy Policy](https://www.anthropic.com/privacy) - Claude CLI privacy
```

**Step 2: Verify the file was created**

Run: `head -20 docs/standards/privacy.md`
Expected: Shows front-matter and title

**Step 3: Commit privacy policy**

```bash
git add docs/standards/privacy.md
git commit -m "docs: add privacy policy documenting no telemetry

Establishes privacy-first policy:
- No telemetry, analytics, or usage data collection
- All data stays local on user's machine
- Only network connection is to Claude CLI (Anthropic)
- Diagnostics command outputs locally only
- User controls all data

Refs: #4"
```

---

## Task 8: Final Verification and Push

**Files:**

- All files from previous tasks

**Step 1: Verify all files exist**

Run: `ls -la .github/SECURITY.md .github/dependabot.yml .github/workflows/codeql.yml .secretlintrc.json package.json docs/standards/accessibility.md docs/standards/privacy.md`
Expected: All 7 files listed

**Step 2: Run secretlint to verify no secrets in new files**

Run: `npx secretlint "**/*" --no-terminalLink`
Expected: No secrets detected

**Step 3: Verify git log shows all commits**

Run: `git log --oneline -10`
Expected: Shows 7 commits for this feature

**Step 4: Push branch to remote**

```bash
git push -u origin feature/4-security-compliance
```

**Step 5: Create pull request**

```bash
gh pr create --title "feat: establish security and compliance baseline (#4)" --body "$(cat <<'EOF'
## Summary

Establishes security scanning and compliance standards for the repository (Phase 2).

- Add security vulnerability reporting policy (SECURITY.md)
- Configure Dependabot for NuGet, npm, and GitHub Actions
- Configure secretlint for pre-commit secret scanning
- Add CodeQL workflow for SAST
- Document WCAG 2.1 AA accessibility standards
- Document privacy policy (no telemetry)

## Changes

| File | Purpose |
|------|---------|
| `.github/SECURITY.md` | Vulnerability reporting policy |
| `.github/dependabot.yml` | Automated dependency updates |
| `.secretlintrc.json` | Secret scanning rules |
| `package.json` | secretlint dependencies |
| `.github/workflows/codeql.yml` | SAST workflow |
| `docs/standards/accessibility.md` | WCAG 2.1 AA requirements |
| `docs/standards/privacy.md` | No telemetry policy |

## ADRs Implemented

- [ADR-0005: Security Scanning](../adr/0005-security-scanning.md)
- [ADR-0006: Accessibility](../adr/0006-accessibility.md)
- [ADR-0007: Telemetry](../adr/0007-telemetry.md)

## Test Plan

- [ ] Verify SECURITY.md renders correctly on GitHub
- [ ] Verify Dependabot configuration is valid (GitHub will validate on merge)
- [ ] Run `npx secretlint "**/*"` - no secrets detected
- [ ] Verify CodeQL workflow YAML is valid
- [ ] Verify documentation has proper front-matter

Closes #4

---

Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

---

## Verification Checklist

After all tasks complete:

- [ ] `.github/SECURITY.md` exists with vulnerability reporting policy
- [ ] `.github/dependabot.yml` configures NuGet, npm, GitHub Actions updates
- [ ] `.secretlintrc.json` has recommended rules
- [ ] `package.json` has secretlint dependencies
- [ ] `npx secretlint --version` works
- [ ] `.github/workflows/codeql.yml` exists
- [ ] `docs/standards/accessibility.md` documents WCAG 2.1 AA
- [ ] `docs/standards/privacy.md` documents no telemetry policy
- [ ] All commits follow conventional commit format
- [ ] All commits reference issue #4
- [ ] PR created and ready for review

---

## Implementation Notes

Notes captured during implementation:

### Secretlint Configuration Simplification

**Issue:** The original spec listed both `@secretlint/secretlint-rule-preset-recommend` AND individual rules (aws, gcp, npm, privatekey). This causes "Duplicated rule.id" errors because the preset already includes these rules.

**Resolution:** The `.secretlintrc.json` uses only `preset-recommend`. The individual rule packages remain in `package.json` for explicit dependency tracking and potential future per-rule configuration.

**ADR Impact:** ADR-0005 should be updated to reflect that only `preset-recommend` is needed in the configuration file.

### CodeQL Workflow CI Validation

**Issue:** The CodeQL workflow initially failed with "MSB1003: Specify a project or solution file" because no .NET solution existed.

**Resolution:** Added a minimal .NET 10 solution to validate CI workflows are correctly configured. This includes:

- `McjCoderOrg.ClaudeAutoResume.sln` - Solution file
- `src/McjCoderOrg.ClaudeAutoResume/` - Minimal console project
  - `McjCoderOrg.ClaudeAutoResume.csproj` - Project file with .NET tool configuration
  - `Program.cs` - Placeholder entry point

This minimal solution will be expanded in Phase 4 (Solution Structure) with the full project configuration, analyzers, and test projects. The current implementation is sufficient to:

1. Validate CodeQL workflow builds and analyzes C# code
2. Validate the solution structure conventions
3. Provide a foundation for Phase 4

**Note:** This is an amendment to the original plan which intended to defer all .NET code until Phase 4. The minimal solution was added to ensure CI workflow validation in Phase 2.

### Documentation Front-Matter Fixes

**Issue:** PR review identified missing front-matter on SECURITY.md and inconsistent `related` field paths.

**Resolution:**

- Added YAML front-matter to `.github/SECURITY.md` per ADR-0008
- Fixed `related` paths in `accessibility.md` and `privacy.md` (removed `docs/` prefix)
