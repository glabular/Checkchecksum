using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

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
        _navigationStore.CurrentViewModel = new ChecksumsViewModel(_navigationStore, CreateSettingsViewModel);

        MainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel(_navigationStore)
        };

        MainWindow.Show();

        base.OnStartup(e);
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

