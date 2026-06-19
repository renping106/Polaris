# 02-foundation-libs: Upgrade foundation and infrastructure libraries

Upgrade the lowest-tier class libraries that have no dependencies on other solution projects. These include: `Polaris.Abp.Extension.Abstractions`, `Polaris.Abp.DatabaseManagement`, `Polaris.Abp.DatabaseManagement.Sqlite`, `Polaris.Abp.DatabaseManagement.SqlServer`.

Update target frameworks from `net8.0` to `net10.0`, bump all NuGet packages to versions compatible with .NET 10, and apply any necessary code fixes for breaking API changes in the framework or packages.

**Done when**:
- Foundation library projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings
- No API breaking changes remain unaddressed
