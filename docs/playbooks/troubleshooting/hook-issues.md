---
title: Git Hook Issues
summary: Troubleshoot pre-commit, commit message, and pre-push hook failures
audience: [developer, agent]
topics: [troubleshooting, git-hooks, husky, commitlint, lint-staged]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# Git Hook Issues

## Problem: Pre-commit Hook Fails

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

---

## Problem: Commit Message Rejected

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

- Missing type: `add feature` -> `feat: add feature`
- Wrong type: `feature:` -> `feat:`
- Missing reference: Add `Refs: #X` to footer
- Subject too long: Keep under 72 characters

---

## Problem: Pre-push Hook Fails

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

## Problem: Hooks Fail on Windows

**Symptoms:**

- Hook script errors
- Command not found errors
- Path issues

**Solutions:**

1. **Use Git Bash (not PowerShell/CMD)**

   Git hooks use shell scripts. On Windows, Git Bash provides the required
   Unix-like environment:

   ```bash
   # Open Git Bash (not PowerShell)
   # Run git commands from Git Bash terminal
   ```

2. **Line ending issues**

   If hooks fail with cryptic errors, check line endings:

   ```bash
   # Ensure hooks have Unix line endings (LF, not CRLF)
   git config core.autocrlf false

   # Re-install hooks
   npm install
   npx husky install
   ```

3. **Emoji display issues**

   Windows terminals may not display emojis correctly. This is cosmetic only
   and doesn't affect hook functionality.

4. **GPG signing on Windows**

   Install GPG4Win and ensure it's in your PATH:

   ```bash
   # Verify GPG is available
   gpg --version

   # If not found, add GPG4Win to PATH or use:
   git config --global gpg.program "C:/Program Files (x86)/GnuPG/bin/gpg.exe"
   ```

---

## Cross-Platform Compatibility Notes

The hook scripts are designed to work across:

- **Linux**: bash
- **macOS**: zsh/bash (BSD tools)
- **Windows**: Git Bash (MinGW)

**Known differences:**

| Feature       | Linux/macOS        | Windows          |
| ------------- | ------------------ | ---------------- |
| Shell         | /bin/sh (bash/zsh) | Git Bash (MinGW) |
| GPG           | gpg                | GPG4Win          |
| Line endings  | LF                 | CRLF (converted) |
| Emoji support | Full               | Limited          |

If you encounter platform-specific issues not covered here, please open an issue
