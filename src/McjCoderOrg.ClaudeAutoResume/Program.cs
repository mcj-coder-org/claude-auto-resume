namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Minimal entry point for CI workflow validation.
/// This will be replaced with the full implementation in Phase 4.
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--version")
        {
            Console.WriteLine("claude-auto-resume 0.0.0-dev");
            return 0;
        }

        Console.WriteLine("Claude Auto Resume - Development Build");
        Console.WriteLine("This is a placeholder for CI workflow validation.");
        return 0;
    }
}
