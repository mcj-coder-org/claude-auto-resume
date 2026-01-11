namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Provides logging configuration and path resolution.
/// </summary>
/// <remarks>
/// See ADR-0017 and ADR-0018 for logging and path design decisions.
/// </remarks>
internal static class LoggingConfiguration
{
    private const string AppName = "claude-auto-resume";

    /// <summary>
    /// Gets the log directory path for the current platform.
    /// </summary>
    /// <returns>The absolute path to the log directory.</returns>
    public static string GetLogDirectory()
    {
        var basePath = GetPlatformLogBasePath();
        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(basePath, AppName);
        }
        return Path.Combine(basePath, AppName, "logs");
    }

    /// <summary>
    /// Gets the log file path for the current day.
    /// </summary>
    /// <returns>The absolute path to the log file.</returns>
    public static string GetLogFilePath()
    {
        var directory = GetLogDirectory();
        var fileName = string.Create(System.Globalization.CultureInfo.InvariantCulture, $"{DateTime.UtcNow:yyyy-MM-dd}.log");
        return Path.Combine(directory, fileName);
    }

    private static string GetPlatformLogBasePath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library",
                "Logs");
        }

        // Linux and others
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local",
            "share");
    }
}
