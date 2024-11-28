using ChecksumCalculatorWpf.Infrastructure.Commands.Base;
using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels;
using ChecksumCalculatorWpf.ViewModels.Base;

namespace ChecksumCalculatorWpf.Infrastructure.Commands;

public class NavigateCommand : CommandBase
{
    private readonly NavigationStore _navigationStore;
    private readonly Func<ViewModelBase> _createViewModel;

    public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
    {
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _createViewModel = createViewModel ?? throw new ArgumentNullException(nameof(createViewModel));
    }

    public override void Execute(object? parameter)
    {
        _navigationStore.CurrentViewModel = _createViewModel();
    }
}
