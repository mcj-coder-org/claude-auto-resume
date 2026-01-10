---
title: Code Review Practices
summary: Code review criteria, process, and checklist for the project
audience: [developer, agent]
topics: [code-review, pull-request, quality, process]
prerequisites: []
related: [../standards/coding-standards.md, ../adr/0004-contribution-workflow.md]
last_validated: 2026-01-09
---

# Code Review Practices

This document defines the code review process and criteria for the
McjCoderOrg.ClaudeAutoResume project. All code changes must be reviewed
before merging to main.

## Review Philosophy

Code review serves multiple purposes:

1. **Quality assurance** - Catch bugs and issues before they reach main
2. **Knowledge sharing** - Spread understanding of the codebase
3. **Consistency** - Ensure adherence to standards
4. **Improvement** - Learn from each other

### Key Principles

- **Be constructive** - Suggest improvements, don't just criticise
- **Be specific** - Point to exact lines, provide examples
- **Be timely** - Review within 24 hours when possible
- **Be thorough** - Check against all criteria, not just obvious issues
- **Be respectful** - Focus on the code, not the author

## Review Process

### For Authors

1. **Self-review first**
   - Review your own diff before requesting review
   - Check against the review checklist below
   - Fix obvious issues before involving reviewers

2. **Create quality PR**
   - Use the PR template
   - Write clear description of changes
   - Link to related issue
   - Include testing evidence

3. **Respond to feedback**
   - Address all comments
   - Explain reasoning if disagreeing
   - Push fixes promptly
   - Re-request review when ready

### For Reviewers

1. **Understand the context**
   - Read the linked issue
   - Understand the goal
   - Check the PR description

2. **Review thoroughly**
   - Check against all criteria
   - Run the code if needed
   - Test edge cases mentally

3. **Provide actionable feedback**
   - Be specific about issues
   - Suggest fixes or alternatives
   - Distinguish blockers from suggestions

4. **Approve or request changes**
   - Approve if all criteria met
   - Request changes for blockers
   - Don't block on nitpicks

## Review Criteria

### Correctness

**Does the code do what it's supposed to do?**

- [ ] Implements the requirements from the linked issue
- [ ] Handles edge cases appropriately
- [ ] No obvious bugs or logic errors
- [ ] Error handling is appropriate
- [ ] Works correctly with null/empty inputs

### Code Quality

**Is the code well-written and maintainable?**

- [ ] Follows coding standards (`docs/standards/coding-standards.md`)
- [ ] Uses appropriate abstractions
- [ ] No unnecessary complexity
- [ ] DRY - no duplicated logic
- [ ] YAGNI - no additional features added
- [ ] Single Responsibility - classes/methods do one thing
- [ ] Meaningful names for variables, methods, classes
- [ ] No dead code or commented-out code

### Testing

**Is the code adequately tested?**

- [ ] New code has corresponding tests
- [ ] New Tests have been executed by reviewer (or CI)
- [ ] Tests cover happy path and edge cases
- [ ] Tests are meaningful (not just for coverage)
- [ ] Test names describe what's being tested
- [ ] No flaky tests introduced
- [ ] BDD scenarios updated if behaviour change is in scope of expected feature

### Documentation

**Is the code properly documented?**

- [ ] Public APIs have XML documentation
- [ ] API's have documented Open API contracts (with examples)
- [ ] Complex logic has explanatory comments
- [ ] README (or relevant in-repo documents) updated if needed
- [ ] No misleading or outdated comments

### Performance

**Are there performance concerns?**

- [ ] No obvious performance issues
- [ ] No unnecessary allocations in hot paths
- [ ] Async used where appropriate
- [ ] No blocking calls that should be async

### Security

**Are there security concerns?**

- [ ] No hardcoded secrets
- [ ] Input validation on boundaries
- [ ] No injection vulnerabilities
- [ ] Error messages don't leak sensitive info
- [ ] Process spawning is safe

### Compatibility

**Does the code work across environments?**

- [ ] Cross-platform considerations (Windows/macOS/Linux)
- [ ] No breaking changes to public API (or properly documented)
- [ ] Backwards compatible where expected

## Comment Format

### Inline Comments (Preferred)

**Use file/line-based comments for code-specific feedback:**

- Post comments on specific files and lines using GitHub's inline comment feature
- Start conversation threads on relevant code
- Tag with comment type prefix (blocker, issue, etc.)
- Keep discussion focused on the specific code location

**Example Inline Comment (File: `src/Core/RateLimitDetector.cs`, Line 45):**

```csharp
blocker: This can throw NullReferenceException if `response` is null.
Add a null check or use null-conditional operator: `response?.Headers`
```

### PR-Level Comments

**Use general PR comments only for:**

- Overall architecture feedback
- Cross-cutting concerns
- High-level summary
- Process feedback
- Review summary (after inline comments posted)

### Conversation Resolution

#### Critical Rule: Reviewer resolves conversations, not author

**Conversation Flow:**

1. **Reviewer opens conversation** (inline on specific line)
2. **Author responds** in the conversation thread:
   - Explains what was changed
   - Links to commit with fix
   - Asks for clarification if needed
3. **Reviewer verifies and resolves** the conversation when satisfied
4. **Author does NOT resolve conversations** (even after fixing)

**Example Conversation:**

**Reviewer (Line 45):**

```text
blocker: Missing null check for `response` parameter
```

**Author response:**

```text
✅ Fixed in commit abc123f

Added null guard clause:
if (response == null) throw new ArgumentNullException(nameof(response));

Also added unit test: `Detect_WhenResponseIsNull_ThrowsArgumentNullException`
```

**Reviewer verifies and resolves:**

```text
Verified in abc123f. Looks good! ✓
[Resolves conversation]
```

**All conversations must be resolved before merge** (enforced by branch protection)

## Comment Types

Use these prefixes to categorise comments:

| Prefix        | Meaning                     | Blocks Merge |
| ------------- | --------------------------- | ------------ |
| `blocker:`    | Must fix before merge       | Yes          |
| `issue:`      | Should fix, but can discuss | Usually      |
| `suggestion:` | Nice to have, optional      | No           |
| `question:`   | Clarification needed        | Maybe        |
| `nit:`        | Minor style/formatting      | No           |
| `praise:`     | Highlight good work         | No           |

### Example Comments

**Blocker:**

```text
blocker: This can throw NullReferenceException if `config` is null.
Consider adding a null check or using null-conditional operator.
```

**Issue:**

```text
issue: This allocates a new Regex on every call. Consider making it
a static readonly field to avoid repeated compilation.
```

**Suggestion:**

```text
suggestion: This could be simplified using pattern matching:
if (result is { Success: true, Value: var value })
```

**Question:**

```text
question: Is it intentional that this returns null instead of throwing?
The caller doesn't seem to handle null.
```

**Nit:**

```text
nit: Variable name `x` could be more descriptive, perhaps `retryCount`?
```

**Praise:**

```text
praise: Nice use of `Span<T>` here - avoiding allocations in this hot path.
```

## Review Checklist

### Quick Review (< 50 lines)

- [ ] PR title follows conventional commit format
- [ ] Changes match PR description
- [ ] Code compiles and tests pass
- [ ] No obvious bugs
- [ ] Follows coding standards

### Standard Review (50-300 lines)

All quick review items, plus:

- [ ] Thoroughly read every changed line
- [ ] Check test coverage for changes
- [ ] Verify error handling
- [ ] Check for security issues
- [ ] Verify documentation is updated

### Large Review (> 300 lines)

All standard review items, plus:

- [ ] Consider requesting split into smaller PRs
- [ ] Review architecture/design decisions
- [ ] Check integration with existing code
- [ ] Verify no unintended side effects
- [ ] May need multiple reviewers

## Approval Requirements

| Change Type        | Required Approvals | Special Requirements   |
| ------------------ | ------------------ | ---------------------- |
| Bug fix            | 1                  | None                   |
| New feature        | 1                  | Tests required         |
| Breaking change    | 1                  | ADR update required    |
| Security fix       | 1                  | Expedited review       |
| Documentation only | 1                  | Spelling/grammar check |
| Infrastructure/CI  | 1                  | Test in non-prod first |

## Handling Disagreements

When author and reviewer disagree:

1. **Discuss in PR comments** - Explain reasoning clearly
2. **Provide evidence** - Link to docs, examples, benchmarks
3. **Consider alternatives** - Find a middle ground
4. **Escalate if needed** - Get another opinion
5. **Document decision** - Update ADR if architectural

### When to Defer

- Style preferences not covered by standards → defer to author
- Equally valid approaches → defer to author
- Minor improvements unrelated to PR scope → create follow-up issue

### When to Push Back

- Security vulnerabilities → must fix
- Violations of coding standards → must fix
- Breaking changes without documentation → must fix
- Missing tests for critical code → must fix

## Response Time Expectations

| PR Size               | Expected Response |
| --------------------- | ----------------- |
| Small (< 50 lines)    | Same day          |
| Medium (50-300 lines) | Within 24 hours   |
| Large (> 300 lines)   | Within 48 hours   |

If you can't review in time:

- Comment that you'll review later
- Suggest another reviewer
- Don't leave PRs hanging without communication

## Automated Checks

These checks must pass before review:

| Check         | What It Verifies               |
| ------------- | ------------------------------ |
| `lint`        | Formatting, spelling, secrets  |
| `build`       | Code compiles on all platforms |
| `test-unit`   | Unit tests pass                |
| `test-system` | BDD system tests pass          |
| `test-arch`   | Architecture rules enforced    |
| `codeql`      | Security analysis              |
| `pr-title`    | Conventional commit format     |

Reviewers should not duplicate what automated checks verify.

## Agent-Specific Guidance

### For AI Reviewers

When performing code review as an AI agent:

1. **Adopt the appropriate persona** - See `docs/agents/PERSONAS.md`
2. **Structure your review** - Use the comment prefixes
3. **Be specific** - Point to exact lines
4. **Provide examples** - Show how to fix issues
5. **Prioritise** - Focus on blockers first

### Review Output Format

```markdown
## Code Review: PR #{number}

### Summary

[1-2 sentence overview of the changes]

### Blockers

[Must fix before merge - empty if none]

### Issues

[Should fix - list each issue]

### Suggestions

[Optional improvements]

### Questions

[Clarifications needed]

### Positive Observations

[Good patterns, well-done sections]

### Checklist Verification

- [x] Follows coding standards
- [x] Has appropriate tests
- [ ] Documentation updated (missing XML docs on `NewMethod`)
- [x] No security concerns
- [x] Performance acceptable
```

## Common Review Mistakes

### For Authors

- Not self-reviewing before requesting review
- Not running tests locally
- Ignoring automated check failures
- Large PRs that are hard to review
- Vague PR descriptions
- Not responding to feedback promptly

### For Reviewers

- Rubber-stamping without thorough review
- Blocking on style preferences
- Not explaining the reasoning for requests
- Reviewing only parts of the change
- Conflating blockers with suggestions
- Personal criticism instead of constructive feedback

## Continuous Improvement

After each review:

1. **Authors:** Consider if feedback reveals knowledge gaps
2. **Reviewers:** Consider if issues should be caught by automation
3. **Team:** Update standards if patterns emerge

Add new items to this document when:

- Recurring issues are found in reviews
- New tools or patterns are adopted
- Team agreements are made
