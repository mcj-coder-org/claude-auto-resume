---
title: Test Issues
summary: Troubleshoot test discovery, execution, and assertion failures
audience: [developer, agent]
topics: [troubleshooting, testing, xunit, dotnet-test]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# Test Issues

## Problem: Tests Won't Run

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

---

## Problem: Tests Fail

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
