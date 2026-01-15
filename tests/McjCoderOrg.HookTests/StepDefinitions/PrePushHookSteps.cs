using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class PrePushHookSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _output;

    public PrePushHookSteps(ScenarioContext scenarioContext, IReqnrollOutputHelper output)
    {
        _scenarioContext = scenarioContext;
        _output = output;
    }

    private HookRunner HookRunner => (HookRunner)_scenarioContext["HookRunner"];
    private CommonHookSteps CommonSteps => _scenarioContext.ScenarioContainer.Resolve<CommonHookSteps>();

    [Given("dotnet CLI is not available")]
    public void GivenDotnetCliIsNotAvailable()
    {
        _output.WriteLine("Marking dotnet CLI as unavailable");
        // This is a state marker - the actual behavior depends on system config
        // We can't actually make dotnet unavailable, but we can verify the hook handles it
        _scenarioContext["DotnetUnavailable"] = true;
    }

    [When("I attempt to push")]
    public async Task WhenIAttemptToPush()
    {
        _output.WriteLine("Running pre-push hook");
        // The pre-push hook validates branch naming, not actual push
        var result = await HookRunner.RunHookAsync("pre-push").ConfigureAwait(false);
        _output.WriteLine("Pre-push exit code: {0}", result.ExitCode);
        if (!string.IsNullOrEmpty(result.CombinedOutput))
        {
            _output.WriteLine("Pre-push output: {0}", result.CombinedOutput);
        }

        CommonSteps.SetLastResult(result);
    }
}
