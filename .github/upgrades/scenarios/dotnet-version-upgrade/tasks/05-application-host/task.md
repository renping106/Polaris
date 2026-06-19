# 05-application-host: Upgrade main host and theme projects

Upgrade the application host and theme projects that consume the upgraded libraries: `Polaris.Abp.Host` (AspNetCore), `Polaris.Abp.NewFireTheme`.

Update target frameworks to `net10.0` and bump all NuGet packages. Pay special attention to any ASP.NET Core-specific API changes and configuration updates.

**Done when**:
- Host and theme projects target `net10.0`
- All packages updated to .NET 10-compatible versions
- Projects build without errors or warnings
- ASP.NET Core APIs verified for compatibility
