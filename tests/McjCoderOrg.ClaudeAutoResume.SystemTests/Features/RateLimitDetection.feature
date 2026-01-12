@rate-limit
Feature: Rate Limit Detection
    As a user running Claude CLI
    I want the wrapper to detect rate limit messages
    So that it can automatically wait and resume

    Background:
        Given the default wrapper configuration

    @unit
    Scenario: Detects standard rate limit message
        Given the output buffer contains "Your usage limit has been reached"
        And the buffer contains "limit" and "reached"
        When the rate limit check runs
        Then a rate limit should be detected

    @unit
    Scenario: Detects rate limit with reset message
        Given the output buffer contains "Rate limit exceeded. Resets in 15 minutes"
        And the buffer contains "limit" and "reset"
        When the rate limit check runs
        Then a rate limit should be detected

    @unit
    Scenario: Does not detect normal output as rate limit
        Given the output buffer contains "Claude is processing your request"
        When the rate limit check runs
        Then no rate limit should be detected

    @unit
    Scenario: Does not trigger during cooldown period
        Given a rate limit was recently detected
        And the cooldown period has not elapsed
        And the output buffer contains "limit reached"
        When the rate limit check runs
        Then no rate limit should be detected

    @unit
    Scenario Outline: Detects various rate limit patterns
        Given the output buffer contains "<message>"
        And the buffer contains "limit" or "requests"
        When the rate limit check runs
        Then a rate limit should be detected

        Examples:
            | message                              |
            | claude ai usage limit reached        |
            | too many requests, please wait       |
            | rate limit exceeded                  |
            | quota exceeded, limit reached        |
