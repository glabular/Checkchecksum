﻿using ChecksumCalculatorWpf.Infrastructure.Commands;
using ChecksumCalculatorWpf.Models;
using ChecksumCalculatorWpf.Services;
using ChecksumCalculatorWpf.Services.ChecksumCalculators;
using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels.Base;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ChecksumCalculatorWpf.ViewModels;

public class ChecksumsViewModel : ViewModelBase
{
    private string _fileName = string.Empty;
    private string _sha256 = string.Empty;
    private string _sha384 = string.Empty;
    private string _sha512 = string.Empty;
    private string _sha1 = string.Empty;
    private string _md5 = string.Empty;

    private bool _sha256Checked;
    private bool _sha384Checked;
    private bool _sha512Checked = true;
    private bool _sha1Checked;
    private bool _md5Checked = true;
    private bool _selectAllChecked;
    private bool _isLowercaseChecked = true;
    private bool _allowDrop = true;

    private readonly AppSettings _settings;

    public ChecksumsViewModel(NavigationStore navigationStore, Func<SettingsViewModel> createSettingsViewModel)
    {
        _settings = SettingsService.LoadSettings();

        CopySha256Command = new RelayCommand(_ => CopyToClipboard(Sha256));
        CopySha384Command = new RelayCommand(_ => CopyToClipboard(Sha384));
        CopySha512Command = new RelayCommand(_ => CopyToClipboard(Sha512));
        CopySha1Command = new RelayCommand(_ => CopyToClipboard(Sha1));
        CopyMd5Command = new RelayCommand(_ => CopyToClipboard(Md5));
        CopyAllCommand = new RelayCommand(_ => CopyToClipboard(GetAllChecksums()));

        NavigateSettingsCommand = new NavigateCommand(navigationStore, createSettingsViewModel);
        HandleFileDropCommand = new RelayCommandAsync<string>(OnFileDroppedAsync);

        InitializeCheckboxesValues();

        CheckForEasterEgg();
    }
    

    public bool AllowDrop
    {
        get => _allowDrop;
        set
        {
            if (_allowDrop != value)
            {
                _allowDrop = value;
                OnPropertyChanged(nameof(AllowDrop));
            }
        }
    }

    public bool IsLowercaseChecked
    {
        get => _isLowercaseChecked;
        set
        {
            if (_isLowercaseChecked != value)
            {
                _isLowercaseChecked = value;
                OnPropertyChanged(nameof(IsLowercaseChecked));
                _settings.IsLowercaseChecked = value;
                SettingsService.SaveSettings(_settings);
                UpdateTextboxesCase();
            }
        }
    }

    public string FileName
    {
        get => _fileName;
        private set
        {
            if (_fileName != value)
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }
    } 

    public string Sha256
    {
        get => _sha256;
        set
        {
            _sha256 = value;
            OnPropertyChanged(nameof(Sha256));
        }
    }

    public string Sha384
    {
        get => _sha384;
        set
        {
            _sha384 = value;
            OnPropertyChanged(nameof(Sha384));
        }
    }

    public string Sha512
    {
        get => _sha512;
        set
        {
            _sha512 = value;
            OnPropertyChanged(nameof(Sha512));
        }
    }

    public string Sha1
    {
        get => _sha1;
        set
        {
            _sha1 = value;
            OnPropertyChanged(nameof(Sha1));
        }
    }

    public string Md5
    {
        get => _md5;
        set
        {
            _md5 = value;
            OnPropertyChanged(nameof(Md5));
        }
    }

    #region Copy commands
    public ICommand CopySha256Command { get; }

    public ICommand CopySha384Command { get; }

    public ICommand CopySha512Command { get; }

    public ICommand CopySha1Command { get; }

    public ICommand CopyMd5Command { get; } 

    public ICommand CopyAllCommand { get; } 
    #endregion

    public ICommand NavigateSettingsCommand { get; }

    public ICommand HandleFileDropCommand { get; }

    #region Algorithms checkboxes bool properties
    public bool SHA256Checked
    {
        get => _sha256Checked;
        set
        {
            if (_allowDrop && _sha256Checked != value)
            {
                _sha256Checked = value;
                OnPropertyChanged(nameof(SHA256Checked));
                _settings.SHA256Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool SHA384Checked
    {
        get => _sha384Checked;
        set
        {
            if (_allowDrop && _sha384Checked != value)
            {
                _sha384Checked = value;
                OnPropertyChanged(nameof(SHA384Checked));
                _settings.SHA384Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool SHA512Checked
    {
        get => _sha512Checked;
        set
        {
            if (_allowDrop && _sha512Checked != value)
            {
                _sha512Checked = value;
                OnPropertyChanged(nameof(SHA512Checked));
                _settings.SHA512Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool SHA1Checked
    {
        get => _sha1Checked;
        set
        {
            if (_allowDrop && _sha1Checked != value)
            {
                _sha1Checked = value;
                OnPropertyChanged(nameof(SHA1Checked));
                _settings.SHA1Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool MD5Checked
    {
        get => _md5Checked;
        set
        {
            if (_allowDrop && _md5Checked != value)
            {
                _md5Checked = value;
                OnPropertyChanged(nameof(MD5Checked));
                _settings.MD5Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool SelectAllChecked
    {
        get => _selectAllChecked;
        set
        {
            if (_allowDrop)
            {
                _selectAllChecked = value;
                OnPropertyChanged(nameof(SelectAllChecked));
                UpdateAllCheckboxes(value);
            }
        }
    } 
    #endregion

    private async Task OnFileDroppedAsync(string filePath)
    {
        AllowDrop = false;
        FileName = string.Empty;

        ClearTextboxes();

        if (IsNoAlgorithmSelected())
        {
            MessageBox.Show("Please, select at least one hash algorithm.", "None selected", MessageBoxButton.OK, MessageBoxImage.Error);
            AllowDrop = true;

            return;
        }

        FileName = Path.GetFileName(filePath);

        try
        {
            await CalculateAndUpdateChecksumsAsync(filePath);

            UpdateTextboxesCase();

            if (_settings.EnableChecksumSaving)
            {
                SaveChecksumsToFile();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading file: {ex.Message}");
            FileName = string.Empty;
        }
        finally
        {
            AllowDrop = true;
        }
    }

    private void SaveChecksumsToFile()
    {
        var checksums = GetChecksumsDictionary();
        var checksumSaveService = new ChecksumSaveService(_settings);
        if (string.IsNullOrWhiteSpace(_settings.DefaultPathForSavingChecksums))
        {
            var folderDialog = new OpenFolderDialog();
            if (folderDialog.ShowDialog() == true)
            {
                var chosenPath = folderDialog.FolderName;
                checksumSaveService.SaveChecksums(FileName, checksums, chosenPath);
            }
        }
        else
        {
            checksumSaveService.SaveChecksums(FileName, checksums);
        }
    }

    /// <summary>
    /// Gathers the calculated checksum values and returns them in a dictionary.
    /// The dictionary maps algorithm names (e.g., "SHA512") to their corresponding checksum values.
    /// </summary>
    /// <returns>A dictionary containing the algorithm names as keys and their respective checksum values as values.</returns>
    private Dictionary<string, string> GetChecksumsDictionary()
    {
        var checksums = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(Sha512)) checksums["SHA512"] = Sha512;
        if (!string.IsNullOrEmpty(Md5)) checksums["MD5"] = Md5;
        if (!string.IsNullOrEmpty(Sha256)) checksums["SHA256"] = Sha256;
        if (!string.IsNullOrEmpty(Sha384)) checksums["SHA384"] = Sha384;
        if (!string.IsNullOrEmpty(Sha1)) checksums["SHA1"] = Sha1;

        return checksums;
    }

    private async Task CalculateAndUpdateChecksumsAsync(string filePath)
    {
        var checksumTasks = new List<Task<string>>();

        if (SHA512Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha512Calculator.GetSha512Checksum(filePath)));
        }

        if (MD5Checked)
        {
            checksumTasks.Add(Task.Run(() => Md5Calculator.CalculateMd5Checksum(filePath)));
        }

        if (SHA256Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha256Calculator.GetSHA256Checksum(filePath)));
        }

        if (SHA384Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha384Calculator.GetSha384Checksum(filePath)));
        }

        if (SHA1Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha1Calculator.GetSha1Checksum(filePath)));
        }

        var results = await Task.WhenAll(checksumTasks);

        var resultIndex = 0;

        // TODO: Make independent from the order.
        if (SHA512Checked) Sha512 = results[resultIndex++];
        if (MD5Checked) Md5 = results[resultIndex++];
        if (SHA256Checked) Sha256 = results[resultIndex++];
        if (SHA384Checked) Sha384 = results[resultIndex++];
        if (SHA1Checked) Sha1 = results[resultIndex++];
    }

    private bool IsNoAlgorithmSelected()
    {
        return !SHA512Checked && !MD5Checked && !SHA256Checked && !SHA384Checked && !SHA1Checked;
    }

    private string GetAllChecksums()
    {
        var checksums = new List<string>();

        if (!string.IsNullOrWhiteSpace(Sha256)) 
        { 
            checksums.Add($"SHA-256: {Sha256}"); 
        }

        if (!string.IsNullOrWhiteSpace(Sha384))
        { 
            checksums.Add($"SHA-384: {Sha384}"); 
        }

        if (!string.IsNullOrWhiteSpace(Sha512))
        {
            checksums.Add($"SHA-512: {Sha512}");
        }

        if (!string.IsNullOrWhiteSpace(Sha1))
        {
            checksums.Add($"SHA-1: {Sha1}");
        }

        if (!string.IsNullOrWhiteSpace(Md5))
        { 
            checksums.Add($"MD5: {Md5}"); 
        }

        return string.Join(Environment.NewLine, checksums);
    }

    private void CopyToClipboard(string? checksum)
    {
        if (!string.IsNullOrWhiteSpace(checksum))
        {
            Clipboard.SetText(checksum);
            MessageBox.Show(checksum, "Copied to clipboard!");
        }
    }

    private void UpdateSelectAllState()
    {
        _selectAllChecked = SHA256Checked && SHA384Checked && SHA512Checked && SHA1Checked && MD5Checked;
        OnPropertyChanged(nameof(SelectAllChecked));
    }

    private void UpdateTextboxesCase()
    {
        if (IsLowercaseChecked)
        {
            Sha256 = Sha256.ToLower();
            Sha384 = Sha384.ToLower();
            Sha512 = Sha512.ToLower();
            Sha1 = Sha1.ToLower();
            Md5 = Md5.ToLower();
        }
        else
        {
            Sha256 = Sha256.ToUpper();
            Sha384 = Sha384.ToUpper();
            Sha512 = Sha512.ToUpper();
            Sha1 = Sha1.ToUpper();
            Md5 = Md5.ToUpper();
        }
    }

    private void ClearTextboxes()
    {
        Sha256 = string.Empty;
        Sha384 = string.Empty;
        Sha512 = string.Empty;
        Sha1 = string.Empty;
        Md5 = string.Empty;
    }

    private void UpdateAllCheckboxes(bool isChecked)
    {
        SHA256Checked = isChecked;
        SHA384Checked = isChecked;
        SHA512Checked = isChecked;
        SHA1Checked = isChecked;
        MD5Checked = isChecked;
    }

    private void InitializeCheckboxesValues()
    {
        SHA256Checked = _settings.SHA256Checked;
        SHA384Checked = _settings.SHA384Checked;
        SHA512Checked = _settings.SHA512Checked;
        SHA1Checked = _settings.SHA1Checked;
        MD5Checked = _settings.MD5Checked;

        IsLowercaseChecked = _settings.IsLowercaseChecked;
    }

    private void CheckForEasterEgg()
    {
        if (new Random().NextDouble() < 0.01) // 1% probability
        {
            Sha256 = "🎉 You found the Easter Egg!";
            Sha384 = "🎉 You found the Easter Egg!";
            Sha512 = "🎉 You found the Easter Egg!";
            Sha1 = "🎉 You found the Easter Egg!";
            Md5 = "🎉 You found the Easter Egg!";
        }
    }
}
