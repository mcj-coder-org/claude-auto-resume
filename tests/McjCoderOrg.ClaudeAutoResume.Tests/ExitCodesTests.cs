namespace McjCoderOrg.ClaudeAutoResume;

public sealed class ExitCodesTests
{
    [Fact]
    public void Success_ShouldBeZero()
    {
        ExitCodes.Success.Should().Be(0);
    }

    [Fact]
    public void GeneralError_ShouldBeOne()
    {
        ExitCodes.GeneralError.Should().Be(1);
    }

    [Fact]
    public void ConfigurationError_ShouldBeTwo()
    {
        ExitCodes.ConfigurationError.Should().Be(2);
    }

    [Fact]
    public void DependencyMissing_ShouldBeThree()
    {
        ExitCodes.DependencyMissing.Should().Be(3);
    }

    [Fact]
    public void RateLimitDetected_ShouldBeFour()
    {
        ExitCodes.RateLimitDetected.Should().Be(4);
    }

    [Fact]
    public void UserCancelled_ShouldBeFive()
    {
        ExitCodes.UserCancelled.Should().Be(5);
    }
}
