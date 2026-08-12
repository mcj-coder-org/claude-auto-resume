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
    private static readonly CompositeFormat _startupAutoResumeFormat = CompositeFormat.Parse(Strings.StartupAutoResume);

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
            _console.WriteLine(Strings.ErrorHeadlessRequiresDangerous);
            _console.ResetColor();
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
        _console.WriteLine(Strings.StartupMessage);
        _console.WriteLine(string.Format(CultureInfo.InvariantCulture, _startupAutoResumeFormat, config.WaitMinutes));

        if (headless)
        {
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.WriteLine(Strings.StartupHeadlessMode);
            _console.ResetColor();
        }

        if (dangerous)
        {
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.WriteLine(Strings.StartupDangerousMode);
            _console.ResetColor();
        }

        _console.WriteLine(Strings.StartupExitHint);
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
        _console.WriteLine(Strings.AppDescription);
        _console.WriteLine(string.Empty);
        _console.WriteLine(Strings.HelpUsage);
        _console.WriteLine(string.Empty);
        _console.WriteLine(Strings.HelpOptions);
        _console.WriteLine(string.Empty);
        _console.WriteLine(Strings.HelpEnvironment);
        _console.WriteLine(string.Empty);
        _console.WriteLine(Strings.HelpExamples);
        _console.WriteLine(string.Empty);
        _console.WriteLine(Strings.HelpModes);
        _console.WriteLine(string.Empty);
        _console.WriteLine(Strings.HelpWarning);
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
