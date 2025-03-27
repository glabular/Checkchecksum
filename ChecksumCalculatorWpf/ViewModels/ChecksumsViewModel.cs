using ChecksumCalculatorWpf.Extensions;
using ChecksumCalculatorWpf.Infrastructure.Commands;
using ChecksumCalculatorWpf.Models;
using ChecksumCalculatorWpf.Services;
using ChecksumCalculatorWpf.Services.ChecksumCalculators;
using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels.Base;
using Microsoft.Win32;
using System.Diagnostics;
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
    private string _sha3_256 = string.Empty;
    private string _sha3_384 = string.Empty;
    private string _sha3_512 = string.Empty;

    private bool _sha256Checked;
    private bool _sha384Checked;
    private bool _sha512Checked = true;
    private bool _sha1Checked;
    private bool _md5Checked = true;
    private bool _sha3_256Checked;
    private bool _sha3_384Checked;
    private bool _sha3_512Checked;

    private bool _selectAllChecked;
    private bool _isLowercaseChecked = true;
    private bool _allowDrop = true;

    private bool _isCalculatingSha256;
    private bool _isCalculatingSha384;
    private bool _isCalculatingSha512;
    private bool _isCalculatingSha1;
    private bool _isCalculatingMd5;
    private bool _isCalculatingSha3_256;
    private bool _isCalculatingSha3_384;
    private bool _isCalculatingSha3_512;

    private double _sha256Progress;
    private double _sha384Progress;
    private double _sha512Progress;
    private double _sha1Progress;
    private double _Md5Progress;
    private double _sha3_256Progress;
    private double _sha3_384Progress;
    private double _sha3_512Progress;

    private readonly AppSettings _settings;

    public ChecksumsViewModel(NavigationStore navigationStore, Func<SettingsViewModel> createSettingsViewModel)
    {
        _settings = SettingsService.LoadSettings();

        CopySha256Command = new RelayCommand(_ => CopyToClipboard(Sha256));
        CopySha384Command = new RelayCommand(_ => CopyToClipboard(Sha384));
        CopySha512Command = new RelayCommand(_ => CopyToClipboard(Sha512));
        CopySha1Command = new RelayCommand(_ => CopyToClipboard(Sha1));
        CopyMd5Command = new RelayCommand(_ => CopyToClipboard(Md5));
        CopySha3_256Command = new RelayCommand(_ => CopyToClipboard(Sha3_256));
        CopySha3_384Command = new RelayCommand(_ => CopyToClipboard(Sha3_384));
        CopySha3_512Command = new RelayCommand(_ => CopyToClipboard(Sha3_512));
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
            if (_allowDrop && _isLowercaseChecked != value)
            {
                _isLowercaseChecked = value;
                OnPropertyChanged(nameof(IsLowercaseChecked));
                _settings.IsLowercaseChecked = value;
                SettingsService.SaveSettings(_settings);
                UpdateTextboxesCase();
            }
        }
    }

    public bool IsSettingsEnabled => !AllowDrop;

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

    #region IsCalculating properties
    public bool IsCalculatingSha256
    {
        get => _isCalculatingSha256;
        set
        {
            _isCalculatingSha256 = value;
            OnPropertyChanged(nameof(IsCalculatingSha256));
        }
    }
    
    public bool IsCalculatingSha384
    {
        get => _isCalculatingSha384;
        set
        {
            _isCalculatingSha384 = value;
            OnPropertyChanged(nameof(IsCalculatingSha384));
        }
    }
    
    public bool IsCalculatingSha512
    {
        get => _isCalculatingSha512;
        set
        {
            _isCalculatingSha512 = value;
            OnPropertyChanged(nameof(IsCalculatingSha512));
        }
    }
    
    public bool IsCalculatingSha1
    {
        get => _isCalculatingSha1;
        set
        {
            _isCalculatingSha1 = value;
            OnPropertyChanged(nameof(IsCalculatingSha1));
        }
    }

    public bool IsCalculatingMd5
    {
        get => _isCalculatingMd5;
        set
        {
            _isCalculatingMd5 = value;
            OnPropertyChanged(nameof(IsCalculatingMd5));
        }
    }

    public bool IsCalculatingSha3_256
    {
        get => _isCalculatingSha3_256;
        set
        {
            _isCalculatingSha3_256 = value;
            OnPropertyChanged(nameof(IsCalculatingSha3_256));
        }
    }

    public bool IsCalculatingSha3_384
    {
        get => _isCalculatingSha3_384;
        set
        {
            _isCalculatingSha3_384 = value;
            OnPropertyChanged(nameof(IsCalculatingSha3_384));
        }
    }

    public bool IsCalculatingSha3_512
    {
        get => _isCalculatingSha3_512;
        set
        {
            _isCalculatingSha3_512 = value;
            OnPropertyChanged(nameof(IsCalculatingSha3_512));
        }
    }
    #endregion    
    
    #region Checksum strings
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

    public string Sha3_256
    {
        get => _sha3_256;
        set
        {
            _sha3_256 = value;
            OnPropertyChanged(nameof(Sha3_256));
        }
    }

    public string Sha3_384
    {
        get => _sha3_384;
        set
        {
            _sha3_384 = value;
            OnPropertyChanged(nameof(Sha3_384));
        }
    }

    public string Sha3_512
    {
        get => _sha3_512;
        set
        {
            _sha3_512 = value;
            OnPropertyChanged(nameof(Sha3_512));
        }
    }
    #endregion

    #region Copy commands
    public ICommand CopySha256Command { get; }

    public ICommand CopySha384Command { get; }

    public ICommand CopySha512Command { get; }

    public ICommand CopySha1Command { get; }

    public ICommand CopyMd5Command { get; }

    public ICommand CopySha3_256Command { get; }

    public ICommand CopySha3_384Command { get; }

    public ICommand CopySha3_512Command { get; }

    public ICommand CopyAllCommand { get; }
    #endregion

    #region Progress bar values
    public double Sha256Progress
    {
        get => _sha256Progress;
        set
        {
            _sha256Progress = value;
            OnPropertyChanged(nameof(Sha256Progress));
        }
    }

    public double Sha384Progress
    {
        get => _sha384Progress;
        set
        {
            _sha384Progress = value;
            OnPropertyChanged(nameof(Sha384Progress));
        }
    }

    public double Sha512Progress
    {
        get => _sha512Progress;
        set
        {
            _sha512Progress = value;
            OnPropertyChanged(nameof(Sha512Progress));
        }
    }

    public double Sha3_256Progress
    {
        get => _sha3_256Progress;
        set
        {
            _sha3_256Progress = value;
            OnPropertyChanged(nameof(Sha3_256Progress));
        }
    }

    public double Sha3_384Progress
    {
        get => _sha3_384Progress;
        set
        {
            _sha3_384Progress = value;
            OnPropertyChanged(nameof(Sha3_384Progress));
        }
    }

    public double Sha3_512Progress
    {
        get => _sha3_512Progress;
        set
        {
            _sha3_512Progress = value;
            OnPropertyChanged(nameof(Sha3_512Progress));
        }
    }

    public double Md5Progress
    {
        get => _Md5Progress;
        set
        {
            _Md5Progress = value;
            OnPropertyChanged(nameof(Md5Progress));
        }
    }

    public double Sha1Progress
    {
        get => _sha1Progress;
        set
        {
            _sha1Progress = value;
            OnPropertyChanged(nameof(Sha1Progress));
        }
    }
    #endregion

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

    public bool SHA3_256Checked
    {
        get => _sha3_256Checked;
        set
        {
            if (_allowDrop && _sha3_256Checked != value)
            {
                _sha3_256Checked = value;
                OnPropertyChanged(nameof(SHA3_256Checked));
                _settings.SHA3_256Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool SHA3_384Checked
    {
        get => _sha3_384Checked;
        set
        {
            if (_allowDrop && _sha3_384Checked != value)
            {
                _sha3_384Checked = value;
                OnPropertyChanged(nameof(SHA3_384Checked));
                _settings.SHA3_384Checked = value;
                SettingsService.SaveSettings(_settings);
                UpdateSelectAllState();
            }
        }
    }

    public bool SHA3_512Checked
    {
        get => _sha3_512Checked;
        set
        {
            if (_allowDrop && _sha3_512Checked != value)
            {
                _sha3_512Checked = value;
                OnPropertyChanged(nameof(SHA3_512Checked));
                _settings.SHA3_512Checked = value;
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

    public ICommand NavigateSettingsCommand { get; }

    public ICommand HandleFileDropCommand { get; }

    private async Task OnFileDroppedAsync(string filePath)
    {
        AllowDrop = false;
        FileName = string.Empty;
        ClearTextboxes();

        if (IsNoAlgorithmSelected())
        {
            var title = App.GetLocalizedString("NoneSelectedTitle");
            var message = App.GetLocalizedString("SelectAlgorithmMessage");
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            AllowDrop = true;

            return;
        }

        FileName = Path.GetFileName(filePath);

        try
        {
            await CalculateAndUpdateChecksumsAsync(filePath);

            if (_settings.EnableChecksumSaving)
            {
                SaveChecksumsToFile();
            }
        }
        catch (Exception ex)
        {
            var message = App.GetLocalizedString("ErrorReadingFileMessage");
            MessageBox.Show($"{message}: {ex.Message}");
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
        if (!string.IsNullOrEmpty(Sha3_256)) checksums["SHA3_256"] = Sha3_256;
        if (!string.IsNullOrEmpty(Sha3_384)) checksums["SHA3_384"] = Sha3_384;
        if (!string.IsNullOrEmpty(Sha3_512)) checksums["SHA3_512"] = Sha3_512;

        return checksums;
    }

    private async Task CalculateAndUpdateChecksumsAsync(string filePath)
    {
        var checksumTasks = new List<Task>();

        if (SHA512Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha512 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha512Progress = percent);
                    var result = await Sha512Calculator.GetSha512ChecksumAsync(filePath, progress);
                    Sha512 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha512Progress = 0;
                    IsCalculatingSha512 = false;
                }
            }));
        }

        if (MD5Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingMd5 = true;
                try
                {
                    var progress = new Progress<double>(percent => Md5Progress = percent);
                    var result = await Md5Calculator.GetMd5ChecksumAsync(filePath, progress);
                    Md5 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Md5Progress = 0;
                    IsCalculatingMd5 = false;
                }
            }));
        }

        if (SHA256Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha256 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha256Progress = percent);
                    var result = await Sha256Calculator.GetSHA256ChecksumAsync(filePath, progress);
                    Sha256 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha256Progress = 0;
                    IsCalculatingSha256 = false;
                }
            }));
        }

        if (SHA384Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha384 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha384Progress = percent);
                    var result = await Sha384Calculator.GetSha384ChecksumAsync(filePath, progress);
                    Sha384 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha384Progress = 0;
                    IsCalculatingSha384 = false;
                }
            }));
        }

        if (SHA1Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha1 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha1Progress = percent);
                    var result = await Sha1Calculator.GetSha1ChecksumAsync(filePath, progress);
                    Sha1 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha1Progress = 0;
                    IsCalculatingSha1 = false;
                }
            }));
        }

        if (SHA3_256Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha3_256 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha3_256Progress = percent);
                    var result = await Sha3_256Calculator.GetSha3_256ChecksumAsync(filePath, progress);
                    Sha3_256 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha3_256Progress = 0;
                    IsCalculatingSha3_256 = false;
                }
            }));
        }

        if (SHA3_384Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha3_384 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha3_384Progress = percent);
                    var result = await Sha3_384Calculator.GetSha3_384ChecksumAsync(filePath, progress);
                    Sha3_384 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha3_384Progress = 0;
                    IsCalculatingSha3_384 = false;
                }
            }));
        }

        if (SHA3_512Checked)
        {
            checksumTasks.Add(Task.Run(async () =>
            {
                IsCalculatingSha3_512 = true;
                try
                {
                    var progress = new Progress<double>(percent => Sha3_512Progress = percent);
                    var result = await Sha3_512Calculator.GetSha3_512ChecksumAsync(filePath, progress);
                    Sha3_512 = result.CorrectStringCase(IsLowercaseChecked);
                }
                finally
                {
                    Sha3_512Progress = 0;
                    IsCalculatingSha3_512 = false;
                }
            }));
        }

        await Task.WhenAll(checksumTasks);
    }

    private bool IsNoAlgorithmSelected()
    {
        return !SHA512Checked && !MD5Checked && !SHA256Checked && !SHA384Checked && !SHA1Checked && !SHA3_256Checked && !SHA3_384Checked && !SHA3_512Checked;
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

        if (!string.IsNullOrWhiteSpace(Sha3_256))
        {
            checksums.Add($"SHA3-256: {Sha3_256}");
        }

        if (!string.IsNullOrWhiteSpace(Sha3_384))
        {
            checksums.Add($"SHA3-384: {Sha3_384}");
        }

        if (!string.IsNullOrWhiteSpace(Sha3_512))
        {
            checksums.Add($"SHA3-512: {Sha3_512}");
        }

        return string.Join(Environment.NewLine, checksums);
    }

    private static void CopyToClipboard(string? checksum)
    {
        if (!string.IsNullOrWhiteSpace(checksum))
        {
            Clipboard.SetText(checksum);

            if (true) // TODO: Add "Don't show again".
            {
                var message = App.GetLocalizedString("CopiedToClipboardMessage");
                MessageBox.Show(checksum, message);
            }
        }
    }

    private void UpdateSelectAllState()
    {
        _selectAllChecked = SHA256Checked && SHA384Checked && SHA512Checked && SHA1Checked && MD5Checked && SHA3_256Checked && SHA3_384Checked && SHA3_512Checked;
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
            Sha3_256 = Sha3_256.ToLower();
            Sha3_384 = Sha3_384.ToLower();
            Sha3_512 = Sha3_512.ToLower();
        }
        else
        {
            Sha256 = Sha256.ToUpper();
            Sha384 = Sha384.ToUpper();
            Sha512 = Sha512.ToUpper();
            Sha1 = Sha1.ToUpper();
            Md5 = Md5.ToUpper();
            Sha3_256 = Sha3_256.ToUpper();
            Sha3_384 = Sha3_384.ToUpper();
            Sha3_512 = Sha3_512.ToUpper();
        }
    }

    private void ClearTextboxes()
    {
        Sha256 = string.Empty;
        Sha384 = string.Empty;
        Sha512 = string.Empty;
        Sha1 = string.Empty;
        Md5 = string.Empty;
        Sha3_256 = string.Empty;
        Sha3_384 = string.Empty;
        Sha3_512 = string.Empty;
    }

    private void UpdateAllCheckboxes(bool isChecked)
    {
        SHA256Checked = isChecked;
        SHA384Checked = isChecked;
        SHA512Checked = isChecked;
        SHA1Checked = isChecked;
        MD5Checked = isChecked;
        SHA3_256Checked = isChecked;
        SHA3_384Checked = isChecked;
        SHA3_512Checked = isChecked;
    }

    private void InitializeCheckboxesValues()
    {
        SHA256Checked = _settings.SHA256Checked;
        SHA384Checked = _settings.SHA384Checked;
        SHA512Checked = _settings.SHA512Checked;
        SHA1Checked = _settings.SHA1Checked;
        MD5Checked = _settings.MD5Checked;
        SHA3_256Checked = _settings.SHA3_256Checked;
        SHA3_384Checked = _settings.SHA3_384Checked;
        SHA3_512Checked = _settings.SHA3_512Checked;

        IsLowercaseChecked = _settings.IsLowercaseChecked;
    }

    private void CheckForEasterEgg()
    {
        if (new Random().NextDouble() < 0.01) // 1% probability
        {
            var easterEggMessage = App.GetLocalizedString("EasterEgg");

            Sha256 = easterEggMessage;
            Sha384 = easterEggMessage;
            Sha512 = easterEggMessage;
            Sha1 = easterEggMessage;
            Md5 = easterEggMessage;
            Sha3_256 = easterEggMessage;
            Sha3_384 = easterEggMessage;
            Sha3_512 = easterEggMessage;
        }
    }
}
