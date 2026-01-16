using System.Globalization;
using System.Text;

using McjCoderOrg.ClaudeAutoResume;
using McjCoderOrg.ClaudeAutoResume.Resources;
using McjCoderOrg.ClaudeAutoResume.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

// Top-level exception handler must catch all exceptions
#pragma warning disable CA1031

var logPath = LoggingConfiguration.GetLogFilePath();
var logDir = Path.GetDirectoryName(logPath);
if (!string.IsNullOrEmpty(logDir))
{
    Directory.CreateDirectory(logDir);
}

// Check for verbose flag early to configure logging before DI setup
var isVerbose = args.Contains("-V") || args.Contains("--verbose");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(isVerbose ? Serilog.Events.LogEventLevel.Debug : Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        formatProvider: CultureInfo.InvariantCulture)
    .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
    .CreateLogger();

try
{
    Console.OutputEncoding = Encoding.UTF8;
    Console.InputEncoding = Encoding.UTF8;

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddSingleton<IConsoleService, ConsoleService>();
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            services.AddSingleton<IArgumentParser, ArgumentParser>();
            services.AddSingleton<IApplication, Application>();
            services.AddSingleton<ILogger>(_ => Log.Logger);
        })
        .UseSerilog()
        .Build();

    var app = host.Services.GetRequiredService<IApplication>();
    return await app.RunAsync(args).ConfigureAwait(false);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");

    // Intentional: Sync console output in exception handler for reliability
#pragma warning disable CA1849, S6966
    var errorLogLocationFormat = CompositeFormat.Parse(Strings.ErrorLogLocation);
    Console.Error.WriteLine(Strings.ErrorUnhandledException);
    Console.Error.WriteLine(string.Format(
        CultureInfo.InvariantCulture,
        errorLogLocationFormat,
        LoggingConfiguration.GetLogFilePath()));
#pragma warning restore CA1849, S6966

    return ExitCodes.GeneralError;
}
finally
{
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}

#pragma warning restore CA1031
