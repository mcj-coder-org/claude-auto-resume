using System.Diagnostics;
using System.Globalization;

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
    /// Runs the application via shell with piped input after a delay.
    /// Uses bash to handle piping natively, which works correctly with PTY input.
    /// </summary>
    public static async Task<ProcessResult> RunViaShellWithPipedInputAsync(
        string input,
        int delaySeconds,
        int timeoutSeconds)
    {
        var exePath = GetExecutablePath();
        var bashPath = GetBashPath()
            ?? throw new InvalidOperationException("Bash not found. Git Bash is required on Windows.");

        // Escape the input and path for bash
        var escapedInput = input.Replace("'", "'\\''", StringComparison.Ordinal);
        var escapedPath = exePath.Replace("\\", "/", StringComparison.Ordinal);

        // Build the bash command: (sleep N && echo 'input') | ./app.exe
        var bashCommand = string.Create(
            CultureInfo.InvariantCulture,
            $"(sleep {delaySeconds} && echo '{escapedInput}') | \"{escapedPath}\"");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = bashPath,
                Arguments = $"-c \"{bashCommand.Replace("\"", "\\\"", StringComparison.Ordinal)}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var stdoutTask = process.StandardOutput.ReadToEndAsync(cts.Token);
        var stderrTask = process.StandardError.ReadToEndAsync(cts.Token);

        try
        {
            await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);
            var stdout = await stdoutTask.ConfigureAwait(false);
            var stderr = await stderrTask.ConfigureAwait(false);
            return new ProcessResult(process.ExitCode, stdout, stderr);
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            throw new TimeoutException(string.Create(
                CultureInfo.InvariantCulture,
                $"Shell command timed out after {timeoutSeconds} seconds"));
        }
    }

    /// <summary>
    /// Gets the path to bash (Git Bash on Windows, /bin/bash on Unix).
    /// </summary>
    public static string? GetBashPath()
    {
        if (!OperatingSystem.IsWindows())
        {
            return File.Exists("/bin/bash") ? "/bin/bash" : "/bin/sh";
        }

        // Try common Git Bash locations on Windows
        string[] bashPaths =
        [
            @"C:\Program Files\Git\bin\bash.exe",
            @"C:\Program Files (x86)\Git\bin\bash.exe",
            Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Programs\Git\bin\bash.exe"),
        ];

        return bashPaths.FirstOrDefault(File.Exists);
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
