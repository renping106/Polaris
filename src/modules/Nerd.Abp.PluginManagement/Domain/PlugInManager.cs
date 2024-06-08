using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Domain.Entities;
using System.Text;
using System.Text.Json;

namespace Nerd.Abp.PluginManagement.Domain
{
    internal class PlugInManager : IPlugInManager
    {
        private readonly List<IPlugInDescriptor> _plugInDescriptors = new();
        private IPlugInDescriptor? _preEnabledPlugIn;
        private readonly string settingFileName = "plugInSettings.json";

        public PlugInManager()
        {
            LoadData();
        }

        public void ClearPreEnabledPlugIn()
        {
            _preEnabledPlugIn = null;
        }

        public void DisablePlugIn(IPlugInDescriptor plugIn)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
            if (target != null)
            {
                target.IsEnabled = false;
                ((IPlugInContext)target.PlugInSource).UnloadContext();
                SaveState();
            }
        }

        public void EnablePlugIn(IPlugInDescriptor plugIn)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
            if (target != null)
            {
                target.IsEnabled = true;
                target.PlugInSource = plugIn.PlugInSource;
                ClearPreEnabledPlugIn();
                SaveState();
            }
        }

        public IReadOnlyList<IPlugInDescriptor> GetAllPlugIns(bool refresh = false)
        {
            if (refresh)
            {
                LoadData();
            }
            return _plugInDescriptors.AsReadOnly();
        }

        public IReadOnlyList<IPlugInDescriptor> GetEnabledPlugIns()
        {
            var enabledPlugIns = _plugInDescriptors.Where(t => t.IsEnabled).ToList();
            if (_preEnabledPlugIn != null) enabledPlugIns.Add(_preEnabledPlugIn);
            return enabledPlugIns;
        }

        public IPlugInDescriptor GetPlugIn(string name)
        {
            var plugins = GetAllPlugIns();
            return plugins.First(p => p.Name == name);
        }

        public void RemovePlugIn(IPlugInDescriptor plugIn)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
            if (target != null)
            {
                _plugInDescriptors.Remove(target);
                SaveState();
            }
        }

        public void SetPreEnabledPlugIn(IPlugInDescriptor plugIn)
        {
            _preEnabledPlugIn = plugIn;
        }

        private void LoadData()
        {
            var plugInList = PlugInPackageUtil.LoadFromFolder();
            var configuredPlugIns = LoadState();

            foreach (var plugIn in plugInList)
            {
                var configured = configuredPlugIns.FirstOrDefault(t => t.Name == plugIn.Name);
                var isEnabled = configured?.IsEnabled ?? false;
                var exist = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
                if (exist != null)
                {
                    exist.IsEnabled = isEnabled;
                    exist.Version = plugIn.Version;
                    exist.Description = plugIn.Description;
                }
                else
                {
                    plugIn.IsEnabled = isEnabled;
                    _plugInDescriptors.Add(plugIn);
                }
            }

            SaveState();
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
    }
}