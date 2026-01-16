using System.Globalization;
using System.Reflection;
using System.Text;

using McjCoderOrg.ClaudeAutoResume.Resources;
using McjCoderOrg.ClaudeAutoResume.Services;

using Serilog;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Main application implementation with injected dependencies.
/// </summary>
internal sealed class Application : IApplication
{
    private static readonly CompositeFormat _errorLogLocationFormat = CompositeFormat.Parse(Strings.ErrorLogLocation);
    private static readonly CompositeFormat _diagnoseRuntimeInfoFormat = CompositeFormat.Parse(Strings.DiagnoseRuntimeInfo);
    private static readonly CompositeFormat _diagnoseOsInfoFormat = CompositeFormat.Parse(Strings.DiagnoseOsInfo);

    private readonly IArgumentParser _argumentParser;
    private readonly IConsoleService _console;
    private readonly IEnvironmentService _environment;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Application"/> class.
    /// </summary>
    /// <param name="argumentParser">The argument parser.</param>
    /// <param name="console">The console service.</param>
    /// <param name="environment">The environment service.</param>
    /// <param name="logger">The logger.</param>
    public Application(
        IArgumentParser argumentParser,
        IConsoleService console,
        IEnvironmentService environment,
        ILogger logger)
    {
        _argumentParser = argumentParser;
        _console = console;
        _environment = environment;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        var parseResult = _argumentParser.Parse(args);

        if (parseResult.ShowHelp)
        {
            PrintHelp();
            return ExitCodes.Success;
        }

        if (parseResult.ShowVersion)
        {
            PrintVersion();
            return ExitCodes.Success;
        }

        if (parseResult.ShowDiagnose)
        {
            PrintDiagnostics();
            return ExitCodes.Success;
        }

        if (parseResult.ErrorMessage is not null)
        {
            WriteArgumentError(parseResult.ErrorMessage);
            return ExitCodes.InvalidArguments;
        }

        var validationError = ValidateArguments(parseResult);
        if (validationError is not null)
        {
            WriteValidationError(validationError);
            return ExitCodes.InvalidArguments;
        }

        var config = BuildConfig(parseResult);

        LogStartup();
        PrintStartupInfo(config, parseResult.Headless, parseResult.Dangerous);

        using var monitor = CreateMonitor(config);
        var claudeFound = await monitor.RunAsync(parseResult.ClaudeArgs).ConfigureAwait(false);

        return claudeFound ? ExitCodes.Success : ExitCodes.DependencyMissing;
    }

    private ClaudeMonitor CreateMonitor(WrapperConfig config)
    {
        return new ClaudeMonitor(config, _console, _environment, _logger);
    }

    private static string? ValidateArguments(ParseResult result)
    {
        if (result.Headless && !result.Dangerous)
        {
            return "headless-requires-dangerous";
        }

        return null;
    }

    private void WriteArgumentError(string message)
    {
        _console.WriteErrorLine(message);
    }

    private void WriteValidationError(string errorCode)
    {
        if (string.Equals(errorCode, "headless-requires-dangerous", StringComparison.Ordinal))
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine("Error: --headless mode requires --dangerously-skip-permissions");
            _console.ResetColor();
            _console.WriteLine(string.Empty);
            _console.WriteLine("Headless mode auto-responds to prompts without user confirmation.");
            _console.WriteLine("This is equivalent to claude-auto-resume's behavior.");
            _console.WriteLine(string.Empty);
            _console.WriteLine("To enable, run:");
            _console.WriteLine("  claude-auto-resume --headless --dangerously-skip-permissions");
        }
    }

    private static WrapperConfig BuildConfig(ParseResult result)
    {
        return WrapperConfig.Default with
        {
            WaitMinutes = result.WaitMinutes ?? WrapperConfig.Default.WaitMinutes,
            Headless = result.Headless,
            DangerouslySkipPermissions = result.Dangerous,
            InitialPrompt = result.InitialPrompt,
            ContinueConversation = result.ContinueConversation,
        };
    }

    private void LogStartup()
    {
        var platform = PlatformInfo.Current;
        _logger.Information(Strings.StartingApp, platform.AppVersion);
    }

    private void PrintStartupInfo(WrapperConfig config, bool headless, bool dangerous)
    {
        _console.WriteLine("[claude-auto-resume] Starting Claude Code...");
        _console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"[claude-auto-resume] Auto-continue on rate limit: enabled (wait {config.WaitMinutes} min)"));

        if (headless)
        {
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.WriteLine("[claude-auto-resume] HEADLESS MODE - auto-responding to prompts");
            _console.ResetColor();
        }

        if (dangerous)
        {
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.WriteLine("[claude-auto-resume] --dangerously-skip-permissions enabled");
            _console.ResetColor();
        }

        _console.WriteLine("[claude-auto-resume] Press Ctrl+C to exit");
        _console.WriteLine(string.Empty);
    }

    private void PrintVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
        _console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"claude-auto-resume {version.Major}.{version.Minor}.{version.Build}"));
    }

    private void PrintHelp()
    {
        PrintHelpHeader();
        PrintHelpOptions();
        PrintHelpEnvironment();
        PrintHelpExamples();
        PrintHelpModes();
        PrintHelpWarning();
    }

    private void PrintHelpHeader()
    {
        _console.WriteLine(Strings.AppDescription);
        _console.WriteLine(string.Empty);
        _console.WriteLine("USAGE:");
        _console.WriteLine("    claude-auto-resume [OPTIONS] [-- CLAUDE_ARGS...]");
        _console.WriteLine(string.Empty);
    }

    private void PrintHelpOptions()
    {
        _console.WriteLine("OPTIONS:");
        _console.WriteLine("    -h, --help                      Show this help");
        _console.WriteLine("    -v, --version                   Show version information");
        _console.WriteLine("    -p, --prompt <PROMPT>           Initial prompt to send to Claude");
        _console.WriteLine("    -c, --continue                  Continue previous conversation");
        _console.WriteLine("    -w, --wait <MINUTES>            Minutes to wait on rate limit (default: 15)");
        _console.WriteLine("    -V, --verbose                   Enable verbose logging to file");
        _console.WriteLine("    --headless                      Run without user input (auto-respond to prompts)");
        _console.WriteLine("    --dangerously-skip-permissions  Pass dangerous flag to Claude (required for headless)");
        _console.WriteLine("    --dangerous                     Alias for --dangerously-skip-permissions");
        _console.WriteLine("    --diagnose                      Run environment diagnostics");
        _console.WriteLine(string.Empty);
    }

    private void PrintHelpEnvironment()
    {
        _console.WriteLine("ENVIRONMENT:");
        _console.WriteLine("    CLAUDE_WAIT_MINUTES             Override default wait time");
        _console.WriteLine(string.Empty);
    }

    private void PrintHelpExamples()
    {
        _console.WriteLine("EXAMPLES:");
        _console.WriteLine("    # Interactive mode (default)");
        _console.WriteLine("    claude-auto-resume");
        _console.WriteLine(string.Empty);
        _console.WriteLine("    # With initial prompt");
        _console.WriteLine("    claude-auto-resume -p \"implement the login feature\"");
        _console.WriteLine(string.Empty);
        _console.WriteLine("    # Continue previous session");
        _console.WriteLine("    claude-auto-resume -c -p \"continue where we left off\"");
        _console.WriteLine(string.Empty);
        _console.WriteLine("    # Headless mode");
        _console.WriteLine("    claude-auto-resume --headless --dangerous -p \"implement feature\"");
        _console.WriteLine(string.Empty);
        _console.WriteLine("    # Pass additional args to claude");
        _console.WriteLine("    claude-auto-resume -- --model claude-3-opus");
        _console.WriteLine(string.Empty);
    }

    private void PrintHelpModes()
    {
        _console.WriteLine("MODES:");
        _console.WriteLine("    Interactive (default):");
        _console.WriteLine("        - Full PTY pass-through with colors");
        _console.WriteLine("        - You type, Claude responds");
        _console.WriteLine("        - Auto-waits and continues on rate limit");
        _console.WriteLine(string.Empty);
        _console.WriteLine("    Headless (--headless --dangerous):");
        _console.WriteLine("        - No user input required");
        _console.WriteLine("        - Auto-responds 'y' to permission prompts");
        _console.WriteLine("        - Detects when Claude hangs waiting for input");
        _console.WriteLine(string.Empty);
    }

    private void PrintHelpWarning()
    {
        _console.WriteLine("WARNING: --dangerously-skip-permissions allows Claude to execute");
        _console.WriteLine("    commands without confirmation. Use only in trusted environments.");
    }

    private void PrintDiagnostics()
    {
        var platform = PlatformInfo.Current;

        _console.WriteLine(Strings.DiagnoseHeader);
        _console.WriteLine(new string('=', 40));
        _console.WriteLine(string.Empty);

        _console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            _diagnoseRuntimeInfoFormat,
            platform.DotNetVersion,
            platform.RuntimeIdentifier));

        _console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            _diagnoseOsInfoFormat,
            platform.OsDescription,
            platform.ProcessArchitecture));

        _console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"App Version: {platform.AppVersion}"));
        _console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"Container: {platform.IsContainer}"));
        _console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"CI: {platform.IsCI}"));
        _console.WriteLine(string.Empty);
        _console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            _errorLogLocationFormat,
            LoggingConfiguration.GetLogFilePath()));
    }
}
