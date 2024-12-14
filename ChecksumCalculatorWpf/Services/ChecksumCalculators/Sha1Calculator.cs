using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha1Calculator
{
    public static async Task<string> GetSha1ChecksumAsync(string path, IProgress<double> progress = null)
    {
        try
        {
            using var sha1 = SHA1.Create();
            using FileStream fileStream = File.OpenRead(path);
            var buffer = new byte[8192];
            long totalBytes = fileStream.Length;
            long bytesRead = 0;

            int read;
            while ((read = await fileStream.ReadAsync(buffer)) > 0)
            {
                sha1.TransformBlock(buffer, 0, read, null, 0);
                bytesRead += read;

                progress?.Report((double)bytesRead / totalBytes * 100);
            }

            sha1.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return ChecksumHelper.ByteArrayToString(sha1.Hash);
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
