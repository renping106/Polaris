# Upgrade Options — Polaris

Assessment: 11 projects (all net8.0), AspNetCore + ClassLibrary + DotNetCoreApp (tests), 9 packages need update, all SDK-style.

## Strategy

### Upgrade Strategy

Simple modern-to-modern upgrade of all projects from .NET 8 to .NET 10. All projects are on modern .NET; no .NET Framework projects requiring tier-by-tier validation.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single pass. Fastest approach with no multi-targeting overhead. |
| Top-Down | Upgrade entry-point applications first, then consolidate shared libraries. Required only when solution must stay buildable during migration. |

