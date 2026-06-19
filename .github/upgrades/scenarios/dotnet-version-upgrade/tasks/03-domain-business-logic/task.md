# 03-domain-business-logic: Upgrade domain and business logic libraries

Upgrade the mid-tier libraries that build on foundation libraries: `Polaris.Abp.PluginManagement`, `Polaris.Abp.ThemeManagement`, `Polaris.Abp.DatabaseManagement.Tests`, `Abp.CmsKit`.

Update target frameworks to `net10.0` and bump all NuGet packages. Verify that references to foundation libraries (now on .NET 10) resolve correctly and that no additional code changes are needed.

**Done when**:
- Domain library projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings
- Proper references to upgraded foundation libraries
