using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsDialogViewModel ViewModel => (SettingsDialogViewModel)DataContext;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void UseSystemThemeToggle_Toggled(object sender, Microsoft.UI.Xaml.Controls.RoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ToggleSwitch toggleSwitch)
            {
                // When toggling use system theme, we might want to disable/enable the dark mode toggle
                // The actual logic is handled in the ViewModel or SettingsService
            }
        }

        private void ResetToDefaultButton_Click(object sender, Microsoft.UI.Xaml.Controls.RoutedEventArgs e)
        {
            ViewModel.ResetToDefault();
        }

        private void SaveButton_Click(object sender, Microsoft.UI.Xaml.Controls.RoutedEventArgs e)
        {
            ViewModel.SaveSettings();
            // TODO: Show confirmation feedback
        }
    }
}