---
name: namespace-and-project-naming
description: |
  When creating new projects, namespaces, or test assemblies. Apply when naming solutions,
  configuring RootNamespace, setting up InternalsVisibleTo, or structuring test projects.
decision: Use McjCoderOrg prefix with folder-parallel namespaces and test projects sharing production namespace.
status: accepted
type: process
---

# ADR-0012: Namespace and Project Naming Conventions

## Status

Proposed

## Date

2026-01-09

## Context

We need consistent naming conventions for:

1. Solution and project names
2. Root namespaces
3. Folder-to-namespace mapping
4. Test project namespace alignment with production code
5. Internal member visibility for testing

### Requirements

- Organization prefix for brand consistency and NuGet package uniqueness
- Parity between repository name, solution name, and package ID
- Test classes in same namespace as classes they test (for discoverability)
- Unit and System tests can access internal members
- E2E tests only access public API (black-box testing)

## Decision

### Naming Convention

**Organization Prefix:** `McjCoderOrg`

| Item                | Convention                    | Example                                    |
| ------------------- | ----------------------------- | ------------------------------------------ |
| Repository          | `{org}.{project}`             | `McjCoderOrg.ClaudeAutoResume`             |
| Solution file       | `{org}.{project}.sln`         | `McjCoderOrg.ClaudeAutoResume.sln`         |
| Main project        | `{org}.{project}`             | `McjCoderOrg.ClaudeAutoResume`             |
| Unit test project   | `{org}.{project}.Tests`       | `McjCoderOrg.ClaudeAutoResume.Tests`       |
| System test project | `{org}.{project}.SystemTests` | `McjCoderOrg.ClaudeAutoResume.SystemTests` |
| E2E test project    | `{org}.{project}.E2ETests`    | `McjCoderOrg.ClaudeAutoResume.E2ETests`    |
| NuGet Package ID    | `{org}.{project}`             | `McjCoderOrg.ClaudeAutoResume`             |

### Namespace Convention

**Folder-parallel namespaces:** Namespaces mirror the folder structure within each project.

**Main Project:**

```text
src/McjCoderOrg.ClaudeAutoResume/
├── Program.cs                    → McjCoderOrg.ClaudeAutoResume
├── ClaudeMonitor.cs              → McjCoderOrg.ClaudeAutoResume
├── WrapperConfig.cs              → McjCoderOrg.ClaudeAutoResume
├── Configuration/
│   └── ConfigLoader.cs           → McjCoderOrg.ClaudeAutoResume.Configuration
└── Pty/
    └── PtyConnection.cs          → McjCoderOrg.ClaudeAutoResume.Pty
```

**Test Projects (RootNamespace omits "Tests" suffix):**

```text
tests/McjCoderOrg.ClaudeAutoResume.Tests/
├── WrapperConfigTests.cs         → McjCoderOrg.ClaudeAutoResume
├── ClaudeMonitorTests.cs         → McjCoderOrg.ClaudeAutoResume
├── Configuration/
│   └── ConfigLoaderTests.cs      → McjCoderOrg.ClaudeAutoResume.Configuration
└── Pty/
    └── PtyConnectionTests.cs     → McjCoderOrg.ClaudeAutoResume.Pty
```

This allows test classes to reside in the **same namespace** as the classes they test, enabling:

- Access to `internal` members (with `InternalsVisibleTo`)
- Natural discoverability (test next to implementation in IDE namespace view)
- Simpler using statements

### Project Configuration

**Main Project (`src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume</AssemblyName>

    <!-- Expose internals to Unit and System tests -->
    <InternalsVisibleTo>McjCoderOrg.ClaudeAutoResume.Tests</InternalsVisibleTo>
    <InternalsVisibleTo>McjCoderOrg.ClaudeAutoResume.SystemTests</InternalsVisibleTo>

    <!-- .NET Tool Configuration -->
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>claude-auto-resume</ToolCommandName>

    <!-- Package Metadata -->
    <PackageId>McjCoderOrg.ClaudeAutoResume</PackageId>
    <!-- ... other metadata ... -->
  </PropertyGroup>
</Project>
```

**Unit Test Project (`tests/McjCoderOrg.ClaudeAutoResume.Tests/...csproj`):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Omit ".Tests" from namespace - tests in same namespace as code -->
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.Tests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
</Project>
```

**System Test Project (`tests/McjCoderOrg.ClaudeAutoResume.SystemTests/...csproj`):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Omit ".SystemTests" from namespace -->
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.SystemTests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
</Project>
```

**E2E Test Project (`tests/McjCoderOrg.ClaudeAutoResume.E2ETests/...csproj`):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- E2E tests have their own namespace - black-box testing -->
    <RootNamespace>McjCoderOrg.ClaudeAutoResume.E2ETests</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.E2ETests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
</Project>
```

### InternalsVisibleTo Configuration

Using `<InternalsVisibleTo>` in the project file (modern approach, no AssemblyInfo.cs needed):

```xml
<!-- In main project csproj -->
<ItemGroup>
  <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.Tests" />
  <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.SystemTests" />
  <!-- E2E tests deliberately excluded - public API only -->
</ItemGroup>
```

### Test Visibility Matrix

| Test Project   | Namespace          | Can Access Internals | Testing Style          |
| -------------- | ------------------ | -------------------- | ---------------------- |
| `.Tests`       | Same as production | Yes                  | White-box unit testing |
| `.SystemTests` | Same as production | Yes                  | White-box integration  |
| `.E2ETests`    | Own namespace      | No                   | Black-box, public API  |

### Folder Structure

```text
McjCoderOrg.ClaudeAutoResume/
├── src/
│   └── McjCoderOrg.ClaudeAutoResume/
│       ├── McjCoderOrg.ClaudeAutoResume.csproj
│       ├── Program.cs
│       ├── ClaudeMonitor.cs
│       └── WrapperConfig.cs
├── tests/
│   ├── McjCoderOrg.ClaudeAutoResume.Tests/
│   │   ├── McjCoderOrg.ClaudeAutoResume.Tests.csproj
│   │   └── WrapperConfigTests.cs
│   ├── McjCoderOrg.ClaudeAutoResume.SystemTests/
│   │   ├── McjCoderOrg.ClaudeAutoResume.SystemTests.csproj
│   │   ├── Features/
│   │   └── StepDefinitions/
│   └── McjCoderOrg.ClaudeAutoResume.E2ETests/
│       ├── McjCoderOrg.ClaudeAutoResume.E2ETests.csproj
│       ├── Features/
│       └── StepDefinitions/
├── docs/
├── McjCoderOrg.ClaudeAutoResume.sln
└── ...
```

## Consequences

### Positive

- Consistent naming across repo, solution, packages, and namespaces
- Organization prefix prevents NuGet package name collisions
- Test classes in same namespace as production code improves discoverability
- Unit/System tests can test internal implementation details
- E2E tests enforce public API contract stability
- Folder-parallel namespaces are predictable and IDE-friendly

### Negative

- Longer fully-qualified names
- Initial migration effort to rename existing files/namespaces
- Must remember to add new test projects to `InternalsVisibleTo` if created

### Risks

- Forgetting to update `InternalsVisibleTo` when adding new test projects
- E2E tests accidentally depending on internal behavior (mitigated by separate namespace)

## References

- [.NET Naming Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
- [InternalsVisibleTo Attribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.internalsvisibletoattribute)
- [MSBuild InternalsVisibleTo](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#internalsvisibleto)
