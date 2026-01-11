using System.Globalization;
using System.Reflection;
using System.Text;

using McjCoderOrg.ClaudeAutoResume.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    public static int Main(string[] args)
    {
        // Bootstrap logger for startup errors
        ConfigureBootstrapLogger();

        try
        {
            return Run(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            Console.Error.WriteLine(Strings.ErrorUnhandledException);
            Console.Error.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                ErrorLogLocationFormat,
                LoggingConfiguration.GetLogFilePath()));
            return ExitCodes.GeneralError;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
#pragma warning restore CA1031

    private static int Run(string[] args)
    {
        // Handle --version
        if (args.Length > 0 && IsFlag(args[0], "--version", "-v"))
        {
            PrintVersion();
            return ExitCodes.Success;
        }

        // Handle --help
        if (args.Length > 0 && IsFlag(args[0], "--help", "-h"))
        {
            PrintHelp();
            return ExitCodes.Success;
        }

        // Handle --diagnose
        if (args.Length > 0 && IsFlag(args[0], "--diagnose"))
        {
            PrintDiagnostics();
            return ExitCodes.Success;
        }

        // Check for --verbose flag
        var verbose = args.Any(a => IsFlag(a, "--verbose", "-V"));
        if (verbose)
        {
            ConfigureVerboseLogging();
        }

        // Log startup
        var platform = PlatformInfo.Current;
        Log.Information(Strings.StartingApp, platform.AppVersion);

        // Build and run host
        var builder = Host.CreateApplicationBuilder(args);
        ConfigureServices(builder.Services);

        using var host = builder.Build();

        // For now, just return success (actual functionality in later phases)
        return ExitCodes.Success;
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

    private static void ConfigureServices(IServiceCollection services)
    {
        // Services will be added in later phases
        _ = services;
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
        Console.WriteLine(Strings.AppDescription);
        Console.WriteLine();
        Console.WriteLine("Usage: claude-auto-resume [options] [-- <claude-args>...]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -c, --config <path>       Path to configuration file");
        Console.WriteLine("  -V, --verbose             Enable verbose logging to file");
        Console.WriteLine("  --diagnose                Run environment diagnostics");
        Console.WriteLine("  --version                 Show version information");
        Console.WriteLine("  -h, --help                Show help");
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
