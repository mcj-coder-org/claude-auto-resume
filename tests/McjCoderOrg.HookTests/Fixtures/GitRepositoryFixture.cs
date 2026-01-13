using System.Diagnostics;

namespace McjCoderOrg.HookTests.Fixtures;

/// <summary>
/// Creates isolated temporary git repositories for hook testing.
/// </summary>
public sealed class GitRepositoryFixture : IAsyncDisposable
{
    private readonly string _tempDir;
    private bool _disposed;

    public GitRepositoryFixture()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"hook-test-{Guid.NewGuid():N}");
        RepoPath = _tempDir;
        HuskyPath = Path.Combine(_tempDir, ".husky");
    }

    /// <summary>
    /// Gets the path to the test repository.
    /// </summary>
    public string RepoPath { get; }

    /// <summary>
    /// Gets the path to the .husky directory in the test repository.
    /// </summary>
    public string HuskyPath { get; }

    /// <summary>
    /// Gets the current branch name.
    /// </summary>
    public string CurrentBranch { get; private set; } = "main";

    /// <summary>
    /// Gets or sets a value indicating whether GPG signing is configured.
    /// </summary>
    public bool GpgSigningConfigured { get; set; }

    /// <summary>
    /// Initializes the test repository.
    /// </summary>
    /// <param name="sourceHuskyPath">Path to the source .husky directory to copy hooks from.</param>
    public async Task InitializeAsync(string sourceHuskyPath)
    {
        Directory.CreateDirectory(_tempDir);

        // Initialize git repository with main as default branch
        await RunGitAsync("init", "--initial-branch=main").ConfigureAwait(false);
        await RunGitAsync("config", "user.name", "Test User").ConfigureAwait(false);
        await RunGitAsync("config", "user.email", "test@example.com").ConfigureAwait(false);

        // Copy hook scripts from source
        CopyHooksFromSource(sourceHuskyPath);

        // Create initial commit for branch operations
        var readmePath = Path.Combine(_tempDir, "README.md");
        await File.WriteAllTextAsync(readmePath, "# Test Repository\n").ConfigureAwait(false);
        await RunGitAsync("add", "README.md").ConfigureAwait(false);

        // Create a temp commit message file for the initial commit (bypassing hook)
        var commitMsgPath = Path.Combine(_tempDir, ".git", "COMMIT_EDITMSG");
        await File.WriteAllTextAsync(commitMsgPath, "chore: initial commit\n\nRefs: #0").ConfigureAwait(false);
        await RunGitAsync("commit", "--no-verify", "-m", "chore: initial commit\n\nRefs: #0").ConfigureAwait(false);
    }

    /// <summary>
    /// Switches to the specified branch, creating it if necessary.
    /// </summary>
    public async Task SwitchToBranchAsync(string branchName)
    {
        if (string.Equals(branchName, "main", StringComparison.Ordinal))
        {
            await RunGitAsync("checkout", "main").ConfigureAwait(false);
        }
        else
        {
            await RunGitAsync("checkout", "-b", branchName).ConfigureAwait(false);
        }

        CurrentBranch = branchName;
    }

    /// <summary>
    /// Enters detached HEAD state.
    /// </summary>
    public async Task DetachHeadAsync()
    {
        await RunGitAsync("checkout", "--detach", "HEAD").ConfigureAwait(false);
        CurrentBranch = "HEAD";
    }

    /// <summary>
    /// Configures GPG signing in the test repository.
    /// </summary>
    public async Task ConfigureGpgSigningAsync(bool enabled)
    {
        await RunGitAsync("config", "commit.gpgsign", enabled ? "true" : "false").ConfigureAwait(false);

        // When enabling GPG signing for tests, set an empty signing key to prevent
        // inheriting the global config (which would fail email validation)
        if (enabled)
        {
            // Set empty signing key in this repo to skip email validation in the hook
            // The hook only validates email match if SIGNING_KEY is non-empty
            await RunGitAsync("config", "user.signingkey", "").ConfigureAwait(false);
        }

        GpgSigningConfigured = enabled;
    }

    /// <summary>
    /// Stages a file with the specified content.
    /// </summary>
    public async Task StageFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(_tempDir, fileName);
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await File.WriteAllTextAsync(filePath, content).ConfigureAwait(false);
        await RunGitAsync("add", fileName).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a commit message file for commit-msg hook testing.
    /// </summary>
    public async Task CreateCommitMessageFileAsync(string message)
    {
        var commitMsgPath = Path.Combine(_tempDir, ".git", "COMMIT_EDITMSG");
        await File.WriteAllTextAsync(commitMsgPath, message).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the path to the commit message file.
    /// </summary>
    public string GetCommitMessageFilePath()
    {
        return Path.Combine(_tempDir, ".git", "COMMIT_EDITMSG");
    }

    /// <summary>
    /// Creates a fake node_modules directory marker (for hooks checking directory existence).
    /// </summary>
    public void CreateNodeModules()
    {
        var nodeModulesPath = Path.Combine(_tempDir, "node_modules");
        Directory.CreateDirectory(nodeModulesPath);

        // Create a marker file so it's not empty
        File.WriteAllText(Path.Combine(nodeModulesPath, ".package-lock.json"), "{}");
    }

    /// <summary>
    /// Links to the source project's node_modules and copies config files needed for npm tools.
    /// </summary>
    /// <param name="sourceProjectPath">Path to the source project containing node_modules.</param>
    public void LinkNodeModulesAndConfig(string sourceProjectPath)
    {
        var sourceNodeModules = Path.Combine(sourceProjectPath, "node_modules");
        var targetNodeModules = Path.Combine(_tempDir, "node_modules");

        // Remove any existing fake node_modules
        if (Directory.Exists(targetNodeModules))
        {
            Directory.Delete(targetNodeModules, recursive: true);
        }

        // Create a junction/symlink to the source node_modules
        CreateDirectoryLink(targetNodeModules, sourceNodeModules);

        // Copy config files needed by npm tools
        CopyConfigFile(sourceProjectPath, "package.json");
        CopyConfigFile(sourceProjectPath, "commitlint.config.js");
        CopyConfigFile(sourceProjectPath, ".secretlintrc.json");
    }

    private void CopyConfigFile(string sourceDir, string fileName)
    {
        var sourcePath = Path.Combine(sourceDir, fileName);
        if (File.Exists(sourcePath))
        {
            var destPath = Path.Combine(_tempDir, fileName);
            File.Copy(sourcePath, destPath, overwrite: true);
        }
    }

    private static void CreateDirectoryLink(string linkPath, string targetPath)
    {
        if (OperatingSystem.IsWindows())
        {
            // Use directory junction on Windows (doesn't require admin rights)
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c mklink /J \"{linkPath}\" \"{targetPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                },
            };
            process.Start();
            process.WaitForExit();
        }
        else
        {
            // Use symlink on Unix-like systems
            Directory.CreateSymbolicLink(linkPath, targetPath);
        }
    }

    /// <summary>
    /// Removes the node_modules directory if it exists.
    /// </summary>
    public void RemoveNodeModules()
    {
        var nodeModulesPath = Path.Combine(_tempDir, "node_modules");
        if (Directory.Exists(nodeModulesPath))
        {
            Directory.Delete(nodeModulesPath, recursive: true);
        }
    }

    /// <summary>
    /// Creates a solution file for dotnet test detection.
    /// </summary>
    public async Task CreateSolutionFileAsync()
    {
        var slnPath = Path.Combine(_tempDir, "Test.sln");
        await File.WriteAllTextAsync(slnPath, "# Fake solution file for testing").ConfigureAwait(false);
    }

    /// <summary>
    /// Removes all solution files from the repository.
    /// </summary>
    public void RemoveSolutionFiles()
    {
        foreach (var sln in Directory.GetFiles(_tempDir, "*.sln", SearchOption.AllDirectories))
        {
            File.Delete(sln);
        }
    }

    private void CopyHooksFromSource(string sourceHuskyPath)
    {
        Directory.CreateDirectory(HuskyPath);

        var hookFiles = new[] { "pre-commit", "commit-msg", "pre-push" };
        foreach (var hookName in hookFiles)
        {
            var sourcePath = Path.Combine(sourceHuskyPath, hookName);
            if (File.Exists(sourcePath))
            {
                var destPath = Path.Combine(HuskyPath, hookName);
                File.Copy(sourcePath, destPath);

                // Make executable on Unix
                if (!OperatingSystem.IsWindows())
                {
                    MakeExecutable(destPath);
                }
            }
        }
    }

    private static void MakeExecutable(string filePath)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };
            process.Start();
            process.WaitForExit();
        }
#pragma warning disable CA1031, RCS1075 // Catch specific exception types - chmod failure is non-critical
        catch (Exception)
        {
            // Intentionally empty - chmod failure is non-critical for tests
        }
#pragma warning restore CA1031, RCS1075
    }

    private async Task RunGitAsync(params string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = string.Join(" ", args.Select(a => a.Contains(' ', StringComparison.Ordinal) ? $"\"{a}\"" : a)),
                WorkingDirectory = _tempDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        await process.WaitForExitAsync().ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
            throw new InvalidOperationException($"Git command failed: git {string.Join(" ", args)}\n{error}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (Directory.Exists(_tempDir))
        {
            // Give git a moment to release file handles
            await Task.Delay(100).ConfigureAwait(false);

            try
            {
                Directory.Delete(_tempDir, recursive: true);
            }
#pragma warning disable CA1031, RCS1075 // Catch specific exception types - cleanup failure is non-critical
            catch (Exception)
            {
                // Intentionally empty - cleanup failure is non-critical for tests
            }
#pragma warning restore CA1031, RCS1075
        }
    }
}
