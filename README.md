# Claude Auto Resume

[![CI](https://github.com/mcj-coder-org/claude-auto-resume/actions/workflows/ci.yml/badge.svg)](https://github.com/mcj-coder-org/claude-auto-resume/actions/workflows/ci.yml)
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A cross-platform .NET tool that wraps the Claude CLI to provide automatic session
resumption when rate limits are hit.

## Overview

When using the Claude CLI for extended coding sessions, users encounter rate limits
that interrupt their workflow. This tool transparently wraps the Claude CLI, monitors
output for rate limit indicators, and automatically resumes the session when the
limit resets.

## Features

- **Automatic rate limit detection** - Monitors Claude CLI output for rate limit messages
- **Seamless resumption** - Waits for the reset time and automatically resumes your session
- **Headless mode** - Run unattended with auto-response to prompts
- **Transparent operation** - Acts as a passthrough until rate limits are detected
- **Cross-platform** - Runs on Windows, macOS, and Linux
- **Privacy-first** - No telemetry or data collection (see [Privacy Policy](docs/standards/privacy.md))

## Installation

### Prerequisites

- [Claude CLI](https://docs.anthropic.com/en/docs/agents-and-tools/claude-code/overview) installed and in PATH
- .NET 10 Runtime (for .NET tool installation)

### .NET Tool (Recommended)

```bash
dotnet tool install -g McjCoderOrg.ClaudeAutoResume
```

### Standalone Executable

Download the latest release for your platform from the
[Releases](https://github.com/mcj-coder-org/claude-auto-resume/releases) page.

## Quick Start

```bash
# Interactive mode (default) - use like normal claude
claude-auto-resume

# With an initial prompt
claude-auto-resume -p "implement the login feature"

# Continue previous session
claude-auto-resume -c -p "continue where we left off"

# Headless mode for CI/automation
claude-auto-resume --headless --dangerous -p "implement feature"

# Pass additional args to claude
claude-auto-resume -- --model claude-3-opus
```

## Usage

```text
USAGE:
    claude-auto-resume [OPTIONS] [-- CLAUDE_ARGS...]

OPTIONS:
    -h, --help                      Show help
    -v, --version                   Show version information
    -p, --prompt <PROMPT>           Initial prompt to send to Claude
    -c, --continue                  Continue previous conversation
    -w, --wait <MINUTES>            Minutes to wait on rate limit (default: 15)
    -V, --verbose                   Enable verbose logging to file
    --headless                      Run without user input (auto-respond to prompts)
    --dangerously-skip-permissions  Pass dangerous flag to Claude (required for headless)
    --dangerous                     Alias for --dangerously-skip-permissions
    --diagnose                      Run environment diagnostics

ENVIRONMENT:
    CLAUDE_WAIT_MINUTES             Override default wait time
```

## Modes

### Interactive (Default)

- Full PTY pass-through with colors
- You type, Claude responds
- Auto-waits and continues on rate limit

### Headless (`--headless --dangerous`)

- No user input required
- Auto-responds 'y' to permission prompts
- Detects when Claude hangs waiting for input
- Ideal for CI/CD pipelines

> **Warning:** `--dangerously-skip-permissions` allows Claude to execute commands
> without confirmation. Use only in trusted environments.

## Development

### Prerequisites

- .NET 10 SDK
- Node.js 22 LTS
- Git with commit signing configured

### Getting Started

```bash
# Clone the repository
git clone https://github.com/mcj-coder-org/claude-auto-resume.git
cd claude-auto-resume

# Install dependencies and configure hooks
npm install

# Build
dotnet build

# Run tests
dotnet test
```

### For AI Agents

If you're an AI agent working on this codebase, start by reading [AGENTS.md](AGENTS.md)
for orientation and documentation routing guidance.

### Documentation

- [AGENTS.md](AGENTS.md) - AI agent orientation and routing
- [docs/standards/](docs/standards/) - Coding conventions and guidelines
- [docs/practices/](docs/practices/) - Workflows and processes
- [docs/agents/](docs/agents/) - Agent-specific guidance
- [docs/adr/](docs/adr/) - Architecture Decision Records

### Contributing

1. Read the [Contribution Workflow](docs/adr/0004-contribution-workflow.md)
2. Create an issue for your change
3. Create a feature branch: `feature/{issue#}-description`
4. Make changes following [Coding Standards](docs/standards/coding-standards.md)
5. Submit a PR with conventional commit title

## Architecture

```text
┌─────────────────────────────────────────────────────┐
│                     Terminal                         │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│            claude-auto-resume (this tool)           │
│  • Wraps Claude CLI in pseudo-terminal              │
│  • Monitors output for rate limits                  │
│  • Auto-resumes when limit resets                   │
│  • Handles prompts in headless mode                 │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│               Claude CLI (Anthropic)                 │
└─────────────────────────────────────────────────────┘
```

## Exit Codes

| Code | Meaning                 |
| ---- | ----------------------- |
| 0    | Success                 |
| 1    | General error           |
| 2    | Invalid arguments       |
| 3    | Configuration error     |
| 4    | Claude CLI not found    |
| 5    | Rate limit detected     |
| 6    | User cancelled (Ctrl+C) |

## Privacy

This tool collects **no telemetry, analytics, or usage data**. All data remains on your
machine. See our [Privacy Policy](docs/standards/privacy.md) for details.

## License

[MIT](LICENSE)

## Acknowledgements

- [Anthropic](https://www.anthropic.com/) for the Claude CLI
- All contributors to this project
