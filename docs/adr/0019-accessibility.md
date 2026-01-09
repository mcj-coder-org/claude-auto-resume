# ADR-0019: Accessibility

## Status

Proposed

## Date

2026-01-09

## Context

We need accessibility standards for:
1. Documentation website
2. CLI output
3. Error messages

### Requirements

- WCAG compliance for documentation
- Screen reader friendly
- Clear, readable output

## Decision

**WCAG 2.1 AA** compliance for documentation website.

### Documentation Website

- Semantic HTML
- Sufficient color contrast
- Keyboard navigation
- Alt text for images
- Skip navigation links
- ARIA landmarks

### CLI Output

- Clear, concise messages
- Consistent formatting
- No color-only information
- Exit codes for automation

### Validation

- Lighthouse accessibility audits in CI
- axe-core for automated testing

## Consequences

### Positive
- Inclusive documentation
- Better UX for all users
- Legal compliance

### Negative
- Additional design constraints
- Audit overhead

## References

- [WCAG 2.1](https://www.w3.org/WAI/WCAG21/quickref/)
- [Docusaurus Accessibility](https://docusaurus.io/docs/accessibility)
