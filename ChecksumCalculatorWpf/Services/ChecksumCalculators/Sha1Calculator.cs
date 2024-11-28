using System.IO;
using System.Security.Cryptography;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha1Calculator
{
    public static string GetSha1Checksum(string path)
    {
        try
        {
            using var sha1 = SHA1.Create();
            using var fileStream = File.OpenRead(path);
            var hash = sha1.ComputeHash(fileStream);

            return ChecksumHelper.ByteArrayToString(hash);
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

    public static string GetSha1Checksum(byte[] buffer)
    {
        try
        {
            var hash = SHA1.HashData(buffer);
            return ChecksumHelper.ByteArrayToString(hash);
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
