using McjCoderOrg.ClaudeAutoResume.Services;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Tests for the ArgumentParser class.
/// Verifies correct parsing of all CLI flags and arguments.
/// </summary>
public sealed class ArgumentParserTests
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<IEnvironmentService> _mockEnvironment;

    public ArgumentParserTests(ITestOutputHelper output)
    {
        _output = output;
        _mockEnvironment = new Mock<IEnvironmentService>(MockBehavior.Loose);

        // Default setup - no environment variables
        _mockEnvironment
            .Setup(e => e.GetEnvironmentVariable(It.IsAny<string>()))
            .Returns((string?)null);
    }

    private ArgumentParser CreateParser() => new(_mockEnvironment.Object);

    // Info flags (early exit)

    [Fact]
    public void Parse_HelpFlag_ReturnsShowHelp()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--help"]);

        _output.WriteLine("Result: ShowHelp={0}", result.ShowHelp);
        result.ShowHelp.Should().BeTrue();
    }

    [Fact]
    public void Parse_HelpShortFlag_ReturnsShowHelp()
    {
        var parser = CreateParser();

        var result = parser.Parse(["-h"]);

        _output.WriteLine("Result: ShowHelp={0}", result.ShowHelp);
        result.ShowHelp.Should().BeTrue();
    }

    [Fact]
    public void Parse_VersionFlag_ReturnsShowVersion()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--version"]);

        _output.WriteLine("Result: ShowVersion={0}", result.ShowVersion);
        result.ShowVersion.Should().BeTrue();
    }

    [Fact]
    public void Parse_VersionShortFlag_ReturnsShowVersion()
    {
        var parser = CreateParser();

        var result = parser.Parse(["-v"]);

        _output.WriteLine("Result: ShowVersion={0}", result.ShowVersion);
        result.ShowVersion.Should().BeTrue();
    }

    [Fact]
    public void Parse_DiagnoseFlag_ReturnsShowDiagnose()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--diagnose"]);

        _output.WriteLine("Result: ShowDiagnose={0}", result.ShowDiagnose);
        result.ShowDiagnose.Should().BeTrue();
    }

    // Boolean flags

    [Fact]
    public void Parse_VerboseFlag_SetsVerbose()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--verbose"]);

        _output.WriteLine("Result: Verbose={0}", result.Verbose);
        result.Verbose.Should().BeTrue();
    }

    [Fact]
    public void Parse_VerboseShortFlag_WithCaseInsensitivity_MatchesVersionFirst()
    {
        // Note: Parser is case-insensitive, so -V (uppercase) matches -v (version) first
        // because info flags are checked before boolean flags and cause early exit.
        // This test documents that behavior.
        var parser = CreateParser();

        var result = parser.Parse(["-V"]);

        _output.WriteLine("Result: ShowVersion={0}, Verbose={1}", result.ShowVersion, result.Verbose);
        // -V matches -v (version) due to case-insensitivity
        result.ShowVersion.Should().BeTrue();
        result.Verbose.Should().BeFalse();
    }

    [Fact]
    public void Parse_HeadlessFlag_SetsHeadless()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--headless"]);

        _output.WriteLine("Result: Headless={0}", result.Headless);
        result.Headless.Should().BeTrue();
    }

    [Fact]
    public void Parse_DangerousFlag_SetsDangerous()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--dangerous"]);

        _output.WriteLine("Result: Dangerous={0}", result.Dangerous);
        result.Dangerous.Should().BeTrue();
    }

    [Fact]
    public void Parse_DangerouslySkipPermissions_SetsDangerous()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--dangerously-skip-permissions"]);

        _output.WriteLine("Result: Dangerous={0}", result.Dangerous);
        result.Dangerous.Should().BeTrue();
    }

    [Fact]
    public void Parse_ContinueFlag_SetsContinueConversation()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--continue"]);

        _output.WriteLine("Result: ContinueConversation={0}", result.ContinueConversation);
        result.ContinueConversation.Should().BeTrue();
    }

    [Fact]
    public void Parse_ContinueShortFlag_SetsContinueConversation()
    {
        var parser = CreateParser();

        var result = parser.Parse(["-c"]);

        _output.WriteLine("Result: ContinueConversation={0}", result.ContinueConversation);
        result.ContinueConversation.Should().BeTrue();
    }

    // String arguments

    [Fact]
    public void Parse_PromptWithValue_SetsInitialPrompt()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--prompt", "test prompt"]);

        _output.WriteLine("Result: InitialPrompt={0}", result.InitialPrompt);
        result.InitialPrompt.Should().Be("test prompt");
    }

    [Fact]
    public void Parse_PromptShortFlag_SetsInitialPrompt()
    {
        var parser = CreateParser();

        var result = parser.Parse(["-p", "my prompt"]);

        _output.WriteLine("Result: InitialPrompt={0}", result.InitialPrompt);
        result.InitialPrompt.Should().Be("my prompt");
    }

    [Fact]
    public void Parse_PromptWithoutValue_ReturnsError()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--prompt"]);

        _output.WriteLine("Result: ErrorMessage={0}", result.ErrorMessage);
        result.ErrorMessage.Should().Contain("--prompt requires an argument");
    }

    // Integer arguments

    [Fact]
    public void Parse_WaitWithValue_SetsWaitMinutes()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--wait", "30"]);

        _output.WriteLine("Result: WaitMinutes={0}", result.WaitMinutes);
        result.WaitMinutes.Should().Be(30);
    }

    [Fact]
    public void Parse_WaitShortFlag_SetsWaitMinutes()
    {
        var parser = CreateParser();

        var result = parser.Parse(["-w", "15"]);

        _output.WriteLine("Result: WaitMinutes={0}", result.WaitMinutes);
        result.WaitMinutes.Should().Be(15);
    }

    [Fact]
    public void Parse_WaitWithoutValue_ReturnsError()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--wait"]);

        _output.WriteLine("Result: ErrorMessage={0}", result.ErrorMessage);
        result.ErrorMessage.Should().Contain("--wait requires");
    }

    [Fact]
    public void Parse_WaitWithInvalidNumber_ReturnsError()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--wait", "notanumber"]);

        _output.WriteLine("Result: ErrorMessage={0}", result.ErrorMessage);
        result.ErrorMessage.Should().Contain("--wait requires");
    }

    // Environment fallback

    [Fact]
    public void Parse_NoWaitFlag_UsesEnvironmentVariable()
    {
        _mockEnvironment
            .Setup(e => e.GetEnvironmentVariable("CLAUDE_WAIT_MINUTES"))
            .Returns("25");
        var parser = CreateParser();

        var result = parser.Parse([]);

        _output.WriteLine("Result: WaitMinutes={0}", result.WaitMinutes);
        result.WaitMinutes.Should().Be(25);
    }

    [Fact]
    public void Parse_WaitFlag_OverridesEnvironmentVariable()
    {
        _mockEnvironment
            .Setup(e => e.GetEnvironmentVariable("CLAUDE_WAIT_MINUTES"))
            .Returns("25");
        var parser = CreateParser();

        var result = parser.Parse(["--wait", "10"]);

        _output.WriteLine("Result: WaitMinutes={0}", result.WaitMinutes);
        result.WaitMinutes.Should().Be(10);
    }

    [Fact]
    public void Parse_InvalidEnvironmentVariable_ReturnsNull()
    {
        _mockEnvironment
            .Setup(e => e.GetEnvironmentVariable("CLAUDE_WAIT_MINUTES"))
            .Returns("invalid");
        var parser = CreateParser();

        var result = parser.Parse([]);

        _output.WriteLine("Result: WaitMinutes={0}", result.WaitMinutes);
        result.WaitMinutes.Should().BeNull();
    }

    // Pass-through arguments

    [Fact]
    public void Parse_UnknownArgs_AddedToClaudeArgs()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--model", "claude-3-opus"]);

        _output.WriteLine("Result: ClaudeArgs=[{0}]", string.Join(", ", result.ClaudeArgs));
        result.ClaudeArgs.Should().BeEquivalentTo("--model", "claude-3-opus");
    }

    [Fact]
    public void Parse_MixedArgs_CorrectlySeparated()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--verbose", "--model", "opus", "-p", "test"]);

        _output.WriteLine("Result: Verbose={0}, InitialPrompt={1}, ClaudeArgs=[{2}]",
            result.Verbose, result.InitialPrompt, string.Join(", ", result.ClaudeArgs));
        result.Verbose.Should().BeTrue();
        result.InitialPrompt.Should().Be("test");
        result.ClaudeArgs.Should().BeEquivalentTo("--model", "opus");
    }

    // Edge cases

    [Fact]
    public void Parse_EmptyArgs_ReturnsDefaultResult()
    {
        var parser = CreateParser();

        var result = parser.Parse([]);

        _output.WriteLine("Result: all properties default");
        result.ShowHelp.Should().BeFalse();
        result.ShowVersion.Should().BeFalse();
        result.ShowDiagnose.Should().BeFalse();
        result.Verbose.Should().BeFalse();
        result.Headless.Should().BeFalse();
        result.Dangerous.Should().BeFalse();
        result.ContinueConversation.Should().BeFalse();
        result.InitialPrompt.Should().BeNull();
        result.WaitMinutes.Should().BeNull();
        result.ClaudeArgs.Should().BeEmpty();
        result.ErrorMessage.Should().BeNull();
    }

    [Theory]
    [InlineData("--HELP")]
    [InlineData("--Help")]
    [InlineData("--help")]
    public void Parse_CaseInsensitiveFlags_Recognized(string flag)
    {
        var parser = CreateParser();

        var result = parser.Parse([flag]);

        _output.WriteLine("Flag '{0}' -> ShowHelp={1}", flag, result.ShowHelp);
        result.ShowHelp.Should().BeTrue();
    }

    [Fact]
    public void Parse_MultipleFlags_AllRecognized()
    {
        var parser = CreateParser();

        var result = parser.Parse(["--verbose", "--headless", "--dangerous", "-c"]);

        _output.WriteLine("Result: Verbose={0}, Headless={1}, Dangerous={2}, Continue={3}",
            result.Verbose, result.Headless, result.Dangerous, result.ContinueConversation);
        result.Verbose.Should().BeTrue();
        result.Headless.Should().BeTrue();
        result.Dangerous.Should().BeTrue();
        result.ContinueConversation.Should().BeTrue();
    }

    [Fact]
    public void Parse_InfoFlagStopsParsingEarly()
    {
        var parser = CreateParser();

        // --help appears first - should stop and return immediately
        var result = parser.Parse(["--help", "--verbose"]);

        _output.WriteLine("Result: ShowHelp={0}, Verbose={1}", result.ShowHelp, result.Verbose);
        result.ShowHelp.Should().BeTrue();
        // Verbose is false because parsing stopped at --help
        result.Verbose.Should().BeFalse();
    }
}
