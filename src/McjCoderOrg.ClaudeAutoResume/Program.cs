using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using McjCoderOrg.ClaudeAutoResume.Resources;

using Serilog;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Application entry point with CLI framework.
/// </summary>
/// <remarks>
/// See ADR-0017 (Observability) and ADR-0018 (CLI Design) for design decisions.
/// </remarks>
internal static class Program
{
    private static readonly CompositeFormat ErrorLogLocationFormat = CompositeFormat.Parse(Strings.ErrorLogLocation);
    private static readonly CompositeFormat DiagnoseRuntimeInfoFormat = CompositeFormat.Parse(Strings.DiagnoseRuntimeInfo);
    private static readonly CompositeFormat DiagnoseOsInfoFormat = CompositeFormat.Parse(Strings.DiagnoseOsInfo);

    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
#pragma warning disable CA1031 // Intentional: Top-level exception handler must catch all exceptions
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        ConfigureBootstrapLogger();

        try
        {
            return await RunAsync(args).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            WriteStartupError();
            return ExitCodes.GeneralError;
        }
        finally
        {
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }
#pragma warning restore CA1031

    // Console output in startup error handler is intentionally synchronous
#pragma warning disable CA1849, S6966
    private static void WriteStartupError()
    {
        Console.Error.WriteLine(Strings.ErrorUnhandledException);
        Console.Error.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            ErrorLogLocationFormat,
            LoggingConfiguration.GetLogFilePath()));
    }
#pragma warning restore CA1849, S6966

    private static async Task<int> RunAsync(string[] args)
    {
        var parseResult = ParseArguments(args);

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

        if (parseResult.Verbose)
        {
            ConfigureVerboseLogging();
        }

        LogStartup();
        PrintStartupInfo(config, parseResult.Headless, parseResult.Dangerous);

        using var monitor = new ClaudeMonitor(config);
        await monitor.RunAsync(parseResult.ClaudeArgs).ConfigureAwait(false);

        return ExitCodes.Success;
    }

    private sealed record ParseResult
    {
        public bool ShowHelp { get; init; }
        public bool ShowVersion { get; init; }
        public bool ShowDiagnose { get; init; }
        public bool Verbose { get; init; }
        public bool Headless { get; init; }
        public bool Dangerous { get; init; }
        public bool ContinueConversation { get; init; }
        public string? InitialPrompt { get; init; }
        public int? WaitMinutes { get; init; }
        public List<string> ClaudeArgs { get; init; } = [];
        public string? ErrorMessage { get; init; }
    }

    private static ParseResult ParseArguments(string[] args)
    {
        var builder = new ParseResultBuilder();
        var i = 0;

        while (i < args.Length)
        {
            var earlyExit = TryParseInfoFlag(args[i]);
            if (earlyExit is not null)
            {
                return earlyExit;
            }

            var result = ParseSingleArgument(args, ref i, builder);
            if (result is not null)
            {
                return result;
            }
        }

        builder.WaitMinutes ??= GetEnvironmentWaitMinutes();
        return builder.Build();
    }

    private static ParseResult? TryParseInfoFlag(string arg)
    {
        if (IsFlag(arg, "--help", "-h"))
        {
            return new ParseResult { ShowHelp = true };
        }

        if (IsFlag(arg, "--version", "-v"))
        {
            return new ParseResult { ShowVersion = true };
        }

        if (IsFlag(arg, "--diagnose"))
        {
            return new ParseResult { ShowDiagnose = true };
        }

        return null;
    }

    private static ParseResult? ParseSingleArgument(string[] args, ref int i, ParseResultBuilder builder)
    {
        var arg = args[i];

        if (TryParseBooleanFlag(arg, ref i, builder))
        {
            return null;
        }

        var promptResult = TryParseStringArg(args, ref i, "--prompt", "-p");
        if (promptResult.Matched)
        {
            if (promptResult.Value is null)
            {
                return new ParseResult { ErrorMessage = "Error: --prompt requires an argument" };
            }

            builder.InitialPrompt = promptResult.Value;
            return null;
        }

        var waitResult = TryParseIntArg(args, ref i, "--wait", "-w");
        if (waitResult.Matched)
        {
            if (waitResult.Value is null)
            {
                return new ParseResult { ErrorMessage = "Error: --wait requires a number of minutes" };
            }

            builder.WaitMinutes = waitResult.Value;
            return null;
        }

        builder.ClaudeArgs.Add(arg);
        i++;
        return null;
    }

    private static bool TryParseBooleanFlag(string arg, ref int i, ParseResultBuilder builder)
    {
        if (IsFlag(arg, "--verbose", "-V"))
        {
            builder.Verbose = true;
            i++;
            return true;
        }

        if (IsFlag(arg, "--headless"))
        {
            builder.Headless = true;
            i++;
            return true;
        }

        if (IsFlag(arg, "--dangerously-skip-permissions") || IsFlag(arg, "--dangerous"))
        {
            builder.Dangerous = true;
            i++;
            return true;
        }

        if (IsFlag(arg, "--continue", "-c"))
        {
            builder.ContinueConversation = true;
            i++;
            return true;
        }

        return false;
    }

    private sealed class ParseResultBuilder
    {
        public bool Verbose { get; set; }
        public bool Headless { get; set; }
        public bool Dangerous { get; set; }
        public bool ContinueConversation { get; set; }
        public string? InitialPrompt { get; set; }
        public int? WaitMinutes { get; set; }
        public List<string> ClaudeArgs { get; } = [];

        public ParseResult Build() => new()
        {
            Verbose = Verbose,
            Headless = Headless,
            Dangerous = Dangerous,
            ContinueConversation = ContinueConversation,
            InitialPrompt = InitialPrompt,
            WaitMinutes = WaitMinutes,
            ClaudeArgs = ClaudeArgs,
        };
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly record struct StringArgResult(bool Matched, string? Value);

    private static StringArgResult TryParseStringArg(string[] args, ref int i, string longForm, string? shortForm)
    {
        if (!IsFlag(args[i], longForm, shortForm))
        {
            return new StringArgResult(Matched: false, Value: null);
        }

        if (i + 1 < args.Length)
        {
            i += 2;
            return new StringArgResult(Matched: true, Value: args[i - 1]);
        }

        i++;
        return new StringArgResult(Matched: true, Value: null);
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly record struct IntArgResult(bool Matched, int? Value);

    private static IntArgResult TryParseIntArg(string[] args, ref int i, string longForm, string? shortForm)
    {
        if (!IsFlag(args[i], longForm, shortForm))
        {
            return new IntArgResult(Matched: false, Value: null);
        }

        if (i + 1 < args.Length && int.TryParse(args[i + 1], CultureInfo.InvariantCulture, out var value))
        {
            i += 2;
            return new IntArgResult(Matched: true, Value: value);
        }

        i++;
        return new IntArgResult(Matched: true, Value: null);
    }

    private static int? GetEnvironmentWaitMinutes()
    {
        var envValue = Environment.GetEnvironmentVariable("CLAUDE_WAIT_MINUTES");
        if (int.TryParse(envValue, CultureInfo.InvariantCulture, out var mins))
        {
            return mins;
        }

        return null;
    }

    private static string? ValidateArguments(ParseResult result)
    {
        if (result.Headless && !result.Dangerous)
        {
            return "headless-requires-dangerous";
        }

        return null;
    }

#pragma warning disable CA1849, S6966 // Intentional: Sync console output for CLI argument errors
    private static void WriteArgumentError(string message)
    {
        Console.Error.WriteLine(message);
    }
#pragma warning restore CA1849, S6966

    private static void WriteValidationError(string errorCode)
    {
        if (string.Equals(errorCode, "headless-requires-dangerous", StringComparison.Ordinal))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: --headless mode requires --dangerously-skip-permissions");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Headless mode auto-responds to prompts without user confirmation.");
            Console.WriteLine("This is equivalent to claude-auto-resume's behavior.");
            Console.WriteLine();
            Console.WriteLine("To enable, run:");
            Console.WriteLine("  claude-auto-resume --headless --dangerously-skip-permissions");
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

    private static void LogStartup()
    {
        var platform = PlatformInfo.Current;
        Log.Information(Strings.StartingApp, platform.AppVersion);
    }

    private static void PrintStartupInfo(WrapperConfig config, bool headless, bool dangerous)
    {
        Console.WriteLine("[claude-auto-resume] Starting Claude Code...");
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"[claude-auto-resume] Auto-continue on rate limit: enabled (wait {config.WaitMinutes} min)"));

        if (headless)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[claude-auto-resume] HEADLESS MODE - auto-responding to prompts");
            Console.ResetColor();
        }

        if (dangerous)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[claude-auto-resume] --dangerously-skip-permissions enabled");
            Console.ResetColor();
        }

        Console.WriteLine("[claude-auto-resume] Press Ctrl+C to exit");
        Console.WriteLine();
    }

    private static void ConfigureBootstrapLogger()
    {
        var logPath = LoggingConfiguration.GetLogFilePath();
        var logDir = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrEmpty(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
    }

    private static void ConfigureVerboseLogging()
    {
        var logPath = LoggingConfiguration.GetLogFilePath();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
    }

    private static void PrintVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"claude-auto-resume {version.Major}.{version.Minor}.{version.Build}"));
    }

    private static void PrintHelp()
    {
        PrintHelpHeader();
        PrintHelpOptions();
        PrintHelpEnvironment();
        PrintHelpExamples();
        PrintHelpModes();
        PrintHelpWarning();
    }

    private static void PrintHelpHeader()
    {
        Console.WriteLine(Strings.AppDescription);
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("    claude-auto-resume [OPTIONS] [-- CLAUDE_ARGS...]");
        Console.WriteLine();
    }

    private static void PrintHelpOptions()
    {
        Console.WriteLine("OPTIONS:");
        Console.WriteLine("    -h, --help                      Show this help");
        Console.WriteLine("    -v, --version                   Show version information");
        Console.WriteLine("    -p, --prompt <PROMPT>           Initial prompt to send to Claude");
        Console.WriteLine("    -c, --continue                  Continue previous conversation");
        Console.WriteLine("    -w, --wait <MINUTES>            Minutes to wait on rate limit (default: 15)");
        Console.WriteLine("    -V, --verbose                   Enable verbose logging to file");
        Console.WriteLine("    --headless                      Run without user input (auto-respond to prompts)");
        Console.WriteLine("    --dangerously-skip-permissions  Pass dangerous flag to Claude (required for headless)");
        Console.WriteLine("    --dangerous                     Alias for --dangerously-skip-permissions");
        Console.WriteLine("    --diagnose                      Run environment diagnostics");
        Console.WriteLine();
    }

    private static void PrintHelpEnvironment()
    {
        Console.WriteLine("ENVIRONMENT:");
        Console.WriteLine("    CLAUDE_WAIT_MINUTES             Override default wait time");
        Console.WriteLine();
    }

    private static void PrintHelpExamples()
    {
        Console.WriteLine("EXAMPLES:");
        Console.WriteLine("    # Interactive mode (default)");
        Console.WriteLine("    claude-auto-resume");
        Console.WriteLine();
        Console.WriteLine("    # With initial prompt");
        Console.WriteLine("    claude-auto-resume -p \"implement the login feature\"");
        Console.WriteLine();
        Console.WriteLine("    # Continue previous session");
        Console.WriteLine("    claude-auto-resume -c -p \"continue where we left off\"");
        Console.WriteLine();
        Console.WriteLine("    # Headless mode");
        Console.WriteLine("    claude-auto-resume --headless --dangerous -p \"implement feature\"");
        Console.WriteLine();
        Console.WriteLine("    # Pass additional args to claude");
        Console.WriteLine("    claude-auto-resume -- --model claude-3-opus");
        Console.WriteLine();
    }

    private static void PrintHelpModes()
    {
        Console.WriteLine("MODES:");
        Console.WriteLine("    Interactive (default):");
        Console.WriteLine("        - Full PTY pass-through with colors");
        Console.WriteLine("        - You type, Claude responds");
        Console.WriteLine("        - Auto-waits and continues on rate limit");
        Console.WriteLine();
        Console.WriteLine("    Headless (--headless --dangerous):");
        Console.WriteLine("        - No user input required");
        Console.WriteLine("        - Auto-responds 'y' to permission prompts");
        Console.WriteLine("        - Detects when Claude hangs waiting for input");
        Console.WriteLine();
    }

    private static void PrintHelpWarning()
    {
        Console.WriteLine("WARNING: --dangerously-skip-permissions allows Claude to execute");
        Console.WriteLine("    commands without confirmation. Use only in trusted environments.");
    }

    private static void PrintDiagnostics()
    {
        var platform = PlatformInfo.Current;

        Console.WriteLine(Strings.DiagnoseHeader);
        Console.WriteLine(new string('=', 40));
        Console.WriteLine();

        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            DiagnoseRuntimeInfoFormat,
            platform.DotNetVersion,
            platform.RuntimeIdentifier));

        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            DiagnoseOsInfoFormat,
            platform.OsDescription,
            platform.ProcessArchitecture));

        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"App Version: {platform.AppVersion}"));
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"Container: {platform.IsContainer}"));
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"CI: {platform.IsCI}"));
        Console.WriteLine();
        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            ErrorLogLocationFormat,
            LoggingConfiguration.GetLogFilePath()));
    }

    private static bool IsFlag(string arg, string longForm, string? shortForm = null)
    {
        return string.Equals(arg, longForm, StringComparison.OrdinalIgnoreCase)
            || (shortForm is not null && string.Equals(arg, shortForm, StringComparison.OrdinalIgnoreCase));
    }
}
