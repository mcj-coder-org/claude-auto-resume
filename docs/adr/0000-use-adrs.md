---
name: use-adrs
description: |
  Apply when deciding how to document architectural decisions
  or questioning why ADRs are used in this repository.
decision: Use MADR format ADRs in docs/adr/ for significant architectural and process decisions.
status: accepted
---

# 0. Use Architecture Decision Records

Date: 2026-01-05
Deciders: development-skills maintainers
Tags: process, documentation

## Context and Problem Statement

We need to record significant architectural and process decisions for future reference. This includes:

- Major tooling and framework choices
- Significant architectural changes
- Process and workflow decisions

How should we document these decisions for both humans and agents?

## Decision Drivers

- Decisions must be discoverable by both humans and agents
- Need context and rationale, not just the decision
- Should document why specific approaches were chosen
- Industry standard format preferred
- Git-tracked and versioned with code

## Considered Options

- Flat list in README.md
- Wiki or external documentation system
- Architecture Decision Records (ADRs)
- Database or structured data files

## Decision Outcome

Chosen option: "Architecture Decision Records", because:

- Industry standard format (widely recognized in software engineering)
- Human and agent readable (markdown format)
- Git-tracked (versioned with code, shows evolution)
- Includes context, alternatives, and consequences (not just decisions)
- Can be easily searched and referenced by file name
- Lightweight (no external systems required)
- Supported by tooling and IDE plugins

We will use the [MADR format](https://adr.github.io/madr/) (Markdown Architectural Decision Records) for consistency.

### Consequences

- Good: Decisions are documented with full context and alternatives considered
- Good: Easily searchable by humans and agents (file names and grep)
- Good: Git history shows when and why decisions were made
- Good: Standard format means tooling support available (ADR tools, IDE plugins)
- Good: No external dependencies or systems to maintain
- Neutral: Requires discipline to create ADRs for significant decisions
- Neutral: Need to determine what qualifies as "significant" (see guidelines below)
- Bad: Slightly more overhead than informal notes or comments

## Guidelines for Creating ADRs

### When to Create an ADR

**Create ADRs for:**

- Choosing frameworks or major libraries (e.g., "Use .NET Aspire", "Use xUnit")
- Significant architectural changes (e.g., "Move to microservices", "Adopt clean architecture")
- Major tooling decisions (e.g., "Use GitHub Actions for CI", "Use Docker for containerization")
- Process changes with broad impact (e.g., "Adopt trunk-based development")

**Don't create ADRs for:**

- Every skill application or minor configuration
- Routine coding decisions covered by standards
- Temporary experiments or spikes
- Minor refactorings or bug fixes

### When to Update an ADR

ADRs are immutable once accepted. To change a decision:

1. Create a new ADR that supersedes the old one
2. Update old ADR status to "superseded by ADR-NNNN"
3. New ADR should reference old one and explain why change was needed

### ADR Format

Use MADR format with these sections:

- Title: "{NUMBER}. {Decision in active voice}"
- Metadata: Date, Status, Deciders, Tags
- Context and Problem Statement
- Decision Drivers
- Considered Options
- Decision Outcome
- Consequences (Good, Bad, Neutral)

## Links

- [MADR format](https://adr.github.io/madr/)
- [ADR GitHub organization](https://adr.github.io/)
- [Article: Documenting Architecture Decisions](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
