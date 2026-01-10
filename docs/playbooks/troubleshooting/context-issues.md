---
title: Context and Requirements Issues
summary: Troubleshoot context management and requirements clarification for agents
audience: [agent]
topics: [troubleshooting, context, requirements, agent-specific]
parent: ../troubleshooting.md
last_validated: 2026-01-10
---

# Context and Requirements Issues

## Problem: Context Too Large

**Symptoms:**

- Hitting token limits
- Losing earlier context

**Solutions:**

1. **Focus on relevant files**
   - Read only files needed for current task
   - Don't load entire codebase

2. **Use sub-agents**
   - Delegate focused tasks to sub-agents
   - Each sub-agent has fresh context

3. **Summarise before continuing**
   - Document current state
   - Start fresh session with summary

---

## Problem: Unclear Requirements

**Symptoms:**

- Ticket is ambiguous
- Multiple interpretations possible

**Solutions:**

1. **Ask for clarification** (preferred)
   - Don't guess
   - Request specific details

2. **Reference existing patterns**
   - Check how similar features are implemented
   - Follow established conventions

3. **Document assumptions**
   - If proceeding with assumptions, document them
   - Make them visible for review

---

## Problem: Can't Find Relevant Code

**Symptoms:**

- Don't know where to make changes
- Unsure which files to modify

**Solutions:**

1. **Search for related terms**

   ```bash
   grep -r "RateLimit" --include="*.cs"
   ```

2. **Check project structure**
   - Review `docs/agents/ORIENTATION.md`
   - Check solution structure

3. **Use IDE navigation**
   - Find usages
   - Go to definition
   - Find implementations
