using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity.PlugIns;
using Volo.Abp.Modularity;
using Volo.Abp;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;

namespace Nerd.Abp.DynamicPlugin.Domain
{
    internal class FolderSource : IPlugInSource
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        public Func<string, bool>? Filter { get; set; }

        public AssemblyLoadContext Context { get; set; }

        public FolderSource(
            [NotNull] string folder,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Check.NotNull(folder, nameof(folder));

            Folder = folder;
            SearchOption = searchOption;
            Context = new AssemblyLoadContext("plugin", true);
        }

        public Type[] GetModules()
        {
            var modules = new List<Type>();

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
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules.ToArray();
        }

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

            //return assemblyFiles.Select(Context.LoadFromAssemblyPath).ToList();
        }

        private static IEnumerable<string> GetAssemblyFiles(string folderPath, SearchOption searchOption)
        {
            return Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
        }
    }
}
