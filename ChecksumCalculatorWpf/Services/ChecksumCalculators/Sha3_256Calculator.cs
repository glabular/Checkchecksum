using SHA3.Net;
using System.IO;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha3_256Calculator
{
    public static string GetSha3_256Checksum(string path)
    {
        using var sha3_256 = Sha3.Sha3256();
        using FileStream fileStream = File.OpenRead(path);
        var hashValue = sha3_256.ComputeHash(fileStream);

        return ChecksumHelper.ByteArrayToString(hashValue);
    }
}
