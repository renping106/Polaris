using System.Reflection;
using System.Runtime.Loader;

namespace Nerd.Abp.PluginManagement.Domain
{
    internal class AutofacLoadContext : AssemblyLoadContext
    {

        public AutofacLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
