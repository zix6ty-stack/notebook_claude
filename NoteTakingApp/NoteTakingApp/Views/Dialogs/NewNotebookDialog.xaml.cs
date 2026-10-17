using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewNotebookDialog : ContentDialog
    {
        public NewNotebookDialogViewModel ViewModel => (NewNotebookDialogViewModel)DataContext;

        public NewNotebookDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // The ViewModel will handle the actual creation
            // We just need to prevent the dialog from closing if validation fails
            if (string.IsNullOrWhiteSpace(ViewModel.Title))
            {
                // Show validation error
                // In a real implementation, we would show an error message
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User clicked Cancel - nothing to do here
        }
    }
}