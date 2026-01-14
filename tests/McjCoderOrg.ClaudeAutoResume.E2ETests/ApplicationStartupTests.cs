using System.Diagnostics;

namespace McjCoderOrg.ClaudeAutoResume.E2ETests;

/// <summary>
/// End-to-end tests verifying the application starts correctly.
/// These tests run the actual executable as a black-box test.
/// </summary>
public sealed class ApplicationStartupTests
{
    private static readonly string _executablePath = Get_executablePath();

    private static string Get_executablePath()
    {
        // Navigate from test output to the main project output
        var testDir = AppContext.BaseDirectory;
        var solutionDir = Path.GetFullPath(Path.Combine(testDir, "..", "..", "..", "..", ".."));
        var exeName = OperatingSystem.IsWindows()
            ? "McjCoderOrg.ClaudeAutoResume.exe"
            : "McjCoderOrg.ClaudeAutoResume";

        // Try common output locations
        string[] searchPaths =
        [
            Path.Combine(solutionDir, "src", "McjCoderOrg.ClaudeAutoResume", "bin", "Debug", "net10.0", exeName),
            Path.Combine(solutionDir, "src", "McjCoderOrg.ClaudeAutoResume", "bin", "Release", "net10.0", exeName),
            Path.Combine(solutionDir, "artifacts", "bin", "McjCoderOrg.ClaudeAutoResume", "debug", exeName),
            Path.Combine(solutionDir, "artifacts", "bin", "McjCoderOrg.ClaudeAutoResume", "release", exeName),
        ];

        return searchPaths.FirstOrDefault(File.Exists) ?? searchPaths[0];
    }

    [Fact]
    public async Task ApplicationWithVersionFlagStartsSuccessfullyAsync()
    {
        // Arrange
        using var process = CreateProcess("--version");

        // Act
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().Be(0, "application should exit successfully with --version");
        output.Should().Contain("claude-auto-resume", "version output should contain application name");
    }

    [Fact]
    public async Task ApplicationWithHelpFlagStartsSuccessfullyAsync()
    {
        // Arrange
        using var process = CreateProcess("--help");

        // Act
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().Be(0, "application should exit successfully with --help");
        output.Should().Contain("USAGE:", "help output should contain usage information");
        output.Should().Contain("--version", "help should document --version flag");
        output.Should().Contain("--help", "help should document --help flag");
    }

    [Fact]
    public async Task ApplicationWithDiagnoseFlagStartsSuccessfullyAsync()
    {
        // Arrange
        using var process = CreateProcess("--diagnose");

        // Act
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().Be(0, "application should exit successfully with --diagnose");
        output.Should().Contain("Runtime:", "diagnose should show runtime info");
    }

    [Fact]
    public async Task ApplicationWithHeadlessWithoutDangerousReturnsInvalidArgumentsAsync()
    {
        // Arrange
        using var process = CreateProcess("--headless");

        // Act
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        var errorOutput = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().Be(2, "--headless without --dangerously-skip-permissions should return InvalidArguments (2)");

        // Error message may be on stdout or stderr depending on implementation
        var combinedOutput = output + errorOutput;
        combinedOutput.Should().Contain("--dangerously-skip-permissions", "error should mention required flag");
    }

    [SkippableFact]
    public async Task ApplicationWithNoArgsReportsMissingClaudeAsync()
    {
        // Skip if executable doesn't exist (build not run)
        Skip.If(!File.Exists(_executablePath), $"Executable not found at {_executablePath}");

        // Skip if claude IS available - this test is for when it's missing
        Skip.If(IsClaudeAvailable(), "Claude CLI is available - this test is for missing claude");

        // Arrange
        using var process = CreateProcess(string.Empty);

        // Act
        process.Start();
        var errorOutput = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().Be(4,
            "exit code should be DependencyMissing (4) when claude is not found");
        errorOutput.Should().Contain("Could not find 'claude' in PATH",
            "error output should indicate claude was not found");
    }

    [SkippableFact]
    public async Task ApplicationStartsSuccessfullyWhenClaudeIsAvailableAsync()
    {
        // Skip if executable doesn't exist (build not run)
        Skip.If(!File.Exists(_executablePath), $"Executable not found at {_executablePath}");

        // Skip if claude is NOT available - this test requires claude
        Skip.If(!IsClaudeAvailable(), "Claude CLI is not available - skipping happy path test");

        // Arrange
        using var process = CreateProcess(string.Empty);

        // Act
        process.Start();

        // Wait briefly - if claude is available, the app should start the PTY
        // and keep running (not exit immediately)
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));
        var processTask = process.WaitForExitAsync();

        var completedTask = await Task.WhenAny(timeoutTask, processTask);

        if (completedTask == timeoutTask)
        {
            // Process is still running after 3 seconds - this is the expected happy path
            // The app successfully started and spawned claude in a PTY
            process.Kill();
            Assert.True(true, "Application started successfully with claude CLI");
        }
        else
        {
            // Process exited - this is unexpected when claude is available
            var output = await process.StandardOutput.ReadToEndAsync();
            var errorOutput = await process.StandardError.ReadToEndAsync();
            Assert.Fail(string.Create(
                System.Globalization.CultureInfo.InvariantCulture,
                $"Application exited unexpectedly with code {process.ExitCode}. Output: {output} Error: {errorOutput}"));
        }
    }

    private static bool IsClaudeAvailable()
    {
        try
        {
            // On Windows, need to use cmd.exe to resolve claude.cmd
            // On Unix, can call claude directly
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

    private static Process CreateProcess(string arguments)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _executablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
    }
}
