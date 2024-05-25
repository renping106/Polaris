using Nerd.Abp.DynamicPlugin.Domain.Interfaces;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.DynamicPlugin.Domain
{
    internal class PlugInManager : IPlugInManager
    {
        private readonly string folderName = "PlugIns";
        private readonly string settingFileName = "plugInSettings.json";
        private readonly List<IPlugInDescriptor> _plugInDescriptors = new();

        public PlugInManager()
        {
            LoadFromFolder();
        }

        public void DisablePlugIn(IPlugInDescriptor plugIn)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
            if (target != null)
            {
                target.IsEnabled = false;
                SaveState();
            }
        }

        public void EnablePlugIn(IPlugInDescriptor plugIn)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
            if (target != null)
            {
                target.IsEnabled = true;
                SaveState();
            }
        }

        public IReadOnlyList<IPlugInDescriptor> GetAllPlugIns(bool refresh = false)
        {
            if (refresh)
            {
                LoadFromFolder();
            }
            return _plugInDescriptors.AsReadOnly();
        }

        public IReadOnlyList<IPlugInDescriptor> GetEnabledPlugIns()
        {
            return _plugInDescriptors.Where(t => t.IsEnabled).ToList().AsReadOnly();
        }

        private void LoadFromFolder()
        {
            var pluginPath = Path.Combine(AppContext.BaseDirectory, folderName);
            if (Path.Exists(pluginPath))
            {
                var previousStates = LoadState();
                foreach (var plugin in Directory.GetDirectories(pluginPath))
                {
                    var nuspecFile = Directory.GetFiles(plugin).FirstOrDefault(t => t.EndsWith(".nuspec"));
                    if (nuspecFile != null)
                    {
                        using StreamReader reader = new(nuspecFile);
                        var nuspec = XDocument.Load(reader);
                        var name = NuGetUtil.GetMetaValue(nuspec, "id");
                        var version = NuGetUtil.GetMetaValue(nuspec, "version");
                        var description = NuGetUtil.GetMetaValue(nuspec, "description");

                        var stateInConfig = previousStates.FirstOrDefault(t => t.Name == name);
                        var exist = _plugInDescriptors.Find(t => t.Name == name);
                        if (exist != null)
                        {
                            exist.IsEnabled = stateInConfig?.IsEnabled ?? false;
                            exist.Version = version;
                            exist.Description = description;
                        }
                        else
                        {
                            _plugInDescriptors.Add(new PlugInDescriptor()
                            {
                                Name = name,
                                Description = description,
                                Version = version,
                                IsEnabled = stateInConfig?.IsEnabled ?? false,
                                PlugInSource = new FolderPlugInSource(plugin)
                            });
                        }
                    }
                }
            }
            SaveState();
        }

        private void SaveState()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, settingFileName);
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(_plugInDescriptors, options);
            File.WriteAllText(filePath, jsonString, Encoding.UTF8);
        }

        private IReadOnlyList<IPlugInDescriptor> LoadState()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, settingFileName);
            var plugInStates = new List<PlugInDescriptor>();
            if (File.Exists(filePath))
            {
                using StreamReader reader = new(filePath);
                var json = reader.ReadToEnd();
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                plugInStates = JsonSerializer.Deserialize<List<PlugInDescriptor>>(json, options)
                    ?? new List<PlugInDescriptor>();
            }

            return plugInStates.AsReadOnly();
        }
    }
}