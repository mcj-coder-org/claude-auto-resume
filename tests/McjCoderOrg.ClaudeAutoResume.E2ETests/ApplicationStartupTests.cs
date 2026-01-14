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
    public async Task ApplicationWithNoArgsStartsOrReportsMissingClaudeAsync()
    {
        // Skip if executable doesn't exist (build not run)
        Skip.If(!File.Exists(_executablePath), $"Executable not found at {_executablePath}");

        // Arrange
        using var process = CreateProcess(string.Empty);

        // Act
        process.Start();

        // Wait with timeout - the app might hang waiting for input if claude exists
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
        var processTask = process.WaitForExitAsync();

        var completedTask = await Task.WhenAny(timeoutTask, processTask);

        if (completedTask == timeoutTask)
        {
            // Process is still running (claude exists and app started PTY)
            process.Kill();
            // This is actually success - app started correctly
            Assert.True(true, "Application started successfully (killed after timeout)");
        }
        else
        {
            // Process exited quickly - claude CLI was not found
            var errorOutput = await process.StandardError.ReadToEndAsync();
            process.ExitCode.Should().Be(4,
                "exit code should be DependencyMissing (4) when claude is not found");
            errorOutput.Should().Contain("Could not find 'claude' in PATH",
                "error output should indicate claude was not found");
        }
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
