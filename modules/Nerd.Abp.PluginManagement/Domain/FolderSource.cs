using JetBrains.Annotations;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Runtime.Loader;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.PluginManagement.Domain
{
    internal class FolderSource : IPlugInSource, IPlugInContext
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        public Func<string, bool>? Filter { get; set; }

        public AssemblyLoadContext Context { get; private set; }

        private static readonly string _contextName = "plugin";
        private List<Type> _dbContextTypes;

        public FolderSource(
            [NotNull] string folder,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Check.NotNull(folder, nameof(folder));

            Folder = folder;
            SearchOption = searchOption;

            Context = new AssemblyLoadContext(_contextName, true);
            _dbContextTypes = new List<Type>();
        }

        public void ResetContext()
        {
            UnloadContext();
            Context = new AssemblyLoadContext(_contextName, true);
        }

        public void UnloadContext()
        {
            Context.Unload();
        }

        public Type[] GetModules()
        {
            var modules = new List<Type>();
            _dbContextTypes = new List<Type>();

            foreach (var assembly in GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (AbpModule.IsAbpModule(type))
                        {
                            modules.AddIfNotContains(type);
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

        private List<Assembly> GetAssemblies()
        {
            var assemblyFiles = GetAssemblyFiles(Folder, SearchOption);

            if (Filter != null)
            {
                assemblyFiles = assemblyFiles.Where(Filter);
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
