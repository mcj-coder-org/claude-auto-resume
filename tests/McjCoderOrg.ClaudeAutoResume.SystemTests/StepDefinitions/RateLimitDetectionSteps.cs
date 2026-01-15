namespace McjCoderOrg.ClaudeAutoResume.StepDefinitions;

[Binding]
public sealed class RateLimitDetectionSteps
{
    private readonly IReqnrollOutputHelper _output;
    private WrapperConfig _config = WrapperConfig.Default;
    private string _bufferContent = string.Empty;
    private bool _rateLimitDetected;
    private bool _recentRateLimitDetected;
    private bool _cooldownActive;

    public RateLimitDetectionSteps(IReqnrollOutputHelper output)
    {
        _output = output;
    }

    [Given("the default wrapper configuration")]
    public void GivenTheDefaultWrapperConfiguration()
    {
        _output.WriteLine("Using default wrapper configuration");
        _config = WrapperConfig.Default;
    }

    [Given("the output buffer contains {string}")]
    public void GivenTheOutputBufferContains(string content)
    {
        _output.WriteLine("Setting buffer content: '{0}'", content);
        _bufferContent = content;
    }

    [Given("the buffer contains {string} and {string}")]
    public void GivenTheBufferContainsBothKeywords(string keyword1, string keyword2)
    {
        // Validation step - ensure buffer contains both keywords
        _bufferContent.ToUpperInvariant().Should().Contain(keyword1.ToUpperInvariant());
        _bufferContent.ToUpperInvariant().Should().Contain(keyword2.ToUpperInvariant());
    }

    [Given("the buffer contains {string} or {string}")]
    public void GivenTheBufferContainsEitherKeyword(string keyword1, string keyword2)
    {
        // Validation step - ensure buffer contains at least one keyword
        var containsKeyword1 = _bufferContent.Contains(keyword1, StringComparison.OrdinalIgnoreCase);
        var containsKeyword2 = _bufferContent.Contains(keyword2, StringComparison.OrdinalIgnoreCase);
        (containsKeyword1 || containsKeyword2).Should().BeTrue();
    }

    [Given("a rate limit was recently detected")]
    public void GivenARateLimitWasRecentlyDetected()
    {
        _recentRateLimitDetected = true;
    }

    [Given("the cooldown period has not elapsed")]
    public void GivenTheCooldownPeriodHasNotElapsed()
    {
        _cooldownActive = true;
    }

    [When("the rate limit check runs")]
    public void WhenTheRateLimitCheckRuns()
    {
        _output.WriteLine("Running rate limit check on buffer: '{0}'", _bufferContent);

        // Simulate rate limit detection logic
        if (_cooldownActive && _recentRateLimitDetected)
        {
            _output.WriteLine("Skipping detection due to cooldown");
            _rateLimitDetected = false;
            return;
        }

        var matchedPattern = _config.RateLimitPatterns
            .FirstOrDefault(pattern =>
                _bufferContent.Contains(pattern, StringComparison.OrdinalIgnoreCase) &&
                _bufferContent.Contains("limit", StringComparison.OrdinalIgnoreCase) &&
                (_bufferContent.Contains("reached", StringComparison.OrdinalIgnoreCase) ||
                 _bufferContent.Contains("reset", StringComparison.OrdinalIgnoreCase)));

        // Also check for too many requests pattern
        if (matchedPattern == null)
        {
            matchedPattern = _config.RateLimitPatterns
                .FirstOrDefault(pattern =>
                    _bufferContent.Contains(pattern, StringComparison.OrdinalIgnoreCase));

            if (matchedPattern != null)
            {
                var hasLimitIndicator = _bufferContent.Contains("limit", StringComparison.OrdinalIgnoreCase) ||
                                        _bufferContent.Contains("requests", StringComparison.OrdinalIgnoreCase);
                if (!hasLimitIndicator)
                {
                    matchedPattern = null;
                }
            }
        }

        _rateLimitDetected = matchedPattern != null;
        _output.WriteLine("Rate limit detected: {0}, matched pattern: {1}", _rateLimitDetected, matchedPattern ?? "(none)");
    }

    [Then("a rate limit should be detected")]
    public void ThenARateLimitShouldBeDetected()
    {
        _output.WriteLine("Verifying rate limit detected: {0}", _rateLimitDetected);
        _rateLimitDetected.Should().BeTrue("a rate limit pattern should have been matched");
    }

    [Then("no rate limit should be detected")]
    public void ThenNoRateLimitShouldBeDetected()
    {
        _output.WriteLine("Verifying no rate limit detected: {0}", _rateLimitDetected);
        _rateLimitDetected.Should().BeFalse("no rate limit pattern should have been matched");
    }
}
