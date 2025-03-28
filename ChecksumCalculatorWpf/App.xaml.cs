﻿using ChecksumCalculatorWpf.Services;
using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChecksumCalculatorWpf;

public partial class App : Application
{
    private readonly NavigationStore _navigationStore;

    public App()
    {
        _navigationStore = new NavigationStore();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var settings = SettingsService.LoadSettings();

        LanguageManager.ApplyLanguage(settings.Language);
        FontManager.ChangeFont(settings.FontName);        

        _navigationStore.CurrentViewModel = new ChecksumsViewModel(_navigationStore, CreateSettingsViewModel);

        MainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel(_navigationStore)
        };

        MainWindow.Show();
    }    

    /// <summary>
    /// Retrieves a localized string from application resources.
    /// </summary>
    /// <param name="key">The resource key for the localized string.</param>
    /// <returns>The localized string if found; otherwise, returns the key itself.</returns>
    public static string GetLocalizedString(string key)
    {
        return Application.Current.Resources.Contains(key)
            ? Application.Current.Resources[key].ToString()
            : key;
    }

    private ChecksumsViewModel CreateChecksumsViewModel()
    {
        return new ChecksumsViewModel(_navigationStore, CreateSettingsViewModel);
    }

    private SettingsViewModel CreateSettingsViewModel()
    {
        return new SettingsViewModel(_navigationStore, CreateChecksumsViewModel);
    }
}
