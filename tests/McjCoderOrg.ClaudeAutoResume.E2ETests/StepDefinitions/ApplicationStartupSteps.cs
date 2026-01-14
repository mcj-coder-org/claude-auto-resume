using System.Diagnostics;
using System.Globalization;
using McjCoderOrg.ClaudeAutoResume.E2ETests.Helpers;

namespace McjCoderOrg.ClaudeAutoResume.E2ETests.StepDefinitions;

[Binding]
#pragma warning disable CA1822 // SpecFlow step definition methods cannot be static
public sealed class ApplicationStartupSteps : IDisposable
{
    private Process? _process;
    private ProcessResult? _result;

    [Given("the executable exists")]
    public void GivenTheExecutableExists()
    {
        Skip.IfNot(
            ProcessHelper.ExecutableExists(),
            $"Executable not found at {ProcessHelper.GetExecutablePath()}");
    }

    [Given("claude CLI is available")]
    public void GivenClaudeCliIsAvailable()
    {
        Skip.IfNot(
            ProcessHelper.IsClaudeAvailable(),
            "Claude CLI is not available - skipping test");
    }

    [Given("claude CLI is not available")]
    public void GivenClaudeCliIsNotAvailable()
    {
        Skip.If(
            ProcessHelper.IsClaudeAvailable(),
            "Claude CLI is available - this test is for missing claude");
    }

    [When("I run the application with {string}")]
    public async Task WhenIRunTheApplicationWith(string arguments)
    {
        _process = ProcessHelper.CreateProcess(arguments);
        _process.Start();

        var stdout = await _process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var stderr = await _process.StandardError.ReadToEndAsync().ConfigureAwait(false);
        await _process.WaitForExitAsync().ConfigureAwait(false);

        _result = new ProcessResult(_process.ExitCode, stdout, stderr);
    }

    [When("I run the application with no arguments")]
    public async Task WhenIRunTheApplicationWithNoArguments()
    {
        await WhenIRunTheApplicationWith(string.Empty).ConfigureAwait(false);
    }

    [Then("the exit code should be {int}")]
    public void ThenTheExitCodeShouldBe(int expectedExitCode)
    {
        Result.ExitCode.Should().Be(expectedExitCode);
    }

    [Then("the output should contain {string}")]
    public void ThenTheOutputShouldContain(string expected)
    {
        Result.StandardOutput.Should().Contain(expected);
    }

    [Then("the error output should contain {string}")]
    public void ThenTheErrorOutputShouldContain(string expected)
    {
        Result.StandardError.Should().Contain(expected);
    }

    [Then("the combined output should contain {string}")]
    public void ThenTheCombinedOutputShouldContain(string expected)
    {
        Result.CombinedOutput.Should().Contain(expected);
    }

    [Then("the application should keep running for at least {int} seconds")]
    public async Task ThenTheApplicationShouldKeepRunningForAtLeastSeconds(int seconds)
    {
        // For this scenario, we need to start the process differently
        // Don't wait for exit, just check it stays running
        _process = ProcessHelper.CreateProcess(string.Empty);
        _process.Start();

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(seconds));
        var processTask = _process.WaitForExitAsync();

        var completedTask = await Task.WhenAny(timeoutTask, processTask).ConfigureAwait(false);

        if (completedTask == timeoutTask)
        {
            // Process is still running after timeout - success!
            _process.Kill();
            // Set a dummy result for cleanup
            _result = new ProcessResult(0, string.Empty, string.Empty);
        }
        else
        {
            // Process exited unexpectedly
            var stdout = await _process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            var stderr = await _process.StandardError.ReadToEndAsync().ConfigureAwait(false);
            _result = new ProcessResult(_process.ExitCode, stdout, stderr);

            Assert.Fail(string.Format(
                CultureInfo.InvariantCulture,
                "Application exited unexpectedly with code {0}. Output: {1} Error: {2}",
                _process.ExitCode,
                stdout,
                stderr));
        }
    }

    private ProcessResult Result => _result ?? throw new InvalidOperationException("No process result available");

    public void Dispose()
    {
        if (_process is not null)
        {
            if (!_process.HasExited)
            {
                try
                {
                    _process.Kill();
                }
#pragma warning disable CA1031 // Intentional: Best-effort cleanup
                catch
                {
                    // Process may have already exited
                }
#pragma warning restore CA1031
            }

            _process.Dispose();
        }
    }
}
