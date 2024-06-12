using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.Extension.Abstractions.Database;
using Polaris.Abp.Extension.Abstractions.Plugin;
using Polaris.Abp.PluginManagement.Domain.Entities;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using System.Text;
using System.Text.Json;
using Volo.Abp;

namespace Polaris.Abp.PluginManagement.Domain
{
    internal class PlugInManager : IPlugInManager
    {
        private readonly List<IPlugInDescriptor> _plugInDescriptors = new();
        private IPlugInDescriptor? _preEnabledPlugIn;
        private readonly IShellServiceProvider _shellServiceProvider;
        private readonly IWebAppShell _webAppShell;
        private readonly string settingFileName = "plugInSettings.json";

        public PlugInManager(IWebAppShell webAppShell, IShellServiceProvider shellServiceProvider)
        {
            _webAppShell = webAppShell;
            _shellServiceProvider = shellServiceProvider;
            LoadData();
        }

        public async Task DisablePlugInAsync(string plugInName)
        {
            var target = GetPlugIn(plugInName);
            if (target != null)
            {
                DisablePlugIn(target);
                await _webAppShell.UpdateShell();
            }
        }

        public async Task<(bool, string)> EnablePlugInAsync(string plugInName)
        {
            var pluginDescriptor = GetPlugIn(plugInName);
            var targetPlugIn = pluginDescriptor.Clone();

            SetPreEnabledPlugIn(targetPlugIn);

            var tryAddResult = await _webAppShell.UpdateShell();

            if (tryAddResult.Success && _shellServiceProvider.ServiceProvider != null)
            {
                var dbContextUpdator = _shellServiceProvider.ServiceProvider.GetRequiredService<IDbContextUpdater>();
                await dbContextUpdator.UpdateAsync(new DbContextChangedEvent()
                {
                    DbContextTypes = ((IPlugInContext)targetPlugIn.PlugInSource).DbContextTypes
                });
                EnablePlugIn(targetPlugIn);
            }
            else
            {
                ((IPlugInContext)targetPlugIn.PlugInSource).UnloadContext();
                ClearPreEnabledPlugIn();
            }

            return tryAddResult;
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

        public async Task RefreshPlugInAsync(IPlugInDescriptor installedPlugIn)
        {
            try
            {
                ((IPlugInContext)installedPlugIn.PlugInSource).UnloadContext();
                var tryAddResult = await _webAppShell.UpdateShell();

                if (tryAddResult.Success && _shellServiceProvider.ServiceProvider != null)
                {
                    var dbContextUpdator = _shellServiceProvider.ServiceProvider.GetRequiredService<IDbContextUpdater>();
                    await dbContextUpdator.UpdateAsync(new DbContextChangedEvent()
                    {
                        DbContextTypes = ((IPlugInContext)installedPlugIn.PlugInSource).DbContextTypes
                    });
                }
                else
                {
                    throw new AbpException(tryAddResult.Message);
                }
            }
            catch (Exception)
            {
                PlugInPackageUtil.RollbackPackage(installedPlugIn);

                // Rollback shell
                ((IPlugInContext)installedPlugIn.PlugInSource).UnloadContext();
                await _webAppShell.UpdateShell();
                throw;
            }
        }

        public void RemovePlugIn(string plugInName)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugInName);
            if (target != null)
            {
                _plugInDescriptors.Remove(target);
                SaveState();
            }
        }

        private void ClearPreEnabledPlugIn()
        {
            _preEnabledPlugIn = null;
        }

        private void DisablePlugIn(IPlugInDescriptor plugIn)
        {
            var target = _plugInDescriptors.Find(t => t.Name == plugIn.Name);
            if (target != null)
            {
                target.IsEnabled = false;
                ((IPlugInContext)target.PlugInSource).UnloadContext();
                SaveState();
            }
        }

        private void EnablePlugIn(IPlugInDescriptor plugIn)
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

        private void SetPreEnabledPlugIn(IPlugInDescriptor plugIn)
        {
            _preEnabledPlugIn = plugIn;
        }
    }
}