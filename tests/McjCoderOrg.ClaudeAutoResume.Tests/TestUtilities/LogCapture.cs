using System.Globalization;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.InMemory;

namespace McjCoderOrg.ClaudeAutoResume.TestUtilities;

/// <summary>
/// Captures Serilog messages for test assertions.
/// </summary>
/// <remarks>
/// <para>
/// This class provides an isolated logger instance for test assertions.
/// It does NOT modify the global Log.Logger, ensuring test isolation
/// when tests run in parallel.
/// </para>
/// <para>
/// See ADR-0017 for test capture design.
/// </para>
/// </remarks>
internal sealed class LogCapture : IDisposable
{
    private readonly InMemorySink _sink;
    private readonly Logger _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogCapture"/> class.
    /// </summary>
    public LogCapture()
    {
        _sink = new InMemorySink();

        _logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_sink)
            .CreateLogger();
    }

    /// <summary>
    /// Gets the isolated logger instance for this capture.
    /// Use this logger in tests instead of the global Log class.
    /// </summary>
    public ILogger Logger => _logger;

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
    /// Disposes the log capture.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _logger.Dispose();
        _sink.Dispose();
    }
}
