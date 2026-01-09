---
title: Agent Troubleshooting Guide
summary: Common issues and solutions for AI agents working on this project
audience: [agent]
topics: [troubleshooting, debugging, common-issues, solutions]
prerequisites: [AGENTS.md]
related: [ORIENTATION.md, IDE-SETUP.md]
last_validated: 2026-01-09
---

# Agent Troubleshooting Guide

This document helps AI agents diagnose and resolve common issues when working on the
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

## Build Issues

### Problem: Solution Won't Build

**Symptoms:**

- `dotnet build` fails
- Missing references
- SDK version errors

**Diagnostic:**

```bash
dotnet --list-sdks
dotnet restore --verbosity detailed
```

**Solutions:**

1. **Wrong .NET SDK version**

   ```bash
   # Check required version in global.json or Directory.Build.props
   # Install correct SDK from https://dotnet.microsoft.com/download
   ```

2. **Missing NuGet packages**

   ```bash
   dotnet restore
   # Or clear cache and restore
   dotnet nuget locals all --clear
   dotnet restore
   ```

3. **Corrupted build artifacts**

   ```bash
   # Delete build outputs
   rm -rf bin obj
   dotnet build
   ```

### Problem: Analyzer Errors

**Symptoms:**

- Warnings treated as errors
- Build fails on analyzer rules

**Solutions:**

1. **Fix the violation** (preferred)
   - Read the error code (e.g., CA1000)
   - Fix the code to comply

2. **Suppress if justified** (rare)

   ```csharp
   #pragma warning disable CA1000 // Reason for suppression
   // code here
   #pragma warning restore CA1000
   ```

3. **Check if rule is misconfigured**
   - Review `.editorconfig` for rule settings

---

## Git Hook Issues

### Problem: Pre-commit Hook Fails

**Symptoms:**

- Commit rejected
- Hook script errors

**Diagnostic:**

```bash
# Check if hooks exist
ls -la .husky/

# Check npm packages
npm ls lint-staged commitlint secretlint cspell prettier
```

**Solutions:**

1. **Husky not installed**

   ```bash
   npm install
   npx husky install
   ```

2. **lint-staged failing**

   ```bash
   # Run manually to see errors
   npx lint-staged --debug
   ```

3. **commitlint failing**

   ```bash
   # Test commit message format
   echo "feat: test message

   Refs: #1" | npx commitlint
   ```

### Problem: Commit Message Rejected

**Symptoms:**

- `commitlint` error
- Message format invalid

**Solution:**

Ensure commit message follows format:

```text
type(scope): subject

body (optional)

Refs: #issue
```

**Valid example:**

```text
feat(monitor): add retry delay configuration

Allow users to configure the delay between retry attempts.

Refs: #42
```

**Common mistakes:**

- Missing type: `add feature` → `feat: add feature`
- Wrong type: `feature:` → `feat:`
- Missing reference: Add `Refs: #X` to footer
- Subject too long: Keep under 72 characters

### Problem: Pre-push Hook Fails

**Symptoms:**

- Push rejected
- Branch name validation error

**Solutions:**

1. **Branch name doesn't match pattern**
   - Format: `type/issue#-description`
   - Example: `feature/42-add-retry` not `add-retry`

2. **Pushing to main directly**
   - Create a feature branch
   - Push to feature branch instead

---

## Test Issues

### Problem: Tests Won't Run

**Symptoms:**

- Test discovery fails
- No tests found

**Diagnostic:**

```bash
dotnet test --verbosity detailed
```

**Solutions:**

1. **Missing test SDK**
   - Check test project has `Microsoft.NET.Test.Sdk`

2. **Wrong namespace**
   - Test classes must be `public`
   - Methods must have `[Fact]` or `[Theory]`

3. **Build not up to date**

   ```bash
   dotnet build
   dotnet test
   ```

### Problem: Tests Fail

**Symptoms:**

- Test assertions fail
- Unexpected behaviour

**Diagnostic:**

```bash
dotnet test --logger "console;verbosity=detailed"
```

**Solutions:**

1. **Check test assumptions**
   - Verify test setup is correct
   - Check for external dependencies

2. **Check for race conditions**
   - Look for async test issues
   - Ensure proper `await` usage

3. **Environment differences**
   - Check for path separators (Windows vs Unix)
   - Check for environment-specific behaviour

---

## Formatting Issues

### Problem: Format Check Fails

**Symptoms:**

- `dotnet format --verify-no-changes` fails
- CI lint step fails

**Solutions:**

1. **Auto-fix formatting**

   ```bash
   dotnet format
   npx prettier --write .
   ```

2. **Check specific files**

   ```bash
   dotnet format --include path/to/file.cs
   ```

3. **EditorConfig not being respected**
   - Verify `.editorconfig` exists at root
   - Check IDE is configured to use EditorConfig

### Problem: Spelling Errors

**Symptoms:**

- cspell fails
- Unknown words flagged

**Solutions:**

1. **Add to project dictionary** (if valid term)
   - Add to `.cspell.json` `words` array:

   ```json
   {
     "words": ["ClaudeAutoResume", "McjCoderOrg"]
   }
   ```

2. **Fix the spelling** (if actually wrong)
   - Correct the typo in the source

3. **Check word in code vs comment**
   - Technical terms in code may need dictionary
   - Prose should use correct spelling

---

## IDE Issues

### Problem: IntelliSense Not Working

**Solutions by IDE:**

**VS Code:**

```bash
# Restart OmniSharp
Ctrl+Shift+P → "Restart OmniSharp"
# Or reload window
Ctrl+Shift+P → "Reload Window"
```

**Rider:**

- File → Invalidate Caches / Restart

**Visual Studio:**

- Clean and rebuild solution
- Delete `.vs` folder and restart

### Problem: Extensions/Plugins Not Working

**Solutions:**

1. Check extension is installed and enabled
2. Check extension version compatibility
3. Check for conflicting extensions
4. Reinstall the extension

---

## Git Issues

### Problem: Merge Conflicts

**Symptoms:**

- Pull fails with conflicts
- Unable to merge

**Solutions:**

1. **Resolve conflicts manually**

   ```bash
   git status  # See conflicted files
   # Edit files to resolve
   git add .
   git commit -m "fix: resolve merge conflicts"
   ```

2. **Abort and restart**

   ```bash
   git merge --abort
   # or
   git rebase --abort
   ```

### Problem: Detached HEAD

**Symptoms:**

- `(HEAD detached at ...)` message
- Commits not on branch

**Solutions:**

1. **Create branch from current state**

   ```bash
   git checkout -b new-branch-name
   ```

2. **Return to existing branch**

   ```bash
   git checkout main
   ```

---

## Agent-Specific Issues

### Problem: Context Too Large

**Symptoms:**

- Hitting token limits
- Losing earlier context

**Solutions:**

1. **Focus on relevant files**
   - Read only files needed for current task
   - Don't load entire codebase

2. **Use sub-agents**
   - Delegate focused tasks to sub-agents
   - Each sub-agent has fresh context

3. **Summarise before continuing**
   - Document current state
   - Start fresh session with summary

### Problem: Unclear Requirements

**Symptoms:**

- Ticket is ambiguous
- Multiple interpretations possible

**Solutions:**

1. **Ask for clarification** (preferred)
   - Don't guess
   - Request specific details

2. **Reference existing patterns**
   - Check how similar features are implemented
   - Follow established conventions

3. **Document assumptions**
   - If proceeding with assumptions, document them
   - Make them visible for review

### Problem: Can't Find Relevant Code

**Symptoms:**

- Don't know where to make changes
- Unsure which files to modify

**Solutions:**

1. **Search for related terms**

   ```bash
   grep -r "RateLimit" --include="*.cs"
   ```

2. **Check project structure**
   - Review `docs/agents/ORIENTATION.md`
   - Check solution structure

3. **Use IDE navigation**
   - Find usages
   - Go to definition
   - Find implementations

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
