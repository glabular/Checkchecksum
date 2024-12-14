using System.IO;
using System.Security.Cryptography;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Md5Calculator
{
    public static async Task<string> GetMd5ChecksumAsync(string path, IProgress<double> progress = null)
    {
        using var md5 = MD5.Create();
        using FileStream fileStream = File.OpenRead(path);
        var buffer = new byte[8192]; // 8 KB buffer
        long totalBytes = fileStream.Length;
        long bytesRead = 0;

        int read;
        while ((read = await fileStream.ReadAsync(buffer)) > 0)
        {
            md5.TransformBlock(buffer, 0, read, null, 0);
            bytesRead += read;

            progress?.Report((double)bytesRead / totalBytes * 100);
        }

        md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return ChecksumHelper.ByteArrayToString(md5.Hash);
    }

    public static string CalculateMd5Checksum(byte[] buffer)
    {
        try
        {
            var checkSum = MD5.HashData(buffer);
            return BitConverter.ToString(checkSum).Replace("-", string.Empty);
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
