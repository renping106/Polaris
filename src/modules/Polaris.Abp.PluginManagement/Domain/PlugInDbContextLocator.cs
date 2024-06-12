using Polaris.Abp.Extension.Abstractions.Database;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Polaris.Abp.PluginManagement.Domain.Entities;
using System.Reflection;
using System.Runtime.Loader;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.PluginManagement.Domain
{
    [Dependency(ReplaceServices = true)]
    public class PlugInDbContextLocator : DefaultDbContextLocator
    {
        private readonly IPlugInManager _plugInManager;

        public PlugInDbContextLocator(IPlugInManager plugInManager)
        {
            _plugInManager = plugInManager;
        }

        public override string GetLocation(IAbpEfCoreDbContext dbContext)
        {
            var location = base.GetLocation(dbContext);
            if (location.IsNullOrEmpty())
            {
                // In plugin AssemblyLoadContext
                return LocateAssemblyPath(dbContext, dbContext.GetType().Assembly.GetName());
            }
            else
            {
                return location;
            }
        }

        public override string GetReferenceLocation(IAbpEfCoreDbContext dbContext, AssemblyName refAssemblyName)
        {
            var location = "";
            try
            {
                location = base.GetReferenceLocation(dbContext, refAssemblyName);
            }
            catch (Exception)
            { }

            if (location.IsNullOrEmpty())
            {
                // In plugin AssemblyLoadContext
                location = LocateAssemblyPath(dbContext, refAssemblyName);
            }

            return location;
        }

        private string LocateAssemblyPath(IAbpEfCoreDbContext dbContext, AssemblyName assembly)
        {
            var pluginAssemblyContexts = _plugInManager.GetEnabledPlugIns()
                .Select(t => new
                {
                    ((DynamicPlugInSource)t.PlugInSource).Folder,
                    ((IPlugInContext)t.PlugInSource).Context
                });

            var dbAssembly = dbContext.GetType().Assembly;
            var dbAssemblyContext = AssemblyLoadContext.GetLoadContext(dbAssembly);
            if (dbAssemblyContext != null)
            {
                var targetAssemblyContext = pluginAssemblyContexts.First(t => t.Context?.ToString() == dbAssemblyContext.ToString());
                return Path.Combine(targetAssemblyContext.Folder, assembly.Name + ".dll");
            }
            else
            {
                throw new AbpException("Cannot find DbContext Assembly.");
            }
        }
    }
}
