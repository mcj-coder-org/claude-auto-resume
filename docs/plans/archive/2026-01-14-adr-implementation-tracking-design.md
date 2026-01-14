---
status: Implemented
version: v1
issue: '#94'
---

# ADR Implementation Tracking Design

## Overview

Enforce that accepted ADRs requiring code changes have linked implementation issues. This prevents ADRs from being marked "Accepted" without follow-through on implementation.

## Approach

Add two new optional fields to ADR frontmatter:

- `type`: Distinguishes process ADRs from implementation ADRs
- `implementation_issue`: Links to the tracking issue (required for implementation type)

CI validation ensures implementation-type ADRs have valid issue references.

## Frontmatter Schema

New fields added to the existing ADR frontmatter:

```yaml
---
name: pr-validation-automation
description: |
  Automated PR validation using DangerJS...
decision: Use DangerJS for automated PR validation
status: accepted
type: implementation # NEW: process | implementation
implementation_issue: '#93' # NEW: Required when type=implementation
---
```

### Field Definitions

| Field                  | Values                        | Required                             |
| ---------------------- | ----------------------------- | ------------------------------------ |
| `type`                 | `process` or `implementation` | Optional (defaults to `process`)     |
| `implementation_issue` | `#123` or issue URL           | Required when `type: implementation` |

### Type Meanings

- **process**: Governance/policy decisions requiring no code changes (e.g., "Use ADRs", "License", "Contribution Workflow")
- **implementation**: Requires code, tooling, or infrastructure changes (e.g., "Pre-commit Hooks", "CI/CD Pipeline", "PR Validation")

Backwards compatible: existing ADRs without `type` default to `process`.

## CI Validation

### Workflow

**File:** `.github/workflows/adr-validation.yml`

**Trigger:** On PR when files in `docs/adr/*.md` are modified or created.

### Validation Rules

1. Parse frontmatter from each ADR file
2. Check type field:
   - If `type: implementation` → `implementation_issue` field is required
   - If `type: process` or missing → no issue required
3. Validate issue reference format:
   - Must match pattern `#\d+` or full GitHub issue URL
4. Optionally verify issue exists via GitHub API (warn if not found)
5. Check issue state:
   - If ADR `status: accepted` and issue is closed → warn (may need status update)

### Error Messages

```
❌ ADR-0031: type is 'implementation' but implementation_issue is missing
❌ ADR-0031: implementation_issue '#999' does not exist
⚠️ ADR-0031: implementation_issue '#93' is closed - consider updating ADR status
```

### Implementation

Node.js script using `gray-matter` for frontmatter parsing:

**File:** `scripts/validate-adr.js`

```javascript
// Pseudocode
for each ADR file:
  frontmatter = parse(file)
  if frontmatter.type === 'implementation':
    if !frontmatter.implementation_issue:
      fail("implementation_issue required")
    if !validIssueFormat(frontmatter.implementation_issue):
      fail("invalid issue format")
```

## Existing ADR Migration

### Classification Criteria

| Type             | Criteria                               | Examples                     |
| ---------------- | -------------------------------------- | ---------------------------- |
| `process`        | Governance, standards, conventions     | ADR-0000, ADR-0001, ADR-0004 |
| `implementation` | Requires code, tooling, infrastructure | ADR-0011, ADR-0021, ADR-0031 |

### Migration Steps

1. Review each ADR and assign `type: process` or `type: implementation`
2. For `implementation` ADRs, find the associated issue/PR that implemented it
3. Add `implementation_issue` field with the reference
4. If no issue exists for an implemented ADR, link to the implementing PR

### Estimated Breakdown

- ~15 process ADRs (documentation, standards, conventions)
- ~16 implementation ADRs (tooling, infrastructure, features)

## Documentation Updates

### Updates to `docs/adr/README.md`

1. **Frontmatter Standard section**: Add `type` and `implementation_issue` fields

2. **New section "Implementation Tracking"**:

   ```markdown
   ## Implementation Tracking

   ADRs that require code changes must track their implementation:

   1. Set `type: implementation` in frontmatter
   2. Create a GitHub issue for the implementation work
   3. Add `implementation_issue: '#123'` to frontmatter
   4. CI will fail if implementation ADRs lack issue links
   ```

3. **Creating New ADRs section**: Add step to determine type and create issue

4. **Examples section**: Add example of implementation-type ADR

## Testing Strategy

### Validation Script Tests

| Scenario                                       | Expected Result |
| ---------------------------------------------- | --------------- |
| `type: process` without issue                  | Pass            |
| `type: implementation` with valid issue        | Pass            |
| `type: implementation` without issue           | Fail            |
| Missing `type` field (defaults to process)     | Pass            |
| Invalid issue format (`123` instead of `#123`) | Fail            |
| Issue reference to non-existent issue          | Warn            |

### Test Fixtures

Add test ADR files in `tests/fixtures/adr/` for CI validation testing.

### Local Validation

Add npm script:

```json
{
  "scripts": {
    "lint:adr": "node scripts/validate-adr.js docs/adr/"
  }
}
```

Integrate with existing `npm run lint` command.

## Implementation Phases

1. **Phase 1: Validation Script**
   - Create `scripts/validate-adr.js`
   - Add unit tests
   - Add `lint:adr` npm script

2. **Phase 2: CI Workflow**
   - Create `.github/workflows/adr-validation.yml`
   - Test with fixture files

3. **Phase 3: Documentation**
   - Update `docs/adr/README.md` with new fields
   - Add examples

4. **Phase 4: Migration**
   - Audit all existing ADRs
   - Add `type` and `implementation_issue` fields
   - Verify all ADRs pass validation

## Success Criteria

- [ ] Validation script catches missing implementation_issue on implementation ADRs
- [ ] CI workflow runs on PR and blocks merge if validation fails
- [ ] All existing ADRs classified and migrated
- [ ] Documentation updated with new process
- [ ] Local `npm run lint:adr` command works

## References

- Issue #94: Process: Enforce ADR implementation tracking
- ADR-0031: Example of unimplemented accepted ADR
- Issue #93: DangerJS implementation
