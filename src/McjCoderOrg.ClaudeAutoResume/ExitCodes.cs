namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Semantic exit codes for the CLI application.
/// </summary>
/// <remarks>
/// See ADR-0018 for exit code design decisions.
/// </remarks>
internal static class ExitCodes
{
    /// <summary>Normal completion.</summary>
    public const int Success = 0;

    /// <summary>Unhandled exception.</summary>
    public const int GeneralError = 1;

    /// <summary>Invalid command line arguments.</summary>
    public const int InvalidArguments = 2;

    /// <summary>Invalid configuration.</summary>
    public const int ConfigurationError = 3;

    /// <summary>Claude CLI not found.</summary>
    public const int DependencyMissing = 4;

    /// <summary>Exited due to rate limit.</summary>
    public const int RateLimitDetected = 5;

    /// <summary>User interrupted (Ctrl+C).</summary>
    public const int UserCancelled = 6;
}
