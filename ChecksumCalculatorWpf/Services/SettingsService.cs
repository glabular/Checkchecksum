using ChecksumCalculatorWpf.Models;
using System.IO;
using System.Text.Json;

namespace ChecksumCalculatorWpf.Services;

public class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Checkchecksum");
    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() 
    { 
        WriteIndented = true, PropertyNameCaseInsensitive = true 
    };

    public static AppSettings LoadSettings()
    {
        if (!Directory.Exists(SettingsDirectory))
        {
            Directory.CreateDirectory(SettingsDirectory);
        }

        if (!File.Exists(SettingsFilePath))
        {
            var defaultSettings = new AppSettings
            {

            };

            SaveSettings(defaultSettings);
        }

        var json = File.ReadAllText(SettingsFilePath);

        return JsonSerializer.Deserialize<AppSettings>(json, _jsonSerializerOptions) ?? new AppSettings();
    }

    public static void SaveSettings(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
        File.WriteAllText(SettingsFilePath, json);
    }
}
