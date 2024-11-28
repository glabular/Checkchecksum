using ChecksumCalculatorWpf.Infrastructure.Commands;
using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChecksumCalculatorWpf.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(NavigationStore navigationStore, Func<ChecksumsViewModel> createChecksumsViewModel)
    {
        NavigateChecksumsCommand = new NavigateCommand(navigationStore, createChecksumsViewModel);
    }

    public ICommand NavigateChecksumsCommand { get; }
}
