@headless
Feature: Headless Mode Prompt Detection
    As a CI/CD pipeline running Claude
    I want prompts to be auto-answered in headless mode
    So that automation runs without human intervention

    Background:
        Given headless mode is enabled
        And dangerous permissions are enabled

    @unit
    Scenario: Detects yes/no prompt
        Given the output buffer contains "[y/n]"
        And no output has been received for 2 seconds
        When the prompt check runs
        Then a prompt should be detected
        And "y\n" should be sent as response

    @unit
    Scenario: Detects continue prompt
        Given the output buffer contains "Do you want to continue?"
        And no output has been received for 2 seconds
        When the prompt check runs
        Then a prompt should be detected

    @unit
    Scenario: Does not auto-respond when output is still arriving
        Given the output buffer contains "[y/n]"
        And output was received within the last second
        When the prompt check runs
        Then no prompt response should be sent

    @unit
    Scenario Outline: Detects various prompt patterns
        Given the output buffer contains "<pattern>"
        And no output has been received for 2 seconds
        When the prompt check runs
        Then a prompt should be detected

        Examples:
            | pattern         |
            | [y/n]           |
            | [yes/no]        |
            | proceed?        |
            | continue?       |
            | approve         |
            | allow this      |
