using McjCoderOrg.ClaudeAutoResume.Services;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Tests for the Application class that handles CLI entry point logic.
/// These tests verify argument parsing and exit codes through the Application interface.
/// </summary>
public sealed class ProgramTests
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<IConsoleService> _mockConsole;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private Application _app = null!;

    public ProgramTests(ITestOutputHelper output)
    {
        _output = output;
        _mockConsole = new Mock<IConsoleService>(MockBehavior.Loose);
        _mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Loose);

        // Setup console mock
        _mockConsole.Setup(c => c.WindowWidth).Returns(120);
        _mockConsole.Setup(c => c.WindowHeight).Returns(30);
    }

    private Application CreateApplication(Mock<IArgumentParser>? parserMock = null)
    {
        var mockEnvironment = new Mock<IEnvironmentService>(MockBehavior.Loose);
        mockEnvironment.Setup(e => e.CurrentDirectory).Returns(Environment.CurrentDirectory);
        mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(new Dictionary<string, string>(StringComparer.Ordinal));

        var parser = parserMock?.Object ?? new ArgumentParser(mockEnvironment.Object);

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IConsoleService)))
            .Returns(_mockConsole.Object);
        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IEnvironmentService)))
            .Returns(mockEnvironment.Object);

        return new Application(parser, _mockConsole.Object, _mockServiceProvider.Object);
    }

    [Fact]
    public async Task RunAsync_WithVersionFlag_ShouldReturnSuccessAsync()
    {
        _output.WriteLine("Testing --version flag");
        _app = CreateApplication();

        var result = await _app.RunAsync(["--version"]);

        _output.WriteLine("Exit code: {0}", result);
        result.Should().Be(ExitCodes.Success);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("claude-auto-resume"))), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_WithHelpFlag_ShouldReturnSuccessAsync()
    {
        _output.WriteLine("Testing --help flag");
        _app = CreateApplication();

        var result = await _app.RunAsync(["--help"]);

        _output.WriteLine("Exit code: {0}", result);
        result.Should().Be(ExitCodes.Success);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("USAGE:"))), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_WithDiagnoseFlag_ShouldReturnSuccessAsync()
    {
        _output.WriteLine("Testing --diagnose flag");
        _app = CreateApplication();

        var result = await _app.RunAsync(["--diagnose"]);

        _output.WriteLine("Exit code: {0}", result);
        result.Should().Be(ExitCodes.Success);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("Diagnostics"))), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_WithHeadlessWithoutDangerous_ShouldReturnInvalidArgumentsAsync()
    {
        _output.WriteLine("Testing --headless without --dangerously-skip-permissions");
        _app = CreateApplication();

        var result = await _app.RunAsync(["--headless"]);

        _output.WriteLine("Exit code: {0} (expected: {1})", result, ExitCodes.InvalidArguments);
        result.Should().Be(ExitCodes.InvalidArguments);
        _mockConsole.VerifySet(c => c.ForegroundColor = ConsoleColor.Red, Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_WithPromptWithoutValue_ShouldReturnInvalidArgumentsAsync()
    {
        _output.WriteLine("Testing --prompt without value");
        _app = CreateApplication();

        var result = await _app.RunAsync(["--prompt"]);

        _output.WriteLine("Exit code: {0} (expected: {1})", result, ExitCodes.InvalidArguments);
        result.Should().Be(ExitCodes.InvalidArguments);
        _mockConsole.Verify(c => c.WriteErrorLine(It.Is<string>(s => s.Contains("--prompt requires an argument"))), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WithWaitWithoutValue_ShouldReturnInvalidArgumentsAsync()
    {
        _output.WriteLine("Testing --wait without value");
        _app = CreateApplication();

        var result = await _app.RunAsync(["--wait"]);

        _output.WriteLine("Exit code: {0} (expected: {1})", result, ExitCodes.InvalidArguments);
        result.Should().Be(ExitCodes.InvalidArguments);
        _mockConsole.Verify(c => c.WriteErrorLine(It.Is<string>(s => s.Contains("--wait requires"))), Times.Once);
    }
}
