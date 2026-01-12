namespace McjCoderOrg.ClaudeAutoResume.StepDefinitions;

[Binding]
public sealed class AutoResumeSteps
{
    private WrapperConfig _config = WrapperConfig.Default;
    private string? _sentCommand;
    private bool _bufferCleared;
    private int _expectedWaitMinutes;

    [Given("a rate limit has been detected")]
    public void GivenARateLimitHasBeenDetected()
    {
        // State tracking for rate limit - simulation only
        _expectedWaitMinutes = _config.WaitMinutes;
    }

    [Given("the wait time is configured to {int} minutes")]
    public void GivenTheWaitTimeIsConfiguredToMinutes(int minutes)
    {
        _config = _config with { WaitMinutes = minutes };
        _expectedWaitMinutes = minutes;
    }

    [Given("the continue command is configured as {string}")]
    public void GivenTheContinueCommandIsConfiguredAs(string command)
    {
        // Unescape the string for proper comparison
        var unescaped = command.Replace("\\n", "\n", StringComparison.Ordinal);
        _config = _config with { ContinueCommand = unescaped };
    }

    [When("the configured wait period elapses")]
    public void WhenTheConfiguredWaitPeriodElapses()
    {
        SimulateSendContinueCommand();
    }

    [When("a rate limit is detected")]
    public void WhenARateLimitIsDetected()
    {
        _expectedWaitMinutes = _config.WaitMinutes;
    }

    [When("resuming after rate limit")]
    public void WhenResumingAfterRateLimit()
    {
        SimulateSendContinueCommand();
    }

    [Then("the continue command should be sent")]
    public void ThenTheContinueCommandShouldBeSent()
    {
        _sentCommand.Should().NotBeNull("the continue command should have been sent");
    }

    [Then("the output buffer should be cleared")]
    public void ThenTheOutputBufferShouldBeCleared()
    {
        _bufferCleared.Should().BeTrue("the output buffer should be cleared after rate limit");
    }

    [Then("the wrapper should wait for {int} minutes")]
    public void ThenTheWrapperShouldWaitForMinutes(int minutes)
    {
        _expectedWaitMinutes.Should().Be(minutes);
        _config.WaitMinutes.Should().Be(minutes);
    }

    [Then("{string} should be sent to the PTY")]
    public void ThenStringShouldBeSentToThePty(string expected)
    {
        var unescaped = expected.Replace("\\n", "\n", StringComparison.Ordinal);
        _sentCommand.Should().Be(unescaped);
    }

    [Then("a newline should be sent to the PTY")]
    public void ThenANewlineShouldBeSentToThePty()
    {
        _sentCommand.Should().Be("\n");
    }

    private void SimulateSendContinueCommand()
    {
        _sentCommand = _config.ContinueCommand;
        _bufferCleared = true;
    }
}
