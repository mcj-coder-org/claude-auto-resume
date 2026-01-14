@pre-push @Integration
Feature: Pre-push hook validation
    As a developer
    I want branch naming and tests validated before push
    So that the remote repository maintains quality standards

    Background:
        Given I have a git repository with hooks configured
        And node_modules directory exists

    @branch-naming
    Scenario Outline: Accept valid branch naming patterns
        Given I am on a "<branch>" branch
        When I attempt to push
        Then the hook should succeed

        Examples:
            | branch              |
            | feature/123-login   |
            | fix/456-null-check  |
            | docs/789-readme     |
            | chore/101-cleanup   |
            | refactor/202-split  |
            | test/303-coverage   |

    @branch-naming
    Scenario: Allow push from main branch
        Given I am on the "main" branch
        When I attempt to push
        Then the hook should succeed

    @branch-naming
    Scenario: Reject invalid branch name format
        Given I am on a "my-feature" branch
        When I attempt to push
        Then the hook should fail
        And the output should contain "Invalid branch name"

    @branch-naming
    Scenario: Reject branch missing issue number
        Given I am on a "feature/add-login" branch
        When I attempt to push
        Then the hook should fail
        And the output should contain "Invalid branch name"

    @ci-integration
    Scenario: Skip validation on detached HEAD
        Given I am in detached HEAD state
        When I attempt to push
        Then the hook should succeed
        And the output should contain "Detached HEAD"

    @test-execution
    Scenario: Skip tests when solution file missing
        Given no .sln file exists
        And I am on a "feature/123-test" branch
        When I attempt to push
        Then the hook should succeed

    @prerequisites
    Scenario: Require node_modules to be installed
        Given I am on a "feature/123-test" branch
        And node_modules directory does not exist
        When I attempt to push
        Then the hook should fail with exit code 1
        And the output should contain "Dependencies not installed"
