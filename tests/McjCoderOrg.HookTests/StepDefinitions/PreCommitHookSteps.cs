using McjCoderOrg.HookTests.Fixtures;
using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class PreCommitHookSteps
{
    private readonly ScenarioContext _scenarioContext;

    public PreCommitHookSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private GitRepositoryFixture Fixture => (GitRepositoryFixture)_scenarioContext["Fixture"];
    private HookRunner HookRunner => (HookRunner)_scenarioContext["HookRunner"];
    private CommonHookSteps CommonSteps => _scenarioContext.ScenarioContainer.Resolve<CommonHookSteps>();

    [Given("I have staged a file containing {string} AWS key pattern")]
    public async Task GivenIHaveStagedAFileContainingAwsKeyPattern(string pattern)
    {
        var secretContent = $"aws_access_key_id = {pattern}EXAMPLEKEY123456789";
        await Fixture.StageFileAsync("config.txt", secretContent).ConfigureAwait(false);
    }

    [Given("I have staged a clean file")]
    public async Task GivenIHaveStagedACleanFile()
    {
        await Fixture.StageFileAsync("clean.txt", "This is clean content with no secrets.").ConfigureAwait(false);
    }

    [When("I attempt to commit")]
    public async Task WhenIAttemptToCommit()
    {
        // Stage a test file if nothing is staged
        await Fixture.StageFileAsync("test.txt", $"Test content {DateTime.UtcNow:O}").ConfigureAwait(false);

        var result = await HookRunner.RunHookAsync("pre-commit").ConfigureAwait(false);
        CommonSteps.SetLastResult(result);
    }
}
