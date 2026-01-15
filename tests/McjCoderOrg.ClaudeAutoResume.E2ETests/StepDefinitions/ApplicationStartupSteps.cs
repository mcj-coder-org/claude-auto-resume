using System.Diagnostics;
using System.Globalization;
using McjCoderOrg.ClaudeAutoResume.E2ETests.Helpers;

namespace McjCoderOrg.ClaudeAutoResume.E2ETests.StepDefinitions;

[Binding]
#pragma warning disable CA1822 // SpecFlow step definition methods cannot be static
public sealed class ApplicationStartupSteps : IDisposable
{
    private readonly IReqnrollOutputHelper _output;
    private Process? _process;
    private ProcessResult? _result;

    public ApplicationStartupSteps(IReqnrollOutputHelper output)
    {
        _output = output;
    }

    [Given("the executable exists")]
    public void GivenTheExecutableExists()
    {
        var execPath = ProcessHelper.GetExecutablePath();
        _output.WriteLine("Checking executable at: {0}", execPath);
        Skip.IfNot(
            ProcessHelper.ExecutableExists(),
            $"Executable not found at {execPath}");
        _output.WriteLine("Executable found");
    }

    [Given("claude CLI is available")]
    public void GivenClaudeCliIsAvailable()
    {
        _output.WriteLine("Checking Claude CLI availability");
        var isAvailable = ProcessHelper.IsClaudeAvailable();
        _output.WriteLine("Claude CLI available: {0}", isAvailable);
        Skip.IfNot(
            isAvailable,
            "Claude CLI is not available - skipping test");
    }

    [Given("claude CLI is not available")]
    public void GivenClaudeCliIsNotAvailable()
    {
        _output.WriteLine("Checking Claude CLI is NOT available");
        var isAvailable = ProcessHelper.IsClaudeAvailable();
        _output.WriteLine("Claude CLI available: {0}", isAvailable);
        Skip.If(
            isAvailable,
            "Claude CLI is available - this test is for missing claude");
    }

    [When("I run the application with {string}")]
    public async Task WhenIRunTheApplicationWith(string arguments)
    {
        _output.WriteLine("Running application with arguments: {0}", arguments);
        _process = ProcessHelper.CreateProcess(arguments);
        _process.Start();

        var stdout = await _process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var stderr = await _process.StandardError.ReadToEndAsync().ConfigureAwait(false);
        await _process.WaitForExitAsync().ConfigureAwait(false);

        _result = new ProcessResult(_process.ExitCode, stdout, stderr);
        _output.WriteLine("Exit code: {0}", _result.ExitCode);
        if (!string.IsNullOrEmpty(_result.StandardOutput))
        {
            _output.WriteLine("Stdout: {0}", _result.StandardOutput);
        }

        if (!string.IsNullOrEmpty(_result.StandardError))
        {
            _output.WriteLine("Stderr: {0}", _result.StandardError);
        }
    }

    [When("I run the application with no arguments")]
    public async Task WhenIRunTheApplicationWithNoArguments()
    {
        await WhenIRunTheApplicationWith(string.Empty).ConfigureAwait(false);
    }

    [Then("the exit code should be {int}")]
    public void ThenTheExitCodeShouldBe(int expectedExitCode)
    {
        _output.WriteLine("Verifying exit code: expected={0}, actual={1}", expectedExitCode, Result.ExitCode);
        Result.ExitCode.Should().Be(expectedExitCode);
    }

    [Then("the output should contain {string}")]
    public void ThenTheOutputShouldContain(string expected)
    {
        _output.WriteLine("Verifying stdout contains: {0}", expected);
        Result.StandardOutput.Should().Contain(expected);
    }

    [Then("the error output should contain {string}")]
    public void ThenTheErrorOutputShouldContain(string expected)
    {
        _output.WriteLine("Verifying stderr contains: {0}", expected);
        Result.StandardError.Should().Contain(expected);
    }

    [Then("the combined output should contain {string}")]
    public void ThenTheCombinedOutputShouldContain(string expected)
    {
        _output.WriteLine("Verifying combined output contains: {0}", expected);
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

    [When("I start the application interactively")]
    public void WhenIStartTheApplicationInteractively()
    {
        _process = ProcessHelper.CreateProcess(string.Empty);
        _process.Start();

        // Start reading stderr in background to prevent buffer from filling up
        _ = Task.Run(async () =>
        {
            var buffer = new char[4096];
            while (_process is not null && !_process.HasExited)
            {
                var bytesRead = await _process.StandardError.ReadAsync(buffer.AsMemory()).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    break;
                }
            }
        });
    }

    [When("I wait for the application to be ready")]
    public async Task WhenIWaitForTheApplicationToBeReady()
    {
        if (_process is null)
        {
            throw new InvalidOperationException("Process not started");
        }

        // Wait for Claude to initialize - look for the prompt indicator
        // Claude Code shows a prompt like "❯" when ready
        var output = new System.Text.StringBuilder();
        var timeout = TimeSpan.FromSeconds(60);
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            // Start reading output in background
            var readTask = ReadOutputUntilReadyAsync(_process, output, cts.Token);
            await readTask.ConfigureAwait(false);

            // Give Claude time to fully initialize its input handling
            // Claude takes a few seconds after showing the banner to be ready for input
            await Task.Delay(3000).ConfigureAwait(false);

            // Start background stdout reader to prevent buffer from filling up
            StartBackgroundStdoutReader(_process);
        }
        catch (OperationCanceledException)
        {
            Assert.Fail(string.Format(
                CultureInfo.InvariantCulture,
                "Timeout waiting for application to be ready. Output so far: {0}",
                output.ToString()));
        }
    }

    private static void StartBackgroundStdoutReader(Process process)
    {
        // Claude writes a lot of output and will block if no one is reading
        // This background reader ensures the stdout buffer doesn't fill up
        _ = Task.Run(async () =>
        {
            var buffer = new char[8192];
            while (!process.HasExited)
            {
                try
                {
                    var bytesRead = await process.StandardOutput.ReadAsync(buffer.AsMemory()).ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        });
    }

    private static async Task ReadOutputUntilReadyAsync(Process process, System.Text.StringBuilder output, CancellationToken ct)
    {
        var buffer = new char[4096];

        while (!ct.IsCancellationRequested)
        {
            var bytesRead = await process.StandardOutput.ReadAsync(buffer.AsMemory(), ct).ConfigureAwait(false);

            if (bytesRead > 0)
            {
                output.Append(buffer, 0, bytesRead);
                var outputStr = output.ToString();

                // Check for Claude prompt indicator (❯) or other ready signals
                // Also check for "Claude Code" banner which indicates Claude started
                if (outputStr.Contains('❯', StringComparison.Ordinal) ||
                    outputStr.Contains("Claude Code", StringComparison.Ordinal))
                {
                    return;
                }
            }

            // Check if process exited unexpectedly
            if (process.HasExited)
            {
                var stderr = await process.StandardError.ReadToEndAsync(ct).ConfigureAwait(false);
                Assert.Fail(string.Format(
                    CultureInfo.InvariantCulture,
                    "Application exited unexpectedly during startup with code {0}. Output: {1} Error: {2}",
                    process.ExitCode,
                    output.ToString(),
                    stderr));
            }
        }
    }

    [When("I send {string} to the application")]
    public async Task WhenISendToTheApplication(string input)
    {
        if (_process is null)
        {
            throw new InvalidOperationException("Process not started");
        }

        await _process.StandardInput.WriteLineAsync(input).ConfigureAwait(false);
        await _process.StandardInput.FlushAsync().ConfigureAwait(false);

        // Don't close stdin - let the wrapper keep waiting for input
        // Claude should process the command and exit on its own
    }

    [Given("bash is available")]
    public void GivenBashIsAvailable()
    {
        var bashPath = ProcessHelper.GetBashPath();
        _output.WriteLine("Checking bash availability: {0}", bashPath ?? "(not found)");
        Skip.IfNot(
            bashPath is not null,
            "Bash not found - Git Bash is required on Windows");
    }

    [When("I run the application via shell with {string} piped after {int} seconds")]
    public async Task WhenIRunTheApplicationViaShellWithPipedInput(string input, int delaySeconds)
    {
        _output.WriteLine("Running application via shell with input '{0}' piped after {1} seconds", input, delaySeconds);
        // Use a generous timeout: delay + time for claude to start + time to process exit
        var timeoutSeconds = delaySeconds + 60;
        _result = await ProcessHelper.RunViaShellWithPipedInputAsync(input, delaySeconds, timeoutSeconds)
            .ConfigureAwait(false);
        _output.WriteLine("Shell execution completed with exit code: {0}", _result.ExitCode);
    }

    [Then("the application should exit within {int} seconds")]
    public async Task ThenTheApplicationShouldExitWithinSeconds(int seconds)
    {
        if (_process is null)
        {
            throw new InvalidOperationException("Process not started");
        }

        var exitTask = _process.WaitForExitAsync();
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(seconds));

        var completedTask = await Task.WhenAny(exitTask, timeoutTask).ConfigureAwait(false);

        if (completedTask == timeoutTask)
        {
            _process.Kill();
            Assert.Fail(string.Format(
                CultureInfo.InvariantCulture,
                "Application did not exit within {0} seconds",
                seconds));
        }

        var stdout = await _process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var stderr = await _process.StandardError.ReadToEndAsync().ConfigureAwait(false);
        _result = new ProcessResult(_process.ExitCode, stdout, stderr);
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
