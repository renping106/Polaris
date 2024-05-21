
using System.Text;
using System.Text.Json;
using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.DynamicPlugin.Domain.Plugin
{
    internal class PlugInManager : IPlugInManager
    {
        private readonly string folderName = "PlugIns";
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
                _plugInDescriptors.Clear();
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
            var plugInStates = LoadState();
            var pluginPath = Path.Combine(AppContext.BaseDirectory, folderName);
            if (Path.Exists(pluginPath))
            {
                foreach (var plugin in Directory.GetDirectories(pluginPath))
                {
                    var plugInInfoFile = Path.Combine(plugin, "plugin.json");
                    if (Path.Exists(plugInInfoFile))
                    {
                        using StreamReader reader = new(plugInInfoFile);
                        var json = reader.ReadToEnd();
                        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                        var plugInDescriptor = JsonSerializer.Deserialize<PlugInDescriptor>(json, options);
                        if (plugInDescriptor != null)
                        {
                            var stateInConfig = plugInStates.FirstOrDefault(t => t.Name == plugInDescriptor.Name);
                            plugInDescriptor.IsEnabled = stateInConfig?.IsEnabled ?? false;
                            plugInDescriptor.PlugInSource = new FolderPlugInSource(plugin);
                            _plugInDescriptors.Add(plugInDescriptor);
                        }
                    }
                }
            }
        }

        private void SaveState()
        {
            var fileName = "plugInSettings.json";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(_plugInDescriptors, options);
            File.WriteAllText(filePath, jsonString, Encoding.UTF8);
        }

        private IReadOnlyList<IPlugInDescriptor> LoadState()
        {
            var fileName = "plugInSettings.json";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
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