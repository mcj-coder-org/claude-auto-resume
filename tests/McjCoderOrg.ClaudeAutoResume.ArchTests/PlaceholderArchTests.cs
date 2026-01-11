using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace McjCoderOrg.ClaudeAutoResume.ArchTests;

/// <summary>
/// Placeholder architecture test to verify infrastructure works.
/// Will be replaced with real tests in Phase 7.
/// </summary>
public sealed class PlaceholderArchTests
{
    [Fact]
    public void MainAssemblyShouldExist()
    {
        // Arrange
        var assembly = typeof(Program).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .GetTypes();

        // Assert
        result.Should().NotBeEmpty();
    }
}
