---
status: Implemented
version: v1
issue: '#94'
---

# ADR Implementation Tracking - Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Enforce that accepted ADRs with `type: implementation` have linked implementation issues via CI validation.

**Architecture:** Node.js validation script parses ADR frontmatter using gray-matter, validates fields, and reports errors. GitHub Actions workflow runs validation on PRs touching ADR files.

**Tech Stack:** Node.js 22+, gray-matter, GitHub Actions

---

## Task 1: Add gray-matter Dependency

**Files:**

- Modify: `package.json`

**Step 1: Install gray-matter**

Run:

```bash
npm install --save-dev gray-matter
```

**Step 2: Verify installation**

Run: `npm ls gray-matter`
Expected: `gray-matter@4.x.x`

**Step 3: Commit**

```bash
git add package.json package-lock.json
git commit -m "build: add gray-matter for adr frontmatter parsing

Refs: #94"
```

---

## Task 2: Create Validation Script with Tests

**Files:**

- Create: `scripts/validate-adr.js`

**Step 1: Create validation script**

```javascript
#!/usr/bin/env node
/**
 * ADR Frontmatter Validation Script
 *
 * Validates that implementation-type ADRs have implementation_issue field.
 *
 * Usage: node scripts/validate-adr.js [directory]
 * Default directory: docs/adr/
 */

import { readFileSync, readdirSync } from 'node:fs';
import { join, basename } from 'node:path';
import matter from 'gray-matter';

const ISSUE_PATTERN = /^#\d+$|^https:\/\/github\.com\/.+\/issues\/\d+$/;

/**
 * Validate a single ADR file
 * @param {string} filePath - Path to ADR file
 * @returns {{file: string, errors: string[], warnings: string[]}}
 */
export function validateAdr(filePath) {
  const fileName = basename(filePath);
  const errors = [];
  const warnings = [];

  // Skip README
  if (fileName === 'README.md' || fileName === 'exclusions.md') {
    return { file: fileName, errors, warnings };
  }

  let content;
  try {
    content = readFileSync(filePath, 'utf-8');
  } catch {
    errors.push(`Could not read file`);
    return { file: fileName, errors, warnings };
  }

  let frontmatter;
  try {
    const parsed = matter(content);
    frontmatter = parsed.data;
  } catch {
    errors.push(`Invalid frontmatter YAML`);
    return { file: fileName, errors, warnings };
  }

  // Check type field
  const adrType = frontmatter.type || 'process';
  if (adrType !== 'process' && adrType !== 'implementation') {
    errors.push(`Invalid type '${adrType}' - must be 'process' or 'implementation'`);
    return { file: fileName, errors, warnings };
  }

  // If implementation type, require implementation_issue
  if (adrType === 'implementation') {
    const issue = frontmatter.implementation_issue;
    if (!issue) {
      errors.push(`type is 'implementation' but implementation_issue is missing`);
    } else if (typeof issue !== 'string') {
      errors.push(`implementation_issue must be a string (got ${typeof issue})`);
    } else if (!ISSUE_PATTERN.test(issue)) {
      errors.push(
        `implementation_issue '${issue}' has invalid format - use '#123' or full GitHub URL`
      );
    }
  }

  return { file: fileName, errors, warnings };
}

/**
 * Validate all ADR files in a directory
 * @param {string} directory - Path to ADR directory
 * @returns {{results: Array, hasErrors: boolean}}
 */
export function validateDirectory(directory) {
  const files = readdirSync(directory).filter((f) => f.endsWith('.md'));
  const results = files.map((f) => validateAdr(join(directory, f)));
  const hasErrors = results.some((r) => r.errors.length > 0);
  return { results, hasErrors };
}

/**
 * Format results for console output
 * @param {Array} results - Validation results
 * @returns {string}
 */
export function formatResults(results) {
  const lines = [];

  for (const result of results) {
    if (result.errors.length === 0 && result.warnings.length === 0) {
      continue;
    }

    for (const error of result.errors) {
      lines.push(`❌ ${result.file}: ${error}`);
    }
    for (const warning of result.warnings) {
      lines.push(`⚠️  ${result.file}: ${warning}`);
    }
  }

  return lines.join('\n');
}

// CLI entry point
if (process.argv[1].endsWith('validate-adr.js')) {
  const directory = process.argv[2] || 'docs/adr';
  const { results, hasErrors } = validateDirectory(directory);
  const output = formatResults(results);

  if (output) {
    console.log(output);
  }

  if (hasErrors) {
    console.log('\n❌ ADR validation failed');
    process.exit(1);
  } else {
    console.log('✅ All ADR files valid');
    process.exit(0);
  }
}
```

**Step 2: Run script to verify it works**

Run: `node scripts/validate-adr.js docs/adr/`
Expected: `✅ All ADR files valid` (all current ADRs default to process type)

**Step 3: Commit**

```bash
git add scripts/validate-adr.js
git commit -m "feat(scripts): add adr frontmatter validation script

Validates that implementation-type ADRs have implementation_issue field.
Defaults to 'process' type for backwards compatibility.

Refs: #94"
```

---

## Task 3: Add npm Script

**Files:**

- Modify: `package.json`

**Step 1: Add lint:adr script**

Add to scripts section in package.json:

```json
"lint:adr": "node scripts/validate-adr.js docs/adr/"
```

**Step 2: Update main lint script to include ADR validation**

Update lint script:

```json
"lint": "npm run lint:format && npm run lint:markdown && npm run lint:spelling && npm run lint:secrets && npm run lint:adr"
```

**Step 3: Run to verify**

Run: `npm run lint:adr`
Expected: `✅ All ADR files valid`

**Step 4: Commit**

```bash
git add package.json
git commit -m "build: add lint:adr npm script

Integrates ADR validation into main lint command.

Refs: #94"
```

---

## Task 4: Create CI Workflow

**Files:**

- Create: `.github/workflows/adr-validation.yml`

**Step 1: Create workflow file**

```yaml
name: ADR Validation

on:
  pull_request:
    paths:
      - 'docs/adr/**/*.md'
  push:
    branches:
      - main
    paths:
      - 'docs/adr/**/*.md'

jobs:
  validate:
    name: Validate ADR Frontmatter
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'

      - name: Install dependencies
        run: npm ci

      - name: Validate ADR files
        run: npm run lint:adr
```

**Step 2: Verify workflow syntax**

Run: `cat .github/workflows/adr-validation.yml | head -5`
Expected: Shows valid YAML header

**Step 3: Commit**

```bash
git add .github/workflows/adr-validation.yml
git commit -m "ci: add adr validation workflow

Runs ADR frontmatter validation on PRs touching docs/adr/.

Refs: #94"
```

---

## Task 5: Update ADR README Documentation

**Files:**

- Modify: `docs/adr/README.md`

**Step 1: Add type and implementation_issue to Required Fields table**

After the existing `status` row in the Required Fields table, add:

```markdown
| `type` | One of: `process`, `implementation` (default: `process`) |
| `implementation_issue` | Issue reference like `#123` (required when type=implementation) |
```

**Step 2: Add Implementation Tracking section**

After the "Field Specifications" section, add:

````markdown
## Implementation Tracking

ADRs that require code, tooling, or infrastructure changes must track their implementation:

1. Set `type: implementation` in frontmatter
2. Create a GitHub issue for the implementation work
3. Add `implementation_issue: '#123'` to frontmatter
4. CI validation fails if implementation ADRs lack issue links

### Type Classification

| Type             | Use For                               | Example ADRs                    |
| ---------------- | ------------------------------------- | ------------------------------- |
| `process`        | Governance, standards, conventions    | Use ADRs, License, Contribution |
| `implementation` | Code, tooling, infrastructure changes | Pre-commit Hooks, CI/CD, CLI    |

### Example Implementation ADR

```yaml
---
name: pr-validation-automation
description: |
  Automated PR validation using DangerJS.
decision: Use DangerJS for automated PR validation
status: accepted
type: implementation
implementation_issue: '#93'
---
```
````

````

**Step 3: Verify markdown linting passes**

Run: `npm run lint:markdown -- docs/adr/README.md`
Expected: No errors

**Step 4: Commit**

```bash
git add docs/adr/README.md
git commit -m "docs: add implementation tracking to adr readme

Documents type and implementation_issue fields for ADR frontmatter.

Refs: #94"
````

---

## Task 6: Migrate Existing ADRs - Process Type

**Files:**

- Modify: All ADRs that are process-type (no code changes needed)

**Step 1: Identify process ADRs**

These ADRs are governance/standards with no implementation needed:

- 0000-use-adrs.md
- 0001-license.md
- 0002-github-platform.md
- 0003-work-item-management.md
- 0004-contribution-workflow.md
- 0006-accessibility.md
- 0008-documentation-strategy.md
- 0009-agent-onboarding.md
- 0012-namespace-and-project-naming.md

**Step 2: Add type: process to each**

For each file, add after the `status:` line:

```yaml
type: process
```

**Step 3: Run validation**

Run: `npm run lint:adr`
Expected: `✅ All ADR files valid`

**Step 4: Commit**

```bash
git add docs/adr/*.md
git commit -m "docs(adr): add type: process to governance adrs

Classifies governance and standards ADRs as process type.

Refs: #94"
```

---

## Task 7: Migrate Existing ADRs - Implementation Type

**Files:**

- Modify: All ADRs that required code/tooling implementation

**Step 1: Identify implementation ADRs and their issues/PRs**

| ADR                                       | Topic             | Implementation Issue/PR |
| ----------------------------------------- | ----------------- | ----------------------- |
| 0005-security-scanning.md                 | Security scanning | #5 or PR that added it  |
| 0007-telemetry.md                         | Telemetry         | Find implementing PR    |
| 0010-code-formatting.md                   | Code formatting   | Find implementing PR    |
| 0011-pre-commit-hooks.md                  | Pre-commit hooks  | #24                     |
| 0013-testing-framework.md                 | Testing framework | Find implementing PR    |
| 0014-test-project-structure.md            | Test structure    | Find implementing PR    |
| 0015-code-analyzers.md                    | Analyzers         | Find implementing PR    |
| 0016-architecture-testing.md              | Arch tests        | Find implementing PR    |
| 0017-observability.md                     | Observability     | Find implementing PR    |
| 0018-cli-design.md                        | CLI               | Find implementing PR    |
| 0019-internationalization.md              | i18n              | Find implementing PR    |
| 0020-breaking-change-detection.md         | Breaking changes  | Find implementing PR    |
| 0021-cicd-pipeline.md                     | CI/CD             | #40                     |
| 0022-versioning-and-changelog-strategy.md | Versioning        | #49                     |
| 0023-dependency-management.md             | Dependencies      | Find implementing PR    |
| 0024-code-coverage.md                     | Coverage          | Find implementing PR    |
| 0025-performance-testing.md               | Perf testing      | Find implementing PR    |
| 0026-mutation-testing.md                  | Mutation testing  | Find implementing PR    |
| 0027-release-artifacts.md                 | Releases          | Find implementing PR    |
| 0028-documentation-versioning.md          | Doc versioning    | Find implementing PR    |
| 0029-developer-environment.md             | Dev env           | #50                     |
| 0030-feature-flags.md                     | Feature flags     | Find implementing PR    |
| 0031-pr-validation-automation.md          | PR validation     | #93                     |

**Step 2: For each implementation ADR, add type and issue**

```yaml
type: implementation
implementation_issue: '#XX'
```

**Step 3: Run validation**

Run: `npm run lint:adr`
Expected: `✅ All ADR files valid`

**Step 4: Commit**

```bash
git add docs/adr/*.md
git commit -m "docs(adr): add type: implementation with issue links

Links implementation ADRs to their tracking issues/PRs.

Refs: #94"
```

---

## Task 8: Create Test Fixtures

**Files:**

- Create: `tests/fixtures/adr/valid-process.md`
- Create: `tests/fixtures/adr/valid-implementation.md`
- Create: `tests/fixtures/adr/invalid-missing-issue.md`
- Create: `tests/fixtures/adr/invalid-bad-format.md`

**Step 1: Create valid process ADR fixture**

```markdown
---
name: test-process
description: Test process ADR
decision: Use process type
status: accepted
type: process
---

# Test Process ADR

This is a test fixture.
```

**Step 2: Create valid implementation ADR fixture**

```markdown
---
name: test-implementation
description: Test implementation ADR
decision: Use implementation type
status: accepted
type: implementation
implementation_issue: '#123'
---

# Test Implementation ADR

This is a test fixture.
```

**Step 3: Create invalid missing issue fixture**

```markdown
---
name: test-missing-issue
description: Test missing issue
decision: Missing issue reference
status: accepted
type: implementation
---

# Test Missing Issue ADR

This should fail validation.
```

**Step 4: Create invalid format fixture**

```markdown
---
name: test-bad-format
description: Test bad format
decision: Bad issue format
status: accepted
type: implementation
implementation_issue: '123'
---

# Test Bad Format ADR

This should fail validation (missing # prefix).
```

**Step 5: Test validation against fixtures**

Run: `node scripts/validate-adr.js tests/fixtures/adr/`
Expected:

```
❌ invalid-missing-issue.md: type is 'implementation' but implementation_issue is missing
❌ invalid-bad-format.md: implementation_issue '123' has invalid format - use '#123' or full GitHub URL

❌ ADR validation failed
```

**Step 6: Commit**

```bash
git add tests/fixtures/adr/
git commit -m "test: add adr validation fixtures

Includes valid and invalid test cases for ADR frontmatter validation.

Refs: #94"
```

---

## Task 9: Final Verification and PR

**Step 1: Run full lint**

Run: `npm run lint`
Expected: All checks pass

**Step 2: Run ADR validation specifically**

Run: `npm run lint:adr`
Expected: `✅ All ADR files valid`

**Step 3: Push branch**

```bash
git push -u origin docs/94-adr-implementation-tracking
```

**Step 4: Create PR**

```bash
gh pr create --title "feat: enforce adr implementation tracking" --body "$(cat <<'EOF'
## Summary
- Add `type` and `implementation_issue` fields to ADR frontmatter schema
- Create validation script (`scripts/validate-adr.js`)
- Add CI workflow for ADR validation
- Update ADR README with new fields documentation
- Migrate all existing ADRs with appropriate types and issue links

## Test Plan
- [x] Validation script passes on valid ADRs
- [x] Validation script fails on invalid ADRs (test fixtures)
- [x] CI workflow triggers on ADR file changes
- [x] All existing ADRs pass validation

Closes #94

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

**Step 5: Enable auto-merge**

```bash
gh pr merge --auto --squash
```

---

## Success Criteria

- [ ] `npm run lint:adr` passes on all existing ADRs
- [ ] CI workflow runs on PRs touching `docs/adr/`
- [ ] Test fixtures demonstrate valid/invalid cases
- [ ] ADR README documents new fields
- [ ] All implementation ADRs have issue links
