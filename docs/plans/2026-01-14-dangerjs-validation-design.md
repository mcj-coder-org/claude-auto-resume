---
status: Approved v1
version: v1
issue: '#93'
created: 2026-01-14
last_updated: 2026-01-14
---

# Design Plan: DangerJS PR and Issue Validation

## Version History

| Version | Date       | Changes        | Discussion | Approved By    | Follow-up Issues |
| ------- | ---------- | -------------- | ---------- | -------------- | ---------------- |
| v1      | 2026-01-14 | Initial design | #93        | @martincjarvis | #94              |

## Approval

**Design Approved By:**

- [x] Tech Lead: @martincjarvis (2026-01-14) [Approval](https://github.com/mcj-coder-org/claude-auto-resume/pull/96#pullrequestreview-2708420428)

**Status:** `Approved v1`

---

## Summary

Comprehensive PR and issue validation system using DangerJS to enforce Definition of Done, evidence requirements, plan file governance, and issue completion before merge. Replaces the minimal Phase 1 implementation with full ADR-0031 compliance.

## Issue Context

**Parent Issue:** #93 - Implement DangerJS PR validation
**Related Issues:** #94 - Process: Enforce ADR implementation tracking
**Related ADRs:** [ADR-0031](../adr/0031-pr-validation-automation.md)

## Architecture Overview

**Two-component system:**

```
┌─────────────────────────────────────────────────────────────┐
│                    PR Validation (DangerJS)                  │
│                                                              │
│  Triggers: pull_request [opened, synchronize, edited]        │
│                                                              │
│  Validates:                                                  │
│  - PR body (Test Plan, evidence links, description)          │
│  - Plan files (status, DoD, approvals, scope changes)        │
│  - Auto-merge configuration                                  │
│  - Squash commit message format                              │
│  - Linked issue completeness (sub-issues, task lists)        │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│               Issue Close Validation (Workflow)              │
│                                                              │
│  Triggers: issues [closed]                                   │
│                                                              │
│  Purpose: Safety net for manual issue closes                 │
│  Validates:                                                  │
│  - All sub-issues closed                                     │
│  - All task lists complete with evidence (markdown links)    │
│  - Reopens issue with comment if validation fails            │
└─────────────────────────────────────────────────────────────┘
```

## Validation Rules

### PR Body Validations

| Rule                                    | Severity | Description                                   |
| --------------------------------------- | -------- | --------------------------------------------- |
| Test Plan unchecked items               | FAIL     | All `- [ ]` items must be checked             |
| Test Plan items missing evidence        | FAIL     | All `- [x]` items need `[text](url)`          |
| Missing Summary/Description section     | FAIL     | PR must have `## Summary` or `## Description` |
| Auto-merge not enabled                  | FAIL     | PR must have auto-merge enabled               |
| Squash message not conventional commits | FAIL     | Title must match `type(scope): description`   |
| Squash message missing issue reference  | FAIL     | Body must contain `Refs: #XX` or `Closes #XX` |
| PR size > 500 lines                     | WARN     | Advisory only                                 |
| No plan file found                      | WARN     | Comment added to PR and linked issue          |

### Plan File Detection

1. Scan `danger.git.modified_files` for `docs/plans/**/*.md` (including `archive/`)
2. Scan all `docs/plans/**/*.md` for `issue: #XX` matching PR's linked issue
3. If no plan found → WARN + comment on PR and linked issue

### Plan File Validations

| Rule                                       | Severity | Description                                                     |
| ------------------------------------------ | -------- | --------------------------------------------------------------- |
| Success Criteria unchecked                 | FAIL     | All DoD items must be checked                                   |
| Success Criteria missing evidence          | FAIL     | All checked items need `[text](url)`                            |
| Approval checkboxes unchecked              | FAIL     | Required approvers must have checked items                      |
| Approval missing links                     | FAIL     | Each approval needs `[text](url)`                               |
| New scope item not in DoD/AC               | FAIL     | Amendments must update Success Criteria and Acceptance Criteria |
| New scope item missing approval link       | FAIL     | New items need `[Approved](url)` reference                      |
| Completed new item missing evidence        | FAIL     | When checked, needs `[Evidence](url)`                           |
| Struck-through item missing approval link  | FAIL     | Descoped items need `[text](url)` to approval                   |
| Version History not updated for amendments | FAIL     | Amendments must add row to version table                        |
| Final PR: status not `Implemented`         | FAIL     | When closing parent issue, status must be `Implemented`         |
| Final PR: plan not in `archive/`           | FAIL     | Completed plans must be moved to archive                        |

### Plan Archive Handling

| Scenario                                  | Validation                                         |
| ----------------------------------------- | -------------------------------------------------- |
| Plan in `docs/plans/` (not archived)      | Standard validations apply                         |
| Plan moved to `archive/` in this PR       | Validate status = `Implemented` + all DoD complete |
| Plan already in `archive/` (not modified) | Skip validation (already completed)                |
| Plan in `archive/` but modified           | Validate changes                                   |

### Linked Issue Validations

When PR contains `Closes #XX`:

| Rule                             | Severity | Description                               |
| -------------------------------- | -------- | ----------------------------------------- |
| Sub-issues not all closed        | FAIL     | All referenced sub-issues must be closed  |
| Task list items unchecked        | FAIL     | All `- [ ]` in issue body must be `- [x]` |
| Task list items missing evidence | FAIL     | All `- [x]` need `[text](url)`            |

### Final Implementation PR Detection

A PR is the "final" implementation PR when:

1. PR uses `Closes #XX` syntax (closes the parent issue), AND
2. All items in plan's Success Criteria section are checked

## Evidence Link Validation

**Single Pattern (Strict):** Only markdown links are valid evidence.

```javascript
const EVIDENCE_LINK = /\[([^\]]+)\]\(([^)]+)\)/;
```

**Invalid patterns:**

| Pattern            | Example                  | Status  |
| ------------------ | ------------------------ | ------- |
| Raw URL            | `https://github.com/...` | INVALID |
| Issue reference    | `#123`                   | INVALID |
| Commit SHA         | `abc1234`                | INVALID |
| Bare URL in parens | `(https://...)`          | INVALID |

**Valid patterns:**

| Pattern                   | Example                  | Status |
| ------------------------- | ------------------------ | ------ |
| Markdown link             | `[CI Run](https://...)`  | VALID  |
| Markdown link with issue  | `[#123](https://...)`    | VALID  |
| Markdown link with commit | `[abc1234](https://...)` | VALID  |

**Evidence Location:** Must be on same line as checklist item.

```markdown
- [x] Task complete [Evidence](url) ✓
- [x] Task complete ([Evidence](url)) ✓
- [x] Task complete - see [Evidence](url) ✓
- [x] Task complete
      [Evidence](url) ✗ (different line)
```

## Amendment Validation

**Properly amended DoD item example:**

```markdown
## Success Criteria

- [x] Original item ([Evidence](url))
- [x] New scope: additional feature ([Approved](url), [Evidence](url)) <!-- v1.1 -->
- ~~Descoped: removed feature~~ ([Approved](url)) <!-- v1.1 -->
```

**Detection approach:**

- Compare plan file diff to identify new checklist items
- New items must have approval link in same line or version comment
- Struck-through items must have approval link

## Auto-Merge Validation

**Auto-Merge Check:**

```javascript
const autoMerge = danger.github.pr.auto_merge;

if (!autoMerge) {
  fail('PR must have auto-merge enabled. Enable via PR settings.');
}
```

**Squash Commit Message Validation:**

| Rule              | Pattern                                                                      | Severity |
| ----------------- | ---------------------------------------------------------------------------- | -------- |
| Type required     | `^(feat\|fix\|docs\|style\|refactor\|perf\|test\|build\|ci\|chore\|revert):` | FAIL     |
| Subject lowercase | First letter after `:` must be lowercase                                     | FAIL     |
| Subject not empty | Must have text after type                                                    | FAIL     |
| Header max length | ≤ 100 characters                                                             | FAIL     |
| Issue reference   | Body contains `Refs: #XX` or `Closes #XX`                                    | FAIL     |

## Issue Close Validation Workflow

**Trigger:** `issues: [closed]`

**Purpose:** Safety net for manual issue closes that bypass PR flow.

**Validation Steps:**

1. Fetch issue body
2. Detect sub-issues (table format `| [#XXX](url) |` or inline `#XXX`)
3. Query GitHub API: are all sub-issues closed?
4. Parse task lists in issue body
5. Validate all checked items have `[text](url)` evidence
6. If any validation fails → reopen issue + add comment

**Reopen Comment Format:**

```markdown
## Issue Close Validation Failed

This issue was reopened because:

### Incomplete Sub-issues

- [ ] #123 - Still open
- [x] #124 - Closed

### Task List Items Missing Evidence

- `- [x] Task without evidence link`

Please complete all items with evidence links before closing.
```

## File Structure

**DangerJS Structure:**

```
dangerfile.js                    # Main entry point
danger/
├── rules/
│   ├── pr-body.js              # Test Plan, description validation
│   ├── plan-file.js            # Plan detection, status, DoD, amendments
│   ├── auto-merge.js           # Auto-merge and commit message
│   ├── linked-issue.js         # Issue completeness validation
│   └── index.js                # Orchestrates all rules
├── lib/
│   ├── evidence.js             # Markdown link detection
│   ├── checklist.js            # Checkbox parsing
│   ├── frontmatter.js          # YAML frontmatter parsing
│   └── sections.js             # Markdown section extraction
└── constants.js                # Patterns, severity levels
```

**GitHub Workflows:**

```
.github/workflows/
├── danger.yml                  # PR validation (update existing)
└── issue-close-validation.yml  # New issue close validation
```

## Implementation Phases

| Phase | Scope                   | Deliverable                                              |
| ----- | ----------------------- | -------------------------------------------------------- |
| 1     | Fix current DangerJS    | Evidence → FAIL, Description → FAIL, markdown-only links |
| 2     | Plan file validation    | Detection, status, DoD, approvals                        |
| 3     | Amendment validation    | Scope changes, struck-through items, version history     |
| 4     | Auto-merge validation   | Enabled check, squash message format                     |
| 5     | Linked issue validation | Sub-issues, task lists in DangerJS                       |
| 6     | Issue close workflow    | Safety net workflow for manual closes                    |
| 7     | Integration testing     | End-to-end validation scenarios                          |

## Success Criteria

**Definition of Done for this feature:**

- [x] All PR body validations implemented and tested ([PR #97](https://github.com/mcj-coder-org/claude-auto-resume/pull/97), [PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [x] Plan file detection working (modified files + frontmatter lookup) ([PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [x] Plan file validations implemented (DoD, approvals, amendments) ([PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [x] Evidence validation strict (markdown links only) ([PR #97](https://github.com/mcj-coder-org/claude-auto-resume/pull/97))
- [x] Auto-merge validation implemented ([PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [x] Linked issue validation implemented in DangerJS ([PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [x] Issue close workflow implemented as safety net ([PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [x] Existing Phase 1 DangerJS updated/replaced ([PR #97](https://github.com/mcj-coder-org/claude-auto-resume/pull/97), [PR #98](https://github.com/mcj-coder-org/claude-auto-resume/pull/98))
- [ ] Integration tests covering all scenarios
- [ ] Documentation updated

## Testing Approach

### Unit Tests

- Evidence link pattern matching
- Checklist parsing (checked/unchecked/struck-through)
- Frontmatter parsing
- Section extraction

### Integration Tests

- PR with complete Test Plan → pass
- PR with unchecked Test Plan items → fail
- PR with checked items missing evidence → fail
- PR with valid plan file → pass
- PR with incomplete plan DoD → fail
- PR closing issue with open sub-issues → fail
- Manual issue close with incomplete tasks → reopen

## Risks and Mitigations

| Risk                                      | Likelihood | Impact | Mitigation                             |
| ----------------------------------------- | ---------- | ------ | -------------------------------------- |
| False positives blocking valid PRs        | Medium     | High   | Thorough testing, clear error messages |
| Complex regex patterns failing edge cases | Medium     | Medium | Comprehensive test coverage            |
| GitHub API rate limits                    | Low        | Medium | Cache API responses, batch queries     |
| Performance on large PRs                  | Low        | Low    | Optimise file scanning                 |

## References

- [ADR-0031: PR Validation Automation](../adr/0031-pr-validation-automation.md)
- [Design Plan Template](templates/design-plan-template.md)
- [DangerJS Documentation](https://danger.systems/js/)
