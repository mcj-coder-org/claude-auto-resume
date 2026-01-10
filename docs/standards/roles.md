# Team Roles

## Overview

Team role definitions for both human teams and agent perspectives. These roles provide standardized
expert perspectives during code reviews, planning, and validation.

## Frontmatter Standard

All role documents MUST include YAML frontmatter with these fields:

```yaml
---
name: role-name
description: |
  When to use this role and what expertise it provides.
  Include specific trigger conditions for agents.
model: balanced # Task-based model tier
---
```

### Required Fields

| Field         | Description                                                                                  |
| ------------- | -------------------------------------------------------------------------------------------- |
| `name`        | Kebab-case identifier (e.g., `tech-lead`, `senior-developer`)                                |
| `description` | When to use this role; must be sufficient for agents to select without reading full document |
| `model`       | Task-based model tier (see below)                                                            |

### Model Tiers

Task-based values that map to platform-specific models:

| Value       | Use Case                                                   | Claude     | OpenAI     |
| ----------- | ---------------------------------------------------------- | ---------- | ---------- |
| `reasoning` | Complex analysis, architecture design, security compliance | Opus 4.5   | GPT-5.2    |
| `balanced`  | Code review, implementation validation, security review    | Sonnet 4.5 | GPT-5.1    |
| `speed`     | Quick validation, formatting checks, simple lookups        | Haiku 4.5  | GPT-5-nano |
| `inherit`   | Use caller's model (see below)                             | (varies)   | (varies)   |

### Choosing Model Tiers

- **`reasoning`**: Use for roles requiring complex analysis, architectural decisions, threat
  modelling, or regulatory compliance. These tasks involve multi-step reasoning and trade-off
  evaluation.
- **`balanced`**: Use for most development roles - code review, implementation validation,
  testing, documentation. This is the default for implementation-focused work.
- **`speed`**: Use for quick validations, formatting checks, or simple lookups. Currently no
  roles use this tier as all roles require at least balanced capability for meaningful review.
- **`inherit`**: Use when the role's complexity should match the caller's context.

When in doubt, use `balanced`. Only use `reasoning` when the role explicitly requires deep
analysis beyond typical code review.

### Using `inherit`

The `inherit` tier means the role uses whatever model the calling agent or skill is using.
Use this when:

- The role's complexity matches the caller's context
- You want to avoid model switching overhead
- The role is a subprocess of a larger task

Do not use `inherit` for roles that require specific model capabilities (e.g., security
architecture requiring reasoning models).

### Example

```yaml
---
name: tech-lead
description: |
  Use for technical architecture decisions, design reviews, and cross-cutting
  concerns. Validates system design, evaluates trade-offs, and ensures
  architectural consistency.
model: reasoning # Complex analysis → Opus 4.5, GPT-5.2
---
```

## Role Index

### Development Roles

- **[Tech Lead](roles/tech-lead.md)** - Technical architecture and design oversight
- **[Senior Developer](roles/senior-developer.md)** - Code quality and implementation excellence
- **[QA Engineer](roles/qa-engineer.md)** - Quality assurance and testing strategy

### Security and Performance

- **[Security Reviewer](roles/security-reviewer.md)** - Security and threat modelling
- **[Performance Engineer](roles/performance-engineer.md)** - Performance optimization and scalability
- **[Security Architect](roles/security-architect.md)** - Security architecture and compliance

### Operations and Infrastructure

- **[DevOps Engineer](roles/devops-engineer.md)** - Deployment, operations, and infrastructure
- **[Cloud Architect](roles/cloud-architect.md)** - Cloud infrastructure and platform design

### Product and Design

- **[Product Owner](roles/product-owner.md)** - Business value and requirements
- **[Scrum Master](roles/scrum-master.md)** - Process compliance and documentation completeness
- **[UX Expert](roles/ux-expert.md)** - User experience and interface design
- **[Accessibility Expert](roles/accessibility-expert.md)** - Accessibility and inclusive design

### Documentation and Architecture

- **[Documentation Specialist](roles/documentation-specialist.md)** - Documentation quality and accessibility
- **[Technical Architect](roles/technical-architect.md)** - Enterprise architecture and system design
- **[Agent Skill Engineer](roles/agent-skill-engineer.md)** - Agent skill design and optimization

## Using Roles in Skills

When creating or updating skills:

1. **Identify relevant roles**: Determine which expert perspectives apply
2. **Reference in "When to Use"**: Mention which roles should review
3. **Include in BDD tests**: Test scenarios with role perspectives
4. **Document in examples**: Show role usage in examples

### Example

```markdown
## When to Use

This skill should be reviewed by:

- **Tech Lead**: Validate architecture decisions
- **Senior Developer**: Review code quality
- **Security Reviewer**: Check for vulnerabilities
```

### In BDD Tests

```markdown
### GREEN Scenario: Multi-role review

**Given**: Implementation complete
**When**: Requesting code review
**Then**: Each role provides perspective:

- Tech Lead: ✅ Architecture sound
- Security Reviewer: ✅ No vulnerabilities found
- Senior Developer: ✅ Code quality excellent
```

### In Code Review Requests

```markdown
Please review from these perspectives:

**@role:security** - Authentication flow and input validation
**@role:performance** - Database query efficiency
**@role:senior-dev** - Code organization and clarity
```

## Canonical Names

Use these exact names when referencing roles:

- `Tech Lead` (not: "Technical Lead", "Architect", "Tech Arch")
- `Senior Developer` (not: "Developer", "Sr Dev", "Engineer")
- `QA Engineer` (not: "QA", "Tester", "Quality Assurance")
- `Security Reviewer` (not: "Security", "Sec Engineer", "AppSec")
- `Performance Engineer` (not: "Performance", "Perf Engineer")
- `DevOps Engineer` (not: "DevOps", "SRE", "Ops")
- `Product Owner` (not: "PO", "Product", "Product Manager")
- `Scrum Master` (not: "SM", "Scrum", "Process Manager", "Agile Coach")
- `Documentation Specialist` (not: "Docs", "Tech Writer")
- `UX Expert` (not: "UX", "UX Designer", "UI/UX")
- `Accessibility Expert` (not: "A11y", "Accessibility", "WCAG Expert")
- `Agent Skill Engineer` (not: "Skill Engineer", "Agent Engineer", "Prompt Engineer")
- `Technical Architect` (not: "Architect", "Enterprise Architect", "Solutions Architect")
- `Security Architect` (not: "Sec Arch", "AppSec Architect", "InfoSec")
- `Cloud Architect` (not: "Cloud Engineer", "Infrastructure Architect", "Platform Architect")

## External Role References

- **Code Reviewer**: See `superpowers:receiving-code-review` (not editable here)

## Reference Format

**In markdown**: `**Tech Lead**` or `@role:tech-lead`

**In code comments**: `@role tech-lead` or `// Review: Tech Lead perspective needed`

## Role Selection Guide

When multiple roles seem applicable, use these criteria:

### By Scope

| Scope            | Use These Roles                                |
| ---------------- | ---------------------------------------------- |
| Single component | Senior Developer, Security Reviewer, QA        |
| Cross-cutting    | Tech Lead, Performance Engineer, DevOps        |
| Enterprise-wide  | Technical Architect, Security Architect, Cloud |

### By Phase

| Phase          | Use These Roles                                 |
| -------------- | ----------------------------------------------- |
| Design         | Tech Lead, Technical Architect, Cloud Architect |
| Implementation | Senior Developer, Security Reviewer             |
| Review         | QA Engineer, Documentation Specialist           |

### Overlapping Domains

- **Tech Lead vs Technical Architect**: Tech Lead for project-level decisions; Technical
  Architect for enterprise or cross-project concerns
- **Security Reviewer vs Security Architect**: Security Reviewer for code-level issues;
  Security Architect for compliance, threat modelling, or system-level security
- **Senior Developer vs QA Engineer**: Senior Developer for code quality; QA Engineer
  for test strategy and coverage

## Validation

Role frontmatter is validated during:

- **Pre-commit hooks**: Format and syntax validation via prettier
- **CI pipeline**: Markdown linting via markdownlint-cli2
- **Manual check**: Run `npm run lint` to validate all role files

Required fields (`name`, `description`, `model`) are enforced by convention. Invalid
frontmatter will cause YAML parsing errors when roles are loaded by agents.

## Troubleshooting

**Issue**: Agent selects wrong role for task

- Check the `description` field includes specific trigger conditions
- Ensure overlapping roles have clear differentiation (see Role Selection Guide)
- Consider adding "use X instead" guidance for ambiguous cases

**Issue**: Model tier seems incorrect for role complexity

- Review the Choosing Model Tiers guidance above
- Use `balanced` as the default; only use `reasoning` for complex analysis
- Add escalation guidance in description if role may need higher tier

**Issue**: Frontmatter validation fails

- Ensure YAML syntax is correct (proper indentation, `|` for multiline)
- Check all required fields are present: `name`, `description`, `model`
- Run `npm run format` to auto-fix formatting issues

**Issue**: Multiple roles apply to a task

- Use scope criteria: single component vs cross-cutting vs enterprise
- Use phase criteria: design vs implementation vs review
- When still unclear, prefer the more specific role over general ones
