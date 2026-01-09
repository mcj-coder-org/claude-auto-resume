# ADR-0005: Security Scanning

## Status

Proposed

## Date

2026-01-09

## Context

We need automated security scanning to detect:

1. Secrets accidentally committed to the repository
2. Vulnerabilities in application code (SAST)
3. Vulnerabilities in dependencies

### Requirements

- Pre-commit secret detection (local, fast)
- Push-time secret detection (remote, comprehensive)
- Static Application Security Testing (SAST)
- Dependency vulnerability scanning
- Cross-platform tooling for local development

### Options Considered

#### Secret Scanning

#### Option A: secretlint (Selected)

- npm-based, cross-platform
- Pre-commit hook integration via lint-staged
- Extensible with plugins

#### Option B: gitleaks

- Go binary, fast
- Complex cross-platform installation
- No npm integration

**Decision:** secretlint for pre-commit (npm ecosystem), GitHub Secret Scanning for push protection.

#### SAST

#### Option A: GitHub CodeQL (Selected)

- Native GitHub integration
- Free for public repositories
- C# support included

#### Option B: SonarQube

- More comprehensive
- Requires separate infrastructure
- Overkill for single project

**Decision:** GitHub CodeQL for SAST scanning.

#### Dependency Scanning

#### Option A: Dependabot (Selected)

- Native GitHub integration
- Automatic PRs for updates
- Security advisories

#### Option B: OWASP Dependency-Check

- More comprehensive database
- Requires CI setup
- Additional maintenance

**Decision:** Dependabot for dependency scanning with `dotnet list package --vulnerable` in CI.

## Decision

Multi-layered security scanning:

| Layer      | Tool                               | Trigger               |
| ---------- | ---------------------------------- | --------------------- |
| Pre-commit | secretlint                         | Local commit          |
| Push       | GitHub Secret Scanning             | Remote push           |
| PR/Main    | GitHub CodeQL                      | PR, main push, weekly |
| Daily      | Dependabot                         | Scheduled             |
| PR         | `dotnet list package --vulnerable` | PR CI                 |

### Configuration

**.secretlintrc.json:**

```json
{
  "rules": [
    { "id": "@secretlint/secretlint-rule-preset-recommend" },
    { "id": "@secretlint/secretlint-rule-aws" },
    { "id": "@secretlint/secretlint-rule-gcp" },
    { "id": "@secretlint/secretlint-rule-npm" },
    { "id": "@secretlint/secretlint-rule-privatekey" }
  ]
}
```

## Consequences

### Positive

- Multiple layers of defense
- Local detection before push
- Automated dependency updates
- Free for public repos

### Negative

- npm dependency for secretlint
- CodeQL adds CI time
- False positives require triage

## References

- [secretlint](https://github.com/secretlint/secretlint)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning)
- [GitHub CodeQL](https://codeql.github.com/)
- [Dependabot](https://docs.github.com/en/code-security/dependabot)
