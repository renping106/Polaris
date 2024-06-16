using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.DependencyInjection;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class ConfigFileManager(IConfiguration configRoot) : IConfigFileManager, ISingletonDependency
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private readonly IConfigurationRoot _config = (IConfigurationRoot)configRoot;

    public string? GetConnectionString()
    {
        return GetSetting($"ConnectionStrings:Default", null);
    }

    public string? GetDatabaseProvider()
    {
        return GetSetting($"DatabaseProvider", null);
    }

    public void SetConnectionString(string connectionString)
    {
        AddOrUpdateSetting($"ConnectionStrings:Default", connectionString, true);
    }

    public void SetDatabaseProvider(string databaseProvider)
    {
        AddOrUpdateSetting($"DatabaseProvider", databaseProvider, true);
    }

    private void AddOrUpdateSetting<T>(string key, T value, bool reload)
    {
        try
        {
            var file = "appsettings.json";
            var path = Path.Combine(Directory.GetCurrentDirectory(), file);
            var node = JsonNode.Parse(File.ReadAllText(path));
            SetValueRecursively(node, key, value);
            File.WriteAllText(path, JsonSerializer.Serialize(node, _jsonSerializerOptions));
            if (reload) { _config.Reload(); }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error Updating App Setting {key} - {ex}");
        }
    }

    private string? GetSetting(string key, string? defaultValue)
    {
        var value = _config.GetValue(key, defaultValue) ?? defaultValue;
        return value;
    }

    private void SetValueRecursively<T>(JsonNode? json, string key, T value)
    {
        if (json != null && key != null && !object.Equals(value, default(T)))
        {
            var remainingSections = key.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                var nextSection = remainingSections[1];
                var newJson = json[currentSection] ??= new JsonObject();
                SetValueRecursively(newJson, nextSection, value);
            }
            else
            {
                if ((value is string) && (value.ToString()!.StartsWith('[') || value.ToString()!.StartsWith('{')))
                {
                    json[currentSection] = JsonNode.Parse(value.ToString()!);
                }
                else
                {
                    json[currentSection] = JsonValue.Create(value);
                }
            }
        }
    }
}
