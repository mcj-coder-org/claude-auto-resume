---
title: Verification & Review Pattern
summary: Asynchronous review pattern for specialised validation from specific perspectives
audience: [developer, agent]
topics: [patterns, review, verification, quality]
parent: ../collaboration-patterns.md
last_validated: 2026-01-10
---

# Verification & Review Pattern

Use this pattern when performing specialised validation from a specific perspective.

## Characteristics

| Aspect                | Details                   |
| --------------------- | ------------------------- |
| **Mode**              | Asynchronous review       |
| **Human Role**        | Requestor, final approver |
| **Collaborator Role** | Specialist reviewer       |
| **Autonomy Level**    | Supervised                |

## When to Use

- Code review (general or specialised)
- Security audit
- Accessibility review
- Performance analysis
- Documentation review
- Architecture review

## Collaborator Behaviours

1. **Focus on assigned perspective** - Don't scope creep
2. **Be specific** - Point to exact lines, provide examples
3. **Prioritise findings** - Critical > Major > Minor
4. **Suggest fixes** - Don't just identify, propose solutions
5. **Justify concerns** - Reference standards, explain impact

## Specialist Roles

See `docs/standards/roles.md` for detailed role definitions.

| Role                   | Focus           | Key Concerns                             |
| ---------------------- | --------------- | ---------------------------------------- |
| Security Reviewer      | Vulnerabilities | OWASP top 10, input validation, secrets  |
| Accessibility Auditor  | WCAG compliance | Screen readers, keyboard nav, contrast   |
| Performance Analyst    | Efficiency      | Memory, CPU, async patterns, allocations |
| Architecture Reviewer  | Structure       | Dependencies, coupling, SOLID principles |
| Documentation Reviewer | Clarity         | Accuracy, completeness, examples         |

## Example: Security Review

**Human:** "Review this PR from a security perspective."

**Collaborator should:**

1. Check input validation
2. Look for injection vulnerabilities
3. Verify authentication/authorisation
4. Check for hardcoded secrets
5. Review error handling for information leakage

**Response format:**

```markdown
## Security Review: PR #123

### Critical Issues

None found.

### Major Issues

1. **Potential command injection** (src/ClaudeMonitor.cs:145)
   - User input is passed to `Process.Start()` without sanitisation
   - Recommend: Use argument array instead of string interpolation

### Minor Issues

1. **Verbose error messages** (src/Program.cs:89)
   - Exception stack trace included in user-facing error
   - Recommend: Log full trace, show generic message to user

### Positive Observations

- Proper use of parameterised queries in config loading
- CancellationToken correctly propagated
```

## Outputs

- Review comments (structured)
- Severity classifications
- Specific fix recommendations
- Summary reports

## Documentation to Reference

- Relevant checklists in `docs/checklists/` (when available)
- `docs/standards/accessibility.md` for accessibility reviews
- `docs/standards/privacy.md` for privacy considerations
- `docs/standards/roles.md` for role definitions
