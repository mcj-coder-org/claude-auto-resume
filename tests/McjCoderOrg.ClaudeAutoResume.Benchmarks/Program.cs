using BenchmarkDotNet.Running;

namespace McjCoderOrg.ClaudeAutoResume.Benchmarks;

/// <summary>
/// Entry point for running benchmarks.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
