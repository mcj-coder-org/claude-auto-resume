---
sidebar_position: 2
---

# Coding Standards

Code style and quality guidelines for contributors.

## Code Formatting

We use automated formatting tools:

- **C#**: `dotnet format` with `.editorconfig` rules
- **Markdown/JSON/YAML**: Prettier

Run formatting check:

```bash
dotnet format --verify-no-changes
npm run lint:format
```

## Commit Messages

We use [Conventional Commits](https://www.conventionalcommits.org/):

```text
type(scope): description

[optional body]

[optional footer]
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `build`: Build system changes
- `ci`: CI/CD changes
- `chore`: Maintenance tasks

## Pre-commit Hooks

Git hooks automatically run on commit:

- **commitlint**: Validates commit message format
- **prettier**: Formats staged files
- **cspell**: Checks spelling
- **secretlint**: Detects secrets
