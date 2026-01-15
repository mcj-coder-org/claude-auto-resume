@E2E
Feature: Application startup
    As a user
    I want the application to start correctly with various flags
    So that I can use claude-auto-resume effectively

    @cli-flags
    Scenario: Display version information
        When I run the application with "--version"
        Then the exit code should be 0
        And the output should contain "claude-auto-resume"

    @cli-flags
    Scenario: Display help information
        When I run the application with "--help"
        Then the exit code should be 0
        And the output should contain "USAGE:"
        And the output should contain "--version"
        And the output should contain "--help"

    @cli-flags
    Scenario: Display diagnostic information
        When I run the application with "--diagnose"
        Then the exit code should be 0
        And the output should contain "Runtime:"

    @cli-flags
    Scenario: Reject headless mode without dangerous flag
        When I run the application with "--headless"
        Then the exit code should be 2
        And the combined output should contain "--dangerously-skip-permissions"

    @dependency-check @skip-if-claude-available
    Scenario: Report missing claude CLI
        Given the executable exists
        And claude CLI is not available
        When I run the application with no arguments
        Then the exit code should be 4
        And the error output should contain "Could not find 'claude' in PATH"

    @happy-path @skip-if-claude-missing
    Scenario: Start successfully when claude is available
        Given the executable exists
        And claude CLI is available
        When I run the application with no arguments
        Then the application should keep running for at least 3 seconds

    # NOTE: This test requires manual execution - Claude Code's input handler
    # doesn't execute commands from piped stdin the same way as interactive input.
    # The piped stdin support is implemented but Claude Code's PTY input handling
    # requires further investigation to fully support automated /exit testing.
    @happy-path @skip-if-claude-missing @manual
    Scenario: Exit cleanly via /exit command
        Given the executable exists
        And claude CLI is available
        When I start the application interactively
        And I wait for the application to be ready
        And I send "/exit" to the application
        Then the application should exit within 30 seconds
        And the exit code should be 0
