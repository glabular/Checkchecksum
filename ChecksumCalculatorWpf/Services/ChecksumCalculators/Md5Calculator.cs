using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class Md5Calculator
{
    public static string CalculateMd5Checksum(string filePath)
    {
        try
        {
            using var md5 = MD5.Create();
            using var fileStream = File.OpenRead(filePath);
            var checkSum = md5.ComputeHash(fileStream);

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
