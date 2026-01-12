namespace McjCoderOrg.ClaudeAutoResume;

public sealed class WrapperConfigTests
{
    [Fact]
    public void Default_ShouldReturnNonNullInstance()
    {
        var config = WrapperConfig.Default;

        config.Should().NotBeNull();
    }

    [Fact]
    public void Default_WaitMinutes_ShouldBeFifteen()
    {
        var config = WrapperConfig.Default;

        config.WaitMinutes.Should().Be(15);
    }

    [Fact]
    public void Default_ContinueCommand_ShouldBeNewline()
    {
        var config = WrapperConfig.Default;

        config.ContinueCommand.Should().Be("\n");
    }

    [Fact]
    public void Default_OutputBufferSize_ShouldBe2000()
    {
        var config = WrapperConfig.Default;

        config.OutputBufferSize.Should().Be(2000);
    }

    [Fact]
    public void Default_CooldownSeconds_ShouldBe30()
    {
        var config = WrapperConfig.Default;

        config.CooldownSeconds.Should().Be(30);
    }

    [Fact]
    public void Default_Headless_ShouldBeFalse()
    {
        var config = WrapperConfig.Default;

        config.Headless.Should().BeFalse();
    }

    [Fact]
    public void Default_DangerouslySkipPermissions_ShouldBeFalse()
    {
        var config = WrapperConfig.Default;

        config.DangerouslySkipPermissions.Should().BeFalse();
    }

    [Fact]
    public void Default_DefaultPromptResponse_ShouldBeYesNewline()
    {
        var config = WrapperConfig.Default;

        config.DefaultPromptResponse.Should().Be("y\n");
    }

    [Fact]
    public void Default_PromptTimeoutSeconds_ShouldBeTwo()
    {
        var config = WrapperConfig.Default;

        config.PromptTimeoutSeconds.Should().Be(2.0);
    }

    [Fact]
    public void Default_ClaudePath_ShouldBeNull()
    {
        var config = WrapperConfig.Default;

        config.ClaudePath.Should().BeNull();
    }

    [Fact]
    public void Default_InitialPrompt_ShouldBeNull()
    {
        var config = WrapperConfig.Default;

        config.InitialPrompt.Should().BeNull();
    }

    [Fact]
    public void Default_ContinueConversation_ShouldBeFalse()
    {
        var config = WrapperConfig.Default;

        config.ContinueConversation.Should().BeFalse();
    }

    [Fact]
    public void RateLimitPatterns_ShouldContainLimitReached()
    {
        var config = WrapperConfig.Default;

        config.RateLimitPatterns.Should().Contain("limit reached");
    }

    [Fact]
    public void RateLimitPatterns_ShouldContainRateLimit()
    {
        var config = WrapperConfig.Default;

        config.RateLimitPatterns.Should().Contain("rate limit");
    }

    [Fact]
    public void RateLimitPatterns_ShouldContainTooManyRequests()
    {
        var config = WrapperConfig.Default;

        config.RateLimitPatterns.Should().Contain("too many requests");
    }

    [Fact]
    public void RateLimitPatterns_ShouldNotBeEmpty()
    {
        var config = WrapperConfig.Default;

        config.RateLimitPatterns.Should().NotBeEmpty();
    }

    [Fact]
    public void PromptPatterns_ShouldContainYesNoPattern()
    {
        var config = WrapperConfig.Default;

        config.PromptPatterns.Should().Contain("[y/n]");
    }

    [Fact]
    public void PromptPatterns_ShouldContainContinueQuestion()
    {
        var config = WrapperConfig.Default;

        config.PromptPatterns.Should().Contain("continue?");
    }

    [Fact]
    public void PromptPatterns_ShouldContainApprove()
    {
        var config = WrapperConfig.Default;

        config.PromptPatterns.Should().Contain("approve");
    }

    [Fact]
    public void PromptPatterns_ShouldNotBeEmpty()
    {
        var config = WrapperConfig.Default;

        config.PromptPatterns.Should().NotBeEmpty();
    }

    [Fact]
    public void WithExpression_ShouldCreateModifiedCopy()
    {
        var original = WrapperConfig.Default;

        var modified = original with { WaitMinutes = 30 };

        modified.WaitMinutes.Should().Be(30);
        original.WaitMinutes.Should().Be(15);
    }

    [Fact]
    public void WithExpression_ShouldPreserveUnchangedValues()
    {
        var original = WrapperConfig.Default;

        var modified = original with { Headless = true };

        modified.Headless.Should().BeTrue();
        modified.WaitMinutes.Should().Be(original.WaitMinutes);
        modified.CooldownSeconds.Should().Be(original.CooldownSeconds);
    }
}
