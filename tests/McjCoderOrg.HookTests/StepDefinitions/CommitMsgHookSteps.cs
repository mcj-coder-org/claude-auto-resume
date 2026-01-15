using McjCoderOrg.HookTests.Fixtures;
using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class CommitMsgHookSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _output;

    public CommitMsgHookSteps(ScenarioContext scenarioContext, IReqnrollOutputHelper output)
    {
        _scenarioContext = scenarioContext;
        _output = output;
    }

    private GitRepositoryFixture Fixture => (GitRepositoryFixture)_scenarioContext["Fixture"];
    private HookRunner HookRunner => (HookRunner)_scenarioContext["HookRunner"];
    private CommonHookSteps CommonSteps => _scenarioContext.ScenarioContainer.Resolve<CommonHookSteps>();

    [When("I create a commit message {string}")]
    public async Task WhenICreateACommitMessage(string message)
    {
        _output.WriteLine("Creating commit message: {0}", message);
        await Fixture.CreateCommitMessageFileAsync(message).ConfigureAwait(false);
    }

    [When("I create a commit message {string} with body {string}")]
    public async Task WhenICreateACommitMessageWithBody(string header, string body)
    {
        var fullMessage = $"{header}\n\n{body}";
        _output.WriteLine("Creating commit message with body: {0}", fullMessage);
        await Fixture.CreateCommitMessageFileAsync(fullMessage).ConfigureAwait(false);
    }

    [When("I run the commit-msg hook")]
    public async Task WhenIRunTheCommitMsgHook()
    {
        _output.WriteLine("Running commit-msg hook");
        ToolAvailability.SkipIfNodeMissing();

        var commitMsgPath = Fixture.GetCommitMessageFilePath();
        var result = await HookRunner.RunHookAsync("commit-msg", commitMsgPath).ConfigureAwait(false);
        _output.WriteLine("Commit-msg exit code: {0}", result.ExitCode);
        if (!string.IsNullOrEmpty(result.CombinedOutput))
        {
            _output.WriteLine("Commit-msg output: {0}", result.CombinedOutput);
        }

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
