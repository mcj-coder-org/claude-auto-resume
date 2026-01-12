namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ClaudeMonitorTests : IDisposable
{
    private readonly ClaudeMonitor _monitor;

    public ClaudeMonitorTests()
    {
        _monitor = new ClaudeMonitor(WrapperConfig.Default);
    }

    public void Dispose()
    {
        _monitor.Dispose();
    }

    [Fact]
    public void BuildCommandLine_WithDefaultConfig_ReturnsEmptyList()
    {
        var result = _monitor.BuildCommandLine([]);

        result.Should().BeEmpty();
    }

    [Fact]
    public void BuildCommandLine_WithAdditionalArgs_IncludesThemAtEnd()
    {
        var result = _monitor.BuildCommandLine(["--model", "claude-3-opus"]);

        result.Should().Contain("--model");
        result.Should().Contain("claude-3-opus");
    }

    [Fact]
    public void BuildCommandLine_WithDangerouslySkipPermissions_IncludesFlag()
    {
        var config = WrapperConfig.Default with { DangerouslySkipPermissions = true };
        using var monitor = new ClaudeMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().Contain("--dangerously-skip-permissions");
    }

    [Fact]
    public void BuildCommandLine_WithContinueConversation_IncludesFlag()
    {
        var config = WrapperConfig.Default with { ContinueConversation = true };
        using var monitor = new ClaudeMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().Contain("-c");
    }

    [Fact]
    public void BuildCommandLine_WithInitialPrompt_IncludesPromptAndValue()
    {
        var config = WrapperConfig.Default with { InitialPrompt = "test prompt" };
        using var monitor = new ClaudeMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().ContainInOrder("-p", "test prompt");
    }

    [Fact]
    public void BuildCommandLine_WithAllOptions_IncludesAllInCorrectOrder()
    {
        var config = WrapperConfig.Default with
        {
            DangerouslySkipPermissions = true,
            ContinueConversation = true,
            InitialPrompt = "my prompt",
        };
        using var monitor = new ClaudeMonitor(config);

        var result = monitor.BuildCommandLine(["--extra"]);

        // Verify order: dangerous, continue, prompt, additional args
        result.Should().HaveCount(5);
        result[0].Should().Be("--dangerously-skip-permissions");
        result[1].Should().Be("-c");
        result[2].Should().Be("-p");
        result[3].Should().Be("my prompt");
        result[4].Should().Be("--extra");
    }

    [Fact]
    public void BuildCommandLine_WithEmptyInitialPrompt_DoesNotIncludePrompt()
    {
        var config = WrapperConfig.Default with { InitialPrompt = "" };
        using var monitor = new ClaudeMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().NotContain("-p");
    }

    [Fact]
    public void BuildCommandLine_WithNullInitialPrompt_DoesNotIncludePrompt()
    {
        var config = WrapperConfig.Default with { InitialPrompt = null };
        using var monitor = new ClaudeMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().NotContain("-p");
    }

    [Fact]
    public void Constructor_WithConfig_DoesNotThrow()
    {
        var config = WrapperConfig.Default;

        var act = () => new ClaudeMonitor(config);

        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var config = WrapperConfig.Default;
        using var monitor = new ClaudeMonitor(config);

        // First explicit dispose, second via using - should not throw
        var act = () => monitor.Dispose();

        act.Should().NotThrow();
    }
}
