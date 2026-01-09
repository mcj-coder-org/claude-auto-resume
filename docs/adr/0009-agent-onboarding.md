# ADR-0009: Agent Onboarding

## Status

Proposed

## Date

2026-01-09

## Context

AI agents (Claude, Copilot, etc.) need efficient onboarding to:
1. Understand project structure and conventions
2. Find relevant documentation quickly
3. Load context progressively (not all at once)
4. Follow project-specific workflows

### Requirements

- Single entry point for agents
- Progressive loading guidance
- Structured metadata for filtering
- Immutable links for context stability

### Options Considered

#### Option 1: AGENTS.md + docs/agents/ (Selected)

Hybrid approach with root routing file and detailed agent docs.

**Pros:**
- Clear entry point (AGENTS.md)
- Progressive loading rules
- Detailed guidance in dedicated folder
- Separation of concerns

#### Option 2: Single AGENTS.md

Everything in one file.

**Pros:**
- Simple
- Single file to maintain

**Cons:**
- Gets large quickly
- No progressive loading

#### Option 3: .claude/ directory

Claude-specific configuration.

**Pros:**
- Claude-native
- Project-scoped

**Cons:**
- Claude-specific, not universal
- Less discoverable

## Decision

**Hybrid approach:** `AGENTS.md` at root for routing, `docs/agents/` for detailed guidance.

### AGENTS.md Structure

```markdown
# Agent Orientation

## Quick Start
{Immediate context for any task}

## Documentation Loading Rules

### Always Read (Full Content)
- AGENTS.md
- docs/agents/ORIENTATION.md
- docs/agents/CONVENTIONS.md

### Read Front-Matter First
- docs/standards/*
- docs/practices/*
- docs/playbooks/*

### Read On-Demand
- docs/adr/*
- CHANGELOG.md

## Project Structure
{Brief overview with links}

## Common Tasks
{Links to relevant docs by task type}
```

### docs/agents/ Contents

| File | Purpose |
|------|---------|
| ORIENTATION.md | Project overview, architecture |
| CONVENTIONS.md | Coding standards, commit format |
| WORKFLOWS.md | Common development workflows |
| TROUBLESHOOTING.md | Common issues and solutions |

## Consequences

### Positive
- Efficient agent onboarding
- Progressive context loading
- Universal (not agent-specific)
- Maintainable structure

### Negative
- Additional documentation to maintain
- Agents must follow loading rules

## References

- [Claude Code AGENTS.md](https://docs.anthropic.com/en/docs/claude-code/memory#agentsmd)
