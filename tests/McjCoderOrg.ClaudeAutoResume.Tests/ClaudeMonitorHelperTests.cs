using McjCoderOrg.ClaudeAutoResume.Services;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Tests for helper methods in ClaudeMonitor.
/// Covers EscapeForDisplay, FindClaudeInPath, GetEnvironment, and AppendToBuffer.
/// </summary>
public sealed class ClaudeMonitorHelperTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<IConsoleService> _mockConsole;
    private readonly Mock<IEnvironmentService> _mockEnvironment;
    private ClaudeMonitor? _monitor;

    public ClaudeMonitorHelperTests(ITestOutputHelper output)
    {
        _output = output;
        _mockConsole = new Mock<IConsoleService>(MockBehavior.Loose);
        _mockEnvironment = new Mock<IEnvironmentService>(MockBehavior.Loose);

        // Setup default mock behavior
        _mockConsole.Setup(c => c.WindowWidth).Returns(value: 120);
        _mockConsole.Setup(c => c.WindowHeight).Returns(value: 30);
        _mockEnvironment.Setup(e => e.CurrentDirectory).Returns(value: Environment.CurrentDirectory);
        _mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(
            value: new Dictionary<string, string>(StringComparer.Ordinal));
    }

    public void Dispose()
    {
        _monitor?.Dispose();
    }

    private ClaudeMonitor CreateMonitor(WrapperConfig? config = null)
    {
        _monitor = new ClaudeMonitor(
            config ?? WrapperConfig.Default,
            _mockConsole.Object,
            _mockEnvironment.Object);
        return _monitor;
    }

    // EscapeForDisplay tests

    [Fact]
    public void EscapeForDisplay_NewlineCharacter_ReturnsBackslashN()
    {
        var result = ClaudeMonitor.EscapeForDisplay("hello\nworld");

        _output.WriteLine("Result: {0}", result);
        result.Should().Be("hello\\nworld");
    }

    [Fact]
    public void EscapeForDisplay_CarriageReturn_ReturnsBackslashR()
    {
        var result = ClaudeMonitor.EscapeForDisplay("hello\rworld");

        _output.WriteLine("Result: {0}", result);
        result.Should().Be("hello\\rworld");
    }

    [Fact]
    public void EscapeForDisplay_MixedEscapes_ReplacesAll()
    {
        var result = ClaudeMonitor.EscapeForDisplay("line1\r\nline2\nline3");

        _output.WriteLine("Result: {0}", result);
        result.Should().Be("line1\\r\\nline2\\nline3");
    }

    [Fact]
    public void EscapeForDisplay_NoEscapes_ReturnsOriginal()
    {
        var result = ClaudeMonitor.EscapeForDisplay("hello world");

        _output.WriteLine("Result: {0}", result);
        result.Should().Be("hello world");
    }

    [Fact]
    public void EscapeForDisplay_EmptyString_ReturnsEmpty()
    {
        var result = ClaudeMonitor.EscapeForDisplay(string.Empty);

        _output.WriteLine("Result: [{0}]", result);
        result.Should().BeEmpty();
    }

    // FindClaudeInPath tests

    [Fact]
    public void FindClaudeInPath_InSystemPath_ReturnsPath()
    {
        // Unix uses : as path separator
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(value: "/usr/bin:/usr/local/bin");
        _mockEnvironment.Setup(e => e.IsWindows).Returns(value: false);
        _mockEnvironment.Setup(e => e.UserProfile).Returns(value: "/home/user");
        // Use Path.Combine to match what the code generates on any platform
        var claudePath = Path.Combine("/usr/local/bin", "claude");
        _mockEnvironment.Setup(e => e.FileExists(claudePath)).Returns(value: true);
        var monitor = CreateMonitor();

        var result = monitor.FindClaudeInPath();

        _output.WriteLine("Result: {0}", result);
        result.Should().Be(claudePath);
    }

    [Fact]
    public void FindClaudeInPath_Windows_ChecksClaudeCmd()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(value: "C:\\Windows;C:\\npm");
        _mockEnvironment.Setup(e => e.IsWindows).Returns(value: true);
        _mockEnvironment.Setup(e => e.UserProfile).Returns(value: "C:\\Users\\test");
        // Use Path.Combine to match what the code generates on any platform
        var claudeCmdPath = Path.Combine("C:\\npm", "claude.cmd");
        _mockEnvironment.Setup(e => e.FileExists(claudeCmdPath)).Returns(value: true);
        var monitor = CreateMonitor();

        var result = monitor.FindClaudeInPath();

        _output.WriteLine("Result: {0}", result);
        result.Should().Be(claudeCmdPath);
    }

    [Fact]
    public void FindClaudeInPath_Windows_ChecksClaudeExe()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(value: "C:\\Windows;C:\\npm");
        _mockEnvironment.Setup(e => e.IsWindows).Returns(value: true);
        _mockEnvironment.Setup(e => e.UserProfile).Returns(value: "C:\\Users\\test");
        // Use Path.Combine to match what the code generates on any platform
        var claudeCmdPath = Path.Combine("C:\\npm", "claude.cmd");
        var claudeExePath = Path.Combine("C:\\npm", "claude.exe");
        _mockEnvironment.Setup(e => e.FileExists(claudeCmdPath)).Returns(value: false);
        _mockEnvironment.Setup(e => e.FileExists(claudeExePath)).Returns(value: true);
        var monitor = CreateMonitor();

        var result = monitor.FindClaudeInPath();

        _output.WriteLine("Result: {0}", result);
        result.Should().Be(claudeExePath);
    }

    [Fact]
    public void FindClaudeInPath_NotInPath_ChecksNpmGlobal()
    {
        // Unix uses : as path separator
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(value: "/usr/bin");
        _mockEnvironment.Setup(e => e.IsWindows).Returns(value: false);
        _mockEnvironment.Setup(e => e.UserProfile).Returns(value: "/home/user");
        // First the PATH check returns false - use Path.Combine to match what the code generates
        _mockEnvironment.Setup(e => e.FileExists(Path.Combine("/usr/bin", "claude"))).Returns(value: false);
        // Then npm-global check returns true - Path.Combine generates platform-specific paths
        var npmGlobalPath = Path.Combine("/home/user", ".npm-global", "bin", "claude");
        _mockEnvironment.Setup(e => e.FileExists(npmGlobalPath)).Returns(value: true);
        var monitor = CreateMonitor();

        var result = monitor.FindClaudeInPath();

        _output.WriteLine("Result: {0}", result ?? "null");
        result.Should().Be(npmGlobalPath);
    }

    [Fact]
    public void FindClaudeInPath_NotFound_ReturnsNull()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(value: "/usr/bin");
        _mockEnvironment.Setup(e => e.IsWindows).Returns(value: false);
        _mockEnvironment.Setup(e => e.UserProfile).Returns(value: "/home/user");
        _mockEnvironment.Setup(e => e.FileExists(It.IsAny<string>())).Returns(value: false);
        var monitor = CreateMonitor();

        var result = monitor.FindClaudeInPath();

        _output.WriteLine("Result: {0}", result ?? "null");
        result.Should().BeNull();
    }

    [Fact]
    public void FindClaudeInPath_EmptyPath_ReturnsNull()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(value: string.Empty);
        _mockEnvironment.Setup(e => e.IsWindows).Returns(value: false);
        _mockEnvironment.Setup(e => e.UserProfile).Returns(value: "/home/user");
        _mockEnvironment.Setup(e => e.FileExists(It.IsAny<string>())).Returns(value: false);
        var monitor = CreateMonitor();

        var result = monitor.FindClaudeInPath();

        _output.WriteLine("Result: {0}", result ?? "null");
        result.Should().BeNull();
    }

    // GetEnvironment tests

    [Fact]
    public void GetEnvironment_NoTerm_SetsXterm256Color()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(
            value: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["PATH"] = "/usr/bin",
            });
        var monitor = CreateMonitor();

        var result = monitor.GetEnvironment();

        _output.WriteLine("TERM: {0}", result["TERM"]);
        result["TERM"].Should().Be("xterm-256color");
    }

    [Fact]
    public void GetEnvironment_ExistingTerm_PreservesValue()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(
            value: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["TERM"] = "vt100",
            });
        var monitor = CreateMonitor();

        var result = monitor.GetEnvironment();

        _output.WriteLine("TERM: {0}", result["TERM"]);
        result["TERM"].Should().Be("vt100");
    }

    [Fact]
    public void GetEnvironment_EmptyTerm_SetsXterm256Color()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(
            value: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["TERM"] = string.Empty,
            });
        var monitor = CreateMonitor();

        var result = monitor.GetEnvironment();

        _output.WriteLine("TERM: {0}", result["TERM"]);
        result["TERM"].Should().Be("xterm-256color");
    }

    [Fact]
    public void GetEnvironment_CopiesAllEnvironmentVariables()
    {
        _mockEnvironment.Setup(e => e.GetEnvironmentVariables()).Returns(
            value: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["PATH"] = "/usr/bin",
                ["HOME"] = "/home/user",
                ["CUSTOM_VAR"] = "custom_value",
            });
        var monitor = CreateMonitor();

        var result = monitor.GetEnvironment();

        _output.WriteLine("Environment keys: {0}", string.Join(", ", result.Keys));
        result.Should().ContainKey("PATH");
        result.Should().ContainKey("HOME");
        result.Should().ContainKey("CUSTOM_VAR");
        result["PATH"].Should().Be("/usr/bin");
        result["HOME"].Should().Be("/home/user");
        result["CUSTOM_VAR"].Should().Be("custom_value");
    }

    // AppendToBuffer tests

    [Fact]
    public void AppendToBuffer_UnderLimit_AppendsAll()
    {
        var config = WrapperConfig.Default with { OutputBufferSize = 1000 };
        var monitor = CreateMonitor(config);

        monitor.AppendToBuffer("Hello ");
        monitor.AppendToBuffer("World");

        _output.WriteLine("Buffer contents: {0}", monitor.OutputBufferContents);
        monitor.OutputBufferContents.Should().Be("Hello World");
    }

    [Fact]
    public void AppendToBuffer_OverLimit_TruncatesOldContent()
    {
        var config = WrapperConfig.Default with { OutputBufferSize = 10 };
        var monitor = CreateMonitor(config);

        monitor.AppendToBuffer("12345678901234567890"); // 20 chars, exceeds buffer of 10

        _output.WriteLine("Buffer contents: {0}", monitor.OutputBufferContents);
        monitor.OutputBufferContents.Should().HaveLength(10);
        // Should keep the last 10 characters
        monitor.OutputBufferContents.Should().Be("1234567890");
    }

    [Fact]
    public void AppendToBuffer_ExactLimit_NoTruncation()
    {
        var config = WrapperConfig.Default with { OutputBufferSize = 10 };
        var monitor = CreateMonitor(config);

        monitor.AppendToBuffer("1234567890"); // Exactly 10 chars

        _output.WriteLine("Buffer contents: {0}", monitor.OutputBufferContents);
        monitor.OutputBufferContents.Should().Be("1234567890");
    }

    [Fact]
    public void AppendToBuffer_MultipleAppends_TruncatesCorrectly()
    {
        var config = WrapperConfig.Default with { OutputBufferSize = 10 };
        var monitor = CreateMonitor(config);

        monitor.AppendToBuffer("aaaaa"); // 5 chars
        monitor.AppendToBuffer("bbbbb"); // 5 more = 10 total
        monitor.AppendToBuffer("ccccc"); // 5 more = 15 total, truncate to 10

        _output.WriteLine("Buffer contents: {0}", monitor.OutputBufferContents);
        monitor.OutputBufferContents.Should().HaveLength(10);
        // Should keep the last 10 characters
        // cspell:disable-next-line
        monitor.OutputBufferContents.Should().Be("bbbbbccccc");
    }
}
