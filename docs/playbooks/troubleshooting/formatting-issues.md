---
title: Formatting Issues
summary: Troubleshoot code formatting and spelling check failures
audience: [developer, agent]
topics: [troubleshooting, formatting, dotnet-format, cspell, prettier]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# Formatting Issues

## Problem: Format Check Fails

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

---

## Problem: Spelling Errors

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
