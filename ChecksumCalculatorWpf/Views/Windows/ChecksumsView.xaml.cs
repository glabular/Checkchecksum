using ChecksumCalculatorWpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChecksumCalculatorWpf.Views.Windows;

/// <summary>
/// Interaction logic for ChecksumsView.xaml
/// </summary>
public partial class ChecksumsView : UserControl
{
    public ChecksumsView()
    {
        InitializeComponent();
    }

    private void OnFileDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        { 
            return; 
        }

        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        if (files.Length <= 0)
        { 
            return;
        }

        var firstFile = files[0];

        if (files.Length > 1)
        {
            var message = App.GetLocalizedString("MultipleFilesDroppedMessage", firstFile);
            var title = App.GetLocalizedString("OnlyOneFileAllowedTitle");

            var userChoice = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


            if (userChoice != MessageBoxResult.Yes)
            {
                return; // User chose not to process                
            } 
        }

        // Ensure DataContext is of type ChecksumsViewModel before executing the command
        if (DataContext is ChecksumsViewModel viewModel)
        {
            viewModel.HandleFileDropCommand.Execute(firstFile);
        }
    }
}
