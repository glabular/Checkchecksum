namespace ChecksumCalculatorWpf.Models;

/// <summary>
/// Class was created to bind FileFormat enum with displaying text in the UI.
/// </summary>
public static class FileFormatDisplayMapper
{
    private static readonly Dictionary<FileFormat, string> _displayNames = new()
    {
        { FileFormat.txt, "Plain Text" },
        { FileFormat.json, "JSON" },
        { FileFormat.csv, "CSV" },
    };

    public static string GetDisplayName(FileFormat format)
    {
        return _displayNames.TryGetValue(format, out var displayName) ? displayName : format.ToString();
    }

    public static FileFormat? GetFileFormatByDisplayName(string displayName)
    {
        return _displayNames.FirstOrDefault(pair => pair.Value == displayName).Key;
    }

    public static IEnumerable<string> GetAllDisplayNames()
    {
        return _displayNames.Values;
    }
}
