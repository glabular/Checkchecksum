namespace ChecksumCalculatorWpf.Services.ChecksumFileWriters;

public interface IChecksumWriter
{
    void WriteChecksums(Dictionary<string, string> checksums, string filePath);
}
