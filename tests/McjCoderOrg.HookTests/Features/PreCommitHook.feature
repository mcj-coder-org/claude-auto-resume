@pre-commit
Feature: Pre-commit hook validation
    As a developer
    I want the pre-commit hook to enforce quality standards
    So that code quality is maintained before commits

    Background:
        Given I have a git repository with hooks configured

    @branch-protection
    Scenario: Block direct commits to main branch
        Given node_modules directory exists
        And I am on the "main" branch
        When I attempt to commit
        Then the hook should fail with exit code 1
        And the output should contain "Direct commits to main are not allowed"

    @branch-protection
    Scenario: Allow commits on feature branches
        Given I am on a "feature/123-test" branch
        And GPG signing is configured
        And node_modules directory exists
        When I attempt to commit
        Then the hook should succeed

    @signed-commits
    Scenario: Require GPG signing configuration
        Given node_modules directory exists
        And I am on a "feature/123-test" branch
        And GPG signing is not configured
        When I attempt to commit
        Then the hook should fail with exit code 1
        And the output should contain "commit.gpgsign"

    @signed-commits
    Scenario: Accept commits with GPG signing enabled
        Given I am on a "feature/123-test" branch
        And GPG signing is configured
        And node_modules directory exists
        When I attempt to commit
        Then the hook should succeed

    @prerequisites
    Scenario: Require node_modules to be installed
        Given I am on a "feature/123-test" branch
        And GPG signing is configured
        And node_modules directory does not exist
        When I attempt to commit
        Then the hook should fail with exit code 1
        And the output should contain "Dependencies not installed"
