namespace ChecksumCalculatorWpf.Models;

/// <summary>
/// Defines the available file formats for saving checksums.
/// </summary>
public enum FileFormat
{
    // Is used as file extention as well.
    // Use lowercase.
    txt,
    json,
    csv
}
