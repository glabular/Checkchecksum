using System.IO;

namespace ChecksumCalculatorWpf.Services.ChecksumFileWriters;

public class TxtChecksumWriter : IChecksumWriter
{
    public void WriteChecksums(Dictionary<string, string> checksums, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        foreach (var checksum in checksums)
        {
            writer.WriteLine($"{checksum.Key}: {checksum.Value}");
        }
    }
}
