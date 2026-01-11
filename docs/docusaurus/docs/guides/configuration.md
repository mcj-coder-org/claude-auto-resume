---
sidebar_position: 1
---

# Configuration

Claude Auto Resume can be configured through environment variables and configuration files.

## Configuration File

Create a configuration file at `~/.config/claude-auto-resume/config.json`:

```json
{
  "autoSave": true,
  "saveInterval": 60,
  "maxSessions": 10
}
```

## Environment Variables

| Variable               | Description              | Default |
| ---------------------- | ------------------------ | ------- |
| `CLAUDE_AUTO_SAVE`     | Enable auto-save         | `true`  |
| `CLAUDE_SAVE_INTERVAL` | Save interval in seconds | `60`    |
| `CLAUDE_MAX_SESSIONS`  | Maximum saved sessions   | `10`    |
