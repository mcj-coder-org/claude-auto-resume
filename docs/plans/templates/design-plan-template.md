---
status: Draft
version: v1
issue: #{issue-number}
created: YYYY-MM-DD
last_updated: YYYY-MM-DD
---

# Design Plan: {Feature Name}

## Version History

| Version | Date       | Changes        | Discussion | Approved By   | Follow-up Issues |
| ------- | ---------- | -------------- | ---------- | ------------- | ---------------- |
| v1      | YYYY-MM-DD | Initial design | #{issue}   | Tech Lead, PO | -                |

**Note:** Amendments during implementation are appended here with new version numbers (v1.1, v1.2, etc.)

## Approval

**Design Approved By:**

- [ ] Product Owner: @username (YYYY-MM-DD) #{Approval link}
- [ ] Tech Lead: @username (YYYY-MM-DD) #{Approval link}
- [ ] Security Reviewer: @username (YYYY-MM-DD) [if required] #{Approval link}
- [ ] Architecture Reviewer: @username (YYYY-MM-DD) [if required] #{Approval link}
- [ ] QA Reviewer: @username (YYYY-MM-DD) [if required] #{Approval link}

**Status:** `Draft` → `Approved v1` (when all approvals received)
**Approved Date:** YYYY-MM-DD
Ready to move to implementation.

### Amendment #N Approval (if applicable)

- [ ] Product Owner: @username (YYYY-MM-DD) #{Approval link}
- [ ] Tech Lead: @username (YYYY-MM-DD) #{Approval link}
- [ ] {Specialist} Reviewer: @username (YYYY-MM-DD) [if required] #{Approval link}

**Status:** `Draft` → `Approved v1.N` (when all approvals received)
**Approved Date:** YYYY-MM-DD
Ready to move to implementation.

---

## Summary

Brief 2–3 sentence summary of what this feature does and why it's necessary.

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
- **Security:** Authentication, authorisation, encryption requirements
- **Compatibility:** Cross-platform (Windows/macOS/Linux), .NET 10
- **Documentation:** Complete and up to date
- **Stability:** No regressions in existing functionality
- **Versioning:** Appropriate Semantic versioning for Components and Contracts

## Success Criteria

**Definition of Done for this feature:**

- [ ] All sub-issues are completed and merged
- [ ] All acceptance criteria are met and verified
- [ ] All tests passing (unit, integration, system, E2E)
- [ ] Test coverage meets the target (85% line, 78% branch)
- [ ] Documentation complete (XML docs, README, guides)
- [ ] Security review completed with no blockers
- [ ] Performance requirements met
- [ ] Feature flags enabled (Strategy A) OR feature branch merged (Strategy B)
- [ ] Monitoring shows stability (24h observation)
- [ ] No regressions in existing functionality
- [ ] ADRs updated to Accepted
- [ ] Design plan archived
- [ ] Feature flag removal tickets created (if Strategy A)

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
- Use xUnit and AwesomeAssertions

### Integration Tests

**Scope:** Cross-component interactions

**Key Scenarios:**

- End-to-end flow through multiple components
- Database interactions
- External service integrations (mocked)

**Test Organization:**

- Test project: `tests/Integration/{Namespace}.Tests`

### System Tests (BDD)

**Scope:** Behaviour changes visible to users

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
- [ ] Authentication and authorisation implemented
- [ ] Error messages don't leak sensitive information
- [ ] Secure communication (HTTPS, TLS)
- [ ] Process spawning is safe (if applicable)
- [ ] SQL injection prevention (parameterized queries)
- [ ] XSS prevention (if web UI)

## Breaking Changes

### Behavioural Changes

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

## Expected Artefacts

### New Documentation

- [ ] XML documentation for all public APIs
- [ ] README section: {Section name}
- [ ] User guide: {Guide name} (if applicable)
- [ ] API documentation: OpenAPI/Swagger specs

### Documentation Changes

- [ ] Update the existing README section: {Section}
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

### For Epics – Choose One:

#### Strategy A: Feature Flags (Preferred)

**Feature Flags:**

**CRITICAL**: No breaking changes or behaviour changes are allowed in this strategy while feature flags are disabled.

| Flag Name                   | Sub-Issue | Purpose                                | Default State |
| --------------------------- | --------- | -------------------------------------- | ------------- |
| `ParentFeature`             | Epic      | Parent flag controlling entire feature | `disabled`    |
| `ParentFeature.SubFeature1` | #XXX      | Sub-feature 1                          | `disabled`    |
| `ParentFeature.SubFeature2` | #XXX      | Sub-feature 2                          | `disabled`    |

**Flag Configuration:**

- **Local Dev:** `appsettings.Development.json`
- **Production:** Azure App Configuration Service

**Enablement Plan:**

1. All sub-issues merge to the main with flags disabled
2. Test each sub-feature independently by enabling its flag
3. When all sub-issues complete, enable parent flag
4. Monitor for 24 hours
5. Document enablement date in this plan

**Flag Removal:**

- **Target:** two releases after enablement
- **Ticket:** Create a follow-up issue for flag removal

**Base Branch:** All sub-issues branch from `main`

#### Strategy B: Feature Branch (Fallback)

**Feature Branch:** `feature/{issue#}-{feature-name}`

**Process:**

1. Create feature branch from `main`
2. Keep feature branch rebased with `main` (epic owner responsibility)
3. All sub-issues branch from the feature branch
4. Sub-issues merge to feature branch
5. Final epic PR merges feature branch → `main`

**Base Branch:** All sub-issues branch from `feature/{issue#}-{feature-name}`

**Why Strategy B:**

- Cannot safely deploy sub-issues independently
- Sub-features tightly coupled
- Breaking changes require all-or-nothing deployment

## Work Breakdown

### Strategy & Granularity

- **For Epics:** Provide a high-level breakdown at the component/deliverable unit level. Capture the epic's requirements
  and deliverables into the sub-tickets listed below. Explain how the sub-tickets interrelate and which skill sets should
  be involved in each.
- **For Sub-issues/Single Ticket Features:** Provide a detailed plan with a task breakdown. Each task should be small enough (1–4 hours) to be
  completed by a single person or role.

### Sub-Issues

| Sub-Issue      | Title   | Description       | Skillsets Required | Dependencies | Estimate |
| -------------- | ------- | ----------------- | ------------------ | ------------ | -------- |
| [#XXX]({Link}) | {Title} | Brief description | e.g. skill:dotnet  | None         | X days   |
| [#XXX]({Link}) | {Title} | Brief description | e.g. skill:testing | #YYY         | X days   |

All sub-issues must link back to this ticket and the design plan with an immutable URL.

### Task Breakdown

| Task | Description | Role/Skillset | Estimate |
| ---- | ----------- | ------------- | -------- |
| 1    | {Task name} | e.g. DotNet   | 2h       |
| 2    | {Task name} | e.g. QA       | 1h       |

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

---

## Follow-up Issues

Issues created during implementation/review for future work:

| Issue      | Title | Reason | Target Release | Status |
| ---------- | ----- | ------ | -------------- | ------ |
| (none yet) | -     | -      | -              | -      |

**Note:** All follow-up issues should link back to this design plan and parent epic.

### When to Create Follow-ups

- Performance optimisations (if current performance is acceptable)
- Additional test scenarios (if coverage adequate)
- Documentation improvements (if basics are complete)
- Future enhancements identified during implementation
- Refactoring opportunities (if code acceptable)

### Not Acceptable for Follow-up

- Blockers (security, bugs, violations) – must fix in current PR
- Missing acceptance criteria – must be complete now
- Broken tests must be fixed now
- Missing required documentation – must add now

### Amendment Process

When implementation deviates from design or scope changes:

1. Append amendment to the version history table and Approvals header
2. Append "Amendment #N" Section to the end of the design plan
3. Add update details to relevant sections within the amendment section (diff from original details)
4. Link to PR comment or issue discussion
5. Get Tech Lead approval (Approval link)
6. Get Product Owner approval (Approval link) (if applicable/change in scope/functionality)
7. Add updated immutable links in sub-issues if needed

---

## Retrospective

After implementation has been approved and verified

**Completed:** YYYY-MM-DD
**Participants:** @participant1, @participant2, @participant3

### What Went Well

- Success 1: Description of what worked well
- Success 2: Another positive outcome
- Success 3: Process or technical win

### What Could Be Improved

- Challenge 1: What didn't go as planned
  - **Root Cause:** Why it happened
  - **Impact:** How it affected the project
- Challenge 2: Another area for improvement
  - **Root Cause:** Why it happened
  - **Impact:** How it affected the project

### Action Items

| Action                                      | Owner     | Target Date | Status      |
| ------------------------------------------- | --------- | ----------- | ----------- |
| Action item 1 to improve future work (link) | @username | YYYY-MM-DD  | Open/Closed |
| Action item 2 to improve future work (link) | @username | YYYY-MM-DD  | Open/Closed |

### Metrics

**Planned vs Actual:**

- **Estimated Effort:** X days/weeks
- **Actual Effort:** Y days/weeks
- **Variance:** +/- Z days/weeks

**Quality Metrics:**

- **Test Coverage Achieved:** X% line, Y% branch
- **Bugs Found Post-Release:** X (P0: Y, P1: Z)
- **Rework Required:** X% of total effort

**Deployment Metrics:**

- **Time to Production:** X days from approval
- **Rollback Required:** Yes/No
- **Monitoring Alerts:** X incidents in first 24h

### Lessons Learned

Key takeaways for future similar work:

1. **Technical:** Specific technical lesson
2. **Process:** Specific process lesson
3. **Communication:** Specific communication lesson
