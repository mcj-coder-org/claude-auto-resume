---
name: pre-commit-hooks
description: |
  When setting up Git hooks or commit validation. Apply when configuring pre-commit,
  commit-msg, or pre-push hooks, or when enforcing conventional commits and work item references.
decision: Use Husky + commitlint + lint-staged for pre-commit hooks with CI enforcement as backup.
status: accepted
---

# ADR-0011: Pre-commit Hooks

## Status

Proposed

## Date

2026-01-09

## Context

We need Git hooks to enforce code quality and commit standards locally before code reaches CI. Requirements:

1. Block direct commits to main branch
2. Enforce conventional commit message format
3. Require work item references in commits
4. Format code automatically on commit
5. Run unit tests before push

### Options Considered

#### Option 1: Husky.Net

.NET port of Husky for managing Git hooks.

**Pros:**

- Pure .NET, no Node dependency
- Integrates with .NET tooling

**Cons:**

- Less mature than Node ecosystem
- Fewer plugins and integrations
- No commitlint equivalent in .NET

#### Option 2: Node.js (Husky + commitlint + lint-staged) (Selected)

Industry-standard toolchain for Git hooks.

**Pros:**

- Mature, battle-tested ecosystem
- commitlint has extensive configuration
- lint-staged enables efficient staged-file-only checks
- Large community and documentation
- Works across all platforms

**Cons:**

- Requires Node.js as dev dependency
- Additional package.json in .NET project

#### Option 3: Shell Scripts Only

Custom shell scripts in `.git/hooks/`.

**Pros:**

- No additional dependencies
- Full control

**Cons:**

- Manual maintenance
- Not portable across platforms
- No standardized commit linting
- Hooks not version-controlled by default

## Decision

We will use **Node.js-based tooling** (Husky + commitlint + lint-staged) for pre-commit hooks, with CI enforcement as backup.

### Package Configuration

**package.json:**

```json
{
  "private": true,
  "devDependencies": {
    "@commitlint/cli": "^19.6.1",
    "@commitlint/config-conventional": "^19.6.0",
    "husky": "^9.1.7",
    "lint-staged": "^15.3.0",
    "prettier": "^3.4.2"
  },
  "scripts": {
    "prepare": "husky"
  },
  "lint-staged": {
    "*.cs": ["dotnet format --include"],
    "*.md": ["prettier --write"],
    "*.{json,yml,yaml}": ["prettier --write"]
  }
}
```

### Commit Message Configuration

**commitlint.config.js:**

```javascript
export default {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [
      2,
      'always',
      [
        'feat',
        'fix',
        'perf',
        'refactor',
        'docs',
        'test',
        'build',
        'ci',
        'chore',
        'revert',
        'style',
      ],
    ],
    'scope-case': [2, 'always', 'lower-case'],
    'subject-case': [2, 'always', 'lower-case'],
    'subject-empty': [2, 'never'],
    'subject-full-stop': [2, 'never', '.'],
    'header-max-length': [2, 'always', 100],
    'references-empty': [2, 'never'],
  },
  parserPreset: {
    parserOpts: {
      issuePrefixes: ['#', 'GH-', 'AB#'],
    },
  },
  plugins: [
    {
      rules: {
        'references-empty': ({ references }) => {
          const valid = references && references.length > 0;
          return [valid, 'commit must reference a work item (e.g., "Refs: #123")'];
        },
      },
    },
  ],
};
```

### Hook Definitions

**.husky/pre-commit:**

```bash
#!/bin/sh

# Block commits directly to main
branch="$(git rev-parse --abbrev-ref HEAD)"
if [ "$branch" = "main" ]; then
  echo "Direct commits to main are not allowed."
  echo "Create a feature branch and open a PR instead."
  exit 1
fi

# Format and lint staged files
npx lint-staged

# Verify build compiles
dotnet build --no-restore -c Release -warnaserror --verbosity quiet
if [ $? -ne 0 ]; then
  echo "Build failed. Fix errors before committing."
  exit 1
fi
```

**.husky/commit-msg:**

```bash
#!/bin/sh
npx --no -- commitlint --edit $1
```

**.husky/pre-push:**

```bash
#!/bin/sh
echo "Running unit tests..."
dotnet test --no-build -c Release --filter "Category!=Integration" --verbosity minimal
if [ $? -ne 0 ]; then
  echo "Unit tests failed. Fix before pushing."
  exit 1
fi
echo "All checks passed."
```

### Setup Instructions

```bash
# One-time setup (after clone)
npm install

# Hooks are auto-installed via 'prepare' script
```

### Bypass (Emergency Only)

```bash
# Skip hooks (use sparingly, CI will still enforce)
git commit --no-verify -m "emergency: fix production issue"
```

## Consequences

### Positive

- Fast feedback loop for developers
- Consistent commit messages enable changelog generation
- Work item traceability for all changes
- Code formatted consistently before review
- Tests run before code reaches remote

### Negative

- Node.js required as dev dependency
- Initial setup step for new developers
- Hooks can be bypassed with `--no-verify`
- Slower commits due to format/build/test checks

### Risks

- Developers bypassing hooks degrades quality (mitigated by CI)
- Node version conflicts with other projects (mitigated by .nvmrc)
- Hook failures can frustrate developers (mitigated by clear error messages)

## References

- [Husky Documentation](https://typicode.github.io/husky/)
- [commitlint Documentation](https://commitlint.js.org/)
- [lint-staged Documentation](https://github.com/lint-staged/lint-staged)
- [Conventional Commits](https://www.conventionalcommits.org/)
