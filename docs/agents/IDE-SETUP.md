---
title: IDE Setup Guide
summary: Configuration guidance for AI agents in VS Code, JetBrains Rider, and Visual Studio
audience: [agent, developer]
topics: [ide, vscode, rider, visual-studio, configuration]
prerequisites: [AGENTS.md]
related: [ORIENTATION.md, ../adr/0009-agent-onboarding.md]
last_validated: 2026-01-09
---

# IDE Setup Guide

This document provides configuration guidance for AI agents operating in different IDEs.
Each IDE has different strengths and agent integration approaches.

## IDE Comparison

| IDE             | Primary Agent | Strengths                         | Best For                  |
| --------------- | ------------- | --------------------------------- | ------------------------- |
| VS Code         | Claude Code   | Terminal integration, flexibility | Cross-platform, scripting |
| JetBrains Rider | Junie         | .NET tooling, refactoring         | Heavy C# development      |
| Visual Studio   | Copilot       | Microsoft stack integration       | Windows-first development |

---

## VS Code Setup

### Prerequisites

- VS Code 1.85+
- .NET 10 SDK
- Node.js 22 LTS
- Claude Code CLI (`npm install -g @anthropic-ai/claude-code`)

### Recommended Extensions

Install these extensions for the best experience:

| Extension           | ID                                      | Purpose             |
| ------------------- | --------------------------------------- | ------------------- |
| C# Dev Kit          | `ms-dotnettools.csdevkit`               | C# language support |
| .NET Extension Pack | `ms-dotnettools.vscode-dotnet-pack`     | .NET tools          |
| EditorConfig        | `editorconfig.editorconfig`             | Code style          |
| Prettier            | `esbenp.prettier-vscode`                | Formatting          |
| Code Spell Checker  | `streetsidesoftware.code-spell-checker` | Spelling            |
| GitLens             | `eamodio.gitlens`                       | Git history         |
| Reqnroll            | `reqnroll.reqnroll-vscode`              | BDD support         |

### Settings Configuration

Add to `.vscode/settings.json`:

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp"
  },
  "csharp.format.enable": true,
  "dotnet.defaultSolution": "McjCoderOrg.ClaudeAutoResume.sln",
  "files.eol": "\n",
  "files.trimTrailingWhitespace": true,
  "files.insertFinalNewline": true,
  "cSpell.language": "en-GB",
  "cSpell.words": ["McjCoderOrg", "ClaudeAutoResume", "Reqnroll"]
}
```

### Claude Code Integration

Claude Code runs in the integrated terminal. Key commands:

```bash
# Start Claude Code
claude

# Start with verbose logging
claude --verbose

# Continue a previous conversation
claude --continue

# Start with a specific context
claude "Help me implement issue #42"
```

### Agent Workflow in VS Code

1. **Open integrated terminal** - Ctrl+`
2. **Start Claude Code** - `claude`
3. **Reference files** - Claude can read/edit files via terminal
4. **Use side-by-side view** - Terminal + editor

### Tips for Agents

- Use `@file` syntax to reference files in Claude Code
- Run `dotnet build` to verify changes
- Use `dotnet test` to run tests
- Check Git status frequently with GitLens

---

## JetBrains Rider Setup

### Prerequisites

- JetBrains Rider 2024.3+
- .NET 10 SDK
- Node.js 22 LTS
- Junie AI Assistant subscription

### Recommended Plugins

| Plugin                  | Purpose              |
| ----------------------- | -------------------- |
| Reqnroll for Rider      | BDD support          |
| .NET Core User Secrets  | Secret management    |
| Heap Allocations Viewer | Performance analysis |

### Settings Configuration

Configure via Settings > Editor > Code Style:

1. **C#**
   - Use EditorConfig: Yes
   - Import `Directory.Build.props` settings

2. **General**
   - Line separator: Unix (LF)
   - Strip trailing whitespace: Yes
   - Ensure newline at EOF: Yes

3. **File Encodings**
   - Global/Project: UTF-8
   - BOM for UTF-8: No

### Junie AI Integration

Junie is Rider's built-in AI assistant. Access via:

- **Alt+Enter** on code for suggestions
- **AI Actions** menu
- **Search Everywhere** (Shift+Shift) then type "AI:"

### Agent Workflow in Rider

1. **Use AI Assistant panel** - View > Tool Windows > AI Assistant
2. **Context-aware suggestions** - Junie understands C# context
3. **Refactoring support** - Use Rider's refactoring tools
4. **Test runner** - Use built-in test runner

### Tips for Agents

- Rider has superior refactoring - use it for renames, extractions
- Use "Find Usages" before changing signatures
- Run tests with coverage via Rider's tools
- Leverage Rider's inspections for code quality

### Terminal Agent Integration

For Claude Code in Rider:

1. Open Terminal tool window
2. Run `claude` command
3. Use as in VS Code

---

## Visual Studio Setup

### Prerequisites

- Visual Studio 2022 17.12+ (or VS 2024)
- .NET 10 SDK
- GitHub Copilot subscription

### Recommended Extensions

| Extension                     | Purpose           |
| ----------------------------- | ----------------- |
| GitHub Copilot                | AI assistance     |
| GitHub Copilot Chat           | Conversational AI |
| EditorConfig Language Service | Code style        |
| Reqnroll for Visual Studio    | BDD support       |
| CodeMaid                      | Code cleanup      |

### Settings Configuration

Configure via Tools > Options:

1. **Text Editor > C# > Code Style**
   - Use EditorConfig conventions: Yes

2. **Text Editor > All Languages**
   - Insert final newline: Yes
   - Trim trailing whitespace: Yes

3. **Source Control > Git Global Settings**
   - Default line ending: Line Feed

### Copilot Integration

GitHub Copilot provides:

- **Inline completions** - Tab to accept
- **Copilot Chat** - Conversational assistance
- **Code explanations** - Select code, ask questions

Access Copilot Chat:

- View > GitHub Copilot Chat
- Or Ctrl+\ then Ctrl+C

### Agent Workflow in Visual Studio

1. **Use Copilot Chat** - For complex questions
2. **Inline suggestions** - For code completion
3. **Solution Explorer** - Navigate structure
4. **Test Explorer** - Run and debug tests

### Tips for Agents

- Visual Studio has deep .NET integration
- Use Solution Explorer for project navigation
- Test Explorer shows all test projects
- Use "Go to Definition" (F12) extensively

### Terminal Agent Integration

For Claude Code in Visual Studio:

1. Open Developer PowerShell (View > Terminal)
2. Run `claude` command
3. Use as in VS Code

---

## Cross-IDE Consistency

Regardless of IDE, ensure these settings match:

### .editorconfig (Root of Repository)

```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.cs]
dotnet_sort_system_directives_first = true
csharp_new_line_before_open_brace = all
csharp_indent_case_contents = true
csharp_indent_switch_labels = true

[*.{json,yml,yaml}]
indent_size = 2

[*.md]
trim_trailing_whitespace = false
```

### Git Configuration

All IDEs should respect:

```bash
# Line endings
git config core.autocrlf input  # Unix/macOS
git config core.autocrlf true   # Windows

# Editor
git config core.editor "code --wait"  # VS Code
git config core.editor "rider --wait" # Rider
```

---

## Agent-Specific Guidance

### For Claude Code Agents

**Strengths:**

- Deep reasoning about code
- Long context handling
- Task tool for sub-agents
- File editing via CLI

**Workflow:**

1. Use terminal-based interaction
2. Edit files with Edit tool or direct writes
3. Run build/test commands directly
4. Use Task tool for parallel work

**Best Practices:**

- Reference `AGENTS.md` at session start
- Use structured tool calls
- Verify changes with `dotnet build`

### For Copilot Agents

**Strengths:**

- Inline code suggestions
- IDE integration
- Quick completions

**Limitations:**

- Shorter context window
- Less reasoning depth
- IDE-bound operation

**Best Practices:**

- Use for code completion
- Pair with manual review
- Follow up with tests

### For Junie Agents

**Strengths:**

- Rider refactoring awareness
- C#/.NET optimised
- IDE-native experience

**Limitations:**

- JetBrains ecosystem only
- Limited sub-agent support

**Best Practices:**

- Leverage Rider refactoring
- Use inspections for quality
- Follow IDE suggestions

---

## Troubleshooting IDE Issues

### VS Code

| Issue                    | Solution                                  |
| ------------------------ | ----------------------------------------- |
| C# extension not loading | Reload window, check .NET SDK path        |
| OmniSharp errors         | Delete `.vs` and `obj` folders            |
| Git hooks not running    | Ensure husky installed, run `npm install` |

### Rider

| Issue                | Solution                                     |
| -------------------- | -------------------------------------------- |
| Solution not loading | Invalidate caches (File > Invalidate Caches) |
| NuGet restore fails  | Delete `~/.nuget/packages`, restore again    |
| Tests not discovered | Rebuild solution, check test SDK             |

### Visual Studio

| Issue                    | Solution                        |
| ------------------------ | ------------------------------- |
| IntelliSense not working | Clean and rebuild solution      |
| Copilot not responding   | Check subscription, restart VS  |
| Build errors after pull  | Delete `bin` and `obj`, rebuild |

---

## Recommended Workflow by IDE

### Quick Tasks (< 30 minutes)

| Task                 | Best IDE |
| -------------------- | -------- |
| Small bug fix        | Any      |
| Documentation update | VS Code  |
| Test writing         | Rider    |
| Quick refactor       | Rider    |

### Complex Tasks (> 30 minutes)

| Task                   | Best IDE                                        |
| ---------------------- | ----------------------------------------------- |
| Feature implementation | Rider (for refactoring) or VS Code (for Claude) |
| Architecture changes   | VS Code + Claude Code                           |
| Performance tuning     | Rider (profiler)                                |
| Debugging              | Visual Studio or Rider                          |

### Autonomous Agent Work

For autonomous execution (Pattern 4), VS Code + Claude Code is recommended:

- Best terminal integration
- Sub-agent support
- File manipulation tools
- Flexible workflow
