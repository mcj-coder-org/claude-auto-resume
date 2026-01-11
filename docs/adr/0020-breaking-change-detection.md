---
name: breaking-change-detection
description: |
  When modifying public API or managing semantic versioning. Apply when adding, removing,
  or changing public members, or when syncing API changes with conventional commits.
decision: Use Microsoft.CodeAnalysis.PublicApiAnalyzers with PublicAPI.Shipped/Unshipped.txt files.
status: accepted
---

# ADR-0020: Breaking Change Detection

## Status

Proposed

## Date

2026-01-09

## Context

We need to detect breaking changes to public API and sync with semantic versioning.

### Requirements

- Detect additions, removals, and changes to public API
- Sync with conventional commits
- Fail CI on undocumented breaking changes
- Support API evolution

## Decision

**Microsoft.CodeAnalysis.PublicApiAnalyzers** for breaking change detection.

### Implementation

Public API tracked in text files:

- `PublicAPI.Shipped.txt` - Released API
- `PublicAPI.Unshipped.txt` - Unreleased changes

### Sync with Semantic Versioning

| Change Type          | PublicAPI File      | Commit Type        | Version |
| -------------------- | ------------------- | ------------------ | ------- |
| New public member    | Add to Unshipped    | `feat:`            | Minor   |
| Remove public member | Remove from Shipped | `BREAKING CHANGE:` | Major   |
| Change signature     | Update both         | `BREAKING CHANGE:` | Major   |

### Workflow

1. Add new public API → analyzer warns
2. Run `dotnet format` to update Unshipped
3. Commit with `feat:` message
4. On release, move Unshipped → Shipped

### CI Enforcement

```yaml
- name: Check public API
  run: dotnet build -warnaserror
```

Undocumented API changes fail the build.

## Consequences

### Positive

- Automated breaking change detection
- Synced with versioning
- CI enforcement
- API documentation

### Negative

- File maintenance overhead
- Learning curve

## References

- [PublicApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md)
