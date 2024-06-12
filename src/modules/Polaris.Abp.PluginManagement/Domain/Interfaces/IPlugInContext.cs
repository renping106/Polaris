using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Runtime.Loader;

namespace Polaris.Abp.PluginManagement.Domain.Interfaces
{
    public interface IPlugInContext
    {
        AssemblyLoadContext? Context { get; }
        void UnloadContext();
        IReadOnlyList<Type> DbContextTypes { get; }
        IReadOnlyList<CompiledRazorAssemblyPart> CompiledRazorAssemblyParts { get; }
    }
}
