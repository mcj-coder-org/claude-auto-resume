@commit-msg
Feature: Commit message validation
    As a developer
    I want commit messages validated against conventions
    So that changelog generation and traceability work correctly

    Background:
        Given I have a git repository with hooks configured
        And node_modules directory exists

    @conventional-commits
    Scenario Outline: Accept valid conventional commit types
        When I create a commit message "<type>: add feature" with body "Refs: #123"
        And I run the commit-msg hook
        Then the hook should succeed

        Examples:
            | type     |
            | feat     |
            | fix      |
            | docs     |
            | style    |
            | refactor |
            | perf     |
            | test     |
            | build    |
            | ci       |
            | chore    |
            | revert   |

    @conventional-commits
    Scenario: Reject invalid commit type
        When I create a commit message "invalid: some change" with body "Refs: #123"
        And I run the commit-msg hook
        Then the hook should fail
        And the output should contain "type-enum"

    @work-item-traceability
    Scenario: Require work item reference
        When I create a commit message "feat: add feature without reference"
        And I run the commit-msg hook
        Then the hook should fail
        And the output should contain "references-empty"

    @work-item-traceability
    Scenario: Accept various reference formats
        When I create a commit message "feat: add feature" with body "Refs: #123"
        And I run the commit-msg hook
        Then the hook should succeed

    @conventional-commits
    Scenario: Reject uppercase in subject
        When I create a commit message "feat: Add Feature" with body "Refs: #123"
        And I run the commit-msg hook
        Then the hook should fail
        And the output should contain "subject-case"

    @conventional-commits
    Scenario: Reject header exceeding max length
        When I commit with a 101 character header
        Then the hook should fail
        And the output should contain "header-max-length"

    @conventional-commits
    Scenario: Auto-ignore merge commits
        When I create a commit message "Merge branch 'feature/123-test'"
        And I run the commit-msg hook
        Then the hook should succeed
