# ADR-0009: Agent Onboarding

## Status

Proposed

## Date

2026-01-09

## Context

AI agents need efficient onboarding and clear usage patterns to:

1. Understand project structure and conventions
2. Find relevant documentation quickly
3. Load context progressively (not all at once)
4. Follow project-specific workflows
5. Work across different IDEs and environments
6. Operate at varying levels of autonomy

### Requirements

- Single entry point for agents
- Progressive loading guidance
- Support for multiple agent platforms (Claude, Codex, Junie)
- IDE integration guidance (VS Code, Rider, Visual Studio)
- Usage patterns for different work modes
- Sub-agent/persona support for context isolation
- Fallback strategies for capability differences
- Optional CI/CD integration for autonomous workflows

## Decision

**Hybrid approach:** `AGENTS.md` at root for routing, `docs/agents/` for detailed guidance,
with defined usage patterns and multi-agent support.

---

## Agent Usage Patterns

### Pattern 1: Planning & Requirements

Agents assist with driving out requirements, designs, and ADRs.

| Aspect             | Details                                 |
| ------------------ | --------------------------------------- |
| **Mode**           | Collaborative dialogue                  |
| **Human Role**     | Decision maker, domain expert           |
| **Agent Role**     | Facilitator, documenter, challenger     |
| **Artifacts**      | ADRs, design docs, implementation plans |
| **Autonomy Level** | Guided (human leads)                    |

**Documentation Needs:**

- ADR templates and examples
- Design document templates
- Issue/epic templates
- Decision frameworks

**Example Tasks:**

- "Help me design the authentication system"
- "What ADRs do we need for this feature?"
- "Break down this epic into sub-issues"

---

### Pattern 2: Pair Programming

Agent works alongside human developer in an IDE.

| Aspect             | Details                 |
| ------------------ | ----------------------- |
| **Mode**           | Real-time collaboration |
| **Human Role**     | Driver or navigator     |
| **Agent Role**     | Navigator or driver     |
| **Artifacts**      | Code, tests, commits    |
| **Autonomy Level** | Guided to Supervised    |

**Documentation Needs:**

- Quick reference cards (commit format, branch naming)
- Coding standards (inline-friendly format)
- Common patterns and idioms
- IDE-specific guidance

**IDE Support Matrix:**

| IDE             | Primary Agent | Integration Method           |
| --------------- | ------------- | ---------------------------- |
| VS Code         | Claude Code   | CLI + terminal, Copilot Chat |
| JetBrains Rider | Junie         | AI Assistant, terminal       |
| Visual Studio   | Codex/Copilot | Copilot Chat, terminal       |

**Example Tasks:**

- "Help me implement this interface"
- "Write tests for this class"
- "Refactor this method to use async/await"

---

### Pattern 3: Verification & Review

Specialised agents perform validation from specific perspectives.

| Aspect             | Details                                   |
| ------------------ | ----------------------------------------- |
| **Mode**           | Asynchronous review                       |
| **Human Role**     | Requestor, final approver                 |
| **Agent Role**     | Specialist reviewer                       |
| **Artifacts**      | Review comments, reports, recommendations |
| **Autonomy Level** | Supervised                                |

**Specialist Perspectives:**

| Persona                | Focus Area              | Key Concerns                           |
| ---------------------- | ----------------------- | -------------------------------------- |
| Security Reviewer      | Vulnerabilities, OWASP  | Injection, auth, data exposure         |
| Accessibility Auditor  | WCAG 2.1 AA             | Screen readers, keyboard nav, contrast |
| Performance Analyst    | Efficiency, scalability | Memory, CPU, async patterns            |
| API Reviewer           | Contract stability      | Breaking changes, versioning           |
| Documentation Reviewer | Clarity, completeness   | Examples, accuracy, coverage           |
| Architecture Reviewer  | Structure, patterns     | Dependencies, coupling, SOLID          |

**Documentation Needs:**

- Review checklists per perspective
- Domain-specific criteria
- Severity classification guidelines
- Report templates

**Example Tasks:**

- "Review this PR from a security perspective"
- "Audit this component for accessibility"
- "Analyse performance implications of this change"

---

### Pattern 4: Autonomous Execution

Agent independently processes work items from ticket to PR.

| Aspect             | Details                    |
| ------------------ | -------------------------- |
| **Mode**           | Independent execution      |
| **Human Role**     | Ticket author, PR approver |
| **Agent Role**     | Full implementation        |
| **Artifacts**      | Branch, commits, tests, PR |
| **Autonomy Level** | Autonomous                 |

**Prerequisites for Autonomous Work:**

- Complete, unambiguous ticket with acceptance criteria
- All referenced documentation accessible
- Clear scope boundaries
- Defined verification criteria

**Documentation Needs:**

- Complete coding standards (no ambiguity)
- Full contribution workflow (ADR-0004)
- Test requirements and patterns
- PR template and checklist

**Example Tasks:**

- "Implement ticket #42: Add retry logic to HTTP client"
- "Fix bug #87: Handle null response in parser"
- "Refactor per ticket #103: Extract configuration class"

---

## Progressive Autonomy Levels

```text
┌─────────────────────────────────────────────────────────────┐
│                    AUTONOMY PROGRESSION                     │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Level 1: GUIDED                                            │
│  ┌───────────────────────────────────────────────────────┐  │
│  │ Human leads, agent assists                            │  │
│  │ • Pair programming (human drives)                     │  │
│  │ • Planning sessions                                   │  │
│  │ • Learning/exploration                                │  │
│  └───────────────────────────────────────────────────────┘  │
│                           │                                 │
│                           ▼                                 │
│  Level 2: SUPERVISED                                        │
│  ┌───────────────────────────────────────────────────────┐  │
│  │ Agent leads, human reviews                            │  │
│  │ • Pair programming (agent drives)                     │  │
│  │ • Verification tasks                                  │  │
│  │ • Draft implementations                               │  │
│  └───────────────────────────────────────────────────────┘  │
│                           │                                 │
│                           ▼                                 │
│  Level 3: AUTONOMOUS                                        │
│  ┌───────────────────────────────────────────────────────┐  │
│  │ Agent executes, human approves                        │  │
│  │ • Ticket-to-PR execution                              │  │
│  │ • Automated fixes (lint, deps)                        │  │
│  │ • CI/CD triggered tasks                               │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Target Agent Platforms

### Primary Targets

| Agent         | Provider  | Strengths                            | Integration     |
| ------------- | --------- | ------------------------------------ | --------------- |
| Claude Code   | Anthropic | Reasoning, planning, code generation | CLI, API        |
| Codex/Copilot | OpenAI    | Code completion, IDE integration     | VS Code, VS     |
| Junie         | JetBrains | IDE-native, refactoring awareness    | Rider, IntelliJ |

### Testing Requirements

All agent-facing documentation must be tested against all three platforms to ensure clarity and compatibility.

**Test Protocol:**

1. Fresh session (no prior context)
2. Provide only repository access
3. Assign standardised test task
4. Evaluate against success criteria
5. Document platform-specific issues

**Test Tasks:**

- Simple: "Add a comment explaining this method"
- Medium: "Write unit tests for this class"
- Complex: "Implement this feature per ticket #X"

---

## Sub-Agent Pattern (Preferred)

### Context Isolation via Personas

To reduce context pollution across conversations, use sub-agents with defined personas when the platform supports it.

```text
┌─────────────────────────────────────────────────────────────┐
│                     MAIN CONVERSATION                       │
│  (Orchestrator / Human)                                     │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│   ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│   │ Sub-Agent:   │  │ Sub-Agent:   │  │ Sub-Agent:   │     │
│   │ Security     │  │ Accessibility│  │ Implementation│    │
│   │ Reviewer     │  │ Auditor      │  │ Assistant     │    │
│   └──────────────┘  └──────────────┘  └──────────────┘     │
│         │                  │                  │             │
│         ▼                  ▼                  ▼             │
│   Isolated context   Isolated context   Isolated context   │
│   Returns summary    Returns summary    Returns summary    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Benefits:**

- Focused context per task
- Reduced token usage
- Clearer responsibility boundaries
- Parallel execution possible
- Easier debugging of agent behaviour

**When to Use Sub-Agents:**

- Verification tasks (each perspective = sub-agent)
- Multi-file implementations (each component = sub-agent)
- Research vs implementation phases
- Any task benefiting from fresh context

### Platform Support

| Platform      | Sub-Agent Support | Method                        |
| ------------- | ----------------- | ----------------------------- |
| Claude Code   | Yes               | Task tool with subagent_type  |
| Anthropic API | Yes               | Separate conversation threads |
| Codex/Copilot | Limited           | Workspace agents (preview)    |
| Junie         | Limited           | Context splitting             |

### Fallback Strategies

When sub-agents are unavailable:

1. **Context Summarisation**: Periodically summarise and compact context
2. **Explicit Scope Boundaries**: Clear "Starting X task" / "Completed X task" markers
3. **File-Based Handoff**: Write findings to files, read in fresh session
4. **Session Restart**: For long tasks, document state and restart

---

## CI/CD Integration (Optional)

### Autonomous Agent Workflows

For CI/CD triggered autonomous work, an Anthropic API key enables:

| Workflow           | Trigger        | Agent Action                       |
| ------------------ | -------------- | ---------------------------------- |
| Auto-fix lint      | CI failure     | Create fix PR                      |
| Dependency updates | Dependabot PR  | Review and approve/request changes |
| Documentation gaps | New public API | Generate doc stubs                 |
| Test generation    | Coverage drop  | Add missing tests                  |

### Configuration

```yaml
# .github/workflows/agent-tasks.yml (example)
env:
  ANTHROPIC_API_KEY: ${{ secrets.ANTHROPIC_API_KEY }}

jobs:
  agent-review:
    if: github.event_name == 'pull_request'
    runs-on: ubuntu-latest
    steps:
      - uses: anthropics/claude-code-action@v1
        with:
          task: 'Review this PR for security issues'
          persona: 'security-reviewer'
```

**Security Considerations:**

- API key stored as GitHub secret
- Agent actions require human approval (PR review)
- Audit log of all agent actions
- Rate limiting to prevent runaway costs

---

## Documentation Structure

### AGENTS.md (Root)

```markdown
# Agent Orientation

## Quick Start

{Immediate context - project purpose, tech stack}

## Your First Action

Read this file completely, then read docs/agents/ORIENTATION.md

## Documentation Loading Rules

### Always Read (Full Content)

- AGENTS.md (this file)
- docs/agents/ORIENTATION.md
- docs/agents/CONVENTIONS.md

### Read Front-Matter First, Full Content On-Demand

- docs/standards/\* (coding standards)
- docs/practices/\* (workflows, reviews)
- docs/adr/\* (architectural decisions)

### Reference Only When Needed

- docs/playbooks/\* (specific procedures)
- CHANGELOG.md (version history)

## Usage Patterns

See docs/agents/PATTERNS.md for:

- Planning & Requirements
- Pair Programming
- Verification & Review
- Autonomous Execution

## IDE Integration

See docs/agents/IDE-SETUP.md for:

- VS Code configuration
- JetBrains Rider configuration
- Visual Studio configuration

## Common Tasks

| Task              | Start Here                                   |
| ----------------- | -------------------------------------------- |
| Implement feature | ADR-0004, docs/standards/coding-standards.md |
| Review code       | docs/practices/code-review.md                |
| Fix bug           | docs/agents/TROUBLESHOOTING.md               |
| Write tests       | docs/standards/testing-standards.md          |
| Plan work         | docs/agents/PATTERNS.md#planning             |
```

### docs/agents/ Contents

| File               | Purpose                                                |
| ------------------ | ------------------------------------------------------ |
| ORIENTATION.md     | Project overview, architecture, key decisions          |
| CONVENTIONS.md     | Coding standards summary, commit format, branch naming |
| PATTERNS.md        | Detailed usage patterns with examples                  |
| IDE-SETUP.md       | IDE-specific configuration and tips                    |
| PERSONAS.md        | Sub-agent persona definitions                          |
| TROUBLESHOOTING.md | Common issues and solutions                            |

### docs/agents/PERSONAS.md

Define reusable personas for sub-agents:

```markdown
# Agent Personas

## security-reviewer

**Focus:** Security vulnerabilities and best practices
**Checklist:** docs/checklists/security-review.md
**Perspective:** Assume malicious input, verify all trust boundaries

## accessibility-auditor

**Focus:** WCAG 2.1 AA compliance
**Checklist:** docs/checklists/accessibility-audit.md
**Perspective:** Test with screen reader, keyboard-only navigation

## performance-analyst

**Focus:** Efficiency and scalability
**Checklist:** docs/checklists/performance-review.md
**Perspective:** Consider memory, CPU, async patterns, caching

...
```

---

## Consequences

### Positive

- Efficient agent onboarding across platforms
- Clear usage patterns for different work modes
- Context isolation via sub-agents reduces pollution
- IDE-agnostic with specific guidance where needed
- Progressive autonomy enables trust building
- CI/CD integration enables automation

### Negative

- Additional documentation to maintain
- Multi-platform testing overhead
- Sub-agent pattern requires platform support
- CI/CD integration adds complexity

### Mitigations

- Test documentation against all target platforms regularly
- Provide fallback strategies for limited platforms
- Keep CI/CD integration optional
- Use verification gate (Phase 6) to validate documentation

---

## References

- [Claude Code AGENTS.md](https://docs.anthropic.com/en/docs/claude-code/memory#agentsmd)
- [Claude Code Sub-Agents](https://docs.anthropic.com/en/docs/claude-code/sub-agents)
- [GitHub Copilot Workspace](https://githubnext.com/projects/copilot-workspace)
- [JetBrains AI Assistant](https://www.jetbrains.com/ai/)
- [ADR-0004: Contribution Workflow](./0004-contribution-workflow.md)
