using BenchmarkDotNet.Attributes;

namespace McjCoderOrg.ClaudeAutoResume.Benchmarks;

/// <summary>
/// Placeholder benchmark to verify infrastructure works.
/// Will be replaced with real benchmarks in Phase 7.
/// </summary>
[MemoryDiagnoser]
public class PlaceholderBenchmarks
{
    [Benchmark]
    public int Placeholder()
    {
        return 1 + 1;
    }
}
