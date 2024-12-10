using System.IO;
using System.Text.Json;

namespace ChecksumCalculatorWpf.Services.ChecksumFileWriters;

public class JsonChecksumWriter : IChecksumWriter
{
    public void WriteChecksums(Dictionary<string, string> checksums, string filePath)
    {
        var json = JsonSerializer.Serialize(checksums, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}
