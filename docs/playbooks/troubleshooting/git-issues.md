---
title: Git Issues
summary: Troubleshoot merge conflicts, detached HEAD states, and other git problems
audience: [developer, agent]
topics: [troubleshooting, git, merge-conflicts, detached-head]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# Git Issues

## Problem: Merge Conflicts

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

---

## Problem: Detached HEAD

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
