namespace McjCoderOrg.ClaudeAutoResume.Services;

/// <summary>
/// Parses command-line arguments.
/// </summary>
internal interface IArgumentParser
{
    /// <summary>
    /// Parses command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The parse result.</returns>
    ParseResult Parse(string[] args);
}
