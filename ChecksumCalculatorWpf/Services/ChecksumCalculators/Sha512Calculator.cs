using System.IO;
using System.Security.Cryptography;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha512Calculator
{
    public static async Task<string> GetSha512ChecksumAsync(string path, IProgress<double> progress = null)
    {
        try
        {
            using var sha512 = SHA512.Create();
            using FileStream fileStream = File.OpenRead(path);
            var buffer = new byte[8192]; // 8 KB buffer
            long totalBytes = fileStream.Length;
            long bytesRead = 0;

            int read;
            while ((read = await fileStream.ReadAsync(buffer)) > 0)
            {
                sha512.TransformBlock(buffer, 0, read, null, 0);
                bytesRead += read;

                progress?.Report((double)bytesRead / totalBytes * 100);
            }

            sha512.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return ChecksumHelper.ByteArrayToString(sha512.Hash);
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
