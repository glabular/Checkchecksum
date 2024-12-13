using SHA3.Net;
using System.IO;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha3_512Calculator
{
    public static string GetSha3_512Checksum(string path)
    {
        using var sha3_512 = Sha3.Sha3512();
        using FileStream fileStream = File.OpenRead(path);
        var hashValue = sha3_512.ComputeHash(fileStream);

        return ChecksumHelper.ByteArrayToString(hashValue);
    }
}
