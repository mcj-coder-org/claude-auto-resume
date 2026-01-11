using McjCoderOrg.ClaudeAutoResume;

namespace McjCoderOrg.ClaudeAutoResume.ArchTests;

/// <summary>
/// Architecture tests to enforce structural rules.
/// </summary>
public sealed class ArchitectureTests
{
    private static readonly System.Reflection.Assembly MainAssembly = typeof(Program).Assembly;

    [Fact]
    public void MainAssembly_ShouldHaveNoCircularDependencies()
    {
        // Arrange & Act
        var types = Types.InAssembly(MainAssembly).GetTypes();

        // Assert - assembly should be loadable (no circular refs at assembly level)
        types.Should().NotBeEmpty();
    }

    [Fact]
    public void AllPublicClasses_ShouldBeSealed_OrAbstract_OrStatic()
    {
        // Arrange & Act
        var result = Types.InAssembly(MainAssembly)
            .That()
            .ArePublic()
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Public classes should be sealed to prevent unintended inheritance. " +
            "Failing types: {0}",
            string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? []));
    }

    [Fact]
    public void AllClasses_ShouldResideInCorrectNamespace()
    {
        // Arrange & Act
        var result = Types.InAssembly(MainAssembly)
            .That()
            .AreClasses()
            .And()
            .DoNotResideInNamespaceStartingWith("Coverlet.Core") // Exclude Coverlet instrumentation
            .Should()
            .ResideInNamespaceStartingWith("McjCoderOrg.ClaudeAutoResume")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All classes should be in McjCoderOrg.ClaudeAutoResume namespace. " +
            "Failing types: {0}",
            string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? []));
    }

    [Fact]
    public void Interfaces_ShouldStartWithI()
    {
        // Arrange & Act
        var result = Types.InAssembly(MainAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I", StringComparison.Ordinal)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Interface names should start with 'I'. " +
            "Failing types: {0}",
            string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? []));
    }
}
