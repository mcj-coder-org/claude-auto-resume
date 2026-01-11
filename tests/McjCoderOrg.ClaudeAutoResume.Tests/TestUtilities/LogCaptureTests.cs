using Serilog;

namespace McjCoderOrg.ClaudeAutoResume.TestUtilities;

public sealed class LogCaptureTests : IDisposable
{
    private LogCapture? _logCapture;

    public void Dispose()
    {
        _logCapture?.Dispose();
    }

    [Fact]
    public void Messages_WhenLogWritten_ShouldContainMessage()
    {
        _logCapture = new LogCapture();

        Log.Information("Test message");

        _logCapture.Messages.Should().Contain(m => m.Contains("Test message"));
    }

    [Fact]
    public void Messages_WithStructuredData_ShouldContainRenderedValue()
    {
        _logCapture = new LogCapture();

        Log.Information("Value is {Value}", 42);

        _logCapture.Messages.Should().Contain(m => m.Contains("42"));
    }

    [Fact]
    public void Clear_ShouldRemoveAllMessages()
    {
        _logCapture = new LogCapture();
        Log.Information("Message to clear");

        _logCapture.Clear();

        _logCapture.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_ShouldRestorePreviousLogger()
    {
        var originalLogger = Log.Logger;
        _logCapture = new LogCapture();

        _logCapture.Dispose();
        _logCapture = null;

        Log.Logger.Should().NotBe(originalLogger); // Logger was replaced, now silent
    }
}
