using System.IO;

namespace ChecksumCalculatorWpf.Models;

public class ProcessingFile(string fullPath)
{
    public string FullPath { get; set; } = fullPath;

    public string FileName => Path.GetFileName(FullPath);

    public long FileSize => new FileInfo(FullPath).Length;
}
