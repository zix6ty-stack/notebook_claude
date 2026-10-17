using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NoteTakingApp.Views;

namespace NoteTakingApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Set the initial selected item to All Notes
            NavView.SelectedItem = NavView.MenuItems[0];
            // Navigate to the LibraryPage
            ContentFrame.Navigate(typeof(Views.LibraryPage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Views.SettingsPage));
            }
            else
            {
                // Get the tag from the invoked item
                var tag = args.InvokedItemContainer.Tag.ToString();
                switch (tag)
                {
                    case "AllNotes":
                    case "Favorites":
                    case "Recent":
                    case "Folders":
                    case "Trash":
                        ContentFrame.Navigate(typeof(Views.LibraryPage), tag);
                        break;
                    default:
                        // If it's a folder, we might want to pass the folder name
                        ContentFrame.Navigate(typeof(Views.LibraryPage), tag);
                        break;
                }
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // We can handle selection changes if needed
        }
    }
}