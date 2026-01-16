namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Result of parsing command-line arguments.
/// </summary>
internal sealed record ParseResult
{
    /// <summary>
    /// Gets a value indicating whether to show help.
    /// </summary>
    public bool ShowHelp { get; init; }

    /// <summary>
    /// Gets a value indicating whether to show version.
    /// </summary>
    public bool ShowVersion { get; init; }

    /// <summary>
    /// Gets a value indicating whether to show diagnostics.
    /// </summary>
    public bool ShowDiagnose { get; init; }

    /// <summary>
    /// Gets a value indicating whether verbose logging is enabled.
    /// </summary>
    public bool Verbose { get; init; }

    /// <summary>
    /// Gets a value indicating whether headless mode is enabled.
    /// </summary>
    public bool Headless { get; init; }

    /// <summary>
    /// Gets a value indicating whether dangerous mode is enabled.
    /// </summary>
    public bool Dangerous { get; init; }

    /// <summary>
    /// Gets a value indicating whether to continue a previous conversation.
    /// </summary>
    public bool ContinueConversation { get; init; }

    /// <summary>
    /// Gets the initial prompt to send to Claude.
    /// </summary>
    public string? InitialPrompt { get; init; }

    /// <summary>
    /// Gets the number of minutes to wait on rate limit.
    /// </summary>
    public int? WaitMinutes { get; init; }

    /// <summary>
    /// Gets additional arguments to pass to Claude.
    /// </summary>
    public List<string> ClaudeArgs { get; init; } = [];

    /// <summary>
    /// Gets any error message from parsing.
    /// </summary>
    public string? ErrorMessage { get; init; }
}
