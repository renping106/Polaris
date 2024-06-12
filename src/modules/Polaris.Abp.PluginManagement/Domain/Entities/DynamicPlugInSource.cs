using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Runtime.Loader;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;

namespace Polaris.Abp.PluginManagement.Domain.Entities
{
    internal class DynamicPlugInSource : IPlugInSource, IPlugInContext
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        public Func<string, bool>? Filter { get; set; }

        public AssemblyLoadContext? Context { get; private set; }

        private static readonly string _contextName = "plugin";
        private List<Type> _dbContextTypes;
        private List<CompiledRazorAssemblyPart> _compiledRazorAssemblyParts;

        public DynamicPlugInSource(
            [NotNull] string folder,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Check.NotNull(folder, nameof(folder));

            Folder = folder;
            SearchOption = searchOption;

            _dbContextTypes = new();
            _compiledRazorAssemblyParts = new();
        }

        public void UnloadContext()
        {
            Context?.Unload();
            Context = null;
        }

        public Type[] GetModules()
        {
            var modules = new List<Type>();
            _dbContextTypes = new();
            _compiledRazorAssemblyParts = new();

            foreach (var assembly in GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (AbpModule.IsAbpModule(type))
                        {
                            modules.AddIfNotContains(type);
                            _compiledRazorAssemblyParts.AddIfNotContains(new CompiledRazorAssemblyPart(type.Assembly));
                        }

                        if (type.IsAssignableTo<IAbpEfCoreDbContext>())
                        {
                            _dbContextTypes.AddIfNotContains(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules.ToArray();
        }

        public IReadOnlyList<Type> DbContextTypes => _dbContextTypes;

        public IReadOnlyList<CompiledRazorAssemblyPart> CompiledRazorAssemblyParts => _compiledRazorAssemblyParts;

        private List<Assembly> GetAssemblies()
        {
            var assemblyFiles = GetAssemblyFiles(Folder, SearchOption);

            if (Filter != null)
            {
                assemblyFiles = assemblyFiles.Where(Filter);
            }

            if (Context == null)
            {
                Context = new AssemblyLoadContext(_contextName, true);
            }

            var results = new List<Assembly>();
            foreach (var assembly in assemblyFiles)
            {
                using StreamReader reader = new(assembly);
                results.Add(Context.LoadFromStream(reader.BaseStream));
            }

            return results;
        }

        private static IEnumerable<string> GetAssemblyFiles(string folderPath, SearchOption searchOption)
        {
            return Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll"));
        }
    }
}
