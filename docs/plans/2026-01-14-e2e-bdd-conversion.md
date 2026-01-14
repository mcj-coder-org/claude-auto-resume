# E2E Tests BDD Conversion Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Convert existing E2E tests in `ApplicationStartupTests.cs` to BDD format using Reqnroll feature files.

**Architecture:** Create a feature file with Gherkin scenarios, step definitions class with shared state via ScenarioContext, and a helper class for process execution. The existing test logic moves into step definitions.

**Tech Stack:** Reqnroll, xUnit, AwesomeAssertions, Xunit.SkippableFact

---

## Task 1: Create Feature File

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature`

**Step 1: Create the feature file**

Create file `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature`:

```gherkin
@E2E
Feature: Application startup
    As a user
    I want the application to start correctly with various flags
    So that I can use claude-auto-resume effectively

    @cli-flags
    Scenario: Display version information
        When I run the application with "--version"
        Then the exit code should be 0
        And the output should contain "claude-auto-resume"

    @cli-flags
    Scenario: Display help information
        When I run the application with "--help"
        Then the exit code should be 0
        And the output should contain "USAGE:"
        And the output should contain "--version"
        And the output should contain "--help"

    @cli-flags
    Scenario: Display diagnostic information
        When I run the application with "--diagnose"
        Then the exit code should be 0
        And the output should contain "Runtime:"

    @cli-flags
    Scenario: Reject headless mode without dangerous flag
        When I run the application with "--headless"
        Then the exit code should be 2
        And the combined output should contain "--dangerously-skip-permissions"

    @dependency-check @skip-if-claude-available
    Scenario: Report missing claude CLI
        Given the executable exists
        And claude CLI is not available
        When I run the application with no arguments
        Then the exit code should be 4
        And the error output should contain "Could not find 'claude' in PATH"

    @happy-path @skip-if-claude-missing
    Scenario: Start successfully when claude is available
        Given the executable exists
        And claude CLI is available
        When I run the application with no arguments
        Then the application should keep running for at least 3 seconds
```

**Step 2: Verify file was created**

Run: `ls tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/`

Expected: `ApplicationStartup.feature` listed

**Step 3: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature
git commit -m "feat(e2e): add application startup feature file

Refs: #112"
```

---

## Task 2: Create Process Helper Class

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessHelper.cs`

**Step 1: Create the helper class**

Create file `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessHelper.cs`:

```csharp
using System.Diagnostics;

namespace McjCoderOrg.ClaudeAutoResume.E2ETests.Helpers;

/// <summary>
/// Helper for running the application executable in E2E tests.
/// </summary>
public static class ProcessHelper
{
    private static readonly string ExecutablePath = GetExecutablePath();

    /// <summary>
    /// Gets the path to the application executable.
    /// </summary>
    public static string GetExecutablePath()
    {
        var testDir = AppContext.BaseDirectory;
        var solutionDir = Path.GetFullPath(Path.Combine(testDir, "..", "..", "..", "..", ".."));
        var exeName = OperatingSystem.IsWindows()
            ? "McjCoderOrg.ClaudeAutoResume.exe"
            : "McjCoderOrg.ClaudeAutoResume";

        string[] searchPaths =
        [
            Path.Combine(solutionDir, "src", "McjCoderOrg.ClaudeAutoResume", "bin", "Debug", "net10.0", exeName),
            Path.Combine(solutionDir, "src", "McjCoderOrg.ClaudeAutoResume", "bin", "Release", "net10.0", exeName),
            Path.Combine(solutionDir, "artifacts", "bin", "McjCoderOrg.ClaudeAutoResume", "debug", exeName),
            Path.Combine(solutionDir, "artifacts", "bin", "McjCoderOrg.ClaudeAutoResume", "release", exeName),
        ];

        return searchPaths.FirstOrDefault(File.Exists) ?? searchPaths[0];
    }

    /// <summary>
    /// Checks if the executable exists.
    /// </summary>
    public static bool ExecutableExists() => File.Exists(ExecutablePath);

    /// <summary>
    /// Creates a process configured to run the application.
    /// </summary>
    public static Process CreateProcess(string arguments)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ExecutablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
    }

    /// <summary>
    /// Checks if the claude CLI is available on the system.
    /// </summary>
    public static bool IsClaudeAvailable()
    {
        try
        {
            var isWindows = OperatingSystem.IsWindows();
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = isWindows ? "cmd.exe" : "claude",
                    Arguments = isWindows ? "/c claude -v" : "-v",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();
            var completed = process.WaitForExit(5000);

            return completed && process.ExitCode == 0;
        }
#pragma warning disable CA1031 // Intentional: Any failure means claude is not available
        catch
        {
            return false;
        }
#pragma warning restore CA1031
    }
}
```

**Step 2: Verify it compiles**

Run: `dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity quiet`

Expected: Build succeeded

**Step 3: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessHelper.cs
git commit -m "feat(e2e): add process helper for BDD step definitions

Refs: #112"
```

---

## Task 3: Create Process Result Class

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessResult.cs`

**Step 1: Create the result class**

Create file `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessResult.cs`:

```csharp
namespace McjCoderOrg.ClaudeAutoResume.E2ETests.Helpers;

/// <summary>
/// Represents the result of running a process.
/// </summary>
public sealed record ProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError)
{
    /// <summary>
    /// Gets the combined standard output and error.
    /// </summary>
    public string CombinedOutput => StandardOutput + StandardError;
}
```

**Step 2: Verify it compiles**

Run: `dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity quiet`

Expected: Build succeeded

**Step 3: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessResult.cs
git commit -m "feat(e2e): add process result record for step definitions

Refs: #112"
```

---

## Task 4: Create Step Definitions

**Files:**

- Create: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/StepDefinitions/ApplicationStartupSteps.cs`

**Step 1: Create the step definitions**

Create file `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/StepDefinitions/ApplicationStartupSteps.cs`:

```csharp
using System.Diagnostics;
using McjCoderOrg.ClaudeAutoResume.E2ETests.Helpers;

namespace McjCoderOrg.ClaudeAutoResume.E2ETests.StepDefinitions;

[Binding]
public sealed class ApplicationStartupSteps : IDisposable
{
    private Process? _process;
    private ProcessResult? _result;

    [Given("the executable exists")]
    public void GivenTheExecutableExists()
    {
        Skip.IfNot(
            ProcessHelper.ExecutableExists(),
            $"Executable not found at {ProcessHelper.GetExecutablePath()}");
    }

    [Given("claude CLI is available")]
    public void GivenClaudeCliIsAvailable()
    {
        Skip.IfNot(
            ProcessHelper.IsClaudeAvailable(),
            "Claude CLI is not available - skipping test");
    }

    [Given("claude CLI is not available")]
    public void GivenClaudeCliIsNotAvailable()
    {
        Skip.If(
            ProcessHelper.IsClaudeAvailable(),
            "Claude CLI is available - this test is for missing claude");
    }

    [When("I run the application with {string}")]
    public async Task WhenIRunTheApplicationWith(string arguments)
    {
        _process = ProcessHelper.CreateProcess(arguments);
        _process.Start();

        var stdout = await _process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var stderr = await _process.StandardError.ReadToEndAsync().ConfigureAwait(false);
        await _process.WaitForExitAsync().ConfigureAwait(false);

        _result = new ProcessResult(_process.ExitCode, stdout, stderr);
    }

    [When("I run the application with no arguments")]
    public async Task WhenIRunTheApplicationWithNoArguments()
    {
        await WhenIRunTheApplicationWith(string.Empty).ConfigureAwait(false);
    }

    [Then("the exit code should be {int}")]
    public void ThenTheExitCodeShouldBe(int expectedExitCode)
    {
        Result.ExitCode.Should().Be(expectedExitCode);
    }

    [Then("the output should contain {string}")]
    public void ThenTheOutputShouldContain(string expected)
    {
        Result.StandardOutput.Should().Contain(expected);
    }

    [Then("the error output should contain {string}")]
    public void ThenTheErrorOutputShouldContain(string expected)
    {
        Result.StandardError.Should().Contain(expected);
    }

    [Then("the combined output should contain {string}")]
    public void ThenTheCombinedOutputShouldContain(string expected)
    {
        Result.CombinedOutput.Should().Contain(expected);
    }

    [Then("the application should keep running for at least {int} seconds")]
    public async Task ThenTheApplicationShouldKeepRunningForAtLeastSeconds(int seconds)
    {
        // For this scenario, we need to start the process differently
        // Don't wait for exit, just check it stays running
        _process = ProcessHelper.CreateProcess(string.Empty);
        _process.Start();

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(seconds));
        var processTask = _process.WaitForExitAsync();

        var completedTask = await Task.WhenAny(timeoutTask, processTask).ConfigureAwait(false);

        if (completedTask == timeoutTask)
        {
            // Process is still running after timeout - success!
            _process.Kill();
            // Set a dummy result for cleanup
            _result = new ProcessResult(0, string.Empty, string.Empty);
        }
        else
        {
            // Process exited unexpectedly
            var stdout = await _process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            var stderr = await _process.StandardError.ReadToEndAsync().ConfigureAwait(false);
            _result = new ProcessResult(_process.ExitCode, stdout, stderr);

            Assert.Fail($"Application exited unexpectedly with code {_process.ExitCode}. " +
                $"Output: {stdout} Error: {stderr}");
        }
    }

    private ProcessResult Result => _result ?? throw new InvalidOperationException("No process result available");

    public void Dispose()
    {
        if (_process is not null)
        {
            if (!_process.HasExited)
            {
                try
                {
                    _process.Kill();
                }
#pragma warning disable CA1031 // Intentional: Best-effort cleanup
                catch
                {
                    // Process may have already exited
                }
#pragma warning restore CA1031
            }

            _process.Dispose();
        }
    }
}
```

**Step 2: Verify it compiles**

Run: `dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity quiet`

Expected: Build succeeded

**Step 3: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/StepDefinitions/ApplicationStartupSteps.cs
git commit -m "feat(e2e): add step definitions for application startup

Refs: #112"
```

---

## Task 5: Build and Verify Feature File Generates

**Files:**

- Verify: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature.cs` (auto-generated)

**Step 1: Build the project to generate feature.cs**

Run: `dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ -c Debug --verbosity quiet`

Expected: Build succeeded

**Step 2: Verify generated file exists**

Run: `ls tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/`

Expected: Both `ApplicationStartup.feature` and `ApplicationStartup.feature.cs`

**Step 3: Run the new BDD tests**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --filter "FullyQualifiedName~ApplicationStartup" --verbosity normal`

Expected: All 6 scenarios pass (some may skip based on claude availability)

**Step 4: Commit generated file**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature.cs
git commit -m "chore(e2e): add generated feature file

Refs: #112"
```

---

## Task 6: Remove Old Test Class

**Files:**

- Delete: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ApplicationStartupTests.cs`

**Step 1: Verify new tests cover all old tests**

Compare scenarios:
| Old Test | New Scenario |
|----------|--------------|
| `ApplicationWithVersionFlagStartsSuccessfullyAsync` | "Display version information" |
| `ApplicationWithHelpFlagStartsSuccessfullyAsync` | "Display help information" |
| `ApplicationWithDiagnoseFlagStartsSuccessfullyAsync` | "Display diagnostic information" |
| `ApplicationWithHeadlessWithoutDangerousReturnsInvalidArgumentsAsync` | "Reject headless mode without dangerous flag" |
| `ApplicationWithNoArgsReportsMissingClaudeAsync` | "Report missing claude CLI" |
| `ApplicationStartsSuccessfullyWhenClaudeIsAvailableAsync` | "Start successfully when claude is available" |

**Step 2: Run all E2E tests to confirm coverage**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity normal`

Expected: All tests pass

**Step 3: Delete old test file**

Run: `rm tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ApplicationStartupTests.cs`

**Step 4: Verify build still works**

Run: `dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity quiet`

Expected: Build succeeded

**Step 5: Run tests to confirm nothing broke**

Run: `dotnet test tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity normal`

Expected: All tests pass

**Step 6: Commit**

```bash
git add -A
git commit -m "refactor(e2e): remove old xUnit test class in favour of BDD

The ApplicationStartupTests.cs has been replaced by:
- Features/ApplicationStartup.feature (Gherkin scenarios)
- StepDefinitions/ApplicationStartupSteps.cs (step implementations)
- Helpers/ProcessHelper.cs and ProcessResult.cs

Refs: #112"
```

---

## Task 7: Update NoWarn in csproj

**Files:**

- Modify: `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj`

**Step 1: Add CA1812 suppression for Reqnroll**

Update the NoWarn line in the csproj:

From:

```xml
<NoWarn>$(NoWarn);CS1591;CA1515</NoWarn>
```

To:

```xml
<!-- CA1515: Step definition classes must be public for Reqnroll discovery -->
<!-- CA1812: Reqnroll instantiates step classes via reflection -->
<NoWarn>$(NoWarn);CS1591;CA1515;CA1812</NoWarn>
```

**Step 2: Verify build has no warnings**

Run: `dotnet build tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ --verbosity quiet`

Expected: Build succeeded with 0 warnings

**Step 3: Commit**

```bash
git add tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj
git commit -m "chore(e2e): suppress CA1812 for Reqnroll step definitions

Refs: #112"
```

---

## Task 8: Final Verification and PR

**Step 1: Run full test suite**

Run: `dotnet test --verbosity normal`

Expected: All tests pass across all projects

**Step 2: Verify pre-push filter still works**

Run: `dotnet test --filter "Category!=Integration&Category!=E2E" --verbosity minimal`

Expected: E2E tests excluded (they have @E2E tag)

**Step 3: Create PR**

```bash
git push -u origin fix/112-e2e-bdd-conversion
gh pr create --title "refactor(e2e): convert E2E tests to BDD format using Reqnroll" --body "## Summary
- Convert ApplicationStartupTests.cs to Reqnroll BDD format
- Create ApplicationStartup.feature with 6 scenarios
- Create step definitions in ApplicationStartupSteps.cs
- Add ProcessHelper and ProcessResult helper classes
- Remove old xUnit test class

## Test plan
- [x] All 6 scenarios pass
- [x] Skip conditions work correctly (claude available/missing)
- [x] Pre-push filter excludes E2E tests (@E2E tag)

Fixes #112

🤖 Generated with [Claude Code](https://claude.com/claude-code)"
```

---

## Files Summary

| File                                                                                       | Action         |
| ------------------------------------------------------------------------------------------ | -------------- |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature`          | Create         |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Features/ApplicationStartup.feature.cs`       | Auto-generated |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessHelper.cs`                     | Create         |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/Helpers/ProcessResult.cs`                     | Create         |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/StepDefinitions/ApplicationStartupSteps.cs`   | Create         |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/McjCoderOrg.ClaudeAutoResume.E2ETests.csproj` | Modify         |
| `tests/McjCoderOrg.ClaudeAutoResume.E2ETests/ApplicationStartupTests.cs`                   | Delete         |
