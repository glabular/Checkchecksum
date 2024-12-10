using ChecksumCalculatorWpf.Models;
using System.IO;
using System.Text.Json;

namespace ChecksumCalculatorWpf.Services;

public class SettingsService
{
    private static readonly string _settingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Checkchecksum");
    private static readonly string _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() 
    { 
        WriteIndented = true, PropertyNameCaseInsensitive = true 
    };

    public static AppSettings LoadSettings()
    {
        if (!Directory.Exists(_settingsDirectory))
        {
            Directory.CreateDirectory(_settingsDirectory);
        }

        if (!File.Exists(_settingsFilePath))
        {
            var defaultSettings = new AppSettings();

            SaveSettings(defaultSettings);
        }

        var json = File.ReadAllText(_settingsFilePath);

        return JsonSerializer.Deserialize<AppSettings>(json, _jsonSerializerOptions) ?? new AppSettings();
    }

    public static void SaveSettings(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
        File.WriteAllText(_settingsFilePath, json);
    }
}
