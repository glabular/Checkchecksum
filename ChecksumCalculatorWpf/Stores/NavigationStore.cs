using ChecksumCalculatorWpf.ViewModels.Base;

namespace ChecksumCalculatorWpf.Stores;

/// <summary>
/// Stores the current ViewModel for the application.
/// </summary>
public class NavigationStore
{
	private ViewModelBase _currentViewModel;

	public ViewModelBase CurrentViewModel
    {
		get => _currentViewModel;
		set 
		{ 
			_currentViewModel = value;
			OnCurrentViewModelChanged();
		}
	}

	public event Action? CurrentViewModelChanged;

	private void OnCurrentViewModelChanged()
	{
		CurrentViewModelChanged?.Invoke();
	}
}
