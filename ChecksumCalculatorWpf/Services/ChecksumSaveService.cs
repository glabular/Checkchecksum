using ChecksumCalculatorWpf.Models;
using ChecksumCalculatorWpf.Services.ChecksumFileWriters;
using System.IO;

namespace ChecksumCalculatorWpf.Services;

public class ChecksumSaveService
{
    private readonly AppSettings _settings;

    public ChecksumSaveService()
    {
        _settings = SettingsService.LoadSettings();
    }

    public void SaveChecksums(string fileName, Dictionary<string, string> checksums)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        if (checksums == null || checksums.Count == 0)
        {
            throw new ArgumentException("Checksums dictionary is empty or null.");
        }

        var directory = _settings.DefaultPathForSavingChecksums;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Generate full file path with appropriate extension
        var fullFilePath = Path.Combine(directory, $"{fileName}.{_settings.SelectedFileFormat}");

        IChecksumWriter writer = GetWriter();
        
        writer.WriteChecksums(checksums, fullFilePath);
    }

    private IChecksumWriter GetWriter()
    {
        // Choose the appropriate writer based on the app settings
        return _settings.SelectedFileFormat switch
        {
            FileFormat.txt => new TxtChecksumWriter(),
            FileFormat.csv => new CsvChecksumWriter(),
            FileFormat.json => new JsonChecksumWriter(),
            _ => throw new InvalidOperationException("Unsupported file format")
        };
    }
}
