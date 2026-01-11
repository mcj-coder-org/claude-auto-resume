---
sidebar_position: 1
---

# Development Setup

Set up your development environment for contributing to Claude Auto Resume.

## Prerequisites

- .NET 10.0 SDK
- Node.js 22+
- Git

## Quick Setup

### Using Dev Container (Recommended)

1. Install [Docker](https://www.docker.com/) and [VS Code](https://code.visualstudio.com/)
2. Install the [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)
3. Open the repository in VS Code
4. Click "Reopen in Container" when prompted

### Local Setup

Run the setup script for your platform:

**Windows (PowerShell):**

```powershell
./scripts/setup.ps1
```

**macOS/Linux:**

```bash
./scripts/setup.sh
```

## Verify Setup

Build the project:

```bash
dotnet build
```

Run tests:

```bash
dotnet test
```
