using System.Diagnostics;

namespace McjCoderOrg.HookTests.Helpers;

/// <summary>
/// Helper for checking tool availability on the system.
/// </summary>
public static class ToolAvailability
{
    private static readonly Lazy<bool> NodeAvailable = new(CheckNodeAvailable);
    private static readonly Lazy<bool> GpgAvailable = new(CheckGpgAvailable);
    private static readonly Lazy<bool> DotnetAvailable = new(CheckDotnetAvailable);
    private static readonly Lazy<bool> GitBashAvailable = new(CheckGitBashAvailable);

    /// <summary>
    /// Gets a value indicating whether Node.js is available.
    /// </summary>
    public static bool IsNodeAvailable => NodeAvailable.Value;

    /// <summary>
    /// Gets a value indicating whether GPG is available.
    /// </summary>
    public static bool IsGpgAvailable => GpgAvailable.Value;

    /// <summary>
    /// Gets a value indicating whether dotnet CLI is available.
    /// </summary>
    public static bool IsDotnetAvailable => DotnetAvailable.Value;

    /// <summary>
    /// Gets a value indicating whether Git Bash is available (Windows only).
    /// </summary>
    public static bool IsGitBashAvailable => GitBashAvailable.Value;

    /// <summary>
    /// Skips the test if Node.js is not available.
    /// </summary>
    public static void SkipIfNodeMissing()
    {
        Skip.If(!IsNodeAvailable, "Node.js is not installed");
    }

    /// <summary>
    /// Skips the test if GPG is not available.
    /// </summary>
    public static void SkipIfGpgMissing()
    {
        Skip.If(!IsGpgAvailable, "GPG is not installed");
    }

    /// <summary>
    /// Skips the test if dotnet CLI is not available.
    /// </summary>
    public static void SkipIfDotnetMissing()
    {
        Skip.If(!IsDotnetAvailable, "dotnet CLI is not installed");
    }

    /// <summary>
    /// Skips the test if Git Bash is not available (Windows only).
    /// </summary>
    public static void SkipIfGitBashMissing()
    {
        if (OperatingSystem.IsWindows())
        {
            Skip.If(!IsGitBashAvailable, "Git Bash is not installed on Windows");
        }
    }

    private static bool CheckNodeAvailable()
    {
        return IsCommandAvailable("node", "--version");
    }

    private static bool CheckGpgAvailable()
    {
        return IsCommandAvailable("gpg", "--version");
    }

    private static bool CheckDotnetAvailable()
    {
        return IsCommandAvailable("dotnet", "--version");
    }

    private static bool CheckGitBashAvailable()
    {
        if (!OperatingSystem.IsWindows())
        {
            return true; // Not needed on non-Windows
        }

        var candidates = new[]
        {
            @"C:\Program Files\Git\bin\bash.exe",
            @"C:\Program Files (x86)\Git\bin\bash.exe",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Git\bin\bash.exe"),
        };

        return candidates.Any(File.Exists) || IsCommandAvailable("bash", "--version");
    }

    private static bool IsCommandAvailable(string command, string args)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();
            process.WaitForExit(5000);
            return process.ExitCode == 0;
        }
#pragma warning disable CA1031 // Catch specific exception types - any failure means command unavailable
        catch (Exception)
#pragma warning restore CA1031
        {
            return false;
        }
    }
}
