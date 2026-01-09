# ADR-0010: Code Formatting

## Status

Proposed

## Date

2026-01-09

## Context

We need consistent code formatting across:

1. C# source files
2. Markdown documentation
3. JSON/YAML configuration files
4. Line endings across platforms

Formatting should be:

- Automated (no manual formatting)
- Enforced in CI
- Consistent with IDE defaults
- Configurable per project needs

### Options Considered

#### Option 1: dotnet format + Prettier (Selected)

Use `dotnet format` for C# and Prettier for everything else.

**Pros:**
- `dotnet format` respects `.editorconfig` (single source of truth)
- Prettier is industry standard for Markdown/JSON/YAML
- Both integrate with pre-commit hooks
- Both have CI verification modes

**Cons:**
- Two tools instead of one
- Prettier requires Node.js

#### Option 2: dotnet format Only

Use `dotnet format` for C# and rely on IDE formatting for other files.

**Pros:**
- No Node.js dependency
- Simpler setup

**Cons:**
- No automated Markdown/JSON formatting
- Inconsistent non-C# files

#### Option 3: CSharpier + Prettier

Use CSharpier (opinionated C# formatter) instead of `dotnet format`.

**Pros:**
- Opinionated = less configuration
- Faster than `dotnet format`

**Cons:**
- Less control over formatting rules
- Additional tool to maintain
- May conflict with `.editorconfig` preferences

## Decision

We will use **dotnet format** for C# files and **Prettier** for Markdown, JSON, and YAML files. Line endings are enforced via `.gitattributes`.

### Configuration Files

**.editorconfig** (C# formatting - source of truth):

```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.{csproj,props,targets,xml,config}]
indent_size = 2

[*.{json,yml,yaml}]
indent_size = 2

[*.md]
trim_trailing_whitespace = false

[*.cs]
# Namespace
csharp_style_namespace_declarations = file_scoped:error

# var preferences
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = true:suggestion

# Expression-bodied members
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
csharp_style_expression_bodied_constructors = when_on_single_line:suggestion
csharp_style_expression_bodied_properties = true:suggestion

# Pattern matching
csharp_style_pattern_matching_over_is_with_cast_check = true:error
csharp_style_pattern_matching_over_as_with_null_check = true:error
csharp_style_prefer_switch_expression = true:suggestion

# Modern C# features
csharp_style_prefer_primary_constructors = true:suggestion
csharp_prefer_simple_using_statement = true:suggestion
csharp_style_prefer_index_operator = true:suggestion
csharp_style_prefer_range_operator = true:suggestion

# New line preferences
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true

# Naming: private fields with underscore
dotnet_naming_rule.private_fields_underscore.symbols = private_fields
dotnet_naming_rule.private_fields_underscore.style = underscore_prefix
dotnet_naming_rule.private_fields_underscore.severity = error

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

dotnet_naming_style.underscore_prefix.capitalization = camel_case
dotnet_naming_style.underscore_prefix.required_prefix = _
```

**.prettierrc** (Markdown/JSON/YAML):

```json
{
  "printWidth": 100,
  "tabWidth": 2,
  "useTabs": false,
  "semi": true,
  "singleQuote": true,
  "trailingComma": "es5",
  "bracketSpacing": true,
  "proseWrap": "preserve",
  "endOfLine": "lf"
}
```

**.gitattributes** (line ending enforcement):

```gitattributes
* text=auto eol=lf

*.cs text eol=lf
*.csproj text eol=lf
*.props text eol=lf
*.targets text eol=lf
*.sln text eol=lf
*.json text eol=lf
*.yml text eol=lf
*.yaml text eol=lf
*.md text eol=lf
*.sh text eol=lf

*.png binary
*.ico binary
```

### Enforcement

**Pre-commit (lint-staged):**

```json
{
  "lint-staged": {
    "*.cs": ["dotnet format --include"],
    "*.md": ["prettier --write"],
    "*.{json,yml,yaml}": ["prettier --write"]
  }
}
```

**CI Verification:**

```yaml
- name: Check C# formatting
  run: dotnet format --verify-no-changes --verbosity diagnostic

- name: Check Markdown/JSON formatting
  run: npx prettier --check "**/*.md" "**/*.json" "**/*.yml"
```

### Configuration Sync

| File Type | Formatter | Config Source |
|-----------|-----------|---------------|
| C# | `dotnet format` | `.editorconfig` |
| Markdown | Prettier | `.prettierrc` |
| JSON | Prettier | `.prettierrc` |
| YAML | Prettier | `.prettierrc` |
| XML/csproj | `dotnet format` | `.editorconfig` |

### IDE Integration

Both `.editorconfig` and `.prettierrc` are recognized by:

- Visual Studio
- VS Code (with extensions)
- JetBrains Rider
- GitHub web editor

## Consequences

### Positive

- Consistent formatting across all files
- No formatting debates in code review
- Automated formatting reduces manual effort
- Single source of truth per file type
- IDE integration provides real-time feedback

### Negative

- Two formatting tools to maintain
- Prettier requires Node.js
- Initial formatting pass may create large diff

### Risks

- `.editorconfig` and Prettier config drift (mitigated by documentation)
- Tool version differences between local and CI (mitigated by pinned versions)

## References

- [dotnet format Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-format)
- [EditorConfig Specification](https://editorconfig.org/)
- [Prettier Documentation](https://prettier.io/docs/en/index.html)
- [.gitattributes Documentation](https://git-scm.com/docs/gitattributes)
