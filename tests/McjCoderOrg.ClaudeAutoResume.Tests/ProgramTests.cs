namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ProgramTests
{
    [Fact]
    public void Main_WithVersionFlag_ShouldReturnSuccess()
    {
        var result = Program.Main(["--version"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public void Main_WithHelpFlag_ShouldReturnSuccess()
    {
        var result = Program.Main(["--help"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public void Main_WithDiagnoseFlag_ShouldReturnSuccess()
    {
        var result = Program.Main(["--diagnose"]);

        result.Should().Be(ExitCodes.Success);
    }

    [Fact]
    public void Main_WithNoArgs_ShouldReturnSuccess()
    {
        var result = Program.Main([]);

        result.Should().Be(ExitCodes.Success);
    }
}
