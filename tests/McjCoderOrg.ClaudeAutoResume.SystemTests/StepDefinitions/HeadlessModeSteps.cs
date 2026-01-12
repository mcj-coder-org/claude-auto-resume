namespace McjCoderOrg.ClaudeAutoResume.StepDefinitions;

[Binding]
public sealed class HeadlessModeSteps
{
    private WrapperConfig _config = WrapperConfig.Default;
    private string _bufferContent = string.Empty;
    private bool _promptDetected;
    private string? _sentResponse;
    private double _secondsSinceLastOutput;

    [Given("headless mode is enabled")]
    public void GivenHeadlessModeIsEnabled()
    {
        _config = _config with { Headless = true };
    }

    [Given("dangerous permissions are enabled")]
    public void GivenDangerousPermissionsAreEnabled()
    {
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
        // Simulate prompt detection logic from ClaudeMonitor
        var matchFound = _config.PromptPatterns.Any(pattern =>
            _bufferContent.Contains(pattern, StringComparison.OrdinalIgnoreCase));

        if (matchFound && _secondsSinceLastOutput >= _config.PromptTimeoutSeconds)
        {
            _promptDetected = true;
            _sentResponse = _config.DefaultPromptResponse;
        }
        else
        {
            _promptDetected = false;
        }
    }

    [Then("a prompt should be detected")]
    public void ThenAPromptShouldBeDetected()
    {
        _promptDetected.Should().BeTrue("a prompt pattern should have been matched");
    }

    [Then("{string} should be sent as response")]
    public void ThenStringShouldBeSentAsResponse(string expected)
    {
        var unescaped = expected.Replace("\\n", "\n", StringComparison.Ordinal);
        _sentResponse.Should().Be(unescaped);
    }

    [Then("no prompt response should be sent")]
    public void ThenNoPromptResponseShouldBeSent()
    {
        _promptDetected.Should().BeFalse("no prompt should be detected during active output");
        _sentResponse.Should().BeNull();
    }
}
