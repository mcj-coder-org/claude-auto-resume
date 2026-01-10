---
title: IDE Issues
summary: Troubleshoot IntelliSense and extension problems in VS Code, Rider, and Visual Studio
audience: [developer, agent]
topics: [troubleshooting, ide, vscode, rider, visual-studio, intellisense]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# IDE Issues

## Problem: IntelliSense Not Working

**Solutions by IDE:**

**VS Code:**

```bash
# Restart OmniSharp
Ctrl+Shift+P -> "Restart OmniSharp"
# Or reload window
Ctrl+Shift+P -> "Reload Window"
```

**Rider:**

- File -> Invalidate Caches / Restart

**Visual Studio:**

- Clean and rebuild solution
- Delete `.vs` folder and restart

---

## Problem: Extensions/Plugins Not Working

**Solutions:**

1. Check extension is installed and enabled
2. Check extension version compatibility
3. Check for conflicting extensions
4. Reinstall the extension
