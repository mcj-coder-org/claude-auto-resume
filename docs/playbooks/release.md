# Release Playbook

This playbook documents the release process for claude-auto-resume.

## Pre-Release Checklist

### Code Quality

- [ ] All tests pass: `dotnet test -c Release`
- [ ] Build succeeds with no warnings: `dotnet build -c Release -warnaserror`
- [ ] Code formatting verified: `dotnet format --verify-no-changes`
- [ ] No vulnerable packages: `dotnet list package --vulnerable`
- [ ] npm lint passes: `npm run lint`
- [ ] No secrets in code: `npx secretlint .`

### Documentation

- [ ] CHANGELOG.md updated with release notes
- [ ] README.md up to date
- [ ] All ADRs have `status: accepted`
- [ ] API documentation current

### Testing

- [ ] Unit tests pass (62+ tests)
- [ ] BDD scenarios pass (22+ scenarios)
- [ ] E2E tests pass (5+ tests)
- [ ] Architecture tests pass (4+ tests)
- [ ] Manual testing on target platforms completed

### Optional Quality Checks

- [ ] Mutation testing score reviewed: `dotnet stryker`
- [ ] Benchmarks recorded: `dotnet run --project tests/McjCoderOrg.ClaudeAutoResume.Benchmarks -c Release`
- [ ] Code coverage reviewed

## Release Process

### 1. Version Bump

Version is managed by GitVersion based on conventional commits.

```bash
# Check current version
dotnet gitversion /showvariable SemVer

# For a specific version, use git tags
git tag -a v1.0.0 -m "Release v1.0.0"
```

### 2. Create Release Branch (for major/minor)

```bash
git checkout main
git pull
git checkout -b release/v1.0.0
```

### 3. Final Verification

```bash
# Full verification suite
dotnet build -c Release -warnaserror
dotnet test -c Release
dotnet format --verify-no-changes
npm run lint
dotnet list package --vulnerable
```

### 4. Update CHANGELOG

Move items from `[Unreleased]` to a versioned section:

```markdown
## [1.0.0] - 2026-01-12

### Added

- Feature descriptions...
```

### 5. Create GitHub Release

```bash
# Push tag
git push origin v1.0.0

# Create release via GitHub CLI
gh release create v1.0.0 \
  --title "v1.0.0" \
  --notes-file CHANGELOG.md \
  --latest
```

### 6. Publish NuGet Package

The CI/CD pipeline automatically publishes to NuGet on release tags.

Manual publish (if needed):

```bash
dotnet pack -c Release
dotnet nuget push ./artifacts/packages/*.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

### 7. Post-Release

- [ ] Verify NuGet package published
- [ ] Verify GitHub release artifacts
- [ ] Update documentation site if needed
- [ ] Announce release (if applicable)

## Hotfix Process

For critical fixes to released versions:

```bash
# Create hotfix branch from release tag
git checkout v1.0.0
git checkout -b hotfix/v1.0.1-critical-fix

# Make fix, test, commit
# ...

# Merge to main and create new tag
git checkout main
git merge hotfix/v1.0.1-critical-fix
git tag -a v1.0.1 -m "Hotfix: critical fix description"
git push origin main --tags
```

## Rollback Process

If a release has critical issues:

1. **Unlist NuGet package** (does not delete, just hides):

   ```bash
   dotnet nuget delete McjCoderOrg.ClaudeAutoResume 1.0.0 \
     --source https://api.nuget.org/v3/index.json \
     --api-key $NUGET_API_KEY \
     --non-interactive
   ```

2. **Mark GitHub release as pre-release** or delete
3. **Create hotfix** following hotfix process above
4. **Communicate** to affected users if applicable

## Version Numbering

We follow [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backwards compatible)
- **PATCH**: Bug fixes (backwards compatible)

GitVersion determines version from conventional commits:

- `feat:` -> Minor bump
- `fix:` -> Patch bump
- `feat!:` or `BREAKING CHANGE:` -> Major bump
