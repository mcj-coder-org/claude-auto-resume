using System.Diagnostics;
using System.Globalization;
using System.Text;

using Pty.Net;

using Serilog;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Wraps Claude Code in a pseudo-terminal, monitors for rate limits,
/// and automatically continues after the limit resets.
/// Supports headless mode with auto-response to prompts.
/// </summary>
internal sealed class ClaudeMonitor : IDisposable
{
    private readonly WrapperConfig _config;
    private readonly ILogger _logger;
    private readonly StringBuilder _outputBuffer = new();
    private readonly Lock _bufferLock = new();
    private readonly Stopwatch _timeSinceLastOutput = new();
    private readonly Stopwatch _timeSinceLastContinue = new();
    private readonly Stopwatch _timeSincePromptDetected = new();

    private IPtyConnection? _pty;
    private CancellationTokenSource? _cts;
    private bool _disposed;
    private bool _potentialPromptDetected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaudeMonitor"/> class.
    /// </summary>
    /// <param name="config">The wrapper configuration.</param>
    /// <param name="logger">
    /// Optional logger instance. If not provided, falls back to the global Serilog logger.
    /// Inject a logger for testability.
    /// </param>
    public ClaudeMonitor(WrapperConfig config, ILogger? logger = null)
    {
        _config = config;
        _logger = logger ?? Log.Logger;
    }

    /// <summary>
    /// Runs the Claude monitor with the specified additional arguments.
    /// </summary>
    /// <param name="additionalArgs">Additional arguments to pass to Claude.</param>
    /// <returns>True if claude was found and executed; false if claude was not found.</returns>
    public async Task<bool> RunAsync(IReadOnlyList<string> additionalArgs)
    {
        _cts = new CancellationTokenSource();
        SetupCancellationHandler();

        var claudePath = _config.ClaudePath ?? FindClaudeInPath();
        if (claudePath == null)
        {
            _logger.Error("Could not find 'claude' in PATH");
            WriteErrorLine("[claude-auto-resume] Error: Could not find 'claude' in PATH");
            return false;
        }

        await SpawnAndMonitorAsync(claudePath, additionalArgs).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cts?.Cancel();
        _cts?.Dispose();
        _pty?.Dispose();
    }

    private void SetupCancellationHandler()
    {
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            _cts?.Cancel();
        };
    }

    private async Task SpawnAndMonitorAsync(string claudePath, IReadOnlyList<string> additionalArgs)
    {
        var commandLine = BuildCommandLine(additionalArgs);
        LogHeadlessModeInfo(commandLine);

        var options = CreatePtyOptions(claudePath, commandLine);
        _pty = await PtyProvider.SpawnAsync(options, _cts!.Token).ConfigureAwait(false);
        _timeSinceLastOutput.Start();
        _timeSinceLastContinue.Start();

        await RunMonitoringTasksAsync().ConfigureAwait(false);
        await WaitForProcessExitAsync().ConfigureAwait(false);
    }

    private void LogHeadlessModeInfo(List<string> commandLine)
    {
        if (_config.Headless)
        {
            var cmdLine = string.Join(" ", commandLine);
            _logger.Information("Headless mode - Command: claude {CommandLine}", cmdLine);
            WriteLine(string.Create(CultureInfo.InvariantCulture, $"[claude-auto-resume] Command: claude {cmdLine}"));
        }
    }

    private static PtyOptions CreatePtyOptions(string claudePath, List<string> commandLine)
    {
        return new PtyOptions
        {
            Name = "claude-auto-resume",
            Cols = GetConsoleWidth(),
            Rows = GetConsoleHeight(),
            Cwd = Environment.CurrentDirectory,
            App = claudePath,
            CommandLine = [.. commandLine],
            Environment = GetEnvironment(),
        };
    }

    private static int GetConsoleWidth()
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

    private static int GetConsoleHeight()
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

    private async Task RunMonitoringTasksAsync()
    {
        var tasks = new List<Task>
        {
            ReadOutputAsync(_cts!.Token),
            MonitorWindowSizeAsync(_cts.Token),
        };

        if (!_config.Headless)
        {
            tasks.Add(ForwardInputAsync(_cts.Token));
        }
        else
        {
            tasks.Add(MonitorForHangingPromptsAsync(_cts.Token));
        }

        try
        {
            await Task.WhenAny(tasks).ConfigureAwait(false);
        }
#pragma warning disable S6667 // Intentional: Logging without exception is correct for expected cancellation
        catch (OperationCanceledException)
        {
            _logger.Debug("Operation cancelled - normal shutdown");
        }
#pragma warning restore S6667
#pragma warning disable CA1031 // Intentional: Top-level handler for PTY errors
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during PTY operation");
            WriteErrorLine(string.Create(CultureInfo.InvariantCulture, $"\n[claude-auto-resume] Error: {ex.Message}"));
        }
#pragma warning restore CA1031
    }

    private async Task WaitForProcessExitAsync()
    {
        try
        {
            await Task.Run(
                () =>
                {
                    while (!_cts!.Token.IsCancellationRequested && !_pty!.WaitForExit(500))
                    {
                        // Keep waiting
                    }
                },
                _cts!.Token).ConfigureAwait(false);

            if (!_cts.Token.IsCancellationRequested)
            {
                _logger.Information("Claude exited with code {ExitCode}", _pty!.ExitCode);
                WriteLine(string.Create(CultureInfo.InvariantCulture, $"\n[claude-auto-resume] Claude exited with code: {_pty.ExitCode}"));
            }
        }
#pragma warning disable S6667 // Intentional: Logging without exception is correct for expected cancellation
        catch (OperationCanceledException)
        {
            _logger.Information("Shutdown requested by user");
            WriteLine("\n[claude-auto-resume] Shutdown requested");
        }
#pragma warning restore S6667
    }

    internal List<string> BuildCommandLine(IReadOnlyList<string> additionalArgs)
    {
        var args = new List<string>();

        if (_config.DangerouslySkipPermissions)
        {
            args.Add("--dangerously-skip-permissions");
        }

        if (_config.ContinueConversation)
        {
            args.Add("-c");
        }

        if (!string.IsNullOrEmpty(_config.InitialPrompt))
        {
            args.Add("-p");
            args.Add(_config.InitialPrompt);
        }

        args.AddRange(additionalArgs);
        return args;
    }

    private async Task ReadOutputAsync(CancellationToken ct)
    {
        var buffer = new byte[4096];
        var stream = _pty!.ReaderStream;

        while (!ct.IsCancellationRequested)
        {
            int bytesRead;
            try
            {
                bytesRead = await stream.ReadAsync(buffer, ct).ConfigureAwait(false);
            }
            catch (IOException)
            {
                break;
            }

            if (bytesRead == 0)
            {
                break;
            }

            var text = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            _timeSinceLastOutput.Restart();

            Console.Write(text);
            AppendToBuffer(text);

            if (_timeSinceLastContinue.Elapsed.TotalSeconds > _config.CooldownSeconds)
            {
                await CheckForRateLimitAsync(ct).ConfigureAwait(false);
            }

            if (_config.Headless)
            {
                CheckForPromptPattern();
            }
        }
    }

    private void CheckForPromptPattern()
    {
        string bufferText;
        lock (_bufferLock)
        {
            bufferText = _outputBuffer.ToString();
        }

        var matchFound = _config.PromptPatterns.Any(pattern =>
            bufferText.Contains(pattern, StringComparison.OrdinalIgnoreCase));

        if (matchFound)
        {
            if (!_potentialPromptDetected)
            {
                _potentialPromptDetected = true;
                _timeSincePromptDetected.Restart();
            }
        }
        else
        {
            _potentialPromptDetected = false;
        }
    }

    private async Task MonitorForHangingPromptsAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(500, ct).ConfigureAwait(false);

            if (!_potentialPromptDetected)
            {
                continue;
            }

            var timeSincePrompt = _timeSincePromptDetected.Elapsed.TotalSeconds;
            var timeSinceOutput = _timeSinceLastOutput.Elapsed.TotalSeconds;

            if (timeSincePrompt >= _config.PromptTimeoutSeconds &&
                timeSinceOutput >= _config.PromptTimeoutSeconds)
            {
                await HandleHangingPromptAsync(ct).ConfigureAwait(false);
                _potentialPromptDetected = false;
            }
        }
    }

    private async Task HandleHangingPromptAsync(CancellationToken ct)
    {
        var escapedResponse = EscapeForDisplay(_config.DefaultPromptResponse);
        _logger.Information("Detected prompt, auto-responding: {Response}", escapedResponse);

        Console.ForegroundColor = ConsoleColor.Cyan;
        WriteLine(string.Create(CultureInfo.InvariantCulture, $"\n[claude-auto-resume] Detected prompt, auto-responding: {escapedResponse}"));
        Console.ResetColor();

        lock (_bufferLock)
        {
            _outputBuffer.Clear();
        }

        var bytes = Encoding.UTF8.GetBytes(_config.DefaultPromptResponse);
        await _pty!.WriterStream.WriteAsync(bytes, ct).ConfigureAwait(false);
        await _pty.WriterStream.FlushAsync(ct).ConfigureAwait(false);
    }

    private static string EscapeForDisplay(string s)
    {
        return s.Replace("\n", "\\n", StringComparison.Ordinal)
                .Replace("\r", "\\r", StringComparison.Ordinal);
    }

    private async Task ForwardInputAsync(CancellationToken ct)
    {
        var stream = _pty!.WriterStream;

        while (!ct.IsCancellationRequested)
        {
            var key = await Task.Run(
                () => Console.KeyAvailable ? Console.ReadKey(intercept: true) : (ConsoleKeyInfo?)null,
                ct).ConfigureAwait(false);

            if (key == null)
            {
                await Task.Delay(10, ct).ConfigureAwait(false);
                continue;
            }

            var bytes = ConvertKeyToBytes(key.Value);
            await stream.WriteAsync(bytes, ct).ConfigureAwait(false);
            await stream.FlushAsync(ct).ConfigureAwait(false);
        }
    }

    private static byte[] ConvertKeyToBytes(ConsoleKeyInfo k)
    {
        return k.Key switch
        {
            ConsoleKey.Enter => "\r"u8.ToArray(),
            ConsoleKey.Backspace => [0x7F],
            ConsoleKey.Tab => "\t"u8.ToArray(),
            ConsoleKey.Escape => [0x1B],
            ConsoleKey.UpArrow => "\x1b[A"u8.ToArray(),
            ConsoleKey.DownArrow => "\x1b[B"u8.ToArray(),
            ConsoleKey.RightArrow => "\x1b[C"u8.ToArray(),
            ConsoleKey.LeftArrow => "\x1b[D"u8.ToArray(),
            ConsoleKey.Home => "\x1b[H"u8.ToArray(),
            ConsoleKey.End => "\x1b[F"u8.ToArray(),
            ConsoleKey.Delete => "\x1b[3~"u8.ToArray(),
            ConsoleKey.PageUp => "\x1b[5~"u8.ToArray(),
            ConsoleKey.PageDown => "\x1b[6~"u8.ToArray(),
            _ when k.Modifiers.HasFlag(ConsoleModifiers.Control) && k.Key >= ConsoleKey.A && k.Key <= ConsoleKey.Z
                => [(byte)(k.Key - ConsoleKey.A + 1)],
            _ => Encoding.UTF8.GetBytes(k.KeyChar.ToString()),
        };
    }

    private async Task MonitorWindowSizeAsync(CancellationToken ct)
    {
        var lastWidth = GetConsoleWidth();
        var lastHeight = GetConsoleHeight();

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(500, ct).ConfigureAwait(false);

            var width = GetConsoleWidth();
            var height = GetConsoleHeight();

            if (width != lastWidth || height != lastHeight)
            {
                lastWidth = width;
                lastHeight = height;
                _pty?.Resize(width, height);
            }
        }
    }

    private void AppendToBuffer(string text)
    {
        lock (_bufferLock)
        {
            _outputBuffer.Append(text);

            if (_outputBuffer.Length > _config.OutputBufferSize)
            {
                _outputBuffer.Remove(0, _outputBuffer.Length - _config.OutputBufferSize);
            }
        }
    }

    private async Task CheckForRateLimitAsync(CancellationToken ct)
    {
        string bufferText;
        lock (_bufferLock)
        {
            bufferText = _outputBuffer.ToString();
        }

        var matchedPattern = _config.RateLimitPatterns
            .FirstOrDefault(pattern =>
                bufferText.Contains(pattern, StringComparison.OrdinalIgnoreCase) &&
                bufferText.Contains("limit", StringComparison.OrdinalIgnoreCase) &&
                (bufferText.Contains("reached", StringComparison.OrdinalIgnoreCase) ||
                 bufferText.Contains("reset", StringComparison.OrdinalIgnoreCase)));

        if (matchedPattern != null)
        {
            await HandleRateLimitAsync(matchedPattern, ct).ConfigureAwait(false);
        }
    }

    private async Task HandleRateLimitAsync(string matchedPattern, CancellationToken ct)
    {
        _timeSinceLastContinue.Restart();

        lock (_bufferLock)
        {
            _outputBuffer.Clear();
        }

        _logger.Warning("Rate limit detected (matched: {Pattern}), waiting {WaitMinutes} minutes", matchedPattern, _config.WaitMinutes);

        WriteRateLimitDetectedMessage(matchedPattern);
        await WaitWithCountdownAsync(ct).ConfigureAwait(false);
        await SendContinueCommandAsync(ct).ConfigureAwait(false);
    }

    private void WriteRateLimitDetectedMessage(string matchedPattern)
    {
        WriteLine(string.Empty);
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteLine(string.Create(CultureInfo.InvariantCulture, $"[claude-auto-resume] Rate limit detected (matched: \"{matchedPattern}\")"));
        WriteLine(string.Create(CultureInfo.InvariantCulture, $"[claude-auto-resume] Waiting {_config.WaitMinutes} minutes before continuing..."));
        Console.ResetColor();
    }

    private async Task WaitWithCountdownAsync(CancellationToken ct)
    {
        var waitTime = TimeSpan.FromMinutes(_config.WaitMinutes);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < waitTime && !ct.IsCancellationRequested)
        {
            var remaining = waitTime - stopwatch.Elapsed;
            Console.Write(string.Create(CultureInfo.InvariantCulture, $"\r[claude-auto-resume] Resuming in: {remaining:mm\\:ss}   "));
            await Task.Delay(1000, ct).ConfigureAwait(false);
        }
    }

    private async Task SendContinueCommandAsync(CancellationToken ct)
    {
        _logger.Information("Sending continue command after rate limit wait");

        WriteLine(string.Empty);
        Console.ForegroundColor = ConsoleColor.Green;
        WriteLine("[claude-auto-resume] Sending continue command...");
        Console.ResetColor();

        var bytes = Encoding.UTF8.GetBytes(_config.ContinueCommand);
        await _pty!.WriterStream.WriteAsync(bytes, ct).ConfigureAwait(false);
        await _pty.WriterStream.FlushAsync(ct).ConfigureAwait(false);
    }

    private static string? FindClaudeInPath()
    {
        var pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var separator = OperatingSystem.IsWindows() ? ';' : ':';

        // On Windows, npm installs claude.cmd (not claude.exe)
        // Check for .cmd, .exe, and extensionless in order of likelihood
        string[] executableNames = OperatingSystem.IsWindows()
            ? ["claude.cmd", "claude.exe", "claude"]
            : ["claude"];

        var pathDirs = pathVar.Split(separator);
        foreach (var executable in executableNames)
        {
            var pathResult = pathDirs
                .Select(dir => Path.Combine(dir, executable))
                .FirstOrDefault(File.Exists);

            if (pathResult != null)
            {
                return pathResult;
            }
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Check common npm installation paths
        foreach (var executable in executableNames)
        {
            string[] npmPaths =
            [
                Path.Combine(home, ".npm-global", "bin", executable),
                Path.Combine(home, "AppData", "Roaming", "npm", executable),
            ];

            var npmResult = npmPaths.FirstOrDefault(File.Exists);
            if (npmResult != null)
            {
                return npmResult;
            }
        }

        // Unix-specific paths
        if (!OperatingSystem.IsWindows())
        {
            string[] unixPaths = ["/usr/local/bin/claude", "/usr/bin/claude"];
            return unixPaths.FirstOrDefault(File.Exists);
        }

        return null;
    }

    private static Dictionary<string, string> GetEnvironment()
    {
        var env = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var key in Environment.GetEnvironmentVariables().Keys)
        {
            var k = key.ToString()!;
            env[k] = Environment.GetEnvironmentVariable(k) ?? string.Empty;
        }

        if (!env.TryGetValue("TERM", out var term) || string.IsNullOrEmpty(term))
        {
            env["TERM"] = "xterm-256color";
        }

        return env;
    }

    // Console output helpers - sync calls are intentional for PTY user-facing output
#pragma warning disable CA1849 // Console output is intentionally synchronous for terminal UI
    private static void WriteLine(string message) => Console.WriteLine(message);

    private static void WriteErrorLine(string message) => Console.Error.WriteLine(message);
#pragma warning restore CA1849
}
