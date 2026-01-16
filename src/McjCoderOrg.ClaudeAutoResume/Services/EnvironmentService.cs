using System.Diagnostics.CodeAnalysis;

namespace McjCoderOrg.ClaudeAutoResume.Services;

/// <summary>
/// Production implementation of <see cref="IEnvironmentService"/> wrapping System.Environment and System.IO.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin wrapper around System.Environment and System.IO with no testable logic")]
internal sealed class EnvironmentService : IEnvironmentService
{
    /// <inheritdoc/>
    public string? GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

    /// <inheritdoc/>
    public IDictionary<string, string> GetEnvironmentVariables()
    {
        var env = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var key in Environment.GetEnvironmentVariables().Keys)
        {
            var k = key.ToString()!;
            env[k] = Environment.GetEnvironmentVariable(k) ?? string.Empty;
        }

        return env;
    }

    /// <inheritdoc/>
    public string CurrentDirectory => Environment.CurrentDirectory;

    /// <inheritdoc/>
    public string UserProfile => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    /// <inheritdoc/>
    public string GetFolderPath(Environment.SpecialFolder folder) => Environment.GetFolderPath(folder);

    /// <inheritdoc/>
    public bool FileExists(string path) => File.Exists(path);

    /// <inheritdoc/>
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    /// <inheritdoc/>
    public bool IsWindows => OperatingSystem.IsWindows();
}
