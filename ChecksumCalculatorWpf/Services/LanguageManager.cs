using System.Globalization;
using System.Windows;

namespace ChecksumCalculatorWpf.Services;

public static class LanguageManager
{
    public static readonly string DefaultLanguage = "English";

    public static readonly Dictionary<string, string> LanguageMappings = new()
    {
        { "English", "en-US" },
        { "Русский", "ru-RU" }
    };

    /// <summary>
    /// Applies the specified language to the application by setting the current culture and UI culture
    /// and updating the resource dictionary with the appropriate language-specific resources.
    /// </summary>
    /// <param name="lang">The language code (e.g., "en-US" or "fr-FR") to apply to the application.</param>
    public static void ApplyLanguage(string lang)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        Application.Current.Resources.MergedDictionaries.Clear();

        var resdict = new ResourceDictionary()
        {
            Source = new Uri($"/Resources/Dictionary-{lang}.xaml", UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries.Add(resdict);
    }    
}
