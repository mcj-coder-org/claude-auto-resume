# ADR-0016: Architecture Testing

## Status

Proposed

## Date

2026-01-09

## Context

We need automated architecture tests to enforce structural rules:
1. Dependency direction (no circular dependencies)
2. Layer separation (domain doesn't depend on infrastructure)
3. Slice isolation (feature modules don't cross-reference)
4. Naming conventions

### Requirements

- xUnit integration
- Slice support for feature-based architecture
- Readable assertion syntax
- CI integration

### Options Considered

#### Option 1: NetArchTest (Selected)

Fluent API for .NET architecture testing.

**Pros:**
- Fluent, readable syntax
- Slice support via `SliceRuleDefinition`
- Active maintenance
- xUnit integration

**Cons:**
- Less comprehensive than ArchUnitNET
- Smaller community

#### Option 2: ArchUnitNET

C# port of Java's ArchUnit.

**Pros:**
- Comprehensive rule engine
- Large feature set
- Java ArchUnit compatibility

**Cons:**
- Heavier API
- No native slice support
- Steeper learning curve

## Decision

We will use **NetArchTest** with slice support for architecture testing.

### Test Project

`tests/McjCoderOrg.ClaudeAutoResume.ArchTests/` with same namespace as production code for internal access.

### Example Tests

```csharp
[Fact]
public void Domain_ShouldNotDependOn_Infrastructure()
{
    Types.InAssembly(MainAssembly)
        .That().ResideInNamespace("McjCoderOrg.ClaudeAutoResume.Domain")
        .ShouldNot()
        .HaveDependencyOn("McjCoderOrg.ClaudeAutoResume.Infrastructure")
        .GetResult()
        .IsSuccessful.Should().BeTrue();
}

[Fact]
public void Slices_ShouldNotHaveCrossSliceDependencies()
{
    SliceRuleDefinition.FromAssembly(MainAssembly)
        .SlicedByNamespace("McjCoderOrg.ClaudeAutoResume.Features.(*)")
        .ShouldNotHaveDependenciesBetweenSlices()
        .GetResult()
        .IsSuccessful.Should().BeTrue();
}
```

## Consequences

### Positive
- Automated architecture enforcement
- Prevents architecture drift
- Self-documenting structural rules
- Fast feedback in CI

### Negative
- Additional test project to maintain
- Rules need updating as architecture evolves

## References

- [NetArchTest](https://github.com/BenMorris/NetArchTest)
- [NetArchTest.eNhancedEdition](https://github.com/NeVeSpl/NetArchTest.eNhancedEdition)
