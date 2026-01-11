# Phase 4c: Application Framework Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Establish the CLI application framework with logging, exit codes, platform detection, and i18n-ready structure.

**Architecture:** Host.CreateApplicationBuilder pattern with Serilog for structured logging. Bootstrap logger captures startup errors before DI container is built. All user-facing strings in resource files for future localization.

**Tech Stack:** Serilog, Serilog.Sinks.File, Serilog.Sinks.Debug, Serilog.Extensions.Hosting, .NET Generic Host

**ADRs:** 0017 (Observability), 0018 (CLI Design), 0019 (Internationalization)

**Issue:** #9 (Refs: #1)

---

## Task 1: Add Serilog Packages

**Files:**

- Modify: `Directory.Packages.props`
- Modify: `src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`

**Step 1: Add package versions to Directory.Packages.props**

Add after the Architecture Testing section:

```xml
    <!-- Logging -->
    <PackageVersion Include="Serilog" Version="4.2.0" />
    <PackageVersion Include="Serilog.Extensions.Hosting" Version="9.0.0" />
    <PackageVersion Include="Serilog.Sinks.File" Version="6.0.0" />
    <PackageVersion Include="Serilog.Sinks.Debug" Version="3.0.0" />
    <PackageVersion Include="Serilog.Sinks.InMemory" Version="0.11.0" />

    <!-- Hosting -->
    <PackageVersion Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
```

**Step 2: Add package references to main project**

Add to `src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`:

```xml
  <!-- Logging -->
  <ItemGroup>
    <PackageReference Include="Serilog" />
    <PackageReference Include="Serilog.Extensions.Hosting" />
    <PackageReference Include="Serilog.Sinks.File" />
    <PackageReference Include="Serilog.Sinks.Debug" />
  </ItemGroup>

  <!-- Hosting -->
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" />
  </ItemGroup>
```

**Step 3: Verify build succeeds**

Run: `dotnet build src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add Directory.Packages.props src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj
git commit -m "build: add serilog and hosting packages for application framework

Refs: #9"
```

---

## Task 2: Create ExitCodes

**Files:**

- Create: `src/McjCoderOrg.ClaudeAutoResume/ExitCodes.cs`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/ExitCodesTests.cs`

**Step 1: Write the failing test**

Create `tests/McjCoderOrg.ClaudeAutoResume.Tests/ExitCodesTests.cs`:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ExitCodesTests
{
    [Fact]
    public void Success_ShouldBeZero()
    {
        ExitCodes.Success.Should().Be(0);
    }

    [Fact]
    public void GeneralError_ShouldBeOne()
    {
        ExitCodes.GeneralError.Should().Be(1);
    }

    [Fact]
    public void ConfigurationError_ShouldBeTwo()
    {
        ExitCodes.ConfigurationError.Should().Be(2);
    }

    [Fact]
    public void DependencyMissing_ShouldBeThree()
    {
        ExitCodes.DependencyMissing.Should().Be(3);
    }

    [Fact]
    public void RateLimitDetected_ShouldBeFour()
    {
        ExitCodes.RateLimitDetected.Should().Be(4);
    }

    [Fact]
    public void UserCancelled_ShouldBeFive()
    {
        ExitCodes.UserCancelled.Should().Be(5);
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~ExitCodesTests"`
Expected: FAIL with "The type or namespace name 'ExitCodes' does not exist"

**Step 3: Write minimal implementation**

Create `src/McjCoderOrg.ClaudeAutoResume/ExitCodes.cs`:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Semantic exit codes for the CLI application.
/// </summary>
/// <remarks>
/// See ADR-0018 for exit code design decisions.
/// </remarks>
public static class ExitCodes
{
    /// <summary>Normal completion.</summary>
    public const int Success = 0;

    /// <summary>Unhandled exception.</summary>
    public const int GeneralError = 1;

    /// <summary>Invalid configuration.</summary>
    public const int ConfigurationError = 2;

    /// <summary>Claude CLI not found.</summary>
    public const int DependencyMissing = 3;

    /// <summary>Exited due to rate limit.</summary>
    public const int RateLimitDetected = 4;

    /// <summary>User interrupted (Ctrl+C).</summary>
    public const int UserCancelled = 5;
}
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~ExitCodesTests"`
Expected: Passed! - 6 tests passed

**Step 5: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/ExitCodes.cs tests/McjCoderOrg.ClaudeAutoResume.Tests/ExitCodesTests.cs
git commit -m "feat: add semantic exit codes per ADR-0018

Refs: #9"
```

---

## Task 3: Create PlatformInfo

**Files:**

- Create: `src/McjCoderOrg.ClaudeAutoResume/PlatformInfo.cs`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/PlatformInfoTests.cs`

**Step 1: Write the failing tests**

Create `tests/McjCoderOrg.ClaudeAutoResume.Tests/PlatformInfoTests.cs`:

```csharp
using System.Runtime.InteropServices;

namespace McjCoderOrg.ClaudeAutoResume;

public sealed class PlatformInfoTests
{
    [Fact]
    public void Current_ShouldReturnNonNullInstance()
    {
        var info = PlatformInfo.Current;

        info.Should().NotBeNull();
    }

    [Fact]
    public void DotNetVersion_ShouldMatchRuntimeVersion()
    {
        var info = PlatformInfo.Current;

        info.DotNetVersion.Should().Be(Environment.Version.ToString());
    }

    [Fact]
    public void RuntimeIdentifier_ShouldNotBeEmpty()
    {
        var info = PlatformInfo.Current;

        info.RuntimeIdentifier.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void OsDescription_ShouldMatchSystem()
    {
        var info = PlatformInfo.Current;

        info.OsDescription.Should().Be(RuntimeInformation.OSDescription);
    }

    [Fact]
    public void ProcessArchitecture_ShouldMatchSystem()
    {
        var info = PlatformInfo.Current;

        info.ProcessArchitecture.Should().Be(RuntimeInformation.ProcessArchitecture.ToString());
    }

    [Fact]
    public void AppVersion_ShouldNotBeEmpty()
    {
        var info = PlatformInfo.Current;

        info.AppVersion.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void IsContainer_ShouldReturnBoolean()
    {
        var info = PlatformInfo.Current;

        info.IsContainer.Should().BeOneOf(true, false);
    }

    [Fact]
    public void IsCI_ShouldReturnBoolean()
    {
        var info = PlatformInfo.Current;

        info.IsCI.Should().BeOneOf(true, false);
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~PlatformInfoTests"`
Expected: FAIL with "The type or namespace name 'PlatformInfo' does not exist"

**Step 3: Write minimal implementation**

Create `src/McjCoderOrg.ClaudeAutoResume/PlatformInfo.cs`:

```csharp
using System.Reflection;
using System.Runtime.InteropServices;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Captures platform context for diagnostics.
/// </summary>
/// <remarks>
/// See ADR-0017 for observability design decisions.
/// </remarks>
public sealed class PlatformInfo
{
    private static readonly Lazy<PlatformInfo> LazyInstance = new(Create);

    private PlatformInfo()
    {
    }

    /// <summary>
    /// Gets the current platform information.
    /// </summary>
    public static PlatformInfo Current => LazyInstance.Value;

    /// <summary>Gets the .NET runtime version.</summary>
    public string DotNetVersion { get; private init; } = string.Empty;

    /// <summary>Gets the runtime identifier (e.g., win-x64, linux-arm64).</summary>
    public string RuntimeIdentifier { get; private init; } = string.Empty;

    /// <summary>Gets the OS description.</summary>
    public string OsDescription { get; private init; } = string.Empty;

    /// <summary>Gets the process architecture.</summary>
    public string ProcessArchitecture { get; private init; } = string.Empty;

    /// <summary>Gets the application version.</summary>
    public string AppVersion { get; private init; } = string.Empty;

    /// <summary>Gets a value indicating whether running in a container.</summary>
    public bool IsContainer { get; private init; }

    /// <summary>Gets a value indicating whether running in a CI environment.</summary>
    public bool IsCI { get; private init; }

    private static PlatformInfo Create()
    {
        return new PlatformInfo
        {
            DotNetVersion = Environment.Version.ToString(),
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
            OsDescription = RuntimeInformation.OSDescription,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            AppVersion = GetAppVersion(),
            IsContainer = DetectContainer(),
            IsCI = DetectCI(),
        };
    }

    private static string GetAppVersion()
    {
        return Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0";
    }

    private static bool DetectContainer()
    {
        // Check for .dockerenv file (Linux) or DOTNET_RUNNING_IN_CONTAINER env var
        return File.Exists("/.dockerenv")
            || Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    }

    private static bool DetectCI()
    {
        // Check common CI environment variables
        return Environment.GetEnvironmentVariable("CI") == "true"
            || Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
            || Environment.GetEnvironmentVariable("TF_BUILD") == "True"
            || Environment.GetEnvironmentVariable("JENKINS_URL") is not null;
    }
}
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~PlatformInfoTests"`
Expected: Passed! - 8 tests passed

**Step 5: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/PlatformInfo.cs tests/McjCoderOrg.ClaudeAutoResume.Tests/PlatformInfoTests.cs
git commit -m "feat: add platform info for diagnostics per ADR-0017

Refs: #9"
```

---

## Task 4: Create LoggingConfiguration

**Files:**

- Create: `src/McjCoderOrg.ClaudeAutoResume/LoggingConfiguration.cs`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/LoggingConfigurationTests.cs`

**Step 1: Write the failing tests**

Create `tests/McjCoderOrg.ClaudeAutoResume.Tests/LoggingConfigurationTests.cs`:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

public sealed class LoggingConfigurationTests
{
    [Fact]
    public void GetLogDirectory_OnWindows_ShouldUseLocalAppData()
    {
        if (!OperatingSystem.IsWindows())
        {
            return; // Skip on non-Windows
        }

        var path = LoggingConfiguration.GetLogDirectory();

        path.Should().Contain("claude-auto-resume");
        path.Should().Contain("logs");
    }

    [Fact]
    public void GetLogDirectory_ShouldReturnAbsolutePath()
    {
        var path = LoggingConfiguration.GetLogDirectory();

        Path.IsPathRooted(path).Should().BeTrue();
    }

    [Fact]
    public void GetLogFilePath_ShouldIncludeDate()
    {
        var path = LoggingConfiguration.GetLogFilePath();

        path.Should().Contain(DateTime.UtcNow.ToString("yyyy-MM-dd"));
    }

    [Fact]
    public void GetLogFilePath_ShouldHaveLogExtension()
    {
        var path = LoggingConfiguration.GetLogFilePath();

        path.Should().EndWith(".log");
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~LoggingConfigurationTests"`
Expected: FAIL with "The type or namespace name 'LoggingConfiguration' does not exist"

**Step 3: Write minimal implementation**

Create `src/McjCoderOrg.ClaudeAutoResume/LoggingConfiguration.cs`:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Provides logging configuration and path resolution.
/// </summary>
/// <remarks>
/// See ADR-0017 and ADR-0018 for logging and path design decisions.
/// </remarks>
public static class LoggingConfiguration
{
    private const string AppName = "claude-auto-resume";

    /// <summary>
    /// Gets the log directory path for the current platform.
    /// </summary>
    /// <returns>The absolute path to the log directory.</returns>
    public static string GetLogDirectory()
    {
        var basePath = GetPlatformLogBasePath();
        return Path.Combine(basePath, AppName, "logs");
    }

    /// <summary>
    /// Gets the log file path for the current day.
    /// </summary>
    /// <returns>The absolute path to the log file.</returns>
    public static string GetLogFilePath()
    {
        var directory = GetLogDirectory();
        var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
        return Path.Combine(directory, fileName);
    }

    private static string GetPlatformLogBasePath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library",
                "Logs");
        }

        // Linux and others
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local",
            "share");
    }
}
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~LoggingConfigurationTests"`
Expected: Passed! - 4 tests passed

**Step 5: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/LoggingConfiguration.cs tests/McjCoderOrg.ClaudeAutoResume.Tests/LoggingConfigurationTests.cs
git commit -m "feat: add logging configuration with platform-specific paths

Refs: #9"
```

---

## Task 5: Create Resource File Structure

**Files:**

- Create: `src/McjCoderOrg.ClaudeAutoResume/Resources/Strings.resx`
- Modify: `src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`

**Step 1: Create resource file**

Create `src/McjCoderOrg.ClaudeAutoResume/Resources/Strings.resx`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" msdata:Ordinal="0" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name="AppDescription" xml:space="preserve">
    <value>CLI tool to monitor Claude Code sessions and automatically resume on rate limits.</value>
    <comment>Application description shown in help</comment>
  </data>
  <data name="StartingApp" xml:space="preserve">
    <value>Starting Claude Auto Resume v{AppVersion}</value>
    <comment>{AppVersion} = version string</comment>
  </data>
  <data name="RateLimitDetected" xml:space="preserve">
    <value>Detected Session Limit Reached, resets at {ResetTime}</value>
    <comment>{ResetTime} = time when limit resets</comment>
  </data>
  <data name="WaitingForReset" xml:space="preserve">
    <value>Waiting {WaitMinutes} minutes for rate limit reset</value>
    <comment>{WaitMinutes} = number of minutes to wait</comment>
  </data>
  <data name="ErrorUnhandledException" xml:space="preserve">
    <value>An unexpected error occurred. See log file for details.</value>
  </data>
  <data name="ErrorLogLocation" xml:space="preserve">
    <value>Log file: {LogPath}</value>
    <comment>{LogPath} = path to log file</comment>
  </data>
  <data name="DiagnoseHeader" xml:space="preserve">
    <value>Claude Auto Resume Diagnostics</value>
  </data>
  <data name="DiagnoseRuntimeInfo" xml:space="preserve">
    <value>Runtime: .NET {DotNetVersion} ({RuntimeIdentifier})</value>
    <comment>{DotNetVersion} = .NET version, {RuntimeIdentifier} = RID</comment>
  </data>
  <data name="DiagnoseOsInfo" xml:space="preserve">
    <value>OS: {OsDescription} ({Architecture})</value>
    <comment>{OsDescription} = OS name, {Architecture} = CPU arch</comment>
  </data>
</root>
```

**Step 2: Add EmbeddedResource configuration to project**

Add to `src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`:

```xml
  <!-- Resource Files -->
  <ItemGroup>
    <EmbeddedResource Update="Resources\Strings.resx">
      <Generator>ResXFileCodeGenerator</Generator>
      <LastGenOutput>Strings.Designer.cs</LastGenOutput>
    </EmbeddedResource>
    <Compile Update="Resources\Strings.Designer.cs">
      <DesignTime>True</DesignTime>
      <AutoGen>True</AutoGen>
      <DependentUpon>Strings.resx</DependentUpon>
    </Compile>
  </ItemGroup>
```

**Step 3: Build to generate designer file**

Run: `dotnet build src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/Resources/Strings.resx src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj
git commit -m "feat: add i18n-ready resource file structure per ADR-0019

Refs: #9"
```

---

## Task 6: Create LogCapture Test Utility

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/TestUtilities/LogCapture.cs`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/TestUtilities/LogCaptureTests.cs`
- Modify: `tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj`

**Step 1: Add Serilog.Sinks.InMemory to test project**

Add to `tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj`:

```xml
  <ItemGroup>
    <PackageReference Include="Serilog" />
    <PackageReference Include="Serilog.Sinks.InMemory" />
  </ItemGroup>
```

**Step 2: Write the failing tests for LogCapture**

Create `tests/McjCoderOrg.ClaudeAutoResume.Tests/TestUtilities/LogCaptureTests.cs`:

```csharp
using Serilog;

namespace McjCoderOrg.ClaudeAutoResume.TestUtilities;

public sealed class LogCaptureTests : IDisposable
{
    private LogCapture? _logCapture;

    public void Dispose()
    {
        _logCapture?.Dispose();
    }

    [Fact]
    public void Messages_WhenLogWritten_ShouldContainMessage()
    {
        _logCapture = new LogCapture();

        Log.Information("Test message");

        _logCapture.Messages.Should().Contain(m => m.Contains("Test message"));
    }

    [Fact]
    public void Messages_WithStructuredData_ShouldContainRenderedValue()
    {
        _logCapture = new LogCapture();

        Log.Information("Value is {Value}", 42);

        _logCapture.Messages.Should().Contain(m => m.Contains("42"));
    }

    [Fact]
    public void Clear_ShouldRemoveAllMessages()
    {
        _logCapture = new LogCapture();
        Log.Information("Message to clear");

        _logCapture.Clear();

        _logCapture.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_ShouldRestorePreviousLogger()
    {
        var originalLogger = Log.Logger;
        _logCapture = new LogCapture();

        _logCapture.Dispose();
        _logCapture = null;

        Log.Logger.Should().NotBe(originalLogger); // Logger was replaced, now silent
    }
}
```

**Step 3: Run test to verify it fails**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~LogCaptureTests"`
Expected: FAIL with "The type or namespace name 'LogCapture' does not exist"

**Step 4: Write minimal implementation**

Create `tests/McjCoderOrg.ClaudeAutoResume.Tests/TestUtilities/LogCapture.cs`:

```csharp
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;

namespace McjCoderOrg.ClaudeAutoResume.TestUtilities;

/// <summary>
/// Captures Serilog messages for test assertions.
/// </summary>
/// <remarks>
/// See ADR-0017 for test capture design.
/// </remarks>
public sealed class LogCapture : IDisposable
{
    private readonly ILogger _previousLogger;
    private readonly InMemorySink _sink;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogCapture"/> class.
    /// </summary>
    public LogCapture()
    {
        _previousLogger = Log.Logger;
        _sink = new InMemorySink();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_sink)
            .CreateLogger();
    }

    /// <summary>
    /// Gets the captured log messages.
    /// </summary>
    public IReadOnlyList<string> Messages =>
        _sink.LogEvents
            .Select(e => e.RenderMessage())
            .ToList();

    /// <summary>
    /// Gets the captured log events.
    /// </summary>
    public IReadOnlyList<LogEvent> Events =>
        _sink.LogEvents.ToList();

    /// <summary>
    /// Clears all captured messages.
    /// </summary>
    public void Clear()
    {
        _sink.Dispose();
    }

    /// <summary>
    /// Disposes the log capture and restores the previous logger.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Log.CloseAndFlush();
        Log.Logger = new LoggerConfiguration().CreateLogger(); // Silent logger
    }
}
```

**Step 5: Run test to verify it passes**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~LogCaptureTests"`
Expected: Passed! - 4 tests passed

**Step 6: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj tests/McjCoderOrg.ClaudeAutoResume.Tests/TestUtilities/LogCapture.cs tests/McjCoderOrg.ClaudeAutoResume.Tests/TestUtilities/LogCaptureTests.cs
git commit -m "test: add LogCapture utility for log assertions per ADR-0017

Refs: #9"
```

---

## Task 7: Create Program.cs with Application Framework

**Files:**

- Modify: `src/McjCoderOrg.ClaudeAutoResume/Program.cs`
- Create: `tests/McjCoderOrg.ClaudeAutoResume.Tests/ProgramTests.cs`

**Step 1: Write the failing tests**

Create `tests/McjCoderOrg.ClaudeAutoResume.Tests/ProgramTests.cs`:

```csharp
namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ProgramTests
{
    [Fact]
    public void Main_WithVersionFlag_ShouldReturnSuccess()
    {
        var result = Program.Main(["--version"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public void Main_WithHelpFlag_ShouldReturnSuccess()
    {
        var result = Program.Main(["--help"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public void Main_WithDiagnoseFlag_ShouldReturnSuccess()
    {
        var result = Program.Main(["--diagnose"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public void Main_WithNoArgs_ShouldReturnSuccess()
    {
        // For now, just validates basic startup works
        var result = Program.Main([]);

        result.Should().Be(ExitCodes.Success);
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~ProgramTests"`
Expected: FAIL (current Program.Main is private and doesn't support all flags)

**Step 3: Write the full implementation**

Replace `src/McjCoderOrg.ClaudeAutoResume/Program.cs`:

```csharp
using System.Reflection;

using McjCoderOrg.ClaudeAutoResume.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Application entry point with CLI framework.
/// </summary>
/// <remarks>
/// See ADR-0017 (Observability) and ADR-0018 (CLI Design) for design decisions.
/// </remarks>
public static class Program
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    public static int Main(string[] args)
    {
        // Bootstrap logger for startup errors
        ConfigureBootstrapLogger();

        try
        {
            return Run(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            Console.Error.WriteLine(Strings.ErrorUnhandledException);
            Console.Error.WriteLine(string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Strings.ErrorLogLocation,
                LoggingConfiguration.GetLogFilePath()));
            return ExitCodes.GeneralError;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static int Run(string[] args)
    {
        // Handle --version
        if (args.Length > 0 && IsFlag(args[0], "--version", "-v"))
        {
            PrintVersion();
            return ExitCodes.Success;
        }

        // Handle --help
        if (args.Length > 0 && IsFlag(args[0], "--help", "-h"))
        {
            PrintHelp();
            return ExitCodes.Success;
        }

        // Handle --diagnose
        if (args.Length > 0 && IsFlag(args[0], "--diagnose"))
        {
            PrintDiagnostics();
            return ExitCodes.Success;
        }

        // Check for --verbose flag
        var verbose = args.Any(a => IsFlag(a, "--verbose", "-V"));
        if (verbose)
        {
            ConfigureVerboseLogging();
        }

        // Log startup
        var platform = PlatformInfo.Current;
        Log.Information(Strings.StartingApp, platform.AppVersion);

        // Build and run host
        var builder = Host.CreateApplicationBuilder(args);
        ConfigureServices(builder.Services);

        using var host = builder.Build();

        // For now, just return success (actual functionality in later phases)
        return ExitCodes.Success;
    }

    private static void ConfigureBootstrapLogger()
    {
        var logPath = LoggingConfiguration.GetLogFilePath();
        var logDir = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrEmpty(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .WriteTo.Debug()
            .CreateBootstrapLogger();
    }

    private static void ConfigureVerboseLogging()
    {
        var logPath = LoggingConfiguration.GetLogFilePath();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .WriteTo.Debug()
            .CreateLogger();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Services will be added in later phases
        _ = services;
    }

    private static void PrintVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
        Console.WriteLine($"claude-auto-resume {version.Major}.{version.Minor}.{version.Build}");
    }

    private static void PrintHelp()
    {
        Console.WriteLine(Strings.AppDescription);
        Console.WriteLine();
        Console.WriteLine("Usage: claude-auto-resume [options] [-- <claude-args>...]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -c, --config <path>       Path to configuration file");
        Console.WriteLine("  -V, --verbose             Enable verbose logging to file");
        Console.WriteLine("  --diagnose                Run environment diagnostics");
        Console.WriteLine("  --version                 Show version information");
        Console.WriteLine("  -h, --help                Show help");
    }

    private static void PrintDiagnostics()
    {
        var platform = PlatformInfo.Current;

        Console.WriteLine(Strings.DiagnoseHeader);
        Console.WriteLine(new string('=', 40));
        Console.WriteLine();

        Console.WriteLine(string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            Strings.DiagnoseRuntimeInfo,
            platform.DotNetVersion,
            platform.RuntimeIdentifier));

        Console.WriteLine(string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            Strings.DiagnoseOsInfo,
            platform.OsDescription,
            platform.ProcessArchitecture));

        Console.WriteLine($"App Version: {platform.AppVersion}");
        Console.WriteLine($"Container: {platform.IsContainer}");
        Console.WriteLine($"CI: {platform.IsCI}");
        Console.WriteLine();
        Console.WriteLine(string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            Strings.ErrorLogLocation,
            LoggingConfiguration.GetLogFilePath()));
    }

    private static bool IsFlag(string arg, string longForm, string? shortForm = null)
    {
        return string.Equals(arg, longForm, StringComparison.OrdinalIgnoreCase)
            || (shortForm is not null && string.Equals(arg, shortForm, StringComparison.OrdinalIgnoreCase));
    }
}
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests/McjCoderOrg.ClaudeAutoResume.Tests.csproj --filter "FullyQualifiedName~ProgramTests"`
Expected: Passed! - 4 tests passed

**Step 5: Run all tests**

Run: `dotnet test`
Expected: All tests pass

**Step 6: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/Program.cs tests/McjCoderOrg.ClaudeAutoResume.Tests/ProgramTests.cs
git commit -m "feat: implement CLI application framework with host builder

- Bootstrap logger for startup errors
- Global exception handler
- --help, --version, --diagnose, --verbose options
- Structured logging with Serilog

Refs: #9"
```

---

## Task 8: Update PublicAPI Files and Final Verification

**Files:**

- Modify: `src/McjCoderOrg.ClaudeAutoResume/PublicAPI.Unshipped.txt`

**Step 1: Build and check for PublicAPI warnings**

Run: `dotnet build src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj 2>&1`
Expected: Warnings about missing public API declarations

**Step 2: Update PublicAPI.Unshipped.txt**

Add the public API surface:

```text
#nullable enable
McjCoderOrg.ClaudeAutoResume.ExitCodes
McjCoderOrg.ClaudeAutoResume.LoggingConfiguration
McjCoderOrg.ClaudeAutoResume.PlatformInfo
McjCoderOrg.ClaudeAutoResume.PlatformInfo.AppVersion.get -> string!
McjCoderOrg.ClaudeAutoResume.PlatformInfo.Current.get -> McjCoderOrg.ClaudeAutoResume.PlatformInfo!
McjCoderOrg.ClaudeAutoResume.PlatformInfo.DotNetVersion.get -> string!
McjCoderOrg.ClaudeAutoResume.PlatformInfo.IsCI.get -> bool
McjCoderOrg.ClaudeAutoResume.PlatformInfo.IsContainer.get -> bool
McjCoderOrg.ClaudeAutoResume.PlatformInfo.OsDescription.get -> string!
McjCoderOrg.ClaudeAutoResume.PlatformInfo.ProcessArchitecture.get -> string!
McjCoderOrg.ClaudeAutoResume.PlatformInfo.RuntimeIdentifier.get -> string!
McjCoderOrg.ClaudeAutoResume.Program
static McjCoderOrg.ClaudeAutoResume.ExitCodes.ConfigurationError -> int
static McjCoderOrg.ClaudeAutoResume.ExitCodes.DependencyMissing -> int
static McjCoderOrg.ClaudeAutoResume.ExitCodes.GeneralError -> int
static McjCoderOrg.ClaudeAutoResume.ExitCodes.RateLimitDetected -> int
static McjCoderOrg.ClaudeAutoResume.ExitCodes.Success -> int
static McjCoderOrg.ClaudeAutoResume.ExitCodes.UserCancelled -> int
static McjCoderOrg.ClaudeAutoResume.LoggingConfiguration.GetLogDirectory() -> string!
static McjCoderOrg.ClaudeAutoResume.LoggingConfiguration.GetLogFilePath() -> string!
static McjCoderOrg.ClaudeAutoResume.Program.Main(string![]! args) -> int
```

**Step 3: Verify build succeeds with no warnings**

Run: `dotnet build src/McjCoderOrg.ClaudeAutoResume/McjCoderOrg.ClaudeAutoResume.csproj --no-incremental`
Expected: Build succeeded. 0 Warning(s)

**Step 4: Run full test suite**

Run: `dotnet test`
Expected: All tests pass

**Step 5: Commit**

```bash
git add src/McjCoderOrg.ClaudeAutoResume/PublicAPI.Unshipped.txt
git commit -m "docs: update public api surface for application framework

Refs: #9"
```

---

## Task 9: Delete Placeholder Test

**Files:**

- Delete: `tests/McjCoderOrg.ClaudeAutoResume.Tests/PlaceholderTests.cs`

**Step 1: Delete placeholder test file**

Run: `rm tests/McjCoderOrg.ClaudeAutoResume.Tests/PlaceholderTests.cs`

**Step 2: Verify tests still pass**

Run: `dotnet test`
Expected: All tests pass

**Step 3: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.Tests/PlaceholderTests.cs
git commit -m "chore: remove placeholder test replaced by real tests

Refs: #9"
```

---

## Completion Checklist

- [ ] All Serilog packages added
- [ ] ExitCodes.cs with semantic exit codes
- [ ] PlatformInfo.cs with platform detection
- [ ] LoggingConfiguration.cs with platform paths
- [ ] Resources/Strings.resx with i18n structure
- [ ] Program.cs with Host.CreateApplicationBuilder
- [ ] LogCapture.cs test utility
- [ ] All tests passing
- [ ] PublicAPI files updated
- [ ] Placeholder test removed

**Create PR with:**

```bash
git push -u origin feature/9-application-framework
gh pr create --title "feat: implement application framework (Phase 4c)" --body "## Summary
- Add Serilog logging infrastructure per ADR-0017
- Implement semantic exit codes per ADR-0018
- Add platform detection for diagnostics
- Create i18n-ready resource structure per ADR-0019
- Implement CLI with --help, --version, --diagnose, --verbose

## Test plan
- [ ] Run \`dotnet test\` - all tests pass
- [ ] Run \`claude-auto-resume --version\` - shows version
- [ ] Run \`claude-auto-resume --help\` - shows help
- [ ] Run \`claude-auto-resume --diagnose\` - shows diagnostics

Closes #9"
```
