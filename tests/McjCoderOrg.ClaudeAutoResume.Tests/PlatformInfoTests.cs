using System.Runtime.InteropServices;

namespace McjCoderOrg.ClaudeAutoResume;

public sealed class PlatformInfoTests
{
    private static readonly bool[] _validBooleans = [true, false];

    [Fact]
    public void Current_ShouldReturnNonNullInstance()
    {
        var info = PlatformInfo.Current;

        info.Should().NotBeNull();
    }

    [Fact]
    public void DotNetVersion_ShouldMatchRuntimeVersion()
    {
        var info = PlatformInfo.Current;

        info.DotNetVersion.Should().Be(Environment.Version.ToString());
    }

    [Fact]
    public void RuntimeIdentifier_ShouldNotBeEmpty()
    {
        var info = PlatformInfo.Current;

        info.RuntimeIdentifier.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void OsDescription_ShouldMatchSystem()
    {
        var info = PlatformInfo.Current;

        info.OsDescription.Should().Be(RuntimeInformation.OSDescription);
    }

    [Fact]
    public void ProcessArchitecture_ShouldMatchSystem()
    {
        var info = PlatformInfo.Current;

        info.ProcessArchitecture.Should().Be(RuntimeInformation.ProcessArchitecture.ToString());
    }

    [Fact]
    public void AppVersion_ShouldNotBeEmpty()
    {
        var info = PlatformInfo.Current;

        info.AppVersion.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void IsContainer_ShouldReturnBoolean()
    {
        var info = PlatformInfo.Current;

        // Verify boolean property is accessible (value depends on environment)
        _validBooleans.Should().Contain(info.IsContainer);
    }

    [Fact]
    public void IsCI_ShouldReturnBoolean()
    {
        var info = PlatformInfo.Current;

        // Verify boolean property is accessible (value depends on environment)
        _validBooleans.Should().Contain(info.IsCI);
    }
}
