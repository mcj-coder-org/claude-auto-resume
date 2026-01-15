namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ProgramTests
{
    [Fact]
    public async Task Main_WithVersionFlag_ShouldReturnSuccessAsync()
    {
        var result = await Program.Main(["--version"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithHelpFlag_ShouldReturnSuccessAsync()
    {
        var result = await Program.Main(["--help"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithDiagnoseFlag_ShouldReturnSuccessAsync()
    {
        var result = await Program.Main(["--diagnose"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithHeadlessWithoutDangerous_ShouldReturnInvalidArgumentsAsync()
    {
        var result = await Program.Main(["--headless"]);

        result.Should().Be(ExitCodes.InvalidArguments);
    }

    [Fact]
    public async Task Main_WithPromptWithoutValue_ShouldReturnInvalidArgumentsAsync()
    {
        var result = await Program.Main(["--prompt"]);

        result.Should().Be(ExitCodes.InvalidArguments);
    }

    [Fact]
    public async Task Main_WithWaitWithoutValue_ShouldReturnInvalidArgumentsAsync()
    {
        var result = await Program.Main(["--wait"]);

        result.Should().Be(ExitCodes.InvalidArguments);
    }
}
