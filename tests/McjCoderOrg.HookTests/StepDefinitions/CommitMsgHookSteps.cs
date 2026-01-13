using McjCoderOrg.HookTests.Fixtures;
using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class CommitMsgHookSteps
{
    private readonly ScenarioContext _scenarioContext;

    public CommitMsgHookSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private GitRepositoryFixture Fixture => (GitRepositoryFixture)_scenarioContext["Fixture"];
    private HookRunner HookRunner => (HookRunner)_scenarioContext["HookRunner"];
    private CommonHookSteps CommonSteps => _scenarioContext.ScenarioContainer.Resolve<CommonHookSteps>();

    [When("I create a commit message {string}")]
    public async Task WhenICreateACommitMessage(string message)
    {
        await Fixture.CreateCommitMessageFileAsync(message).ConfigureAwait(false);
    }

    [When("I create a commit message {string} with body {string}")]
    public async Task WhenICreateACommitMessageWithBody(string header, string body)
    {
        var fullMessage = $"{header}\n\n{body}";
        await Fixture.CreateCommitMessageFileAsync(fullMessage).ConfigureAwait(false);
    }

    [When("I run the commit-msg hook")]
    public async Task WhenIRunTheCommitMsgHook()
    {
        ToolAvailability.SkipIfNodeMissing();

        var commitMsgPath = Fixture.GetCommitMessageFilePath();
        var result = await HookRunner.RunHookAsync("commit-msg", commitMsgPath).ConfigureAwait(false);
        CommonSteps.SetLastResult(result);
    }

    [When("I commit with a {int} character header")]
    public async Task WhenICommitWithACharacterHeader(int length)
    {
        // Create a header that exceeds the limit
        // Format: "feat: " (6 chars) + description
        var descriptionLength = length - 6;
        var description = new string('x', descriptionLength);
        var header = $"feat: {description}";
        var fullMessage = $"{header}\n\nRefs: #123";

        await Fixture.CreateCommitMessageFileAsync(fullMessage).ConfigureAwait(false);

        ToolAvailability.SkipIfNodeMissing();

        var commitMsgPath = Fixture.GetCommitMessageFilePath();
        var result = await HookRunner.RunHookAsync("commit-msg", commitMsgPath).ConfigureAwait(false);
        CommonSteps.SetLastResult(result);
    }
}
