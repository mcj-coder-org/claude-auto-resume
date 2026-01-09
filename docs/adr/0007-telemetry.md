# ADR-0007: Telemetry

## Status

Proposed

## Date

2026-01-09

## Context

We need to decide whether to collect usage telemetry.

### Options Considered

#### Option 1: No Telemetry (Selected)

No usage data collection.

**Pros:**
- Privacy-first
- No GDPR/privacy concerns
- Simpler implementation
- User trust

**Cons:**
- No usage insights
- No crash reporting
- Harder to prioritize features

#### Option 2: Opt-in Telemetry

Anonymous usage data with user consent.

**Pros:**
- Usage insights
- Crash reporting

**Cons:**
- Privacy concerns
- Opt-in fatigue
- Infrastructure needed

#### Option 3: Opt-out Telemetry

Telemetry by default, can disable.

**Pros:**
- Higher data collection
- Better insights

**Cons:**
- Privacy concerns
- User trust issues
- Potential backlash

## Decision

**No telemetry**. Privacy-first approach.

### Rationale

1. CLI tool wrapping another CLI - minimal value from telemetry
2. Open source project - community feedback via issues
3. Privacy is increasingly important to users
4. Avoids GDPR/CCPA compliance complexity

### Alternative Feedback Mechanisms

- GitHub Issues for bug reports
- GitHub Discussions for feature requests
- `--diagnose` command for user-generated diagnostics

## Consequences

### Positive
- Complete privacy
- No infrastructure needed
- User trust
- No legal compliance overhead

### Negative
- No usage metrics
- No crash analytics
- Rely on user-reported issues

## References

- [GDPR](https://gdpr.eu/)
- [Do Not Track](https://www.eff.org/issues/do-not-track)
