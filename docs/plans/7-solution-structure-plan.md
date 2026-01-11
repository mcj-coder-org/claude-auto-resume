# Phase 4a: Solution Structure Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Create the .NET 10 solution structure with proper project configuration, analyzers, and public API tracking.

**Architecture:** Standard .NET solution with src/ and tests/ separation. Main project is a .NET tool (`PackAsTool`). Test projects mirror production namespace for discoverability. Centralized package versioning via Directory.Packages.props.

**Tech Stack:** .NET 10, C# 14, xUnit, Meziantou.Analyzer, Roslynator, SonarAnalyzer, PublicApiAnalyzers

**Issue:** #7
**Branch:** `feature/7-solution-structure`
**Base:** `main`

---

## Prerequisites

Before starting, verify:

- .NET 10 SDK installed: `dotnet --version` should show 10.x
- Node.js installed (for pre-commit hooks): `node --version`
- npm packages installed: `npm install`
- Current branch is `main`: `git branch --show-current`

---

## Task 1: Create Feature Branch

**Step 1: Ensure clean working directory**

```bash
git status
```

Expected: Working tree clean

**Step 2: Create and switch to feature branch**

```bash
git checkout -b feature/7-solution-structure
```

Expected: Switched to new branch 'feature/7-solution-structure'

**Step 3: Post starting comment on issue**

```bash
gh issue comment 7 --body "Starting work on solution structure.

**Branch:** \`feature/7-solution-structure\`

**Tasks:**
- [ ] Create solution file
- [ ] Create Directory.Build.props
- [ ] Create Directory.Packages.props
- [ ] Create main project
- [ ] Create test project placeholders
- [ ] Create PublicAPI tracking files
- [ ] Verify build"
```

---

## Task 2: Create Solution File

**Files:**

- Create: `McjCoderOrg.ClaudeAutoResume.sln`

**Step 1: Create solution file**

```bash
dotnet new sln --name McjCoderOrg.ClaudeAutoResume
```

Expected: Creates McjCoderOrg.ClaudeAutoResume.sln

**Step 2: Verify solution file exists**

```bash
dir McjCoderOrg.ClaudeAutoResume.sln
```

Expected: File exists

**Step 3: Commit**

```bash
git add McjCoderOrg.ClaudeAutoResume.sln
git commit -m "build: create solution file

Refs: #7"
```

---

## Task 3: Create Directory Structure

**Files:**

- Create: `src/` directory
- Create: `tests/` directory

**Step 1: Create directories**

```bash
mkdir src
mkdir tests
```

**Step 2: Add .gitkeep files (so empty dirs are tracked)**

```bash
echo. > src\.gitkeep
echo. > tests\.gitkeep
```

**Step 3: Commit**

```bash
git add src/ tests/
git commit -m "build: add src and tests directories

Refs: #7"
```

---

## Task 4: Create Directory.Build.props

**Files:**

- Create: `Directory.Build.props`

**Step 1: Create Directory.Build.props**

Create file `Directory.Build.props` with:

```xml
<Project>
  <PropertyGroup>
    <!-- .NET 10 / C# 14 -->
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>14.0</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <!-- Strict Warnings -->
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>

    <!-- Documentation -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>

    <!-- Source Link -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>

  <!-- Analyzers for all projects -->
  <ItemGroup>
    <PackageReference Include="Meziantou.Analyzer" PrivateAssets="all" />
    <PackageReference Include="Roslynator.Analyzers" PrivateAssets="all" />
    <PackageReference Include="SonarAnalyzer.CSharp" PrivateAssets="all" />
  </ItemGroup>

  <!-- Source Link -->
  <ItemGroup>
    <PackageReference Include="Microsoft.SourceLink.GitHub" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

**Step 2: Verify XML is well-formed**

```bash
dotnet msbuild -pp:pp.xml Directory.Build.props 2>&1 | findstr /i error
```

Expected: No errors (may show warnings about missing projects, that's OK)

**Step 3: Commit**

```bash
git add Directory.Build.props
git commit -m "build: add Directory.Build.props with .NET 10 and analyzers

- Configure .NET 10 / C# 14
- Enable strict warnings as errors
- Add Meziantou, Roslynator, SonarAnalyzer
- Configure Source Link

Refs: #7"
```

---

## Task 5: Create Directory.Packages.props

**Files:**

- Create: `Directory.Packages.props`

**Step 1: Create Directory.Packages.props**

Create file `Directory.Packages.props` with:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- Analyzers -->
    <PackageVersion Include="Meziantou.Analyzer" Version="2.0.182" />
    <PackageVersion Include="Roslynator.Analyzers" Version="4.12.9" />
    <PackageVersion Include="SonarAnalyzer.CSharp" Version="10.4.0.108396" />
    <PackageVersion Include="Microsoft.CodeAnalysis.PublicApiAnalyzers" Version="3.3.4" />

    <!-- Source Link -->
    <PackageVersion Include="Microsoft.SourceLink.GitHub" Version="8.0.0" />

    <!-- Testing -->
    <PackageVersion Include="xunit" Version="2.9.2" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="3.0.0" />
    <PackageVersion Include="xunit.analyzers" Version="1.18.0" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageVersion Include="coverlet.collector" Version="6.0.2" />
    <PackageVersion Include="FluentAssertions" Version="8.0.1" />
    <PackageVersion Include="Moq" Version="4.20.72" />
    <PackageVersion Include="Moq.Analyzers" Version="0.0.9" />

    <!-- BDD (Reqnroll) -->
    <PackageVersion Include="Reqnroll" Version="2.2.1" />
    <PackageVersion Include="Reqnroll.xUnit" Version="2.2.1" />

    <!-- Architecture Testing -->
    <PackageVersion Include="NetArchTest.eNhancedEdition" Version="1.0.3" />

    <!-- Benchmarking -->
    <PackageVersion Include="BenchmarkDotNet" Version="0.14.0" />
  </ItemGroup>
</Project>
```

**Step 2: Commit**

```bash
git add Directory.Packages.props
git commit -m "build: add Directory.Packages.props for central package versioning

Refs: #7"
```

---

## Task 6: Create Main Project

**Files:**

- Create: `src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`
- Create: `src/McjCoderOrg.ClaudeAutoResume/Program.cs`
- Create: `src/McjCoderOrg.ClaudeAutoResume/PublicAPI.Shipped.txt`
- Create: `src/McjCoderOrg.ClaudeAutoResume/PublicAPI.Unshipped.txt`

**Step 1: Create project directory**

```bash
mkdir src\McjCoderOrg.ClaudeAutoResume
```

**Step 2: Create project file**

Create file `src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume</AssemblyName>

    <!-- .NET Tool Configuration -->
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>claude-auto-resume</ToolCommandName>

    <!-- Package Metadata -->
    <PackageId>McjCoderOrg.ClaudeAutoResume</PackageId>
    <Title>Claude Auto Resume</Title>
    <Description>CLI tool to monitor Claude Code sessions and automatically resume on rate limits.</Description>
    <Authors>McjCoderOrg</Authors>
    <PackageProjectUrl>https://github.com/mcj-coder-org/claude-auto-resume</PackageProjectUrl>
    <RepositoryUrl>https://github.com/mcj-coder-org/claude-auto-resume.git</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <PackageTags>claude;ai;cli;auto-resume</PackageTags>
  </PropertyGroup>

  <!-- Expose internals to Unit and System tests -->
  <ItemGroup>
    <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.Tests" />
    <InternalsVisibleTo Include="McjCoderOrg.ClaudeAutoResume.SystemTests" />
  </ItemGroup>

  <!-- Public API Analyzer -->
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.PublicApiAnalyzers" PrivateAssets="all" />
  </ItemGroup>

  <!-- Package README -->
  <ItemGroup>
    <None Include="..\..\README.md" Pack="true" PackagePath="\" />
  </ItemGroup>

  <!-- Public API Tracking Files -->
  <ItemGroup>
    <AdditionalFiles Include="PublicAPI.Shipped.txt" />
    <AdditionalFiles Include="PublicAPI.Unshipped.txt" />
  </ItemGroup>
</Project>
```

**Step 3: Create minimal Program.cs**

Create file `src/McjCoderOrg.ClaudeAutoResume/Program.cs` with:

```csharp
// Placeholder - will be replaced in Phase 7 (Code Migration)
Console.WriteLine("Claude Auto Resume - Placeholder");
```

**Step 4: Create PublicAPI tracking files**

Create empty file `src/McjCoderOrg.ClaudeAutoResume/PublicAPI.Shipped.txt` with:

```text
#nullable enable
```

Create empty file `src/McjCoderOrg.ClaudeAutoResume/PublicAPI.Unshipped.txt` with:

```text
#nullable enable
```

**Step 5: Add project to solution**

```bash
dotnet sln add src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj
```

**Step 6: Verify build**

```bash
dotnet build src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj
```

Expected: Build succeeded

**Step 7: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/
git commit -m "feat: add main project with tool configuration

- Configure as .NET tool (PackAsTool)
- Add package metadata
- Configure InternalsVisibleTo for test projects
- Add PublicAPI tracking files

Refs: #7"
```

---

## Task 7: Create Unit Test Project

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/PlaceholderTests.cs`

**Step 1: Create project directory**

```bash
mkdir tests\McjCoderOrg.ClaudeAutoResume.Tests
```

**Step 2: Create project file**

Create file `tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Namespace matches production code for discoverability -->
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.Tests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>

    <!-- Suppress XML doc warnings for tests -->
    <NoWarn>$(NoWarn);CS1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" PrivateAssets="all" />
    <PackageReference Include="xunit.analyzers" PrivateAssets="all" />
    <PackageReference Include="coverlet.collector" PrivateAssets="all" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="Moq" />
    <PackageReference Include="Moq.Analyzers" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\McjCoderOrg.ClaudeAutoResume\McjCoderOrg.ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

**Step 3: Create placeholder test**

Create file `tests/McjCoderOrg.ClaudeAutoResume.Tests/PlaceholderTests.cs` with:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Placeholder test to verify test infrastructure works.
/// Will be replaced with real tests in Phase 7.
/// </summary>
public sealed class PlaceholderTests
{
    [Fact]
    public void Placeholder_ShouldPass()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        actual.Should().Be(expected);
    }
}
```

**Step 4: Add project to solution**

```bash
dotnet sln add tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj
```

**Step 5: Verify tests run**

```bash
dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj
```

Expected: 1 test passed

**Step 6: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.Tests/
git commit -m "test: add unit test project placeholder

Refs: #7"
```

---

## Task 8: Create System Test Project (BDD)

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/McjCoderOrg.ClaudeAutoResume.SystemTests.csproj`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/Features/.gitkeep`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/StepDefinitions/.gitkeep`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/Support/.gitkeep`

**Step 1: Create project directory structure**

```bash
mkdir tests\McjCoderOrg.ClaudeAutoResume.SystemTests
mkdir tests\McjCoderOrg.ClaudeAutoResume.SystemTests\Features
mkdir tests\McjCoderOrg.ClaudeAutoResume.SystemTests\StepDefinitions
mkdir tests\McjCoderOrg.ClaudeAutoResume.SystemTests\Support
```

**Step 2: Create project file**

Create file `tests/McjCoderOrg.ClaudeAutoResume.SystemTests/McjCoderOrg.ClaudeAutoResume.SystemTests.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Namespace matches production code -->
    <RootNamespace>McjCoderOrg.ClaudeAutoResume</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.SystemTests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>

    <!-- Suppress XML doc warnings for tests -->
    <NoWarn>$(NoWarn);CS1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" PrivateAssets="all" />
    <PackageReference Include="xunit.analyzers" PrivateAssets="all" />
    <PackageReference Include="coverlet.collector" PrivateAssets="all" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="Reqnroll" />
    <PackageReference Include="Reqnroll.xUnit" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\McjCoderOrg.ClaudeAutoResume\McjCoderOrg.ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

**Step 3: Add .gitkeep files**

```bash
echo. > tests\McjCoderOrg.ClaudeAutoResume.SystemTests\Features\.gitkeep
echo. > tests\McjCoderOrg.ClaudeAutoResume.SystemTests\StepDefinitions\.gitkeep
echo. > tests\McjCoderOrg.ClaudeAutoResume.SystemTests\Support\.gitkeep
```

**Step 4: Add project to solution**

```bash
dotnet sln add tests/McjCoderOrg.ClaudeAutoResume.SystemTests/McjCoderOrg.ClaudeAutoResume.SystemTests.csproj
```

**Step 5: Verify build**

```bash
dotnet build tests/McjCoderOrg.ClaudeAutoResume.SystemTests/McjCoderOrg.ClaudeAutoResume.SystemTests.csproj
```

Expected: Build succeeded

**Step 6: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.SystemTests/
git commit -m "test: add system test project with BDD structure

Refs: #7"
```

---

## Task 9: Create E2E Test Project

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/.gitkeep`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/StepDefinitions/.gitkeep`

**Step 1: Create project directory structure**

```bash
mkdir tests\McjCoderOrg.ClaudeAutoResume.E2ETests
mkdir tests\McjCoderOrg.ClaudeAutoResume.E2ETests\Features
mkdir tests\McjCoderOrg.ClaudeAutoResume.E2ETests\StepDefinitions
```

**Step 2: Create project file**

Create file `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- E2E tests have their own namespace - black-box testing -->
    <RootNamespace>McjCoderOrg.ClaudeAutoResume.E2ETests</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.E2ETests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>

    <!-- Suppress XML doc warnings for tests -->
    <NoWarn>$(NoWarn);CS1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" PrivateAssets="all" />
    <PackageReference Include="xunit.analyzers" PrivateAssets="all" />
    <PackageReference Include="coverlet.collector" PrivateAssets="all" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="Reqnroll" />
    <PackageReference Include="Reqnroll.xUnit" />
  </ItemGroup>

  <!-- NOTE: No project reference - E2E tests use public API only -->
</Project>
```

**Step 3: Add .gitkeep files**

```bash
echo. > tests\McjCoderOrg.ClaudeAutoResume.E2ETests\Features\.gitkeep
echo. > tests\McjCoderOrg.ClaudeAutoResume.E2ETests\StepDefinitions\.gitkeep
```

**Step 4: Add project to solution**

```bash
dotnet sln add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj
```

**Step 5: Verify build**

```bash
dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj
```

Expected: Build succeeded

**Step 6: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/
git commit -m "test: add E2E test project (black-box testing)

Refs: #7"
```

---

## Task 10: Create Architecture Test Project

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.ArchTests/McjCoderOrg.ClaudeAutoResume.ArchTests.csproj`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.ArchTests/PlaceholderArchTests.cs`

**Step 1: Create project directory**

```bash
mkdir tests\McjCoderOrg.ClaudeAutoResume.ArchTests
```

**Step 2: Create project file**

Create file `tests/McjCoderOrg.ClaudeAutoResume.ArchTests/McjCoderOrg.ClaudeAutoResume.ArchTests.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>McjCoderOrg.ClaudeAutoResume.ArchTests</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.ArchTests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>

    <!-- Suppress XML doc warnings for tests -->
    <NoWarn>$(NoWarn);CS1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" PrivateAssets="all" />
    <PackageReference Include="xunit.analyzers" PrivateAssets="all" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="NetArchTest.eNhancedEdition" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\McjCoderOrg.ClaudeAutoResume\McjCoderOrg.ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

**Step 3: Create placeholder architecture test**

Create file `tests/McjCoderOrg.ClaudeAutoResume.ArchTests/PlaceholderArchTests.cs` with:

```csharp
using NetArchTest.Rules;

namespace McjCoderOrg.ClaudeAutoResume.ArchTests;

/// <summary>
/// Placeholder architecture test to verify infrastructure works.
/// Will be replaced with real tests in Phase 7.
/// </summary>
public sealed class PlaceholderArchTests
{
    [Fact]
    public void MainAssembly_ShouldExist()
    {
        // Arrange
        var assembly = typeof(Program).Assembly;

        // Act
        var types = Types.InAssembly(assembly);

        // Assert
        types.Should().NotBeNull();
    }
}
```

**Step 4: Add project to solution**

```bash
dotnet sln add tests/McjCoderOrg.ClaudeAutoResume.ArchTests/McjCoderOrg.ClaudeAutoResume.ArchTests.csproj
```

**Step 5: Verify tests run**

```bash
dotnet test tests/McjCoderOrg.ClaudeAutoResume.ArchTests/McjCoderOrg.ClaudeAutoResume.ArchTests.csproj
```

Expected: 1 test passed

**Step 6: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.ArchTests/
git commit -m "test: add architecture test project

Refs: #7"
```

---

## Task 11: Create Benchmark Project

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/McjCoderOrg.ClaudeAutoResume.Benchmarks.csproj`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/PlaceholderBenchmarks.cs`

**Step 1: Create project directory**

```bash
mkdir tests\McjCoderOrg.ClaudeAutoResume.Benchmarks
```

**Step 2: Create project file**

Create file `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/McjCoderOrg.ClaudeAutoResume.Benchmarks.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <RootNamespace>McjCoderOrg.ClaudeAutoResume.Benchmarks</RootNamespace>
    <AssemblyName>McjCoderOrg.ClaudeAutoResume.Benchmarks</AssemblyName>
    <IsPackable>false</IsPackable>

    <!-- Not a test project - benchmarks are run manually -->
    <IsTestProject>false</IsTestProject>

    <!-- Suppress XML doc warnings -->
    <NoWarn>$(NoWarn);CS1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\McjCoderOrg.ClaudeAutoResume\McjCoderOrg.ClaudeAutoResume.csproj" />
  </ItemGroup>
</Project>
```

**Step 3: Create placeholder benchmark**

Create file `tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/PlaceholderBenchmarks.cs` with:

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace McjCoderOrg.ClaudeAutoResume.Benchmarks;

/// <summary>
/// Placeholder benchmark to verify infrastructure works.
/// Will be replaced with real benchmarks in Phase 7.
/// </summary>
[MemoryDiagnoser]
public class PlaceholderBenchmarks
{
    [Benchmark]
    public int Placeholder()
    {
        return 1 + 1;
    }
}

/// <summary>
/// Entry point for running benchmarks.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
```

**Step 4: Add project to solution**

```bash
dotnet sln add tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/McjCoderOrg.ClaudeAutoResume.Benchmarks.csproj
```

**Step 5: Verify build**

```bash
dotnet build tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/McjCoderOrg.ClaudeAutoResume.Benchmarks.csproj
```

Expected: Build succeeded

**Step 6: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.Benchmarks/
git commit -m "perf: add benchmark project

Refs: #7"
```

---

## Task 12: Clean Up and Final Verification

**Step 1: Remove .gitkeep files from src/ (now has content)**

```bash
del src\.gitkeep
```

**Step 2: Verify full solution build**

```bash
dotnet build McjCoderOrg.ClaudeAutoResume.sln
```

Expected: Build succeeded with no errors

**Step 3: Run all tests**

```bash
dotnet test McjCoderOrg.ClaudeAutoResume.sln
```

Expected: 2 tests passed (PlaceholderTests + PlaceholderArchTests)

**Step 4: Verify analyzer warnings as errors**

```bash
dotnet build McjCoderOrg.ClaudeAutoResume.sln -warnaserror
```

Expected: Build succeeded

**Step 5: Commit cleanup**

```bash
git add -A
git commit -m "chore: clean up solution structure

Refs: #7"
```

**Step 6: Push branch**

```bash
git push -u origin feature/7-solution-structure
```

---

## Task 13: Create Pull Request

**Step 1: Create PR using gh CLI**

```bash
gh pr create --title "feat(build): add .NET 10 solution structure" --body "## Summary

Creates the .NET 10 solution structure with proper project configuration.

## Changes

- Created solution file: McjCoderOrg.ClaudeAutoResume.sln
- Created Directory.Build.props with .NET 10/C# 14 and analyzers
- Created Directory.Packages.props for central versioning
- Created main project with tool configuration
- Created test projects (Unit, System, E2E, Arch, Benchmarks)
- Configured PublicAPI tracking

## Test Plan

- [x] Solution builds successfully
- [x] All tests pass
- [x] Analyzers enabled with warnings as errors
- [x] Public API tracking configured

Closes #7"
```

**Step 2: Request review**

Note: As Contributor, I cannot assign reviewers. The Tech Lead (Owner) will assign reviewers per ticket-lifecycle.md.

---

## Post-Implementation Checklist

- [ ] Branch created: `feature/7-solution-structure`
- [ ] All tasks committed with conventional commits
- [ ] Solution builds with no errors
- [ ] All tests pass
- [ ] Analyzers enforced (warnings as errors)
- [ ] PublicAPI tracking configured
- [ ] PR created and linked to issue #7
- [ ] Starting comment posted on issue #7
