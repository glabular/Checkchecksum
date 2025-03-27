using ChecksumCalculatorWpf.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ChecksumCalculatorWpf.Services;

/// <summary>
/// Manages the application's font settings, including dynamically changing the font and providing available font options.
/// </summary>
public static class FontManager
{
    // Predefined list of acceptable fonts
    private static readonly List<string> _availableFonts =
    [
        "Onest Light",
        "Ubuntu",
        "Calibri",
        "Times New Roman",
        "Georgia",
    ];

    /// <summary>
    /// Changes the application's default font dynamically based on the provided font name.
    /// </summary>
    /// <param name="fontName">The name of the font to apply. If it is a valid font, it is applied; otherwise, the default system font is used.</param>
    /// <exception cref="ArgumentException">Thrown if the font name is not valid or not in the available font list.</exception>
    public static void ChangeFont(string fontName)
    {
        if (string.IsNullOrWhiteSpace(fontName))
        {
            return; // Skip if null/empty
        }

        if (!IsValidFont(fontName))
        {
            // TODO: Use ILogger
            Debug.WriteLine($"Invalid font '{fontName}' provided. Defaulting to '{_availableFonts[0]}'.");
            
            // Fallback to default font.
            fontName = _availableFonts[0];
            var settings = SettingsService.LoadSettings();
            settings.FontName = _availableFonts[0];
            SettingsService.SaveSettings(settings);
        }

        var resourceKey = fontName switch
        {
            "Ubuntu" => "UbuntuFont",
            "Onest Light" => "OnestFont",
            _ => null
        };

        if (resourceKey != null)
        {
            // Apply the selected font to the resource dynamically if it's a custom font
            Application.Current.Resources["FontFamilyResource"] = Application.Current.Resources[resourceKey];
        }
        else
        {
            // For system-installed fonts, apply directly using the font name
            Application.Current.Resources["FontFamilyResource"] = new FontFamily(fontName);
        }
    }

    /// <summary>
    /// Retrieves the list of available fonts for the application.
    /// </summary>
    /// <returns>A read-only list of available font names.</returns>
    public static IReadOnlyList<string> GetAvailableFonts() => _availableFonts;

    private static bool IsValidFont(string fontName)
    {
        return _availableFonts.Contains(fontName);
    }
}
