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

---

## Edge Cases and Limitations

### Binary File Handling

Hooks that run `prettier` or `dotnet format` skip binary files automatically.

**Behaviour:**

- Binary files are detected by file extension and content
- lint-staged ignores binaries in its pattern matching
- No special handling needed for images, executables, or archives

**If binary files cause issues:**

```bash
# Check if file is being incorrectly processed
git ls-files --stage | grep <filename>

# Force-add binary to .gitattributes if needed
echo "*.dat binary" >> .gitattributes
```

---

### Git LFS Compatibility

Git hooks work with Git LFS-tracked files with some considerations.

**Behaviour:**

- Pre-commit hooks see LFS pointer files, not actual content
- lint-staged processes staged pointer files (small text files)
- secretlint scans pointer content, not actual file content

**Limitations:**

- Secret scanning won't detect secrets in LFS-tracked files
- Format checking doesn't apply to LFS content

**Recommendation:**

Don't track files containing secrets with Git LFS. Use environment
variables or secrets management instead.

---

### Submodule Behaviour

Hooks run only in the parent repository, not in submodules.

**Behaviour:**

- Submodule commits trigger hooks in the submodule directory
- Parent repo commits involving submodule pointer updates run parent hooks
- Each submodule must have its own hook configuration

**Limitation:**

Submodule changes are not validated by parent repository hooks. Each
submodule is responsible for its own commit validation.

---

### Interactive Rebase Operations

During `git rebase -i`, hooks behave differently.

**Pre-commit hook:**

- Runs for `reword`, `edit`, and `squash` operations
- Does NOT run for `pick` (commits are replayed as-is)
- May block rebase if branch protection triggers

**Workaround for rebasing onto main:**

```bash
# Temporarily disable branch check during rebase
GIT_REFLOG_ACTION=rebase git rebase origin/main

# Or skip hooks during rebase (use cautiously)
git rebase -i --no-verify origin/main
```

**Commit-msg hook:**

- Runs for `reword` and `squash` operations
- Original commit messages must be re-validated
- May need to add `Refs: #X` to rebased commits

---

### Empty Commit Handling

Empty commits (no file changes) have special behaviour.

**Pre-commit hook:**

- lint-staged has no files to process (succeeds silently)
- Branch protection still blocks main branch commits

**Commit-msg hook:**

- Still validates commit message format
- Still requires work item reference

**Creating empty commits:**

```bash
# Empty commits are allowed with explicit flag
git commit --allow-empty -m "chore: trigger ci rebuild

Refs: #123"
```

---

### Bypassing Hooks (`--no-verify`)

The `--no-verify` flag skips all local hooks.

**When to use:**

- Emergency production fixes (CI will still validate)
- Merge commits created by Git (hooks auto-skip these)
- Recovery from hook infrastructure issues

**When NOT to use:**

- To avoid fixing commit message format
- To skip tests that are failing
- As a regular workflow habit

**Audit trail:**

Commits that bypassed hooks are still validated by CI. PRs from bypassed
commits may fail required status checks.

```bash
# Emergency bypass example
git commit --no-verify -m "emergency: fix critical production bug

Refs: #999"

# CI will still run all validations on push
```

**Preventing abuse:**

Branch protection rules require CI status checks to pass before merge.
Bypassing local hooks doesn't bypass CI enforcement.

---

### Merge Commit Handling

Merge commits receive special treatment.

**Automatic merge commits:**

- Created by `git merge`, `git pull`, or GitHub merge button
- commit-msg hook auto-detects and skips validation
- Pattern: message starts with "Merge" keyword

**Manual merge commits:**

- If you edit a merge commit message, validation applies
- Must follow conventional commits format if edited

**Fast-forward merges:**

- No merge commit created
- Original commit messages preserved
- Original commits already validated

---

### Stash Operations

Stashing does not trigger hooks.

**Behaviour:**

- `git stash` and `git stash pop` bypass hooks entirely
- Stash messages don't follow conventional commits (expected)
- Restored changes must pass hooks when committed

---

### Worktree Considerations

Git worktrees share hooks with the main repository.

**Behaviour:**

- `.husky/` hooks apply to all worktrees
- `node_modules/` must exist in each worktree
- Each worktree may need `npm install`

**Setup for new worktree:**

```bash
git worktree add ../feature-work feature/123-new-feature
cd ../feature-work
npm install  # Required for hooks to work
```
