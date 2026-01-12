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
    public void InvalidArguments_ShouldBeTwo()
    {
        ExitCodes.InvalidArguments.Should().Be(2);
    }

    [Fact]
    public void ConfigurationError_ShouldBeThree()
    {
        ExitCodes.ConfigurationError.Should().Be(3);
    }

    [Fact]
    public void DependencyMissing_ShouldBeFour()
    {
        ExitCodes.DependencyMissing.Should().Be(4);
    }

    [Fact]
    public void RateLimitDetected_ShouldBeFive()
    {
        ExitCodes.RateLimitDetected.Should().Be(5);
    }

    [Fact]
    public void UserCancelled_ShouldBeSix()
    {
        ExitCodes.UserCancelled.Should().Be(6);
    }
}
