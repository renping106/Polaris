using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Polaris.Abp.PluginManagement.Domain.Interfaces;

public interface IPlugInContext
{
    AssemblyLoadContext? Context { get; }
    void UnloadContext();
    IReadOnlyList<Type> DbContextTypes { get; }
    IReadOnlyList<CompiledRazorAssemblyPart> CompiledRazorAssemblyParts { get; }
}
