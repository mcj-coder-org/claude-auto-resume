namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ProgramTests
{
    private readonly ITestOutputHelper _output;

    public ProgramTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Main_WithVersionFlag_ShouldReturnSuccessAsync()
    {
        _output.WriteLine("Testing --version flag");
        var result = await Program.Main(["--version"]);

        _output.WriteLine("Exit code: {0}", result);
        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithHelpFlag_ShouldReturnSuccessAsync()
    {
        _output.WriteLine("Testing --help flag");
        var result = await Program.Main(["--help"]);

        _output.WriteLine("Exit code: {0}", result);
        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithDiagnoseFlag_ShouldReturnSuccessAsync()
    {
        _output.WriteLine("Testing --diagnose flag");
        var result = await Program.Main(["--diagnose"]);

        _output.WriteLine("Exit code: {0}", result);
        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithHeadlessWithoutDangerous_ShouldReturnInvalidArgumentsAsync()
    {
        _output.WriteLine("Testing --headless without --dangerously-skip-permissions");
        var result = await Program.Main(["--headless"]);

        _output.WriteLine("Exit code: {0} (expected: {1})", result, ExitCodes.InvalidArguments);
        result.Should().Be(ExitCodes.InvalidArguments);
    }

    [Fact]
    public async Task Main_WithPromptWithoutValue_ShouldReturnInvalidArgumentsAsync()
    {
        _output.WriteLine("Testing --prompt without value");
        var result = await Program.Main(["--prompt"]);

        _output.WriteLine("Exit code: {0} (expected: {1})", result, ExitCodes.InvalidArguments);
        result.Should().Be(ExitCodes.InvalidArguments);
    }

    [Fact]
    public async Task Main_WithWaitWithoutValue_ShouldReturnInvalidArgumentsAsync()
    {
        _output.WriteLine("Testing --wait without value");
        var result = await Program.Main(["--wait"]);

        _output.WriteLine("Exit code: {0} (expected: {1})", result, ExitCodes.InvalidArguments);
        result.Should().Be(ExitCodes.InvalidArguments);
    }
}
