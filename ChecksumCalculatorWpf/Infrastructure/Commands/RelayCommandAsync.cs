using ChecksumCalculatorWpf.Infrastructure.Commands.Base;

namespace ChecksumCalculatorWpf.Infrastructure.Commands;

public class RelayCommandAsync<T> : CommandBase
{
    private readonly Func<T, Task> _execute;
    private readonly Predicate<T>? _canExecute;

    public RelayCommandAsync(Func<T, Task> execute, Predicate<T>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        if (parameter is T typedParameter)
        {
            return _canExecute?.Invoke(typedParameter) ?? true;
        }

        return false;
    }

    public override async void Execute(object? parameter)
    {
        if (parameter is T typedParameter)
        {
            await _execute(typedParameter);
        }
    }
}
