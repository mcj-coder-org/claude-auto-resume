---
title: Build Issues
summary: Troubleshoot solution build failures, SDK version errors, and analyzer issues
audience: [developer, agent]
topics: [troubleshooting, build, dotnet, sdk, analyzers]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# Build Issues

## Problem: Solution Won't Build

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

---

## Problem: Analyzer Errors

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
