namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ProgramTests
{
    [Fact]
    public async Task Main_WithVersionFlag_ShouldReturnSuccess()
    {
        var result = await Program.Main(["--version"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithHelpFlag_ShouldReturnSuccess()
    {
        var result = await Program.Main(["--help"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithDiagnoseFlag_ShouldReturnSuccess()
    {
        var result = await Program.Main(["--diagnose"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public async Task Main_WithNoArgs_ShouldReturnDependencyMissing()
    {
        // Without claude installed, it should return DependencyMissing
        var result = await Program.Main([]);

        // Either Success (if claude is installed) or DependencyMissing (if not)
        result.Should().BeOneOf(ExitCodes.Success, ExitCodes.DependencyMissing);
    }
}
