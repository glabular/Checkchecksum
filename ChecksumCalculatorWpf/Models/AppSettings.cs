namespace ChecksumCalculatorWpf.Models;

public class AppSettings
{
    /// <summary>
    /// Gets or sets the default directory path where checksums are saved.
    /// </summary>
    public string DefaultPathForSavingChecksums { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether saving checksums to a file is enabled.
    /// </summary>
    public bool EnableChecksumSaving { get; set; }

    /// <summary>
    /// Indicates whether to create a new file for each checksum.
    /// </summary>
    public bool CreateNewFileForEachChecksum { get;  set; }

    /// <summary>
    /// Gets or sets the selected file format for saving checksums.
    /// </summary>
    public FileFormat SelectedFileFormat { get; set; } = FileFormat.txt;
}