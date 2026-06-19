# Upgrade Plan — .NET 10 Migration

## Strategy

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.

**Rationale**: 11 projects, all on modern .NET 8, clear 2-3 tier dependency structure (foundation libraries → domain libraries → applications). No framework migration complexity; straightforward TFM bump with 9 package updates.

---

## Prerequisites & Validation

### 01-prerequisites: Verify .NET 10 SDK and environment

Ensure the .NET 10 SDK is installed and configured correctly. Verify global.json compatibility (if present) and confirm development environment supports the target framework.

**Done when**:
- .NET 10 SDK confirmed installed locally
- `dotnet --version` returns .NET 10
- global.json (if present) is compatible with .NET 10 or will be updated
- Solution can be opened in Visual Studio

---

## Core Upgrade Tasks

### 02-foundation-libs: Upgrade foundation and infrastructure libraries

Upgrade the lowest-tier class libraries that have no dependencies on other solution projects. These include: `Polaris.Abp.Extension.Abstractions`, `Polaris.Abp.DatabaseManagement`, `Polaris.Abp.DatabaseManagement.Sqlite`, `Polaris.Abp.DatabaseManagement.SqlServer`.

Update target frameworks from `net8.0` to `net10.0`, bump all NuGet packages to versions compatible with .NET 10, and apply any necessary code fixes for breaking API changes in the framework or packages.

**Done when**:
- Foundation library projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings
- No API breaking changes remain unaddressed

### 03-domain-business-logic: Upgrade domain and business logic libraries

Upgrade the mid-tier libraries that build on foundation libraries: `Polaris.Abp.PluginManagement`, `Polaris.Abp.ThemeManagement`, `Polaris.Abp.DatabaseManagement.Tests`, `Abp.CmsKit`.

Update target frameworks to `net10.0` and bump all NuGet packages. Verify that references to foundation libraries (now on .NET 10) resolve correctly and that no additional code changes are needed.

**Done when**:
- Domain library projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings
- Proper references to upgraded foundation libraries

### 04-test-infrastructure: Upgrade test support and helper libraries

Upgrade test-focused libraries: `Polaris.Abp.TestBase`. Update target framework to `net10.0` and bump packages. These changes support test projects.

**Done when**:
- Test infrastructure projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings

### 05-application-host: Upgrade main host and theme projects

Upgrade the application host and theme projects that consume the upgraded libraries: `Polaris.Abp.Host` (AspNetCore), `Polaris.Abp.NewFireTheme`.

Update target frameworks to `net10.0` and bump all NuGet packages. Pay special attention to any ASP.NET Core-specific API changes and configuration updates.

**Done when**:
- Host and theme projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings
- ASP.NET Core APIs verified for compatibility

---

## Validation & Completion

### 06-final-build-test: Full solution build and test validation

Build the entire solution and run all tests to verify the upgrade is complete and functional. Document any deferred recommendations or optional modernizations discovered during the upgrade.

**Done when**:
- Full solution builds without errors
- Solution builds without warnings (or all warnings documented)
- All unit tests pass
- No deployment or runtime issues detected

