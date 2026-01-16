namespace McjCoderOrg.ClaudeAutoResume.Services;

/// <summary>
/// Abstracts environment and filesystem operations for testability.
/// </summary>
internal interface IEnvironmentService
{
    /// <summary>
    /// Gets an environment variable value.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The value, or null if not found.</returns>
    string? GetEnvironmentVariable(string name);

    /// <summary>
    /// Gets all environment variables.
    /// </summary>
    /// <returns>A dictionary of environment variables.</returns>
    IDictionary<string, string> GetEnvironmentVariables();

    /// <summary>
    /// Gets the current working directory.
    /// </summary>
    string CurrentDirectory { get; }

    /// <summary>
    /// Gets the user's home directory.
    /// </summary>
    string UserProfile { get; }

    /// <summary>
    /// Gets a special folder path.
    /// </summary>
    /// <param name="folder">The special folder.</param>
    /// <returns>The path.</returns>
    string GetFolderPath(Environment.SpecialFolder folder);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>True if the file exists.</returns>
    bool FileExists(string path);

    /// <summary>
    /// Creates a directory.
    /// </summary>
    /// <param name="path">The directory path.</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Gets whether the current OS is Windows.
    /// </summary>
    bool IsWindows { get; }
}
