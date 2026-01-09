# ADR-0026: Code Coverage

## Status

Proposed

## Date

2026-01-09

## Context

We need code coverage to:
1. Measure test effectiveness
2. Identify untested code
3. Enforce coverage standards

### Requirements

- Coverage on changed code only (not full repo)
- Ratchet pattern (can only increase)
- Test projects excluded
- PR reporting

## Decision

**Coverlet** with delta coverage reporting.

### Configuration

**coverlet.runsettings:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>cobertura</Format>
          <Exclude>[*Tests*]*,[*Benchmarks*]*</Exclude>
          <ExcludeByFile>**/obj/**</ExcludeByFile>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### Thresholds

- **Line coverage:** 80% on changed code
- **Branch coverage:** 70% on changed code
- **Ratchet:** Coverage can never decrease

### Delta Coverage

Only enforce coverage on changed files:

```yaml
- name: Report coverage
  uses: codecov/codecov-action@v4
  with:
    flags: unittests
    fail_ci_if_error: true

- name: Check delta coverage
  run: |
    # Compare coverage of changed files against threshold
```

### Excluded from Coverage

- Test projects (`*Tests*`, `*Benchmarks*`)
- Generated code (`**/obj/**`)
- Program.cs (entry point)

## Consequences

### Positive
- Focused on changed code
- Prevents coverage regression
- Clear PR feedback

### Negative
- Delta coverage tooling complexity
- Threshold tuning needed
- False sense of quality from coverage alone

## References

- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [Codecov](https://codecov.io/)
