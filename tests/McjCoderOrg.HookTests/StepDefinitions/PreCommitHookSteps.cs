using McjCoderOrg.HookTests.Fixtures;
using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class PreCommitHookSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _output;

    public PreCommitHookSteps(ScenarioContext scenarioContext, IReqnrollOutputHelper output)
    {
        _scenarioContext = scenarioContext;
        _output = output;
    }

    private GitRepositoryFixture Fixture => (GitRepositoryFixture)_scenarioContext["Fixture"];
    private HookRunner HookRunner => (HookRunner)_scenarioContext["HookRunner"];
    private CommonHookSteps CommonSteps => _scenarioContext.ScenarioContainer.Resolve<CommonHookSteps>();

    [When("I attempt to commit")]
    public async Task WhenIAttemptToCommit()
    {
        _output.WriteLine("Staging test file and running pre-commit hook");
        // Stage a test file if nothing is staged
        await Fixture.StageFileAsync("test.txt", $"Test content {DateTime.UtcNow:O}").ConfigureAwait(false);

        var result = await HookRunner.RunHookAsync("pre-commit").ConfigureAwait(false);
        _output.WriteLine("Pre-commit exit code: {0}", result.ExitCode);
        if (!string.IsNullOrEmpty(result.CombinedOutput))
        {
            _output.WriteLine("Pre-commit output: {0}", result.CombinedOutput);
        }

        CommonSteps.SetLastResult(result);
    }
}
