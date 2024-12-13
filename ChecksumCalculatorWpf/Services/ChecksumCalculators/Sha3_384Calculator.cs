using SHA3.Net;
using System.IO;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha3_384Calculator
{
    public static string GetSha3_384Checksum(string path)
    {
        try
        {
            using var sha3_384 = Sha3.Sha3384();
            using FileStream fileStream = File.OpenRead(path);
            var hashValue = sha3_384.ComputeHash(fileStream);

            return ChecksumHelper.ByteArrayToString(hashValue);
        }
        catch (IOException e)
        {
            throw new InvalidOperationException($"Failed to compute checksum due to I/O error: {e.Message}", e);
        }
        catch (UnauthorizedAccessException e)
        {
            throw new UnauthorizedAccessException($"Access denied while computing checksum: {e.Message}", e);
        }
    }
}
