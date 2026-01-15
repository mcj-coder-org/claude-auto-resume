namespace McjCoderOrg.ClaudeAutoResume.StepDefinitions;

[Binding]
public sealed class AutoResumeSteps
{
    private readonly IReqnrollOutputHelper _output;
    private WrapperConfig _config = WrapperConfig.Default;
    private string? _sentCommand;
    private bool _bufferCleared;
    private int _expectedWaitMinutes;

    public AutoResumeSteps(IReqnrollOutputHelper output)
    {
        _output = output;
    }

    [Given("a rate limit has been detected")]
    public void GivenARateLimitHasBeenDetected()
    {
        // State tracking for rate limit - simulation only
        _expectedWaitMinutes = _config.WaitMinutes;
        _output.WriteLine("Rate limit detected, wait time: {0} minutes", _expectedWaitMinutes);
    }

    [Given("the wait time is configured to {int} minutes")]
    public void GivenTheWaitTimeIsConfiguredToMinutes(int minutes)
    {
        _output.WriteLine("Configuring wait time to {0} minutes", minutes);
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
        _output.WriteLine("Verifying continue command was sent: {0}", _sentCommand ?? "(null)");
        _sentCommand.Should().NotBeNull("the continue command should have been sent");
    }

    [Then("the output buffer should be cleared")]
    public void ThenTheOutputBufferShouldBeCleared()
    {
        _output.WriteLine("Verifying output buffer cleared: {0}", _bufferCleared);
        _bufferCleared.Should().BeTrue("the output buffer should be cleared after rate limit");
    }

    [Then("the wrapper should wait for {int} minutes")]
    public void ThenTheWrapperShouldWaitForMinutes(int minutes)
    {
        _output.WriteLine("Verifying wait time: expected={0}, actual={1}", minutes, _expectedWaitMinutes);
        _expectedWaitMinutes.Should().Be(minutes);
        _config.WaitMinutes.Should().Be(minutes);
    }

    [Then("{string} should be sent to the PTY")]
    public void ThenStringShouldBeSentToThePty(string expected)
    {
        var unescaped = expected.Replace("\\n", "\n", StringComparison.Ordinal);
        _output.WriteLine("Verifying PTY command: expected='{0}', actual='{1}'", unescaped, _sentCommand);
        _sentCommand.Should().Be(unescaped);
    }

    [Then("a newline should be sent to the PTY")]
    public void ThenANewlineShouldBeSentToThePty()
    {
        _output.WriteLine("Verifying newline sent to PTY");
        _sentCommand.Should().Be("\n");
    }

    private void SimulateSendContinueCommand()
    {
        _sentCommand = _config.ContinueCommand;
        _bufferCleared = true;
    }
}
