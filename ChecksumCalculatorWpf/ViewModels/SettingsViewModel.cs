using ChecksumCalculatorWpf.Infrastructure.Commands;
using ChecksumCalculatorWpf.Models;
using ChecksumCalculatorWpf.Services;
using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels.Base;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ChecksumCalculatorWpf.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly AppSettings _settings;
    private FileFormat _selectedFormat;
    private string _selectedLanguage;
    private string _selectedFont;

    public SettingsViewModel(NavigationStore navigationStore, Func<ChecksumsViewModel> createChecksumsViewModel)
    {
        _settings = SettingsService.LoadSettings();

        NavigateChecksumsCommand = new NavigateCommand(navigationStore, createChecksumsViewModel);

        
        BrowseCommand = new RelayCommand(SelectAndSetFolderPath);
        OpenDirectoryCommand = new RelayCommand(OpenDirectoryInFileExplorer, CanOpenDirectory);
        ClearDefaultDirectoryPathCommand = new RelayCommand(ClearDefaultDirectoryPath, CanClearDirectoryPath);

        AvailableFormatsDisplay = [.. FileFormatDisplayMapper.GetAllDisplayNames()];
        SelectedFormat = FileFormatDisplayMapper.GetDisplayName(_settings.SelectedFileFormat);

        AvailableLanguages = [.. LanguageMappings.Keys];

        // TODO???
        _selectedLanguage = LanguageMappings.ContainsValue(_settings.Language)
            ? LanguageMappings.First(x => x.Value == _settings.Language).Key
            : "English";

        AvailableFonts = [.. FontManager.GetAvailableFonts()];

        _selectedFont = _settings.FontName;
    }

    public ICommand NavigateChecksumsCommand { get; }

    public ICommand BrowseCommand { get; }

    public ICommand OpenDirectoryCommand { get; }

    public ICommand ClearDefaultDirectoryPathCommand { get; }

    public string DefaultPathForSavingChecksums
    {
        get => _settings.DefaultPathForSavingChecksums;
        set
        {
            if (_settings.DefaultPathForSavingChecksums != value)
            {
                _settings.DefaultPathForSavingChecksums = value;
                SettingsService.SaveSettings(_settings);
                OnPropertyChanged(nameof(DefaultPathForSavingChecksums));

                // Needed to add, because the buttons wouldn't change their state appropriately.
                (OpenDirectoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ClearDefaultDirectoryPathCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool EnableChecksumSaving
    {
        get => _settings.EnableChecksumSaving;
        set
        {
            if (_settings.EnableChecksumSaving != value)
            {
                _settings.EnableChecksumSaving = value;
                SettingsService.SaveSettings(_settings);
                OnPropertyChanged(nameof(EnableChecksumSaving));
            }
        }
    }

    public bool CreateNewFileForEachChecksum
    {
        get => _settings.CreateNewFileForEachChecksum;
        set
        {
            if (_settings.CreateNewFileForEachChecksum != value)
            {
                _settings.CreateNewFileForEachChecksum = value;
                SettingsService.SaveSettings(_settings);
                OnPropertyChanged(nameof(CreateNewFileForEachChecksum));
            }
        }
    }

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
                App.ApplyLanguage(LanguageMappings[_selectedLanguage]);
                _settings.Language = LanguageMappings[_selectedLanguage];
                SettingsService.SaveSettings(_settings);
            }            
        }
    }

    public string SelectedFont
    {
        get => _selectedFont;
        set
        {
            if (_selectedFont != value)
            {
                _selectedFont = value;
                OnPropertyChanged(nameof(SelectedFont));
                _settings.FontName = value;
                SettingsService.SaveSettings(_settings);
                FontManager.ChangeFont(value);
            }
        }
    }

    /// <summary>
    /// The list of available formats for saving checksums.
    /// </summary>
    public ObservableCollection<string> AvailableFormatsDisplay { get; }

    /// <summary>
    /// The list of available languages for the application UI.
    /// </summary>
    public ObservableCollection<string> AvailableLanguages { get; }

    public ObservableCollection<string> AvailableFonts { get; }

    public Dictionary<string, string> LanguageMappings { get; } = new()
    {
        { "English", "en-US" },
        { "Русский", "ru-RU" }
    };

    /// <summary>
    /// The selected file format for saving checksums
    /// </summary>
    public string SelectedFormat
    {
        get => FileFormatDisplayMapper.GetDisplayName(_selectedFormat);
        set
        {
            var newFormat = FileFormatDisplayMapper.GetFileFormatByDisplayName(value);
            if (newFormat.HasValue && newFormat != _selectedFormat)
            {
                _selectedFormat = newFormat.Value;
                _settings.SelectedFileFormat = _selectedFormat;
                SettingsService.SaveSettings(_settings);
                OnPropertyChanged(nameof(SelectedFormat));
            }
        }
    }

    
    private void SelectAndSetFolderPath(object? parameter)
    {
        var folderDialog = new OpenFolderDialog();

        if (folderDialog.ShowDialog() == true)
        {
            DefaultPathForSavingChecksums = folderDialog.FolderName;
        }
    }

    private void OpenDirectoryInFileExplorer(object? parameter)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = DefaultPathForSavingChecksums,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open the directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanOpenDirectory(object? parameter)
    {
        return !string.IsNullOrEmpty(DefaultPathForSavingChecksums) && Directory.Exists(DefaultPathForSavingChecksums);
    }

    private bool CanClearDirectoryPath(object? parameter)
    {
        return !string.IsNullOrEmpty(DefaultPathForSavingChecksums);
    }

    private void ClearDefaultDirectoryPath(object? parameter)
    {
        DefaultPathForSavingChecksums = string.Empty;
    }    
}
