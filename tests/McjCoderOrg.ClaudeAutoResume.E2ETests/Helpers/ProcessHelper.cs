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
