# ADR-0027: Release Artifacts

## Status

Proposed

## Date

2026-01-09

## Context

We need to define release artifacts for:
1. NuGet distribution (dotnet tool)
2. Standalone executables
3. Security and verification

### Requirements

- NuGet package with Source Link
- Cross-platform standalone builds
- SBOM for supply chain security
- Checksums for verification

## Decision

Comprehensive release artifacts with Source Link and SBOM.

### Artifacts

| Artifact | Description |
|----------|-------------|
| `McjCoderOrg.ClaudeAutoResume.x.y.z.nupkg` | NuGet package |
| `McjCoderOrg.ClaudeAutoResume.x.y.z.snupkg` | Symbol package |
| `win-x64/claude-auto-resume.exe` | Windows x64 |
| `linux-x64/claude-auto-resume` | Linux x64 |
| `osx-x64/claude-auto-resume` | macOS Intel |
| `osx-arm64/claude-auto-resume` | macOS Apple Silicon |
| `checksums.sha256` | SHA256 checksums |
| `manifest.spdx.json` | SBOM |

### Source Link

NuGet package includes Source Link for debugging:

```xml
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.SourceLink.GitHub" PrivateAssets="all" />
</ItemGroup>
```

### SBOM Generation

Using Microsoft SBOM tool:
```bash
dotnet tool install --global Microsoft.Sbom.DotNetTool
sbom-tool generate -b ./artifacts -bc . -pn ClaudeAutoResume -pv $VERSION
```

### Build Matrix

```yaml
strategy:
  matrix:
    include:
      - os: windows-latest
        rid: win-x64
      - os: ubuntu-latest
        rid: linux-x64
      - os: macos-latest
        rid: osx-x64
      - os: macos-latest
        rid: osx-arm64
```

Each build uses `continue-on-error: true` with final verification step.

## Consequences

### Positive
- Debugging enabled via Source Link
- Supply chain transparency (SBOM)
- Verification via checksums
- Cross-platform support

### Negative
- Build matrix complexity
- Artifact storage costs
- SBOM tooling overhead

## References

- [Source Link](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink)
- [Microsoft SBOM Tool](https://github.com/microsoft/sbom-tool)
