using System.Windows;
using System.Windows.Input;

namespace ChecksumCalculatorWpf.Infrastructure.Commands;

public class CopyChecksumCommand : ICommand
{
    private readonly Func<string> _getChecksum;

    public CopyChecksumCommand(Func<string> getChecksum)
    {
        _getChecksum = getChecksum ?? throw new ArgumentNullException(nameof(getChecksum));
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        var checksum = _getChecksum();
        if (!string.IsNullOrWhiteSpace(checksum))
        {
            Clipboard.SetText(checksum);
            MessageBox.Show(checksum, "Copied to clipboard!");
        }
    }
}
