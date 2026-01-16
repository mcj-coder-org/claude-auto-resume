using System.Globalization;
using System.Runtime.InteropServices;

namespace McjCoderOrg.ClaudeAutoResume.Services;

/// <summary>
/// Production implementation of <see cref="IArgumentParser"/>.
/// </summary>
internal sealed class ArgumentParser : IArgumentParser
{
    private readonly IEnvironmentService _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParser"/> class.
    /// </summary>
    /// <param name="environment">The environment service.</param>
    public ArgumentParser(IEnvironmentService environment)
    {
        _environment = environment;
    }

    /// <inheritdoc/>
    public ParseResult Parse(string[] args)
    {
        var builder = new ParseResultBuilder();
        var i = 0;

        while (i < args.Length)
        {
            var earlyExit = TryParseInfoFlag(args[i]);
            if (earlyExit is not null)
            {
                return earlyExit;
            }

            var result = ParseSingleArgument(args, ref i, builder);
            if (result is not null)
            {
                return result;
            }
        }

        builder.WaitMinutes ??= GetEnvironmentWaitMinutes();
        return builder.Build();
    }

    private static ParseResult? TryParseInfoFlag(string arg)
    {
        if (IsFlag(arg, "--help", "-h"))
        {
            return new ParseResult { ShowHelp = true };
        }

        if (IsFlag(arg, "--version", "-v"))
        {
            return new ParseResult { ShowVersion = true };
        }

        if (IsFlag(arg, "--diagnose"))
        {
            return new ParseResult { ShowDiagnose = true };
        }

        return null;
    }

    private static ParseResult? ParseSingleArgument(string[] args, ref int i, ParseResultBuilder builder)
    {
        var arg = args[i];

        if (TryParseBooleanFlag(arg, ref i, builder))
        {
            return null;
        }

        var promptResult = TryParseStringArg(args, ref i, "--prompt", "-p");
        if (promptResult.Matched)
        {
            if (promptResult.Value is null)
            {
                return new ParseResult { ErrorMessage = "Error: --prompt requires an argument" };
            }

            builder.InitialPrompt = promptResult.Value;
            return null;
        }

        var waitResult = TryParseIntArg(args, ref i, "--wait", "-w");
        if (waitResult.Matched)
        {
            if (waitResult.Value is null)
            {
                return new ParseResult { ErrorMessage = "Error: --wait requires a number of minutes" };
            }

            builder.WaitMinutes = waitResult.Value;
            return null;
        }

        builder.ClaudeArgs.Add(arg);
        i++;
        return null;
    }

    private static bool TryParseBooleanFlag(string arg, ref int i, ParseResultBuilder builder)
    {
        if (IsFlag(arg, "--verbose", "-V"))
        {
            builder.Verbose = true;
            i++;
            return true;
        }

        if (IsFlag(arg, "--headless"))
        {
            builder.Headless = true;
            i++;
            return true;
        }

        if (IsFlag(arg, "--dangerously-skip-permissions") || IsFlag(arg, "--dangerous"))
        {
            builder.Dangerous = true;
            i++;
            return true;
        }

        if (IsFlag(arg, "--continue", "-c"))
        {
            builder.ContinueConversation = true;
            i++;
            return true;
        }

        return false;
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly record struct StringArgResult(bool Matched, string? Value);

    private static StringArgResult TryParseStringArg(string[] args, ref int i, string longForm, string? shortForm)
    {
        if (!IsFlag(args[i], longForm, shortForm))
        {
            return new StringArgResult(Matched: false, Value: null);
        }

        if (i + 1 < args.Length)
        {
            i += 2;
            return new StringArgResult(Matched: true, Value: args[i - 1]);
        }

        i++;
        return new StringArgResult(Matched: true, Value: null);
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly record struct IntArgResult(bool Matched, int? Value);

    private static IntArgResult TryParseIntArg(string[] args, ref int i, string longForm, string? shortForm)
    {
        if (!IsFlag(args[i], longForm, shortForm))
        {
            return new IntArgResult(Matched: false, Value: null);
        }

        if (i + 1 < args.Length && int.TryParse(args[i + 1], CultureInfo.InvariantCulture, out var value))
        {
            i += 2;
            return new IntArgResult(Matched: true, Value: value);
        }

        i++;
        return new IntArgResult(Matched: true, Value: null);
    }

    private int? GetEnvironmentWaitMinutes()
    {
        var envValue = _environment.GetEnvironmentVariable("CLAUDE_WAIT_MINUTES");
        if (int.TryParse(envValue, CultureInfo.InvariantCulture, out var mins))
        {
            return mins;
        }

        return null;
    }

    private static bool IsFlag(string arg, string longForm, string? shortForm = null)
    {
        return string.Equals(arg, longForm, StringComparison.OrdinalIgnoreCase)
            || (shortForm is not null && string.Equals(arg, shortForm, StringComparison.OrdinalIgnoreCase));
    }

    private sealed class ParseResultBuilder
    {
        public bool Verbose { get; set; }
        public bool Headless { get; set; }
        public bool Dangerous { get; set; }
        public bool ContinueConversation { get; set; }
        public string? InitialPrompt { get; set; }
        public int? WaitMinutes { get; set; }
        public List<string> ClaudeArgs { get; } = [];

        public ParseResult Build() => new()
        {
            Verbose = Verbose,
            Headless = Headless,
            Dangerous = Dangerous,
            ContinueConversation = ContinueConversation,
            InitialPrompt = InitialPrompt,
            WaitMinutes = WaitMinutes,
            ClaudeArgs = ClaudeArgs,
        };
    }
}
