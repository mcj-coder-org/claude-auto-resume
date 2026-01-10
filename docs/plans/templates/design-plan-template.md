---
status: Draft
version: v1
issue: #{issue-number}
created: YYYY-MM-DD
last_updated: YYYY-MM-DD
---

# Design Plan: {Feature Name}

## Summary

Brief 2-3 sentence summary of what this feature does and why it's needed.

## Issue Context

**Parent Issue:** #{issue-number} - {Issue title}
**Epic Issue:** #{epic-number} (if sub-issue) - {Epic title}
**Related ADRs:** [ADR-XXXX](../adr/XXXX-topic.md)

## Refinement Participants

**Required Personas:**

- Product Owner
- Scrum Master
- Tech Lead
- QA Engineer

**Specialist Personas (if applicable):**

- Security Reviewer
- Senior Developer
- Documentation Specialist
- DotNet Developer

**Participants in this refinement:**

- @participant1 (Tech Lead)
- @participant2 (QA Engineer)
- @participant3 (Security Reviewer)

## Key Requirements

### Functional Requirements

1. **Requirement 1:** Description
   - Acceptance criterion: Specific, testable condition
   - Acceptance criterion: Another specific condition

2. **Requirement 2:** Description
   - Acceptance criterion: Specific, testable condition

### Non-Functional Requirements

- **Performance:** Response time < Xms, throughput > Y req/s
- **Scalability:** Handle X concurrent users
- **Reliability:** X% uptime, X retries on failure
- **Security:** Authentication, authorization, encryption requirements
- **Compatibility:** Cross-platform (Windows/macOS/Linux), .NET 10

## Architecture Approach

### High-Level Design

```
[Diagram or description of components and interactions]

┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Client    │────────▶│   Service    │────────▶│  Repository │
└─────────────┘         └──────────────┘         └─────────────┘
```

### Key Components

**Component 1: {Name}**

- **Responsibility:** What it does
- **Dependencies:** What it depends on
- **Public API:** Key methods/interfaces

**Component 2: {Name}**

- **Responsibility:** What it does
- **Dependencies:** What it depends on
- **Public API:** Key methods/interfaces

### Technology Choices

- **Framework/Library:** Choice and rationale
- **Data Storage:** Choice and rationale
- **External Services:** Choice and rationale

## Testing Approach

### Unit Tests

**Coverage Target:** 85% line coverage, 78% branch coverage

**Key Test Scenarios:**

- Happy path: Normal successful operation
- Edge case 1: Boundary condition
- Edge case 2: Another boundary
- Error case 1: Invalid input handling
- Error case 2: External dependency failure

**Test Organization:**

- Test project: `tests/Unit/{Namespace}.Tests`
- Test naming: `Method_Scenario_Expected`
- Use xUnit and FluentAssertions

### Integration Tests

**Scope:** Cross-component interactions

**Key Scenarios:**

- End-to-end flow through multiple components
- Database interactions
- External service integrations (mocked)

**Test Organization:**

- Test project: `tests/Integration/{Namespace}.Tests`

### System Tests (BDD)

**Scope:** Behavior changes visible to users

**Key Scenarios (Gherkin):**

```gherkin
Scenario: User performs action
  Given initial state
  When user action occurs
  Then expected outcome
```

**Test Organization:**

- Test project: `tests/System.Tests`
- Framework: Reqnroll (BDD)

### E2E Tests

**Scope:** User-facing flows (if applicable)

**Key Scenarios:**

- Complete user journey 1
- Complete user journey 2

### Manual Testing

**Test Cases:**

1. **Test Case 1:** Description
   - Steps: Step-by-step actions
   - Expected: What should happen

2. **Test Case 2:** Description
   - Steps: Step-by-step actions
   - Expected: What should happen

## Security Considerations

### Security Concerns

**Concern 1: {Type}**

- **Risk:** Description of the risk
- **Mitigation:** How we're addressing it
- **Validation:** How we'll verify the mitigation

**Concern 2: {Type}**

- **Risk:** Description of the risk
- **Mitigation:** How we're addressing it
- **Validation:** How we'll verify the mitigation

### Security Review Checklist

- [ ] Input validation on all boundaries
- [ ] No hardcoded secrets (use configuration/KeyVault)
- [ ] Authentication and authorization implemented
- [ ] Error messages don't leak sensitive information
- [ ] Secure communication (HTTPS, TLS)
- [ ] Process spawning is safe (if applicable)
- [ ] SQL injection prevention (parameterized queries)
- [ ] XSS prevention (if web UI)

## Breaking Changes

### Behavioral Changes

**Change 1:** Description

- **Impact:** Who/what is affected
- **Migration:** How users adapt
- **Communication:** Release notes, deprecation warnings

### API Changes

**Change 1:** Modified/removed API

- **Old:** `OldMethod(args)`
- **New:** `NewMethod(args)`
- **Breaking:** Yes/No
- **Migration:** Code changes required

### Contract Changes

**Change 1:** Modified request/response

- **Old Schema:** Description
- **New Schema:** Description
- **Breaking:** Yes/No
- **Versioning:** How we handle both versions

## Expected Artifacts

### New Documentation

- [ ] XML documentation for all public APIs
- [ ] README section: {Section name}
- [ ] User guide: {Guide name} (if applicable)
- [ ] API documentation: OpenAPI/Swagger specs

### Documentation Changes

- [ ] Update existing README section: {Section}
- [ ] Update ADR-XXXX: {ADR name}
- [ ] Update `docs/guides/{guide-name}.md`

### Deployable Components

- [ ] New NuGet package: `{PackageName}`
- [ ] New executable: `{ExecutableName}`
- [ ] Configuration changes: {Config file/service}
- [ ] Database migrations: {Migration name}
- [ ] Infrastructure changes: {What needs deployment}

## Deployment Strategy

### For Single Issues

**Deployment:** Standard merge to main, deploy to production

### For Epics - Choose One:

#### Strategy A: Feature Flags (Preferred)

**Feature Flags:**

| Flag Name                   | Sub-Issue | Purpose                                | Default State |
| --------------------------- | --------- | -------------------------------------- | ------------- |
| `ParentFeature`             | Epic      | Parent flag controlling entire feature | `disabled`    |
| `ParentFeature.SubFeature1` | #XXX      | Sub-feature 1                          | `disabled`    |
| `ParentFeature.SubFeature2` | #XXX      | Sub-feature 2                          | `disabled`    |

**Flag Configuration:**

- **Local Dev:** `appsettings.Development.json`
- **Production:** Azure App Configuration Service

**Enablement Plan:**

1. All sub-issues merge to main with flags disabled
2. Test each sub-feature independently by enabling its flag
3. When all sub-issues complete, enable parent flag
4. Monitor for 24 hours
5. Document enablement date in this plan

**Flag Removal:**

- **Target:** 2 releases after enablement
- **Ticket:** Create follow-up issue for flag removal

**Base Branch:** All sub-issues branch from `main`

#### Strategy B: Feature Branch (Fallback)

**Feature Branch:** `feature/{issue#}-{feature-name}`

**Process:**

1. Create feature branch from `main`
2. Keep feature branch rebased with `main` (epic owner responsibility)
3. All sub-issues branch from feature branch
4. Sub-issues merge to feature branch
5. Final epic PR merges feature branch → `main`

**Base Branch:** All sub-issues branch from `feature/{issue#}-{feature-name}`

**Why Strategy B:**

- Cannot safely deploy sub-issues independently
- Sub-features tightly coupled
- Breaking changes require all-or-nothing deployment

## Work Breakdown (For Epics)

### Sub-Issues

| Sub-Issue | Title   | Description       | Dependencies | Estimate |
| --------- | ------- | ----------------- | ------------ | -------- |
| #XXX      | {Title} | Brief description | None         | X days   |
| #XXX      | {Title} | Brief description | #YYY         | X days   |
| #XXX      | {Title} | Brief description | #YYY, #ZZZ   | X days   |

### Sub-Issue Links

- [ ] #XXX - {Sub-issue 1 title}
- [ ] #XXX - {Sub-issue 2 title}
- [ ] #XXX - {Sub-issue 3 title}

All sub-issues must link back to this design plan with immutable URL.

## Risks and Mitigations

| Risk               | Likelihood   | Impact       | Mitigation           |
| ------------------ | ------------ | ------------ | -------------------- |
| Risk 1 description | Low/Med/High | Low/Med/High | How we're mitigating |
| Risk 2 description | Low/Med/High | Low/Med/High | How we're mitigating |

## Dependencies

### Internal Dependencies

- **Dependency 1:** What we need from our codebase
- **Dependency 2:** Another internal dependency

### External Dependencies

- **Service/Library 1:** What we need externally
- **Service/Library 2:** Another external dependency

## Timeline

**Estimated Duration:** X weeks/sprints

**Milestones:**

- Refinement complete: YYYY-MM-DD
- Sub-issue 1 complete: YYYY-MM-DD
- Sub-issue 2 complete: YYYY-MM-DD
- Feature complete: YYYY-MM-DD
- Production deployment: YYYY-MM-DD

## Version History

| Version | Date       | Changes        | Discussion | Approved By   | Follow-up Issues |
| ------- | ---------- | -------------- | ---------- | ------------- | ---------------- |
| v1      | YYYY-MM-DD | Initial design | #{issue}   | Tech Lead, PO | -                |

**Note:** Amendments during implementation are appended here with new version numbers (v1.1, v1.2, etc.)

### Amendment Process

When implementation deviates from design or scope changes:

1. Append amendment to version history table
2. Update relevant sections below
3. Link to PR comment or issue discussion
4. Get Tech Lead approval
5. Update immutable links in sub-issues if needed

## Follow-up Issues

Issues created during implementation/review for future work:

| Issue      | Title | Reason | Target Release | Status |
| ---------- | ----- | ------ | -------------- | ------ |
| (none yet) | -     | -      | -              | -      |

**Note:** All follow-up issues should link back to this design plan and parent epic.

### When to Create Follow-ups

- Performance optimizations (if current performance acceptable)
- Additional test scenarios (if coverage adequate)
- Documentation improvements (if basics complete)
- Future enhancements identified during implementation
- Refactoring opportunities (if code acceptable)

### Not Acceptable for Follow-up

- Blockers (security, bugs, violations) - must fix in current PR
- Missing acceptance criteria - must complete now
- Broken tests - must fix now
- Missing required documentation - must add now

## Implementation Notes

### For Developers

Key implementation guidance:

- Important patterns to follow
- Common pitfalls to avoid
- Specific libraries/frameworks to use

### For Reviewers

What to focus on during code review:

- Critical paths requiring extra scrutiny
- Performance-sensitive areas
- Security-sensitive code
- Complex logic that needs careful review

## Success Criteria

**Definition of Done for this feature:**

- [ ] All sub-issues completed and merged
- [ ] All acceptance criteria met and verified
- [ ] All tests passing (unit, integration, system, E2E)
- [ ] Test coverage meets target (85% line, 78% branch)
- [ ] Documentation complete (XML docs, README, guides)
- [ ] Security review completed with no blockers
- [ ] Performance requirements met
- [ ] Feature flags enabled (Strategy A) OR feature branch merged (Strategy B)
- [ ] Monitoring shows stability (24h observation)
- [ ] No regressions in existing functionality
- [ ] ADRs updated to Accepted
- [ ] Design plan archived
- [ ] Feature flag removal tickets created (if Strategy A)

## Approval

**Design Approved By:**

- [ ] Product Owner: @username (YYYY-MM-DD)
- [ ] Tech Lead: @username (YYYY-MM-DD)
- [ ] Security Reviewer: @username (YYYY-MM-DD) [if required]

**Status:** `Draft` → `Approved v1` (when all approvals received)

**Approved Date:** YYYY-MM-DD

Ready to move to implementation.
