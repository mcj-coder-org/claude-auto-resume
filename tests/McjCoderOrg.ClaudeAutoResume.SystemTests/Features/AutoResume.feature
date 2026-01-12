@auto-resume
Feature: Auto-Resume Functionality
    As a user running Claude CLI with rate limits
    I want the wrapper to automatically resume after waiting
    So that long-running tasks complete without manual intervention

    Background:
        Given the default wrapper configuration

    @unit
    Scenario: Sends continue command after wait period
        Given a rate limit has been detected
        When the configured wait period elapses
        Then the continue command should be sent
        And the output buffer should be cleared

    @unit
    Scenario: Uses configured wait time
        Given the wait time is configured to 10 minutes
        When a rate limit is detected
        Then the wrapper should wait for 10 minutes

    @unit
    Scenario: Uses default wait time when not configured
        When a rate limit is detected
        Then the wrapper should wait for 15 minutes

    @unit
    Scenario: Sends configured continue command
        Given the continue command is configured as "resume\n"
        When resuming after rate limit
        Then "resume\n" should be sent to the PTY

    @unit
    Scenario: Uses default continue command
        When resuming after rate limit
        Then a newline should be sent to the PTY
