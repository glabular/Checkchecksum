namespace ChecksumCalculatorWpf.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Adjusts the case of the given string based on a specified preference.
    /// </summary>
    /// <param name="input">The string to adjust.</param>
    /// <param name="isLowercase">A boolean value indicating whether the string should be converted to lowercase. 
    /// If <c>true</c>, the string will be converted to lowercase; otherwise, it will be converted to uppercase.</param>
    /// <returns>
    /// The adjusted string in either lowercase or uppercase, based on the value of <paramref name="isLowercase"/>.
    /// </returns>    
    public static string CorrectStringCase(this string input, bool isLowercase)
    {
        return isLowercase ? input.ToLower() : input.ToUpper();
    }
}
