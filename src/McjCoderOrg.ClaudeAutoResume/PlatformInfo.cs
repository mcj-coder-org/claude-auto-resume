using System.Reflection;
using System.Runtime.InteropServices;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Captures platform context for diagnostics.
/// </summary>
/// <remarks>
/// See ADR-0017 for observability design decisions.
/// </remarks>
internal sealed class PlatformInfo
{
    private static readonly Lazy<PlatformInfo> LazyInstance = new(Create);

    private PlatformInfo()
    {
    }

    /// <summary>
    /// Gets the current platform information.
    /// </summary>
    public static PlatformInfo Current => LazyInstance.Value;

    /// <summary>Gets the .NET runtime version.</summary>
    public string DotNetVersion { get; private init; } = string.Empty;

    /// <summary>Gets the runtime identifier (e.g., win-x64, linux-arm64).</summary>
    public string RuntimeIdentifier { get; private init; } = string.Empty;

    /// <summary>Gets the OS description.</summary>
    public string OsDescription { get; private init; } = string.Empty;

    /// <summary>Gets the process architecture.</summary>
    public string ProcessArchitecture { get; private init; } = string.Empty;

    /// <summary>Gets the application version.</summary>
    public string AppVersion { get; private init; } = string.Empty;

    /// <summary>Gets a value indicating whether running in a container.</summary>
    public bool IsContainer { get; private init; }

    /// <summary>Gets a value indicating whether running in a CI environment.</summary>
    public bool IsCI { get; private init; }

    private static PlatformInfo Create()
    {
        return new PlatformInfo
        {
            DotNetVersion = Environment.Version.ToString(),
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
            OsDescription = RuntimeInformation.OSDescription,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            AppVersion = GetAppVersion(),
            IsContainer = DetectContainer(),
            IsCI = DetectCI(),
        };
    }

    private static string GetAppVersion()
    {
        return Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0";
    }

    private static bool DetectContainer()
    {
        // Check for .dockerenv file (Linux) or DOTNET_RUNNING_IN_CONTAINER env var
        return File.Exists("/.dockerenv")
            || string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.Ordinal);
    }

    private static bool DetectCI()
    {
        // Check common CI environment variables
        return string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.Ordinal)
            || string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.Ordinal)
            || string.Equals(Environment.GetEnvironmentVariable("TF_BUILD"), "True", StringComparison.Ordinal)
            || Environment.GetEnvironmentVariable("JENKINS_URL") is not null;
    }
}
