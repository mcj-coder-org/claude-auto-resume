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
  .NET Runtime:    [OK] 10.0.0
  Claude CLI:      [OK] Found at /usr/local/bin/claude
  Configuration:   [OK] Valid

Ready to start.
```

### Example: Bad CLI Output

```
Checking environment...
  .NET Runtime:    ✓
  Claude CLI:      ✓
  Configuration:   ✓

Ready!
```

(Bad: relies on colour and symbols that may not render correctly)

## Validation

### Automated Testing

Run these checks in CI:

```bash
# Lighthouse accessibility audit (documentation site)
npx lighthouse https://your-docs-site.com --only-categories=accessibility --output=json

# axe-core for automated testing
npx @axe-core/cli https://your-docs-site.com
```

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
