using System.Reflection;
using System.Runtime.Loader;

namespace Polaris.Abp.PluginManagement.Domain;

internal class AutoFacLoadContext : AssemblyLoadContext
{

    public AutoFacLoadContext() : base(isCollectible: true)
    {
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;
    }
}
