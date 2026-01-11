---
sidebar_position: 3
---

# Testing

Guidelines for writing and running tests.

## Test Projects

| Project                                    | Description        |
| ------------------------------------------ | ------------------ |
| `McjCoderOrg.ClaudeAutoResume.Tests`       | Unit tests         |
| `McjCoderOrg.ClaudeAutoResume.ArchTests`   | Architecture tests |
| `McjCoderOrg.ClaudeAutoResume.E2ETests`    | End-to-end tests   |
| `McjCoderOrg.ClaudeAutoResume.SystemTests` | System tests       |

## Running Tests

Run all tests:

```bash
dotnet test
```

Run specific test project:

```bash
dotnet test tests/McjCoderOrg.ClaudeAutoResume.Tests
```

Run with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Writing Tests

We use xUnit as the testing framework:

```csharp
public class CalculatorTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }
}
```

## Test-Driven Development

Follow the TDD cycle:

1. **Red**: Write a failing test
2. **Green**: Write minimal code to pass
3. **Refactor**: Improve code quality
