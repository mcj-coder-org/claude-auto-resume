using System.Globalization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;

namespace McjCoderOrg.ClaudeAutoResume.TestUtilities;

/// <summary>
/// Captures Serilog messages for test assertions.
/// </summary>
/// <remarks>
/// See ADR-0017 for test capture design.
/// </remarks>
internal sealed class LogCapture : IDisposable
{
    private readonly InMemorySink _sink;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogCapture"/> class.
    /// </summary>
    public LogCapture()
    {
        _sink = new InMemorySink();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_sink)
            .CreateLogger();
    }

    /// <summary>
    /// Gets the captured log messages.
    /// </summary>
    public IReadOnlyList<string> Messages =>
        _sink.LogEvents
            .Select(e => e.RenderMessage(CultureInfo.InvariantCulture))
            .ToList();

    /// <summary>
    /// Gets the captured log events.
    /// </summary>
    public IReadOnlyList<LogEvent> Events =>
        _sink.LogEvents.ToList();

    /// <summary>
    /// Clears all captured messages.
    /// </summary>
    public void Clear()
    {
        _sink.Dispose();
    }

    /// <summary>
    /// Disposes the log capture and restores the previous logger.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Log.CloseAndFlush();
        _sink.Dispose();
        Log.Logger = new LoggerConfiguration().CreateLogger(); // Silent logger
    }
}
