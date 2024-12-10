using System.IO;

namespace ChecksumCalculatorWpf.Services.ChecksumFileWriters;

public class CsvChecksumWriter : IChecksumWriter
{
    public void WriteChecksums(Dictionary<string, string> checksums, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("Algorithm,Checksum"); // CSV header

        foreach (var checksum in checksums)
        {
            writer.WriteLine($"{checksum.Key},{checksum.Value}");
        }
    }
}
