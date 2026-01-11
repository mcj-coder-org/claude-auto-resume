---
title: Troubleshooting Guide
summary: Diagnose and resolve common issues when working on the ClaudeAutoResume project
audience: [developer, agent]
topics: [troubleshooting, debugging, diagnostics, common-issues]
last_validated: 2026-01-10
---

# Troubleshooting Guide

This guide helps diagnose and resolve common issues when working on the
McjCoderOrg.ClaudeAutoResume project.

## Quick Diagnostic Commands

Run these commands to gather diagnostic information:

```bash
# Check .NET SDK
dotnet --list-sdks

# Check Node.js
node --version
npm --version

# Check git hooks
ls -la .husky/

# Check npm packages
npm ls --depth=0

# Build solution
dotnet build

# Run tests
dotnet test

# Check formatting
dotnet format --verify-no-changes

# Check linting
npm run lint
```

---

## Issue Categories

For detailed troubleshooting steps, see the specific guides:

- [Build Issues](troubleshooting/build-issues.md) - Solution build failures, SDK issues, analyzer errors
- [Hook Issues](troubleshooting/hook-issues.md) - Pre-commit, commit message, and pre-push hook failures
- [Test Issues](troubleshooting/test-issues.md) - Test discovery, execution, and assertion failures
- [Formatting Issues](troubleshooting/formatting-issues.md) - Code formatting and spelling errors
- [IDE Issues](troubleshooting/ide-issues.md) - IntelliSense and extension problems
- [Git Issues](troubleshooting/git-issues.md) - Merge conflicts and detached HEAD states
- [Context and Requirements Issues](troubleshooting/context-issues.md) - Agent-specific context and requirements issues

---

## Common Error Messages

### "error CS0246: type or namespace not found"

**Cause:** Missing using statement or reference

**Fix:**

```csharp
// Add missing using
using System.Text.RegularExpressions;
```

Or add package reference to `.csproj`

### "error NETSDK1045: SDK not found"

**Cause:** Wrong .NET SDK version

**Fix:**

- Install required SDK version
- Check `global.json` for version requirement

### "husky - command not found"

**Cause:** npm packages not installed

**Fix:**

```bash
npm install
```

### "secretlint: no files to check"

**Cause:** No staged files match patterns

**Fix:** This is usually fine - no secrets to scan

### "cspell: Unknown word"

**Cause:** Word not in dictionary

**Fix:**

- Add to `.cspell.json` words array
- Or fix the spelling

---

## When to Escalate

Escalate to human if:

1. **Security concern** - Potential vulnerability discovered
2. **Architecture question** - Change affects system design
3. **Requirements unclear** - After asking, still ambiguous
4. **Blocked by infrastructure** - Can't proceed due to tooling issues
5. **Breaking change** - Change would break existing functionality

**How to escalate:**

1. Document what you've tried
2. Explain the blocker clearly
3. Suggest possible approaches
4. Wait for guidance

---

## Diagnostic Checklist

When encountering any issue:

- [ ] Read the full error message
- [ ] Check if it's a known issue in this document
- [ ] Run diagnostic commands
- [ ] Search codebase for similar patterns
- [ ] Check relevant documentation
- [ ] Try the suggested solutions
- [ ] If still stuck, escalate with details
