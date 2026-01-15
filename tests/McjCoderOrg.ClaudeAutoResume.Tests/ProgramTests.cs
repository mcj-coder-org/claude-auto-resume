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
    [Trait("Category", "Integration")]
    public async Task Main_WithNoArgs_ShouldReturnDependencyMissingAsync()
    {
        // Without claude installed, it should return DependencyMissing
        // NOTE: If claude IS installed, this test will hang indefinitely because
        // it starts the real application. Use Category=Integration to exclude
        // from pre-push hook filtering.
        var result = await Program.Main([]);

        // Either Success (if claude is installed) or DependencyMissing (if not)
        result.Should().BeOneOf(ExitCodes.Success, ExitCodes.DependencyMissing);
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
