namespace McjCoderOrg.ClaudeAutoResume.Services;

/// <summary>
/// Production implementation of <see cref="IConsoleService"/> wrapping System.Console.
/// </summary>
internal sealed class ConsoleService : IConsoleService
{
    /// <inheritdoc/>
    public void Write(string value) => Console.Write(value);

    /// <inheritdoc/>
    public void WriteLine(string message) => Console.WriteLine(message);

    /// <inheritdoc/>
    public void WriteErrorLine(string message) => Console.Error.WriteLine(message);

    /// <inheritdoc/>
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    /// <inheritdoc/>
    public void ResetColor() => Console.ResetColor();

    /// <inheritdoc/>
    public bool IsInputRedirected => Console.IsInputRedirected;

    /// <inheritdoc/>
    public bool KeyAvailable => Console.KeyAvailable;

    /// <inheritdoc/>
    public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

    /// <inheritdoc/>
    public TextReader In => Console.In;

    /// <inheritdoc/>
    public int WindowWidth
    {
        get
        {
            try
            {
                var width = Console.WindowWidth;
                return width > 0 ? width : 120;
            }
            catch (IOException)
            {
                // No console attached (e.g., running with redirected streams)
                return 120;
            }
        }
    }

    /// <inheritdoc/>
    public int WindowHeight
    {
        get
        {
            try
            {
                var height = Console.WindowHeight;
                return height > 0 ? height : 30;
            }
            catch (IOException)
            {
                // No console attached (e.g., running with redirected streams)
                return 30;
            }
        }
    }

    /// <inheritdoc/>
    public void SetCancelKeyPressHandler(ConsoleCancelEventHandler handler)
    {
        Console.CancelKeyPress += handler;
    }
}
