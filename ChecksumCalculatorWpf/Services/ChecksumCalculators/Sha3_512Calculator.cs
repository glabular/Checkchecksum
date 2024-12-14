using SHA3.Net;
using System.IO;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Sha3_512Calculator
{
    public static async Task<string> GetSha3_512ChecksumAsync(string path, IProgress<double> progress = null)
    {
        try
        {
            using var sha3_512 = Sha3.Sha3512();
            using FileStream fileStream = File.OpenRead(path);
            var buffer = new byte[8192]; // 8 KB buffer
            long totalBytes = fileStream.Length;
            long bytesRead = 0;

            int read;
            while ((read = await fileStream.ReadAsync(buffer)) > 0)
            {
                sha3_512.TransformBlock(buffer, 0, read, null, 0);
                bytesRead += read;

                progress?.Report((double)bytesRead / totalBytes * 100);
            }

            sha3_512.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return ChecksumHelper.ByteArrayToString(sha3_512.Hash);
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
