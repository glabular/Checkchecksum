using ChecksumCalculatorWpf.Infrastructure.Commands.Base;
using System.Windows;
using System.Windows.Input;

namespace ChecksumCalculatorWpf.Infrastructure.Commands;

public class HandleFileDropCommand<T> : ICommand
{
    private readonly Func<T, Task> _executeAsync;
    private readonly Predicate<T> _canExecute;

    public HandleFileDropCommand(Func<T, Task> executeAsync, Predicate<T> canExecute = null)
    {
        _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute((T)parameter);
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync((T)parameter);
    }

    private async Task ExecuteAsync(T parameter)
    {
        await _executeAsync(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
