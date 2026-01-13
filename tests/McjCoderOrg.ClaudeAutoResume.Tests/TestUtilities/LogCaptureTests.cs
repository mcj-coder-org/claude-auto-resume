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

        _logCapture.Logger.Information("Test message");

        _logCapture.Messages.Should().Contain(m => m.Contains("Test message"));
    }

    [Fact]
    public void Messages_WithStructuredData_ShouldContainRenderedValue()
    {
        _logCapture = new LogCapture();

        _logCapture.Logger.Information("Value is {Value}", 42);

        _logCapture.Messages.Should().Contain(m => m.Contains("42"));
    }

    [Fact]
    public void Clear_ShouldRemoveLoggedMessages()
    {
        _logCapture = new LogCapture();
        const string testMessage = "Unique test message for clear verification";
        _logCapture.Logger.Information(testMessage);

        // Verify message was captured
        _logCapture.Messages.Should().Contain(m => m.Contains(testMessage));

        _logCapture.Clear();

        // Verify the specific message we logged is no longer present
        _logCapture.Messages.Should().NotContain(m => m.Contains(testMessage));
    }

    [Fact]
    public void Logger_ShouldBeIsolatedFromGlobalLogger()
    {
        _logCapture = new LogCapture();

        // This should only capture messages logged through the instance logger
        _logCapture.Logger.Information("Instance message");

        _logCapture.Messages.Should().HaveCount(1);
        _logCapture.Messages.Should().Contain(m => m.Contains("Instance message"));
    }
}
