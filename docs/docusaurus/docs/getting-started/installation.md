---
sidebar_position: 1
---

# Installation

This guide covers how to install Claude Auto Resume.

## Prerequisites

- .NET 10.0 Runtime or later
- Claude CLI installed and configured

## Installation Methods

### Via dotnet tool (Recommended)

```bash
dotnet tool install --global McjCoderOrg.ClaudeAutoResume
```

### Via standalone executable

Download the appropriate binary for your platform from the
[GitHub Releases](https://github.com/mcj-coder-org/claude-auto-resume/releases) page:

- `claude-auto-resume-win-x64.exe` - Windows x64
- `claude-auto-resume-linux-x64` - Linux x64
- `claude-auto-resume-osx-x64` - macOS Intel
- `claude-auto-resume-osx-arm64` - macOS Apple Silicon

## Verification

After installation, verify it's working:

```bash
claude-auto-resume --version
```
