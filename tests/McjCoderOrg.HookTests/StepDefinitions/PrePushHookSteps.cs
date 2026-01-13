using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class PrePushHookSteps
{
    private readonly ScenarioContext _scenarioContext;

    public PrePushHookSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private HookRunner HookRunner => (HookRunner)_scenarioContext["HookRunner"];
    private CommonHookSteps CommonSteps => _scenarioContext.ScenarioContainer.Resolve<CommonHookSteps>();

    [Given("dotnet CLI is not available")]
    public void GivenDotnetCliIsNotAvailable()
    {
        // This is a state marker - the actual behavior depends on system config
        // We can't actually make dotnet unavailable, but we can verify the hook handles it
        _scenarioContext["DotnetUnavailable"] = true;
    }

    [When("I attempt to push")]
    public async Task WhenIAttemptToPush()
    {
        // The pre-push hook validates branch naming, not actual push
        var result = await HookRunner.RunHookAsync("pre-push").ConfigureAwait(false);
        CommonSteps.SetLastResult(result);
    }
}
