# Claude Auto Resume

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
- **Transparent operation** - Acts as a passthrough until rate limits are detected
- **Cross-platform** - Runs on Windows, macOS, and Linux
- **Privacy-first** - No telemetry or data collection (see [Privacy Policy](docs/standards/privacy.md))

## Installation

### .NET Tool (Recommended)

````bash
dotnet tool install -g McjCoderOrg.ClaudeAutoResume
```text

### Standalone Executable

Download the latest release for your platform from the [Releases](https://github.com/mcj-coder-org/claude-auto-resume/releases) page.

## Usage

Use `claude-auto-resume` as a drop-in replacement for `claude`:

```bash
# Basic usage
claude-auto-resume

# With verbose logging
claude-auto-resume --verbose

# Run diagnostics
claude-auto-resume --diagnose

# Pass arguments to Claude CLI
claude-auto-resume -- --help
```text

## Configuration

Configuration can be provided via CLI arguments, environment variables, project config, or user config.

### CLI Arguments

```bash
claude-auto-resume --retry-delay 10000 --max-retries 5
```text

### Environment Variables

```bash
export CLAUDE_AUTO_RESUME_RETRY_DELAY=10000
export CLAUDE_AUTO_RESUME_MAX_RETRIES=5
```text

### Configuration File

Create `.claude-auto-resume.json` in your project directory or `~/.config/claude-auto-resume/config.json` for user-level config:

```json
{
  "retryDelayMs": 10000,
  "maxRetries": 5
}
```text

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
```text

### For AI Agents

If you're an AI agent working on this codebase, start by reading [AGENTS.md](AGENTS.md) for orientation and documentation routing guidance.

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
│  • Wraps Claude CLI process                         │
│  • Monitors output for rate limits                  │
│  • Auto-resumes when limit resets                   │
└────────────────────────┬────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────┐
│               Claude CLI (Anthropic)                 │
└─────────────────────────────────────────────────────┘
```text

## Exit Codes

| Code | Meaning                                      |
| ---- | -------------------------------------------- |
| 0    | Success                                      |
| 1    | General error                                |
| 2    | Configuration error                          |
| 3    | Claude CLI not found                         |
| 4    | Rate limit detected (when not auto-resuming) |
| 5    | User cancelled (Ctrl+C)                      |

## Privacy

This tool collects **no telemetry, analytics, or usage data**. All data remains on your machine. See our [Privacy Policy](docs/standards/privacy.md) for details.

## License

[MIT](LICENSE)

## Acknowledgements

- [Anthropic](https://www.anthropic.com/) for the Claude CLI
- All contributors to this project
````
