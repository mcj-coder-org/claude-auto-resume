---
name: performance-testing
description: |
  When implementing performance benchmarks, detecting regressions, or tracking performance over time. Apply when setting up benchmark projects or CI performance checks.
decision: Use BenchmarkDotNet with CI baseline comparisons and nightly full benchmark suites.
status: accepted
type: implementation
implementation_issue: '#38'
---

# ADR-0025: Performance Testing

## Status

Proposed

## Date

2026-01-09

## Context

We need performance testing to:

1. Detect performance regressions
2. Establish baselines
3. Track improvements over time

### Requirements

- Automated benchmarks
- CI integration
- Historical comparison
- Low overhead

## Decision

**BenchmarkDotNet** for performance testing.

### Implementation

Dedicated benchmark project: `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/`

### Example Benchmark

```csharp
[MemoryDiagnoser]
public class OutputParsingBenchmarks
{
    private readonly RateLimitDetector _detector = new();

    [Benchmark]
    public bool ParseRateLimitMessage()
    {
        return _detector.IsRateLimited("Claude AI usage limit reached");
    }
}
```

### CI Integration

- **PR:** Run benchmarks, compare to main baseline
- **Main:** Run benchmarks, store as new baseline
- **Results:** Published to GitHub Pages for historical tracking

### Execution Strategy

| Trigger   | Action                                  |
| --------- | --------------------------------------- |
| PR        | Compare to baseline, warn on regression |
| Main push | Update baseline                         |
| Nightly   | Full benchmark suite                    |

## Consequences

### Positive

- Automated regression detection
- Historical tracking
- Industry-standard tooling

### Negative

- CI time for benchmarks
- Baseline management
- Environment variability

## References

- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [BenchmarkDotNet GitHub Actions](https://github.com/benchmark-action/github-action-benchmark)
