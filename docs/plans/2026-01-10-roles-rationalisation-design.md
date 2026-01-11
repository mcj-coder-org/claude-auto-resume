# Design: Roles Rationalisation

## Overview

Consolidate PERSONAS.md and roles/ documentation into a single, consistent structure that serves
both human teams and AI agents.

## Problem

Current state has duplication and inconsistency:

- `docs/agents/PERSONAS.md` - 5 agent personas with detailed checklists, output formats
- `docs/standards/roles.md` + `docs/standards/roles/*.md` - 15 team roles with different structure

4 roles appear in both (Security Reviewer, QA Engineer, Senior Developer, Documentation Specialist)
with different frontmatter formats and varying levels of detail.

## Solution

Merge into unified structure following the progressive loading pattern:

```text
docs/standards/roles.md           # Main page with overview, index, selection guide
docs/standards/roles/*.md         # One file per role with full details
```

Delete `docs/agents/PERSONAS.md` and merge its detailed content into role files.

## Unified Frontmatter

```yaml
---
name: senior-developer
description: |
  Use for code quality reviews, SOLID principle validation, and refactoring decisions.
  Apply when reviewing PRs for maintainability or evaluating technical debt.
model: balanced
audience: [developer, agent]
topics: [code-review, quality, solid, refactoring]
last_validated: 2026-01-10
---
```

| Field            | Purpose                                                 |
| ---------------- | ------------------------------------------------------- |
| `name`           | Kebab-case identifier for agent matching                |
| `description`    | Trigger conditions (when to use this role)              |
| `model`          | Model tier: `reasoning`, `balanced`, `speed`, `inherit` |
| `audience`       | Who uses this doc (always `[developer, agent]`)         |
| `topics`         | Searchable keywords                                     |
| `last_validated` | Freshness tracking                                      |

## Role Document Body Structure

````markdown
# {Role Name}

{One-line description of the role's purpose}

## Profile

| Attribute      | Value                          |
| -------------- | ------------------------------ |
| **Focus**      | {Primary area of expertise}    |
| **Model Tier** | {reasoning/balanced/speed}     |
| **Autonomy**   | {Supervised/Guided/Autonomous} |

## Expertise

- {Skill 1}
- {Skill 2}

## When to Use

- {Trigger condition 1}
- {Trigger condition 2}

## Key Concerns

### {Concern Area 1}

- {Specific point}

## Checklist

- [ ] {Actionable verification item}

## Output Format

```text
{Template for structured output when acting in this role}
```
````

## Escalate When

- {Blocking condition requiring human/higher-tier intervention}

```

## Main Page Structure

The `docs/standards/roles.md` becomes an index:

- Role index table with links to detail pages
- Role selection guide (by scope, by phase)
- Usage guidance for humans and agents
- Canonical names list

## Migration Plan

### Delete

- `docs/agents/PERSONAS.md`

### Update

- `AGENTS.md` - change persona references to `docs/standards/roles.md`
- `docs/agents/PATTERNS.md` - update persona references

### Role Consolidation

| Current PERSONAS.md | Current roles/ | Action |
|---------------------|----------------|--------|
| DotNet Developer | (missing) | Add as new role |
| Security Reviewer | security-reviewer.md | Merge PERSONAS content |
| QA Engineer | qa-engineer.md | Merge PERSONAS content |
| Senior Developer | senior-developer.md | Merge PERSONAS content |
| Documentation Specialist | documentation-specialist.md | Merge PERSONAS content |
| - | 10 other roles | Update frontmatter only |

### Final Role Count

16 roles total:

**Development:** Tech Lead, Senior Developer, DotNet Developer, QA Engineer

**Security/Performance:** Security Reviewer, Security Architect, Performance Engineer

**Operations:** DevOps Engineer, Cloud Architect

**Product/Design:** Product Owner, Scrum Master, UX Expert, Accessibility Expert

**Documentation/Architecture:** Documentation Specialist, Technical Architect, Agent Skill Engineer

## Success Criteria

- [ ] Single source of truth for all roles
- [ ] Consistent frontmatter across all role files
- [ ] All role files have complete structure (profile, expertise, checklist, output format)
- [ ] PERSONAS.md deleted
- [ ] AGENTS.md updated to reference roles.md
- [ ] No broken references in documentation
```
