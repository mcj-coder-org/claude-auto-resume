---
sidebar_position: 2
---

# Usage Guide

Detailed guide on using Claude Auto Resume effectively.

## Session Management

### Listing Sessions

```bash
claude-auto-resume list
```

### Resuming a Specific Session

```bash
claude-auto-resume resume --session <session-id>
```

### Deleting a Session

```bash
claude-auto-resume delete --session <session-id>
```

## Command-Line Options

| Option      | Description                    |
| ----------- | ------------------------------ |
| `--resume`  | Resume the most recent session |
| `--session` | Specify a session ID           |
| `--list`    | List all saved sessions        |
| `--version` | Display version information    |
| `--help`    | Display help information       |
