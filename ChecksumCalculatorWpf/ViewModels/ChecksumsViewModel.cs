using ChecksumCalculatorWpf.Infrastructure.Commands;
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
        if (!string.IsNullOrEmpty(Sha3_256)) checksums["SHA3_256"] = Sha3_256;
        if (!string.IsNullOrEmpty(Sha3_384)) checksums["SHA3_384"] = Sha3_384;
        if (!string.IsNullOrEmpty(Sha3_512)) checksums["SHA3_512"] = Sha3_512;

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

        if (SHA3_256Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha3_256Calculator.GetSha3_256Checksum(filePath)));
        }
        
        if (SHA3_384Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha3_384Calculator.GetSha3_384Checksum(filePath)));
        }

        if (SHA3_512Checked)
        {
            checksumTasks.Add(Task.Run(() => Sha3_512Calculator.GetSha3_512Checksum(filePath)));
        }

        var results = await Task.WhenAll(checksumTasks);

        var resultIndex = 0;

        // TODO: Make independent from the order.
        if (SHA512Checked) Sha512 = results[resultIndex++];
        if (MD5Checked) Md5 = results[resultIndex++];
        if (SHA256Checked) Sha256 = results[resultIndex++];
        if (SHA384Checked) Sha384 = results[resultIndex++];
        if (SHA1Checked) Sha1 = results[resultIndex++];
        if (SHA3_256Checked) Sha3_256 = results[resultIndex++];
        if (SHA3_384Checked) Sha3_384 = results[resultIndex++];
        if (SHA3_512Checked) Sha3_512 = results[resultIndex++];
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
                MessageBox.Show(checksum, "Copied to clipboard!");
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
            Sha3_384= Sha3_384.ToUpper();
            Sha3_512= Sha3_512.ToUpper();
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
            var easterEggMessage = "🎉 You found the Easter Egg!";

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
