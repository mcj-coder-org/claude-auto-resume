---
title: Privacy Policy
summary: No telemetry policy and privacy-first principles for the project
audience: [developer, agent, user]
topics: [privacy, telemetry, data-collection]
prerequisites: []
related: [adr/0007-telemetry.md]
last_validated: 2026-01-09
---

# Privacy Policy

This document defines the privacy policy for the McjCoderOrg.ClaudeAutoResume project.

## Core Principle

**This tool collects no telemetry, analytics, or usage data.**

We believe in privacy-first software. Your usage of this tool is entirely private.

## What We Do NOT Collect

- Usage statistics
- Error reports or crash data
- Feature usage metrics
- Session information
- IP addresses
- Any personally identifiable information (PII)
- Any anonymous or pseudonymous identifiers

## What Stays on Your Machine

All data remains local:

- Configuration files (`~/.config/claude-auto-resume/`)
- Log files (when `--verbose` is used)
- Session state

## Network Connections

This tool only makes network connections to:

1. **Claude CLI** - The tool wraps the Claude CLI, which has its own privacy policy managed by Anthropic
2. **No other connections** - The wrapper itself makes no network requests

## Third-Party Services

When you use this tool:

- **Claude CLI**: Subject to [Anthropic's Privacy Policy](https://www.anthropic.com/privacy)
- **GitHub** (for updates): Only if you choose to check for updates manually

## Diagnostics

The `--diagnose` command outputs environment information:

- .NET version
- Operating system
- Configuration validity
- Claude CLI location

This information is:

- Displayed locally only
- Never transmitted anywhere
- Intended for you to include in bug reports (your choice)

## Bug Reports

If you choose to submit a bug report:

1. You decide what information to include
2. We recommend using `--diagnose` output
3. Submissions go to public GitHub Issues (your choice to submit)

## Updates to This Policy

- Changes will be documented in the CHANGELOG
- Major changes will be announced in release notes
- This policy is versioned with the software

## Your Rights

You have complete control:

- **Access**: All your data is on your machine
- **Deletion**: Delete config/log directories anytime
- **Portability**: Files are plain text, easily transferable

## Contact

Questions about privacy:

- Open a [GitHub Discussion](https://github.com/mcj-coder-org/claude-auto-resume/discussions)
- Review our [source code](https://github.com/mcj-coder-org/claude-auto-resume) - it's open source

## References

- [ADR-0007: Telemetry](../adr/0007-telemetry.md) - Decision record for no telemetry
- [Anthropic Privacy Policy](https://www.anthropic.com/privacy) - Claude CLI privacy
