---
sidebar_position: 1
---

# Architecture Overview

High-level architecture of Claude Auto Resume.

## System Components

```text
┌─────────────────┐     ┌─────────────────┐
│   Claude CLI    │────▶│ Claude Auto     │
│                 │     │ Resume          │
└─────────────────┘     └────────┬────────┘
                                 │
                    ┌────────────┼────────────┐
                    ▼            ▼            ▼
              ┌──────────┐ ┌──────────┐ ┌──────────┐
              │ Session  │ │ Config   │ │ Storage  │
              │ Manager  │ │ Manager  │ │ Provider │
              └──────────┘ └──────────┘ └──────────┘
```

## Design Principles

- **Single Responsibility**: Each component has one clear purpose
- **Dependency Injection**: Loose coupling through interfaces
- **Testability**: All components are unit testable
- **Cross-Platform**: Works on Windows, macOS, and Linux

## Key Interfaces

- `ISessionManager`: Manages conversation sessions
- `IConfigurationProvider`: Handles configuration
- `IStorageProvider`: Abstracts file storage
