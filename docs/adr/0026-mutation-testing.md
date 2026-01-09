# ADR-0026: Mutation Testing

## Status

Proposed

## Date

2026-01-09

## Context

We need mutation testing to validate test effectiveness beyond code coverage.

### Requirements

- Automated mutation testing
- Mutation score tracking
- Local execution playbook
- Nightly CI execution

## Decision

**Stryker.NET** for mutation testing.

### Implementation

Configuration via `stryker-config.json`:

```json
{
  "stryker-config": {
    "project": "McjCoderOrg.ClaudeAutoResume.csproj",
    "test-projects": ["McjCoderOrg.ClaudeAutoResume.Tests.csproj"],
    "reporters": ["html", "json", "dashboard"],
    "threshold-high": 80,
    "threshold-low": 60,
    "threshold-break": 50
  }
}
```

### Execution Strategy

| Trigger | Scope                     |
| ------- | ------------------------- |
| Nightly | Full mutation suite       |
| Manual  | Developer-triggered       |
| Local   | Per playbook instructions |

### Playbook

`docs/playbooks/mutation-testing.md` covers:

1. Local installation
2. Running mutations
3. Interpreting results
4. Improving mutation score

### Dashboard

Mutation reports published to GitHub Pages alongside benchmark results.

## Consequences

### Positive

- Validates test quality
- Finds weak tests
- Complements coverage metrics

### Negative

- Long execution time
- Resource intensive
- Nightly-only feedback

## References

- [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/)
- [Mutation Testing](https://en.wikipedia.org/wiki/Mutation_testing)
