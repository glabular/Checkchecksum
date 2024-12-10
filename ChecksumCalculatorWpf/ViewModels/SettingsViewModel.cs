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

    public SettingsViewModel(NavigationStore navigationStore, Func<ChecksumsViewModel> createChecksumsViewModel)
    {
        _settings = SettingsService.LoadSettings();

        NavigateChecksumsCommand = new NavigateCommand(navigationStore, createChecksumsViewModel);

        SaveCommand = new RelayCommand(SaveSettings);
        BrowseCommand = new RelayCommand(SelectAndSetFolderPath);
        OpenDirectoryCommand = new RelayCommand(OpenDirectoryInFileExplorer, CanOpenDirectory);
        ClearDefaultDirectoryPathCommand = new RelayCommand(ClearDefaultDirectoryPath, CanClearDirectoryPath);

        AvailableFormatsDisplay = new ObservableCollection<string>(FileFormatDisplayMapper.GetAllDisplayNames());
        SelectedFormat = FileFormatDisplayMapper.GetDisplayName(_settings.SelectedFileFormat);
    }

    public ICommand NavigateChecksumsCommand { get; }

    public ICommand SaveCommand { get; }

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
                OnPropertyChanged(nameof(CreateNewFileForEachChecksum));
            }
        }
    }

    /// <summary>
    /// The list of available formats for saving checksums.
    /// </summary>
    public ObservableCollection<string> AvailableFormatsDisplay { get; }

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
                OnPropertyChanged(nameof(SelectedFormat));
            }
        }
    }

    private void SaveSettings(object? parameter)
    {
        SettingsService.SaveSettings(_settings);
        MessageBox.Show("Settings saved successfully!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
