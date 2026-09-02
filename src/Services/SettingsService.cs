using System.Text.Json;
using System.Text.Json.Serialization;
using WuWaServerLauncher.Models;

namespace WuWaServerLauncher.Services;

public sealed class LauncherSettings
{
    public Dictionary<string, ServerProfile> Servers { get; set; } = new();
    public string LastServer { get; set; } = "cn";
}

public sealed class SettingsService
{
    private readonly string _configPath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string ConfigPath => _configPath;

    public SettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _configPath = Path.Combine(appData, "WuWaServerLauncher", "settings.json");
    }

    public LauncherSettings Load()
    {
        var defaults = CreateDefaults();

        try
        {
            if (!File.Exists(_configPath))
                return defaults;

            var json = File.ReadAllText(_configPath);
            var loaded = JsonSerializer.Deserialize<LauncherSettings>(json, JsonOptions) ?? defaults;

            foreach (var pair in defaults.Servers)
            {
                if (!loaded.Servers.TryGetValue(pair.Key, out var existing))
                {
                    loaded.Servers[pair.Key] = pair.Value;
                    continue;
                }

                existing.Id = pair.Value.Id;
                existing.AppId = pair.Value.AppId;
                existing.Name = pair.Value.Name;
                existing.Region = pair.Value.Region;
                existing.Description = pair.Value.Description;
                existing.RefreshStatus();
            }

            if (!loaded.Servers.ContainsKey(loaded.LastServer))
                loaded.LastServer = "cn";

            return loaded;
        }
        catch
        {
            return defaults;
        }
    }

    public void Save(LauncherSettings settings)
    {
        var directory = Path.GetDirectoryName(_configPath)!;
        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private static LauncherSettings CreateDefaults()
    {
        var settings = new LauncherSettings();

        settings.Servers["cn"] = new ServerProfile
        {
            Id = "cn",
            AppId = "10003",
            Name = "官服",
            Region = "中国大陆",
            Description = "Wuthering Waves 官方国服客户端"
        };

        settings.Servers["bilibili"] = new ServerProfile
        {
            Id = "bilibili",
            AppId = "10004",
            Name = "B服",
            Region = "哔哩哔哩",
            Description = "Bilibili 渠道客户端"
        };

        settings.Servers["global"] = new ServerProfile
        {
            Id = "global",
            AppId = "50004",
            Name = "国际服",
            Region = "Global",
            Description = "国际服 / Overseas 客户端"
        };

        foreach (var profile in settings.Servers.Values)
            profile.RefreshStatus();

        return settings;
    }
}
