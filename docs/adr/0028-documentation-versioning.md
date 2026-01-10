---
name: documentation-versioning
description: |
  When versioning documentation alongside software releases or managing docs for multiple versions. Apply when configuring Docusaurus versioning or documentation site structure.
decision: Use Docusaurus versioned docs with version snapshots created at each release.
status: accepted
---

# ADR-0028: Documentation Versioning

## Status

Proposed

## Date

2026-01-09

## Context

We need versioned documentation to:

1. Match docs to software versions
2. Support users on older versions
3. Show what's new in each release

### Requirements

- Version docs with releases
- Easy navigation between versions
- Default to latest version
- Archive older versions

## Decision

**Docusaurus versioned docs** aligned with releases.

### Implementation

Docusaurus built-in versioning:

```bash
# Create version snapshot
npm run docusaurus docs:version 1.0.0
```

Creates:

```text
docs/docusaurus/
├── docs/                    # Next/unreleased
├── versioned_docs/
│   ├── version-1.0.0/
│   └── version-1.1.0/
├── versions.json            # Version list
└── versioned_sidebars/
```

### Version Strategy

| Version  | Docs Location                   | When Created      |
| -------- | ------------------------------- | ----------------- |
| Next     | `docs/`                         | Ongoing           |
| Latest   | `versioned_docs/version-x.y.z/` | Each release      |
| Archived | `versioned_docs/version-x.y.z/` | Previous releases |

### Release Workflow

1. Update `docs/` with new content
2. Create version snapshot during release
3. Deploy updated site to GitHub Pages

### Navigation

- Version dropdown in header
- "Latest" badge on current version
- Banner on older versions: "This is documentation for version X.Y.Z"

## Consequences

### Positive

- Users find version-appropriate docs
- History preserved
- Clear upgrade paths

### Negative

- Doc maintenance across versions
- Storage for versioned content
- Complexity in doc site

## References

- [Docusaurus Versioning](https://docusaurus.io/docs/versioning)
