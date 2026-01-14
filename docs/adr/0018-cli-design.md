---
name: cli-design
description: |
  When designing command-line interface or configuration. Apply when implementing CLI arguments,
  layered configuration, exit codes, or diagnostics commands.
decision: Use layered configuration with semantic exit codes and built-in diagnostics command.
status: accepted
type: implementation
implementation_issue: '#39'
---

# ADR-0018: CLI Design

## Status

Proposed

## Date

2026-01-09

## Context

We need CLI design patterns for:

1. User-friendly command interface
2. Layered configuration
3. Semantic exit codes
4. Diagnostics for troubleshooting

### Requirements

- Compatible as .NET global tool
- Layered configuration (CLI > env > config files > defaults)
- Meaningful exit codes
- `--help` and `--version` support
- Diagnostics command

## Decision

### Command Structure

```text
claude-auto-resume [options] [-- <claude-args>...]

Options:
  -c, --config <path>       Path to configuration file
  -v, --verbose             Enable verbose logging to file
  --diagnose                Run environment diagnostics
  --version                 Show version information
  -h, --help                Show help
```

### Layered Configuration

Priority (highest to lowest):

1. CLI arguments
2. Environment variables (`CLAUDE_AUTO_RESUME_*`)
3. Project config (`.claude-auto-resume.json`)
4. User config (`~/.config/claude-auto-resume/config.json`)
5. Defaults

### Semantic Exit Codes

| Code | Name               | Description               |
| ---- | ------------------ | ------------------------- |
| 0    | Success            | Normal completion         |
| 1    | GeneralError       | Unhandled exception       |
| 2    | ConfigurationError | Invalid config            |
| 3    | DependencyMissing  | Claude CLI not found      |
| 4    | RateLimitDetected  | Exited due to rate limit  |
| 5    | UserCancelled      | User interrupted (Ctrl+C) |

### Diagnostics Command

`--diagnose` outputs:

- Runtime environment
- Dependencies (Claude CLI)
- Configuration validity
- Permissions check
- JSON blob for issue reports

### Log Locations

| Platform | Path                                      |
| -------- | ----------------------------------------- |
| Windows  | `%LOCALAPPDATA%\claude-auto-resume\logs\` |
| macOS    | `~/Library/Logs/claude-auto-resume/`      |
| Linux    | `~/.local/share/claude-auto-resume/logs/` |

## Consequences

### Positive

- Familiar CLI patterns
- Flexible configuration
- Clear error communication
- Built-in diagnostics

### Negative

- Configuration precedence complexity
- Multiple config file locations

## References

- [.NET CLI Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/commandline/)
- [System.CommandLine](https://github.com/dotnet/command-line-api)
