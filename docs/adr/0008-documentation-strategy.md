# ADR-0008: Documentation Strategy

## Status

Proposed

## Date

2026-01-09

## Context

We need a documentation strategy that supports:
1. End-user documentation (installation, usage)
2. Developer documentation (contributing, architecture)
3. Agent-friendly documentation (progressive loading, structured metadata)
4. Versioned documentation aligned with releases

### Requirements

- Markdown-based for version control
- Published website for discoverability
- Agent-friendly front-matter
- Versioned docs per release
- WCAG 2.1 AA accessibility

### Options Considered

#### Documentation Platform

**Option A: Docusaurus on GitHub Pages (Selected)**
- React-based static site generator
- Native versioning support
- MDX for interactive docs
- Free hosting on GitHub Pages

**Option B: MkDocs with Material**
- Python-based
- Clean Material Design
- Good search

**Option C: GitBook**
- Hosted solution
- Nice editor
- Paid for advanced features

**Decision:** Docusaurus for feature richness and GitHub Pages for free hosting.

## Decision

### Documentation Structure

```
docs/
├── docusaurus/          # Website source
├── standards/           # Coding conventions
├── practices/           # Workflows, processes
├── playbooks/           # Runbooks
├── agents/              # Agent-specific guidance
├── adr/                 # Architecture Decision Records
└── plans/               # Design documents
```

### Agent-Friendly Front-Matter

All documentation includes structured front-matter:

```yaml
---
title: Coding Standards
summary: C# conventions for the project
audience: [developer, agent]
topics: [csharp, conventions]
prerequisites: [docs/getting-started.md]
related: [docs/practices/code-review.md]
last_validated: 2026-01-09
---
```

### Documentation Types

| Location | Purpose | Audience |
|----------|---------|----------|
| `docs/docusaurus/` | Published website | End users |
| `docs/standards/` | Coding conventions | Developers, agents |
| `docs/practices/` | Workflows | Developers, agents |
| `docs/playbooks/` | Runbooks | Operators |
| `docs/agents/` | Agent guidance | AI agents |
| `docs/adr/` | Architecture decisions | All |
| `AGENTS.md` | Agent routing | AI agents |

## Consequences

### Positive
- Version-controlled documentation
- Published website for users
- Agent-friendly metadata
- Free hosting

### Negative
- Docusaurus learning curve
- Build step for website
- Front-matter maintenance overhead

## References

- [Docusaurus](https://docusaurus.io/)
- [GitHub Pages](https://pages.github.com/)
