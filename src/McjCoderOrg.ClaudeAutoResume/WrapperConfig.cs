using System.Collections.Immutable;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Configuration for the Claude wrapper behavior.
/// </summary>
internal sealed record WrapperConfig
{
    /// <summary>
    /// Patterns to detect in output that indicate a rate limit.
    /// Case-insensitive matching. Aligned with claude-auto-resume patterns.
    /// </summary>
    public string[] RateLimitPatterns { get; init; } = [];

    /// <summary>
    /// Minutes to wait after detecting a rate limit before continuing.
    /// </summary>
    public int WaitMinutes { get; init; } = 15;

    /// <summary>
    /// Command to send to continue after waiting.
    /// </summary>
    public string ContinueCommand { get; init; } = "\n";

    /// <summary>
    /// How many characters of recent output to buffer for pattern matching.
    /// </summary>
    public int OutputBufferSize { get; init; } = 2000;

    /// <summary>
    /// Path to the claude executable. Null means find in PATH.
    /// </summary>
    public string? ClaudePath { get; init; }

    /// <summary>
    /// Seconds to debounce after sending continue command before checking for limits again.
    /// Prevents detecting the same limit message twice.
    /// </summary>
    public int CooldownSeconds { get; init; } = 30;

    /// <summary>
    /// Run in headless mode (no user input, auto-respond to prompts).
    /// </summary>
    public bool Headless { get; init; }

    /// <summary>
    /// Pass --dangerously-skip-permissions to Claude. Required for headless mode.
    /// </summary>
    public bool DangerouslySkipPermissions { get; init; }

    /// <summary>
    /// Patterns that indicate Claude is waiting for user input.
    /// Used in headless mode to auto-respond.
    /// </summary>
    public string[] PromptPatterns { get; init; } = [];

    /// <summary>
    /// Default response when a prompt is detected in headless mode.
    /// </summary>
    public string DefaultPromptResponse { get; init; } = "y\n";

    /// <summary>
    /// Seconds of inactivity after a prompt pattern before auto-responding.
    /// Helps avoid false positives by waiting to see if more output comes.
    /// </summary>
    public double PromptTimeoutSeconds { get; init; } = 2.0;

    /// <summary>
    /// Initial prompt to send to Claude (like -p flag in CLI).
    /// </summary>
    public string? InitialPrompt { get; init; }

    /// <summary>
    /// Continue previous conversation (like -c flag in CLI).
    /// </summary>
    public bool ContinueConversation { get; init; }

    /// <summary>
    /// Default configuration with patterns aligned to claude-auto-resume.
    /// </summary>
    public static WrapperConfig Default => new()
    {
        // Rate limit patterns from claude-auto-resume
        RateLimitPatterns =
        [
            // Old format
            "claude ai usage limit reached",
            // New format
            "limit reached",
            "resets",
            // Additional patterns
            "rate limit",
            "rate-limit",
            "too many requests",
            "usage limit",
            "quota exceeded",
        ],
        WaitMinutes = 15,
        ContinueCommand = "\n",
        OutputBufferSize = 2000,
        CooldownSeconds = 30,
        Headless = false,
        DangerouslySkipPermissions = false,
        // Patterns indicating Claude wants input
        PromptPatterns =
        [
            // Yes/No prompts
            "[y/n]",
            "[yes/no]",
            "(y/n)",
            "(yes/no)",
            "? [y]",
            "? [n]",
            // Permission prompts
            "allow this",
            "proceed?",
            "continue?",
            "confirm",
            // Input prompts
            "enter to continue",
            "press enter",
            "waiting for input",
            // Tool approval
            "approve",
            "deny",
            "allow once",
            "allow always",
        ],
        DefaultPromptResponse = "y\n",
        PromptTimeoutSeconds = 2.0,
    };
}
