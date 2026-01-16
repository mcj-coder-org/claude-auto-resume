namespace McjCoderOrg.ClaudeAutoResume.Services;

/// <summary>
/// Abstracts console I/O operations for testability.
/// </summary>
internal interface IConsoleService
{
    /// <summary>
    /// Writes a string to the console.
    /// </summary>
    /// <param name="value">The value to write.</param>
    void Write(string value);

    /// <summary>
    /// Writes a line to the console.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void WriteLine(string message);

    /// <summary>
    /// Writes a line to stderr.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void WriteErrorLine(string message);

    /// <summary>
    /// Gets or sets the foreground color.
    /// </summary>
    ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// Resets the console colors.
    /// </summary>
    void ResetColor();

    /// <summary>
    /// Gets whether input is redirected.
    /// </summary>
    bool IsInputRedirected { get; }

    /// <summary>
    /// Gets whether a key is available for reading.
    /// </summary>
    bool KeyAvailable { get; }

    /// <summary>
    /// Reads a key from the console.
    /// </summary>
    /// <param name="intercept">True to not display the key.</param>
    /// <returns>The key info.</returns>
    ConsoleKeyInfo ReadKey(bool intercept);

    /// <summary>
    /// Gets the input text reader.
    /// </summary>
    TextReader In { get; }

    /// <summary>
    /// Gets the console window width.
    /// </summary>
    int WindowWidth { get; }

    /// <summary>
    /// Gets the console window height.
    /// </summary>
    int WindowHeight { get; }

    /// <summary>
    /// Sets up a handler for Ctrl+C.
    /// </summary>
    /// <param name="handler">The handler to invoke.</param>
    void SetCancelKeyPressHandler(ConsoleCancelEventHandler handler);
}
