using McjCoderOrg.ClaudeAutoResume.Services;
using McjCoderOrg.ClaudeAutoResume.TestUtilities;

using Serilog;

namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ClaudeMonitorTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<IConsoleService> _mockConsole;
    private readonly Mock<IEnvironmentService> _mockEnvironment;
    private readonly Mock<ILogger> _mockLogger;
    private readonly ClaudeMonitor _monitor;

    public ClaudeMonitorTests(ITestOutputHelper output)
    {
        _output = output;
        _mockConsole = new Mock<IConsoleService>(MockBehavior.Loose);
        _mockEnvironment = new Mock<IEnvironmentService>(MockBehavior.Loose);
        _mockLogger = new Mock<ILogger>(MockBehavior.Loose);

        // Setup default mock behavior
        _mockConsole.Setup(c => c.WindowWidth).Returns(120);
        _mockConsole.Setup(c => c.WindowHeight).Returns(30);
        _mockEnvironment.Setup(e => e.CurrentDirectory).Returns(Environment.CurrentDirectory);
        _mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(new Dictionary<string, string>(StringComparer.Ordinal));

        _monitor = new ClaudeMonitor(WrapperConfig.Default, _mockConsole.Object, _mockEnvironment.Object, _mockLogger.Object);
        _output.WriteLine("ClaudeMonitorTests initialized with default config and mock services");
    }

    public void Dispose()
    {
        _monitor.Dispose();
    }

    private ClaudeMonitor CreateMonitor(WrapperConfig config)
    {
        return new ClaudeMonitor(config, _mockConsole.Object, _mockEnvironment.Object, _mockLogger.Object);
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
        using var monitor = CreateMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().Contain("--dangerously-skip-permissions");
    }

    [Fact]
    public void BuildCommandLine_WithContinueConversation_IncludesFlag()
    {
        var config = WrapperConfig.Default with { ContinueConversation = true };
        using var monitor = CreateMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().Contain("-c");
    }

    [Fact]
    public void BuildCommandLine_WithInitialPrompt_IncludesPromptAndValue()
    {
        var config = WrapperConfig.Default with { InitialPrompt = "test prompt" };
        using var monitor = CreateMonitor(config);

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
        using var monitor = CreateMonitor(config);

        var result = monitor.BuildCommandLine(["--extra"]);

        _output.WriteLine("Built command line: [{0}]", string.Join(", ", result));

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
        using var monitor = CreateMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().NotContain("-p");
    }

    [Fact]
    public void BuildCommandLine_WithNullInitialPrompt_DoesNotIncludePrompt()
    {
        var config = WrapperConfig.Default with { InitialPrompt = null };
        using var monitor = CreateMonitor(config);

        var result = monitor.BuildCommandLine([]);

        result.Should().NotContain("-p");
    }

    [Fact]
    public void Constructor_WithConfig_DoesNotThrow()
    {
        var config = WrapperConfig.Default;

        var act = () => CreateMonitor(config);

        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var config = WrapperConfig.Default;
        using var monitor = CreateMonitor(config);

        // First explicit dispose, second via using - should not throw
        var act = () => monitor.Dispose();

        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithInjectedLogger_UsesProvidedLogger()
    {
        using var logCapture = new LogCapture();
        var config = WrapperConfig.Default;

        using var monitor = new ClaudeMonitor(config, _mockConsole.Object, _mockEnvironment.Object, logCapture.Logger);

        // Verify the monitor was created with the injected logger (no exceptions)
        monitor.Should().NotBeNull();
    }

    // NOTE: The "RunAsync returns false when claude not found" scenario cannot be
    // properly unit tested because FindClaudeInPath uses Environment.GetFolderPath
    // which is cached by .NET and cannot be mocked without refactoring.
    // This scenario is tested in E2E tests: "Report missing claude CLI" scenario.
}
