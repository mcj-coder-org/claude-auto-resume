---
name: feature-flags
description: |
  When implementing progressive feature delivery, runtime toggling, or deploying incomplete features safely. Apply for epic sub-issue deployment or feature rollout strategies.
decision: Use Microsoft.FeatureManagement with Azure App Configuration for production and appsettings for local development.
status: accepted
---

# ADR-0030: Feature Flags

## Status

Proposed

## Date

2026-01-10

## Context

We need a feature flag strategy to support:

1. Progressive delivery of large features (epics) to main branch
2. Independent deployment of sub-issues without affecting existing behavior
3. Runtime toggling of incomplete features
4. Testing features in production before full rollout
5. Quick rollback capability without code deployment

### Requirements

- Enable/disable features at runtime
- Support hierarchical features (epic → sub-features)
- Configuration-driven (no code changes to toggle)
- Type-safe feature checks in code
- Work across all deployment environments
- Support testing of both enabled and disabled states

### Options Considered

#### Option 1: Microsoft.FeatureManagement + Azure App Configuration (Selected)

Microsoft's feature management framework with cloud-based configuration.

**Pros:**

- Native .NET integration
- Type-safe feature flag checks
- Built-in dependency injection support
- Filters for targeting (percentage rollout, time windows, custom)
- Azure App Configuration provides:
  - Centralized management
  - Real-time updates without restart
  - Per-environment configuration
  - Audit logging
- Free tier available (1000 requests/day)

**Cons:**

- Azure dependency for cloud-based config
- Additional infrastructure component
- Network dependency for flag evaluation (can be mitigated with caching)

#### Option 2: Microsoft.FeatureManagement + Local Configuration

Feature management framework with appsettings.json configuration.

**Pros:**

- No external dependencies
- Simple for development
- No infrastructure cost

**Cons:**

- Requires deployment to change flags
- No centralized management across instances
- No real-time toggling in production

#### Option 3: LaunchDarkly or Similar SaaS

Third-party feature flag service.

**Pros:**

- Rich feature set
- Advanced targeting
- A/B testing support
- Excellent UI

**Cons:**

- External vendor dependency
- Cost ($$$)
- Overkill for project scope

## Decision

We will use **Microsoft.FeatureManagement** for feature flag implementation with a **hybrid configuration approach**:

- **Local development:** `appsettings.json`
- **Production/Azure deployments:** Azure App Configuration Service
- **Other cloud providers:** Environment variables or cloud-native config services

### When to Use Feature Flags

Feature flags are **required** when:

1. **Epic deployment (Strategy A):** Sub-issues deploy to main independently
   - Each sub-issue adds code behind feature flag
   - Feature remains hidden until epic complete
   - Epic completion = enable feature flag

2. **Breaking changes:** Changes that could impact existing behavior
   - New behavior behind flag
   - Old behavior remains default
   - Gradual rollout after testing

3. **Experimental features:** Features requiring production validation
   - Enable for testing
   - Disable if issues found
   - Full rollout after confidence established

Feature flags are **optional** for:

- Small, self-contained features (single PR, no risk)
- Bug fixes that don't change behavior
- Internal refactoring with no external impact

### When to Remove Feature Flags

Feature flags are **temporary** and must be removed:

**Timing:**

- After feature is fully rolled out
- After monitoring confirms stability
- Maximum lifetime: **2 releases** after full rollout

**Process:**

1. Enable flag in all environments
2. Monitor for 1-2 releases
3. Create cleanup issue to remove flag
4. Remove flag check from code, make behavior default
5. Remove flag configuration
6. Document removal in changelog

**Exceptions for permanent flags:**

- A/B testing scenarios (explicitly documented)
- Premium/feature tier toggles
- Platform-specific features
- Must be documented as permanent in ADR amendment

### Implementation Patterns

#### Basic Feature Flag

**Configuration (appsettings.json):**

```json
{
  "FeatureManagement": {
    "RateLimitDetection": false,
    "AutoRetryOnLimit": false
  }
}
```

**Configuration (Azure App Configuration):**

```text
FeatureManagement:RateLimitDetection = false
FeatureManagement:AutoRetryOnLimit = false
```

**Code Usage:**

```csharp
using Microsoft.FeatureManagement;

public class MonitorService
{
    private readonly IFeatureManager _featureManager;

    public MonitorService(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task<Response> ProcessResponseAsync(
        Response response,
        CancellationToken cancellationToken)
    {
        if (await _featureManager.IsEnabledAsync("RateLimitDetection"))
        {
            // New behavior: Check for rate limits
            if (IsRateLimitResponse(response))
            {
                return await HandleRateLimitAsync(response, cancellationToken);
            }
        }

        // Existing behavior
        return response;
    }
}
```

#### Dependency Injection Setup

**Program.cs or Startup.cs:**

```csharp
// Local development
builder.Services.AddFeatureManagement();

// Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
           .UseFeatureFlags(featureFlagOptions =>
           {
               featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(5);
           });
});

builder.Services.AddFeatureManagement()
                .AddFeatureFilter<PercentageFilter>()
                .AddFeatureFilter<TimeWindowFilter>();

builder.Services.AddAzureAppConfiguration();
```

#### Testing with Feature Flags

**Unit Tests:**

```csharp
[Fact]
public async Task ProcessResponse_WhenRateLimitDetectionEnabled_DetectsRateLimit()
{
    // Arrange
    var featureManager = new Mock<IFeatureManager>();
    featureManager.Setup(x => x.IsEnabledAsync("RateLimitDetection"))
                  .ReturnsAsync(true);

    var service = new MonitorService(featureManager.Object);

    // Act & Assert
    // ...
}

[Fact]
public async Task ProcessResponse_WhenRateLimitDetectionDisabled_UsesOldBehavior()
{
    // Arrange
    var featureManager = new Mock<IFeatureManager>();
    featureManager.Setup(x => x.IsEnabledAsync("RateLimitDetection"))
                  .ReturnsAsync(false);

    // Test old behavior still works
}
```

**System Tests (BDD):**

```gherkin
@featureflag:RateLimitDetection=true
Scenario: Rate limit is detected and handled
  Given rate limit detection is enabled
  When the API returns a rate limit response
  Then the system should retry after the specified delay

@featureflag:RateLimitDetection=false
Scenario: Rate limit detection disabled falls back to old behavior
  Given rate limit detection is disabled
  When the API returns a rate limit response
  Then the system should process it as a normal response
```

#### Hierarchical Features (Epic)

**For Epic with multiple sub-issues:**

```json
{
  "FeatureManagement": {
    "ClaudeMonitoring": false,
    "ClaudeMonitoring.RateLimitDetection": false,
    "ClaudeMonitoring.AutoRetry": false,
    "ClaudeMonitoring.MetricsCollection": false
  }
}
```

**Code Pattern:**

```csharp
// Parent feature check
if (await _featureManager.IsEnabledAsync("ClaudeMonitoring"))
{
    // Sub-feature checks
    if (await _featureManager.IsEnabledAsync("ClaudeMonitoring.RateLimitDetection"))
    {
        // Sub-feature behavior
    }
}
```

**Rollout Process:**

1. Sub-issues merge with parent flag `ClaudeMonitoring = false`
2. Enable sub-feature flags individually for testing: `ClaudeMonitoring.RateLimitDetection = true`
3. When all sub-issues complete, enable parent: `ClaudeMonitoring = true`
4. After stability confirmed, remove all flags

### Feature Flag Naming Conventions

| Pattern                  | Example                      | Use Case                   |
| ------------------------ | ---------------------------- | -------------------------- |
| `FeatureName`            | `RateLimitDetection`         | Simple feature             |
| `FeatureName.SubFeature` | `ClaudeMonitoring.AutoRetry` | Sub-feature of epic        |
| `FeatureName.Variant`    | `SearchEngine.ElasticSearch` | Alternative implementation |

**Rules:**

- PascalCase naming
- Descriptive, not abbreviated
- Maximum 3 levels deep
- Prefix with epic/parent for sub-features

### Configuration Strategy

#### Local Development (appsettings.Development.json)

```json
{
  "FeatureManagement": {
    "NewFeature": true // Typically enabled for active development
  }
}
```

#### CI/CD Environments

| Environment | Configuration Source    | Flag Default |
| ----------- | ----------------------- | ------------ |
| Development | appsettings.Development | true         |
| Testing     | appsettings.Test        | true         |
| Staging     | Azure App Configuration | true         |
| Production  | Azure App Configuration | false        |

#### Azure App Configuration Setup

**Connection String:**

```bash
# Store in Azure Key Vault or GitHub Secrets
AZURE_APPCONFIG_CONNECTION_STRING="Endpoint=https://..."
```

**Managed Identity (Recommended for Production):**

```csharp
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
           .UseFeatureFlags();
});
```

### Monitoring and Observability

**Telemetry Events:**

```csharp
public async Task<bool> IsEnabledAsync(string feature)
{
    var enabled = await _featureManager.IsEnabledAsync(feature);

    _telemetry.TrackEvent("FeatureFlagEvaluated", new Dictionary<string, string>
    {
        { "FeatureName", feature },
        { "Enabled", enabled.ToString() },
        { "Environment", _environment.EnvironmentName }
    });

    return enabled;
}
```

**Key Metrics:**

- Feature flag evaluation count
- Feature flag enabled/disabled ratio
- Feature flag age (time since creation)
- Feature flag removal rate

### Documentation Requirements

**In Design Plan:**

```markdown
## Feature Flags

| Flag Name                  | Sub-Issue | Purpose                     | Removal Target |
| -------------------------- | --------- | --------------------------- | -------------- |
| ClaudeMonitoring           | #123      | Parent epic flag            | v2.1.0         |
| ClaudeMonitoring.RateLimit | #124      | Enable rate limit detection | v2.1.0         |
| ClaudeMonitoring.AutoRetry | #125      | Enable automatic retry      | v2.1.0         |
```

**In Code (XML Comments):**

```csharp
/// <summary>
/// Processes Claude API responses with optional rate limit detection.
/// </summary>
/// <remarks>
/// Feature flag: ClaudeMonitoring.RateLimitDetection
/// Introduced: v2.0.0 (Issue #124)
/// Planned removal: v2.1.0
/// </remarks>
```

**In CHANGELOG:**

```markdown
## [2.0.0] - 2026-01-15

### Added (Feature Flagged)

- Rate limit detection (flag: `ClaudeMonitoring.RateLimitDetection`, disabled by default)
- Automatic retry on rate limit (flag: `ClaudeMonitoring.AutoRetry`, disabled by default)

## [2.1.0] - 2026-02-15

### Changed

- Rate limit detection now enabled by default (feature flag removed)
```

### Migration Path for Existing Features

If features were deployed without flags:

1. **Wrap with flag:** Add feature flag around new behavior
2. **Default to enabled:** Match current production state
3. **Test disabled state:** Ensure fallback works
4. **Gradually roll out:** Use percentage filters if available
5. **Remove flag:** After confidence established

## Consequences

### Positive

- Safe incremental delivery of large features
- Quick rollback without deployment
- Test in production with limited blast radius
- Clear separation of complete vs incomplete features
- Supports trunk-based development
- Reduced merge conflicts (all to main)

### Negative

- Code complexity (if/else branches)
- Test coverage requirements increase (both paths)
- Technical debt if flags not removed
- Configuration management overhead
- Potential for flag proliferation

### Risks

- **Flag debt:** Flags left in code long-term
  - **Mitigation:** Mandatory removal timeline (2 releases), cleanup issues created
- **Configuration drift:** Different flag states across environments
  - **Mitigation:** Azure App Configuration centralization, IaC for config
- **Testing complexity:** Must test both enabled/disabled states
  - **Mitigation:** Test tagging system, CI runs both scenarios
- **Runtime dependency:** Azure App Configuration outage
  - **Mitigation:** Local caching, fallback to default values

### Maintenance Process

**Quarterly Review:**

1. List all active feature flags
2. Identify flags older than 2 releases
3. Create removal issues
4. Prioritize cleanup

**Flag Lifecycle:**

```text
Created → In Use → Fully Rolled Out → Scheduled for Removal → Removed
  ↓         ↓            ↓                    ↓                  ↓
Issue#   2 releases    2 releases         Cleanup Issue      Complete
         monitoring    observation         created
```

## References

- [Microsoft.FeatureManagement Documentation](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core)
- [Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview)
- [Feature Toggles (Martin Fowler)](https://martinfowler.com/articles/feature-toggles.html)
- [ADR-0004: Contribution Workflow](./0004-contribution-workflow.md)
- [ADR-0003: Work Item Management](./0003-work-item-management.md)
