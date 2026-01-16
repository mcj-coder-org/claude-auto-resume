using System.Text;

namespace McjCoderOrg.ClaudeAutoResume;

/// <summary>
/// Tests for the ConvertKeyToBytes method in ClaudeMonitor.
/// Verifies correct byte sequences for all supported key types.
/// </summary>
public sealed class ClaudeMonitorKeyConversionTests
{
    private readonly ITestOutputHelper _output;

    public ClaudeMonitorKeyConversionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // Basic keys

    [Fact]
    public void ConvertKeyToBytes_Enter_ReturnsCarriageReturn()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\r',
            key: ConsoleKey.Enter,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Enter key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x0D }); // \r
    }

    [Fact]
    public void ConvertKeyToBytes_Tab_ReturnsTabCharacter()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\t',
            key: ConsoleKey.Tab,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Tab key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x09 }); // \t
    }

    [Fact]
    public void ConvertKeyToBytes_Backspace_Returns0x7F()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\b',
            key: ConsoleKey.Backspace,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Backspace key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x7F });
    }

    [Fact]
    public void ConvertKeyToBytes_Escape_Returns0x1B()
    {
        var key = new ConsoleKeyInfo(
            keyChar: (char)0x1B,
            key: ConsoleKey.Escape,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Escape key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B });
    }

    // Arrow keys

    [Fact]
    public void ConvertKeyToBytes_UpArrow_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.UpArrow,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("UpArrow key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'A' }); // ESC[A
    }

    [Fact]
    public void ConvertKeyToBytes_DownArrow_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.DownArrow,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("DownArrow key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'B' }); // ESC[B
    }

    [Fact]
    public void ConvertKeyToBytes_RightArrow_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.RightArrow,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("RightArrow key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'C' }); // ESC[C
    }

    [Fact]
    public void ConvertKeyToBytes_LeftArrow_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.LeftArrow,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("LeftArrow key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'D' }); // ESC[D
    }

    // Navigation keys

    [Fact]
    public void ConvertKeyToBytes_Home_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.Home,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Home key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'H' }); // ESC[H
    }

    [Fact]
    public void ConvertKeyToBytes_End_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.End,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("End key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'F' }); // ESC[F
    }

    [Fact]
    public void ConvertKeyToBytes_Delete_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.Delete,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Delete key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'3', (byte)'~' }); // ESC[3~
    }

    [Fact]
    public void ConvertKeyToBytes_PageUp_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.PageUp,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("PageUp key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'5', (byte)'~' }); // ESC[5~
    }

    [Fact]
    public void ConvertKeyToBytes_PageDown_ReturnsEscapeSequence()
    {
        var key = new ConsoleKeyInfo(
            keyChar: '\0',
            key: ConsoleKey.PageDown,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("PageDown key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x1B, (byte)'[', (byte)'6', (byte)'~' }); // ESC[6~
    }

    // Ctrl combinations

    [Fact]
    public void ConvertKeyToBytes_CtrlC_Returns0x03()
    {
        var key = new ConsoleKeyInfo(
            keyChar: (char)0x03,
            key: ConsoleKey.C,
            shift: false,
            alt: false,
            control: true);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Ctrl+C key bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { 0x03 }); // ETX (End of Text)
    }

    [Theory]
    [InlineData(ConsoleKey.A, 0x01)]
    [InlineData(ConsoleKey.B, 0x02)]
    [InlineData(ConsoleKey.C, 0x03)]
    [InlineData(ConsoleKey.D, 0x04)]
    [InlineData(ConsoleKey.E, 0x05)]
    [InlineData(ConsoleKey.F, 0x06)]
    [InlineData(ConsoleKey.G, 0x07)]
    [InlineData(ConsoleKey.H, 0x08)]
    [InlineData(ConsoleKey.I, 0x09)]
    [InlineData(ConsoleKey.J, 0x0A)]
    [InlineData(ConsoleKey.K, 0x0B)]
    [InlineData(ConsoleKey.L, 0x0C)]
    [InlineData(ConsoleKey.M, 0x0D)]
    [InlineData(ConsoleKey.N, 0x0E)]
    [InlineData(ConsoleKey.O, 0x0F)]
    [InlineData(ConsoleKey.P, 0x10)]
    [InlineData(ConsoleKey.Q, 0x11)]
    [InlineData(ConsoleKey.R, 0x12)]
    [InlineData(ConsoleKey.S, 0x13)]
    [InlineData(ConsoleKey.T, 0x14)]
    [InlineData(ConsoleKey.U, 0x15)]
    [InlineData(ConsoleKey.V, 0x16)]
    [InlineData(ConsoleKey.W, 0x17)]
    [InlineData(ConsoleKey.X, 0x18)]
    [InlineData(ConsoleKey.Y, 0x19)]
    [InlineData(ConsoleKey.Z, 0x1A)]
    public void ConvertKeyToBytes_CtrlLetters_ReturnsControlCodes(ConsoleKey key, byte expected)
    {
        var keyInfo = new ConsoleKeyInfo(
            keyChar: (char)expected,
            key: key,
            shift: false,
            alt: false,
            control: true);

        var result = ClaudeMonitor.ConvertKeyToBytes(keyInfo);

        _output.WriteLine("Ctrl+{0} key bytes: [{1}]", key, string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(new byte[] { expected });
    }

    // Regular characters

    [Theory]
    [InlineData('a', "a")]
    [InlineData('z', "z")]
    [InlineData('A', "A")]
    [InlineData('Z', "Z")]
    [InlineData('0', "0")]
    [InlineData('9', "9")]
    [InlineData(' ', " ")]
    [InlineData('!', "!")]
    [InlineData('@', "@")]
    [InlineData('#', "#")]
    public void ConvertKeyToBytes_PrintableCharacters_ReturnsUtf8Bytes(char c, string expected)
    {
        // Use a generic key that won't match special cases
        var key = new ConsoleKeyInfo(
            keyChar: c,
            key: ConsoleKey.NoName,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Character '{0}' bytes: [{1}]", c, string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(expected));
    }

    [Fact]
    public void ConvertKeyToBytes_UnicodeCharacter_ReturnsCorrectUtf8()
    {
        // Test with a multi-byte UTF-8 character (e.g., é = U+00E9)
        var key = new ConsoleKeyInfo(
            keyChar: 'é',
            key: ConsoleKey.NoName,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Unicode 'é' bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("é"));
        result.Should().HaveCount(2); // é is 2 bytes in UTF-8
    }

    [Fact]
    public void ConvertKeyToBytes_EmojiCharacter_ReturnsCorrectUtf8()
    {
        // Test with a 4-byte UTF-8 character
        var key = new ConsoleKeyInfo(
            keyChar: '→',
            key: ConsoleKey.NoName,
            shift: false,
            alt: false,
            control: false);

        var result = ClaudeMonitor.ConvertKeyToBytes(key);

        _output.WriteLine("Unicode '→' bytes: [{0}]", string.Join(", ", result.Select(b => $"0x{b:X2}")));
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("→"));
    }
}
