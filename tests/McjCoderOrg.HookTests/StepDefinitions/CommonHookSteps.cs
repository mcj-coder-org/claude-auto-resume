using McjCoderOrg.HookTests.Fixtures;
using McjCoderOrg.HookTests.Helpers;

namespace McjCoderOrg.HookTests.StepDefinitions;

[Binding]
public sealed class CommonHookSteps : IAsyncDisposable
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _output;
    private GitRepositoryFixture? _fixture;
    private HookRunner? _hookRunner;
    private HookResult? _lastResult;

    public CommonHookSteps(ScenarioContext scenarioContext, IReqnrollOutputHelper output)
    {
        _scenarioContext = scenarioContext;
        _output = output;
    }

    internal GitRepositoryFixture Fixture => _fixture ?? throw new InvalidOperationException("Fixture not initialized");
    internal HookRunner HookRunner => _hookRunner ?? throw new InvalidOperationException("HookRunner not initialized");
    internal HookResult LastResult => _lastResult ?? throw new InvalidOperationException("No hook result available");

    internal void SetLastResult(HookResult result)
    {
        _lastResult = result;
    }

    [Given("I have a git repository with hooks configured")]
    public async Task GivenIHaveAGitRepositoryWithHooksConfigured()
    {
        _output.WriteLine("Setting up git repository with hooks");
        ToolAvailability.SkipIfGitBashMissing();

        _fixture = new GitRepositoryFixture();

        // Get the source paths from the solution root
        var sourceProjectPath = FindSourceProjectPath();
        var sourceHuskyPath = Path.Combine(sourceProjectPath, ".husky");
        _output.WriteLine("Source project path: {0}", sourceProjectPath);
        _output.WriteLine("Husky path: {0}", sourceHuskyPath);

        await _fixture.InitializeAsync(sourceHuskyPath).ConfigureAwait(false);
        _output.WriteLine("Repository initialized at: {0}", _fixture.RepoPath);

        _hookRunner = new HookRunner(_fixture.RepoPath, _fixture.HuskyPath);

        // Store source project path for later use
        _scenarioContext["SourceProjectPath"] = sourceProjectPath;

        // Share fixture with other step definition classes
        _scenarioContext["Fixture"] = _fixture;
        _scenarioContext["HookRunner"] = _hookRunner;
    }

    [Given("I am on the {string} branch")]
    public async Task GivenIAmOnTheBranch(string branchName)
    {
        _output.WriteLine("Switching to branch: {0}", branchName);
        await Fixture.SwitchToBranchAsync(branchName).ConfigureAwait(false);
    }

    [Given("I am on a {string} branch")]
    public Task GivenIAmOnABranch(string branchName) => GivenIAmOnTheBranch(branchName);

    [Given("I am in detached HEAD state")]
    public async Task GivenIAmInDetachedHeadState()
    {
        _output.WriteLine("Detaching HEAD");
        await Fixture.DetachHeadAsync().ConfigureAwait(false);
    }

    [Given("GPG signing is configured")]
    public async Task GivenGpgSigningIsConfigured()
    {
        _output.WriteLine("Configuring GPG signing");
        await Fixture.ConfigureGpgSigningAsync(enabled: true).ConfigureAwait(false);
    }

    [Given("GPG signing is not configured")]
    public async Task GivenGpgSigningIsNotConfigured()
    {
        _output.WriteLine("Disabling GPG signing");
        await Fixture.ConfigureGpgSigningAsync(enabled: false).ConfigureAwait(false);
    }

    [Given("node_modules directory exists")]
    public void GivenNodeModulesDirectoryExists()
    {
        // Link to real node_modules so npm tools work
        var sourceProjectPath = (string)_scenarioContext["SourceProjectPath"];
        Fixture.LinkNodeModulesAndConfig(sourceProjectPath);
    }

    [Given("node_modules directory does not exist")]
    public void GivenNodeModulesDirectoryDoesNotExist()
    {
        Fixture.RemoveNodeModules();
    }

    [Given("no .sln file exists")]
    public void GivenNoSlnFileExists()
    {
        Fixture.RemoveSolutionFiles();
    }

    [Then("the hook should fail with exit code {int}")]
    public void ThenTheHookShouldFailWithExitCode(int expectedExitCode)
    {
        _output.WriteLine("Verifying exit code: expected={0}, actual={1}", expectedExitCode, LastResult.ExitCode);
        LastResult.ExitCode.Should().Be(expectedExitCode, "Expected hook to fail with exit code {0}", expectedExitCode);
    }

    [Then("the hook should fail")]
    public void ThenTheHookShouldFail()
    {
        _output.WriteLine("Verifying hook failed: exit code={0}", LastResult.ExitCode);
        LastResult.ExitCode.Should().NotBe(0, "Expected hook to fail (exit code != 0)");
    }

    [Then("the hook should succeed")]
    public void ThenTheHookShouldSucceed()
    {
        _output.WriteLine("Verifying hook succeeded: exit code={0}", LastResult.ExitCode);
        if (LastResult.ExitCode != 0)
        {
            _output.WriteLine("Hook output: {0}", LastResult.CombinedOutput);
        }

        LastResult.ExitCode.Should().Be(0, "Expected hook to succeed but got exit code {0}.\nOutput: {1}", LastResult.ExitCode, LastResult.CombinedOutput);
    }

    [Then("the output should contain {string}")]
    public void ThenTheOutputShouldContain(string expected)
    {
        _output.WriteLine("Verifying output contains: {0}", expected);
        LastResult.CombinedOutput.Should().Contain(expected);
    }

    private static string FindSourceProjectPath()
    {
        // Walk up from current directory to find .husky (which is in project root)
        var currentDir = Directory.GetCurrentDirectory();

        while (currentDir is not null)
        {
            var huskyPath = Path.Combine(currentDir, ".husky");
            if (Directory.Exists(huskyPath))
            {
                return currentDir;
            }

            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        // Fallback: Try from assembly location
        var assemblyDir = Path.GetDirectoryName(typeof(CommonHookSteps).Assembly.Location);
        while (assemblyDir is not null)
        {
            var huskyPath = Path.Combine(assemblyDir, ".husky");
            if (Directory.Exists(huskyPath))
            {
                return assemblyDir;
            }

            assemblyDir = Directory.GetParent(assemblyDir)?.FullName;
        }

        throw new InvalidOperationException("Could not find project root with .husky directory");
    }

    public async ValueTask DisposeAsync()
    {
        if (_fixture is not null)
        {
            await _fixture.DisposeAsync().ConfigureAwait(false);
        }
    }
}
