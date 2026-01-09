# ADR-0029: Developer Environment

## Status

Proposed

## Date

2026-01-09

## Context

We need a consistent developer environment that:

1. Works across Windows, macOS, and Linux
2. Isolates project dependencies
3. Includes all required tooling
4. Enables quick onboarding

### Requirements

- Environment isolation
- Pre-configured tooling
- Cross-platform support
- IDE integration

### Options Considered

#### Option 1: Dev Containers (Selected)

Docker-based development containers.

**Pros:**

- Complete environment isolation
- Pre-configured tooling
- Works with VS Code, GitHub Codespaces
- Reproducible across machines

**Cons:**

- Requires Docker
- Resource overhead
- Learning curve

#### Option 2: Local Setup Only

Manual installation with setup scripts.

**Pros:**

- No Docker dependency
- Native performance
- Simpler for small teams

**Cons:**

- Environment drift
- "Works on my machine" issues
- Manual dependency management

#### Option 3: Nix

Declarative package management.

**Pros:**

- Reproducible builds
- No Docker needed
- Lightweight

**Cons:**

- Steep learning curve
- Limited Windows support
- Less IDE integration

## Decision

**Dev Containers** as the recommended development environment, with **bootstrap scripts** for local development fallback.

### Dev Container Configuration

**.devcontainer/devcontainer.json:**

```json
{
  "name": "McjCoderOrg.ClaudeAutoResume",
  "image": "mcr.microsoft.com/devcontainers/dotnet:10.0",
  "features": {
    "ghcr.io/devcontainers/features/node:1": { "version": "22" },
    "ghcr.io/devcontainers/features/github-cli:1": {}
  },
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "esbenp.prettier-vscode",
        "streetsidesoftware.code-spell-checker"
      ]
    }
  },
  "postCreateCommand": "npm install && dotnet restore"
}
```

### Bootstrap Scripts

For local development without containers:

- `scripts/setup.ps1` (Windows)
- `scripts/setup.sh` (Unix)

Scripts check prerequisites, install dependencies, configure git hooks.

## Consequences

### Positive

- Consistent environment across developers
- Pre-configured IDE settings
- Quick onboarding via Codespaces
- Isolated from host system

### Negative

- Docker requirement
- Resource overhead on older machines
- WSL2 required on Windows

## References

- [Dev Containers](https://containers.dev/)
- [VS Code Dev Containers](https://code.visualstudio.com/docs/devcontainers/containers)
