using System.Text;

namespace ChecksumCalculatorWpf.Services.ChecksumCalculators;

public static class ChecksumHelper
{
    public static string ByteArrayToString(byte[] array)
    {
        StringBuilder sb = new();

        for (int i = 0; i < array.Length; i++)
        {
            sb.Append($"{array[i]:X2}");
        }

        return sb.ToString();
    }
}
