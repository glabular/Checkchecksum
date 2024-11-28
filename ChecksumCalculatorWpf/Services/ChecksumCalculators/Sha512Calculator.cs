using System.IO;
using System.Security.Cryptography;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha512Calculator
{
    public static string GetSha512Checksum(string path)
    {
        try
        {
            using var sha512 = SHA512.Create();
            using FileStream fileStream = File.OpenRead(path);
            fileStream.Position = 0;
            var hashValue = sha512.ComputeHash(fileStream);

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

    public static string GetSha512Checksum(byte[] buffer)
    {
        try
        {
            var hashValue = SHA512.HashData(buffer);

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
