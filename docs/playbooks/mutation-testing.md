# Playbook: Mutation Testing

## Overview

This playbook guides you through running mutation testing with Stryker.NET to validate test
effectiveness beyond code coverage. Mutation testing introduces small code changes (mutants)
and verifies that tests catch them.

## Prerequisites

- .NET SDK 10.0+
- Project builds successfully (`dotnet build`)
- Unit tests pass (`dotnet test`)

## Step 1: Install Local Tools

The project uses local tool manifests. Restore them:

```bash
dotnet tool restore
```

This installs:

- `dotnet-stryker` - Mutation testing tool
- `dotnet-gitversion` - Semantic versioning
- `reportgenerator` - Coverage report generation

## Step 2: Run Mutation Testing

### Quick Run (Subset)

For fast feedback during development:

```bash
dotnet stryker --config-file stryker-config.json --since:main
```

This runs mutations only on files changed since `main`.

### Full Run

For comprehensive analysis:

```bash
dotnet stryker --config-file stryker-config.json
```

Full runs take significantly longer (potentially 10x or more).

## Step 3: View Results

After completion, Stryker generates an HTML report:

```bash
# Open the report (location shown in Stryker output)
# Default: StrykerOutput/<timestamp>/reports/mutation-report.html
```

The report shows:

- **Mutation score** - Percentage of mutants killed by tests
- **Survived mutants** - Code changes not caught by tests
- **Killed mutants** - Code changes caught by tests
- **Timeout mutants** - Tests ran too long (usually infinite loops)
- **No coverage** - Mutated code not covered by any test

## Step 4: Interpret Results

### Score Thresholds

| Score  | Status | Action                       |
| ------ | ------ | ---------------------------- |
| 80%+   | High   | Excellent test effectiveness |
| 60-80% | Low    | Review survived mutants      |
| <60%   | Break  | Critical gap - add tests     |

### Common Survived Mutants

**Boundary conditions:**

```csharp
// Original
if (count > 0) { ... }
// Mutant (survived)
if (count >= 0) { ... }
```

Fix: Add edge case tests for `count = 0`.

**Return values:**

```csharp
// Original
return true;
// Mutant (survived)
return false;
```

Fix: Assert on return values, not just lack of exceptions.

**Arithmetic operators:**

```csharp
// Original
result = a + b;
// Mutant (survived)
result = a - b;
```

Fix: Test with values where the difference is observable.

## Step 5: Improve Test Coverage

For each survived mutant:

1. Identify the code location in the report
2. Understand what the mutation changed
3. Write a test that would fail with the mutation
4. Verify the mutation is now killed

```bash
# Re-run on specific file after adding tests
dotnet stryker --config-file stryker-config.json \
  --mutate "src/McjCoderOrg.ClaudeAutoResume/YourFile.cs"
```

## Nightly CI Execution

Mutation testing runs nightly via `.github/workflows/nightly.yml`.

Results are available in:

- Workflow run artifacts
- (Future) GitHub Pages dashboard

## Advanced Configuration

### Targeting Specific Files

```bash
# Mutate specific files
dotnet stryker --mutate "src/**/Calculator.cs"

# Exclude specific files
dotnet stryker --mutate "!src/**/Generated*.cs"
```

### Mutation Level

The `stryker-config.json` uses `Standard` mutation level. Options:

| Level    | Speed   | Thoroughness         |
| -------- | ------- | -------------------- |
| Basic    | Fast    | Fewer mutants        |
| Standard | Medium  | Balanced (default)   |
| Advanced | Slow    | More mutants         |
| Complete | Slowest | All possible mutants |

### Concurrency

Adjust for your machine:

```bash
# Use 8 parallel test runners
dotnet stryker --config-file stryker-config.json --concurrency 8
```

## Troubleshooting

### "Unable to find test project"

Ensure the test project path in `stryker-config.json` is correct:

```json
{
  "stryker-config": {
    "test-projects": [
      "tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj"
    ]
  }
}
```

### Tests pass but mutation score is low

This indicates tests lack assertion coverage:

- Tests may verify no exceptions but not output values
- Tests may use generic assertions (`Assert.NotNull`)
- Test doubles may be too lenient

### Timeout mutants

Usually indicates infinite loops. Review the mutated code location:

```csharp
// Original
while (i < max) { i++; }
// Mutant
while (i < max) { i--; }  // Infinite loop
```

### Out of memory

Reduce concurrency:

```bash
dotnet stryker --concurrency 2
```

## See Also

- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/introduction/)
- [ADR-0026: Mutation Testing](../adr/0026-mutation-testing.md)
- [Mutation Testing Wikipedia](https://en.wikipedia.org/wiki/Mutation_testing)
