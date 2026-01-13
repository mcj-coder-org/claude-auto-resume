using System.Diagnostics;
using System.Text;

namespace McjCoderOrg.HookTests.Helpers;

/// <summary>
/// Cross-platform shell execution helper for running git hook scripts.
/// </summary>
public sealed class HookRunner
{
    private readonly string _repoPath;
    private readonly string _huskyPath;

    public HookRunner(string repoPath, string huskyPath)
    {
        _repoPath = repoPath;
        _huskyPath = huskyPath;
    }

    /// <summary>
    /// Runs a hook script and returns the result.
    /// </summary>
    /// <param name="hookName">The hook name (e.g., "pre-commit", "commit-msg").</param>
    /// <param name="args">Optional arguments to pass to the hook.</param>
    /// <returns>The result containing exit code, stdout, and stderr.</returns>
    public async Task<HookResult> RunHookAsync(string hookName, params string[] args)
    {
        var hookPath = Path.Combine(_huskyPath, hookName);

        if (!File.Exists(hookPath))
        {
            return new HookResult(-1, string.Empty, $"Hook not found: {hookPath}");
        }

        var startInfo = CreateProcessStartInfo(hookPath, args);
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stdout.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stderr.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync().ConfigureAwait(false);

        return new HookResult(process.ExitCode, stdout.ToString(), stderr.ToString());
    }

    private ProcessStartInfo CreateProcessStartInfo(string hookPath, string[] args)
    {
        var (shell, shellArgs) = GetShellCommand(hookPath, args);

        var startInfo = new ProcessStartInfo
        {
            FileName = shell,
            Arguments = shellArgs,
            WorkingDirectory = _repoPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        ConfigureEnvironment(startInfo);
        return startInfo;
    }

    private void ConfigureEnvironment(ProcessStartInfo startInfo)
    {
        // Copy environment variables
        foreach (var (key, value) in Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>())
        {
            if (key is string k && value is string v)
            {
                startInfo.EnvironmentVariables[k] = v;
            }
        }

        // Isolate from global/system git config to ensure test isolation
        startInfo.EnvironmentVariables["GIT_CONFIG_NOSYSTEM"] = "1";
        startInfo.EnvironmentVariables["GIT_CONFIG_GLOBAL"] = "";
        startInfo.EnvironmentVariables["HOME"] = _repoPath;
    }

    private static (string Shell, string Args) GetShellCommand(string hookPath, string[] args)
    {
        if (OperatingSystem.IsWindows())
        {
            var gitBashPath = FindGitBash();
            var unixHookPath = ConvertToUnixPath(hookPath);
            var unixArgs = args.Select(a => $"\"{ConvertToUnixPath(a)}\"");
            var quotedArgs = string.Join(" ", unixArgs);
            return (gitBashPath, $"-c \"{unixHookPath} {quotedArgs}\"");
        }

        var quotedArgsUnix = string.Join(" ", args.Select(a => $"\"{a}\""));
        return ("/bin/sh", $"\"{hookPath}\" {quotedArgsUnix}");
    }

    private static string ConvertToUnixPath(string windowsPath)
    {
        if (string.IsNullOrEmpty(windowsPath))
        {
            return windowsPath;
        }

        var unixPath = windowsPath.Replace('\\', '/');

        if (unixPath.Length >= 2 && unixPath[1] == ':')
        {
            unixPath = "/" + char.ToLowerInvariant(unixPath[0]) + unixPath[2..];
        }

        return unixPath;
    }

    private static string FindGitBash()
    {
        var candidates = new[]
        {
            @"C:\Program Files\Git\bin\bash.exe",
            @"C:\Program Files (x86)\Git\bin\bash.exe",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Git\bin\bash.exe"),
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return "bash";
    }
}
