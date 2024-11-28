using System.IO;
using System.Security.Cryptography;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha256Calculator
{
    public static string GetSHA256Checksum(string path)
    {
        try
        {
            using var sha256 = SHA256.Create();
            using FileStream fileStream = File.OpenRead(path);
            fileStream.Position = 0;
            var hashValue = sha256.ComputeHash(fileStream);

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

    public static string GetSHA256Checksum(byte[] buffer)
    {
        try
        {
            var hashValue = SHA256.HashData(buffer);
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
