# .NET 10 Upgrade Scenario

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Upgrade Options
- **Upgrade Strategy**: All-at-Once

## Strategy
**Selected**: All-at-Once
**Rationale**: 11 projects, all modern .NET 8 with straightforward TFM bump. No .NET Framework migration complexity or CI-green constraints requiring incremental buildability.

### Execution Constraints
- Single atomic upgrade pass — all projects updated together
- No tier-by-tier validation needed (modern-to-modern upgrade)
- Solution may be temporarily unbuildable until all projects complete
- Full solution build verification at the end

## Source Control
- **Source Branch**: master
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Reminders & Deferred Items

### Package Vulnerability Remediation (Future)
**Deferred**: NuGet vulnerability advisory management for pre-existing issues:
- `Scriban` 5.9.0 — Multiple critical/high severity vulnerabilities
- `Microsoft.Data.SqlClient` 5.1.1 — High severity
- `SQLitePCLRaw.lib.e_sqlite3` 2.1.6 — High severity
- `System.IdentityModel.Tokens.Jwt` 7.0.3 — Moderate severity
- `System.Linq.Dynamic.Core` 1.3.5 — High severity

**Status**: Excluded from build errors via `WarningsNotAsErrors` in `Directory.Build.props`. These vulnerabilities existed before the .NET 10 upgrade and are managed separately. Plan package updates as a post-upgrade task.

**Recommended**: Schedule a separate security audit and package update cycle to address these advisories.

