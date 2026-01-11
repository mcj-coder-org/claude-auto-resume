---
name: pr-validation-automation
description: |
  Automated PR validation using DangerJS to enforce Definition of Done
  checklists, evidence requirements, and design plan archiving.
decision: Use DangerJS for automated PR validation with custom rules
status: proposed
audience: [developer, agent]
topics: [ci, automation, pr, validation, dangerjs]
last_validated: 2026-01-11
---

# ADR-0031: PR Validation Automation

## Status

Proposed

## Context

Manual PR review processes are error-prone and inconsistent. We need automated
validation to ensure:

1. Definition of Done checklists are complete and honest
2. Checked items have evidence links (not just checkmarks)
3. Design plans are updated and archived before merge
4. Scope changes are properly documented

Current challenges:

- PR checklists can be checked without actual completion
- No verification that evidence links exist
- Design plan status not enforced
- Manual review burden for process compliance

## Decision

Implement DangerJS with custom rules for PR validation, integrated into branch
protection and CI pipeline.

### Validation Rules

#### 1. DoD Checklist Validation

```javascript
// Pseudo-code for DangerJS rules
const dodItems = parsePRChecklist(danger.github.pr.body);

for (const item of dodItems) {
  if (item.checked && !item.hasEvidenceLink) {
    fail(`DoD item "${item.text}" is checked but has no evidence link`);
  }
  if (item.checked && !isValidEvidenceLink(item.evidenceLink)) {
    warn(`Evidence link for "${item.text}" may be invalid`);
  }
}
```

#### 2. Evidence Link Requirements

Valid evidence links include:

| Evidence Type | Format                                    | Example                      |
| ------------- | ----------------------------------------- | ---------------------------- |
| PR file link  | `https://github.com/.../pull/X/files#...` | Line-specific code reference |
| CI run link   | `https://github.com/.../actions/runs/...` | Test execution proof         |
| PR comment    | `#issuecomment-...`                       | Discussion or test output    |
| Commit SHA    | `abc123f`                                 | Specific implementation      |

#### 3. Design Plan Validation

- Design plan must exist for non-trivial PRs
- Plan status must be `Approved` or `Implemented`
- DoD in plan must match PR checklist
- Plan must be moved to `docs/plans/archive/` as final commit

#### 4. Scope Change Handling

- Out-of-scope items must be struck through: `~~item~~`
- Strike-through items must have:
  - Link to explanation in plan document
  - Link to approval comment

### Implementation Phases

| Phase | Scope                                                 | Timeline           |
| ----- | ----------------------------------------------------- | ------------------ |
| 1     | Basic checklist validation (checked items need links) | With tooling phase |
| 2     | Evidence link validation (verify links are valid)     | +1 sprint          |
| 3     | Design plan integration (status, archiving)           | +1 sprint          |
| 4     | Full DoD sync (plan ↔ PR checklist matching)          | +1 sprint          |

### Branch Protection Integration

```yaml
# Required status checks
- danger / PR Validation
  - dod-checklist-complete
  - evidence-links-valid
  - design-plan-status
  - plan-archived (for merge to main)
```

### PR Checklist Format

```markdown
## Definition of Done

<!-- Each checked item MUST have an evidence link -->

- [x] Unit tests passing ([CI Run](link-to-ci))
- [x] Code review approved ([Review](link-to-review))
- [ ] Documentation updated
- ~~Integration tests~~ ([Out of scope](link-to-plan#scope-change), [Approved](link-to-comment))
```

### Design Plan Archive Requirement

Before auto-merge, the final commit must:

1. Update design plan status to `Implemented`
2. Move plan from `docs/plans/` to `docs/plans/archive/`
3. Commit message: `docs(plans): archive {plan-name} design plan`

DangerJS validates this is the last commit before merge.

## Consequences

### Positive

- Consistent PR quality across all contributors
- Evidence-based completion verification
- Reduced manual review burden for process compliance
- Clear audit trail for decisions and scope changes
- Design plans properly archived and versioned

### Negative

- Initial setup complexity
- Learning curve for contributors
- Additional CI time for validation
- Stricter process may slow initial PRs

### Neutral

- Requires DangerJS expertise for rule maintenance
- Rules must evolve with process changes

## Alternatives Considered

### GitHub Actions Only

- Pro: Native integration, no additional tooling
- Con: Less flexible for complex validation logic
- Con: Harder to test locally

### Custom Bot

- Pro: Full control
- Con: Significant development effort
- Con: Maintenance burden

### Manual Review Only

- Pro: No tooling required
- Con: Inconsistent enforcement
- Con: Review fatigue

## Implementation Notes

### Dependencies

```json
{
  "devDependencies": {
    "danger": "^12.0.0"
  }
}
```

### Configuration

```javascript
// dangerfile.js
import { danger, fail, warn, message } from 'danger';

// Import custom rules
import { validateDoD } from './danger/dod-rules';
import { validateEvidenceLinks } from './danger/evidence-rules';
import { validateDesignPlan } from './danger/plan-rules';

// Run validations
await validateDoD(danger);
await validateEvidenceLinks(danger);
await validateDesignPlan(danger);
```

### Related Documents

- `docs/practices/definition-of-done.md` - DoD criteria
- `docs/practices/ticket-lifecycle.md` - PR process
- `docs/practices/code-review.md` - Review requirements

## References

- [DangerJS Documentation](https://danger.systems/js/)
- [GitHub Branch Protection](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches)
