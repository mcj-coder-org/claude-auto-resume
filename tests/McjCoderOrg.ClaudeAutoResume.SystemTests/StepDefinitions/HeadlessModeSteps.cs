namespace McjCoderOrg.ClaudeAutoResume.StepDefinitions;

[Binding]
public sealed class HeadlessModeSteps
{
    private readonly IReqnrollOutputHelper _output;
    private WrapperConfig _config = WrapperConfig.Default;
    private string _bufferContent = string.Empty;
    private bool _promptDetected;
    private string? _sentResponse;
    private double _secondsSinceLastOutput;

    public HeadlessModeSteps(IReqnrollOutputHelper output)
    {
        _output = output;
    }

    [Given("headless mode is enabled")]
    public void GivenHeadlessModeIsEnabled()
    {
        _output.WriteLine("Enabling headless mode");
        _config = _config with { Headless = true };
    }

    [Given("dangerous permissions are enabled")]
    public void GivenDangerousPermissionsAreEnabled()
    {
        _output.WriteLine("Enabling dangerous permissions");
        _config = _config with { DangerouslySkipPermissions = true };
    }

    [Given("no output has been received for {int} seconds")]
    public void GivenNoOutputHasBeenReceivedForSeconds(int seconds)
    {
        _secondsSinceLastOutput = seconds;
    }

    [Given("output was received within the last second")]
    public void GivenOutputWasReceivedWithinTheLastSecond()
    {
        _secondsSinceLastOutput = 0.5;
    }

    // Shared step definition from RateLimitDetectionSteps - reuse for headless mode
    [Given(@"the output buffer contains ""([^""]*)""")]
    [Scope(Tag = "headless")]
    public void GivenTheOutputBufferContainsForHeadless(string content)
    {
        _bufferContent = content;
    }

    [When("the prompt check runs")]
    public void WhenThePromptCheckRuns()
    {
        _output.WriteLine("Running prompt check, buffer: '{0}', timeout: {1}s, elapsed: {2}s",
            _bufferContent, _config.PromptTimeoutSeconds, _secondsSinceLastOutput);

        // Simulate prompt detection logic from ClaudeMonitor
        var matchFound = _config.PromptPatterns.Any(pattern =>
            _bufferContent.Contains(pattern, StringComparison.OrdinalIgnoreCase));

        if (matchFound && _secondsSinceLastOutput >= _config.PromptTimeoutSeconds)
        {
            _promptDetected = true;
            _sentResponse = _config.DefaultPromptResponse;
            _output.WriteLine("Prompt detected, sending response: '{0}'", _sentResponse);
        }
        else
        {
            _promptDetected = false;
            _output.WriteLine("No prompt detected (matchFound={0})", matchFound);
        }
    }

    [Then("a prompt should be detected")]
    public void ThenAPromptShouldBeDetected()
    {
        _output.WriteLine("Verifying prompt detected: {0}", _promptDetected);
        _promptDetected.Should().BeTrue("a prompt pattern should have been matched");
    }

    [Then("{string} should be sent as response")]
    public void ThenStringShouldBeSentAsResponse(string expected)
    {
        var unescaped = expected.Replace("\\n", "\n", StringComparison.Ordinal);
        _output.WriteLine("Verifying response: expected='{0}', actual='{1}'", unescaped, _sentResponse);
        _sentResponse.Should().Be(unescaped);
    }

    [Then("no prompt response should be sent")]
    public void ThenNoPromptResponseShouldBeSent()
    {
        _output.WriteLine("Verifying no prompt response sent");
        _promptDetected.Should().BeFalse("no prompt should be detected during active output");
        _sentResponse.Should().BeNull();
    }
}
