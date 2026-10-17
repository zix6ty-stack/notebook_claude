using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NoteTakingApp.Models;
using NoteTakingApp.ViewModels;
using NoteTakingApp.Views;

namespace NoteTakingApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {
        public LibraryPageViewModel ViewModel => (LibraryPageViewModel)DataContext;

        public LibraryPage()
        {
            this.InitializeComponent();
        }

        private void NewNotebookButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show dialog to create a new notebook
            ViewModel.CreateNewNotebook();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Notebook notebook)
            {
                // Navigate to the EditorPage for the first page of the notebook, or create a new page if none exists.
                // For simplicity, we'll just create a new page in the notebook.
                // In a real app, we might want to open the last viewed page or show a page selector.
                var pageService = ServiceLocator.PageService;
                var pages = await pageService.GetPagesByNotebookIdAsync(notebook.Id);
                string pageId = null;
                if (pages.Count > 0)
                {
                    // Open the first page for now
                    pageId = pages[0].Id;
                }
                // Navigate to the EditorPage, passing notebookId and pageId
                if (Window.Current.Content is Frame rootFrame)
                {
                    rootFrame.Navigate(typeof(Views.EditorPage), new { notebookId = notebook.Id, pageId });
                }
            }
        }
    }
}