namespace McjCoderOrg.ClaudeAutoResume;

public sealed class LoggingConfigurationTests
{
    [Fact]
    public void GetLogDirectory_OnWindows_ShouldUseLocalAppData()
    {
        if (!OperatingSystem.IsWindows())
        {
            return; // Skip on non-Windows
        }

        var path = LoggingConfiguration.GetLogDirectory();

        path.Should().Contain("claude-auto-resume");
        path.Should().Contain("logs");
    }

    [Fact]
    public void GetLogDirectory_ShouldReturnAbsolutePath()
    {
        var path = LoggingConfiguration.GetLogDirectory();

        Path.IsPathRooted(path).Should().BeTrue();
    }

    [Fact]
    public void GetLogFilePath_ShouldIncludeDate()
    {
        var path = LoggingConfiguration.GetLogFilePath();

        path.Should().Contain(DateTime.UtcNow.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void GetLogFilePath_ShouldHaveLogExtension()
    {
        var path = LoggingConfiguration.GetLogFilePath();

        path.Should().EndWith(".log");
    }
}
