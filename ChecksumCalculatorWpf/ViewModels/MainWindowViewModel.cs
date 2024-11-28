using ChecksumCalculatorWpf.Stores;
using ChecksumCalculatorWpf.ViewModels.Base;

namespace ChecksumCalculatorWpf.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;
    private string _windowTitle = "Checkchecksum";
	
	public string WindowTitle
	{
		get => _windowTitle; 
		set 
		{ 
			_windowTitle = value;
			OnPropertyChanged(nameof(WindowTitle));
		}
	}

    public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

	public MainWindowViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    private void OnCurrentViewModelChanged()
    {
		OnPropertyChanged(nameof(CurrentViewModel));
    }
}
