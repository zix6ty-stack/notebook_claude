using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;
using NoteTakingApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace NoteTakingApp.ViewModels
{
    public partial class LibraryPageViewModel : ObservableObject
    {
        private readonly INotebookService _notebookService;
        private readonly IFolderService _folderService;
        private readonly ISearchService _searchService;

        [ObservableProperty]
        private ObservableCollection<Notebook> _notebooks = new ObservableCollection<Notebook>();

        [ObservableProperty]
        private bool _isGridView = true;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _currentFilter = "All"; // All, Favorites, Recent, Folder name, Trash

        [ObservableProperty]
        private string _sortOption = "Date Modified"; // Date Modified, Date Created, Name

        public LibraryPageViewModel()
        {
            _notebookService = ServiceLocator.NotebookService;
            _folderService = ServiceLocator.FolderService;
            _searchService = ServiceLocator.SearchService;
            LoadNotebooksAsync();
        }

        private async void LoadNotebooksAsync()
        {
            var notebooks = await _notebookService.GetAllNotebooksAsync();
            ApplyFilterAndSort(notebooks);
        }

        partial void OnSearchQueryChanged(string value)
        {
            ApplyFilterAndSort(_notebooks.ToList());
        }

        partial void OnCurrentFilterChanged(string value)
        {
            ApplyFilterAndSort(_notebooks.ToList());
        }

        partial void OnSortOptionChanged(string value)
        {
            ApplyFilterAndSort(_notebooks.ToList());
        }

        private void ApplyFilterAndSort(List<Notebook> notebooks)
        {
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                notebooks = _searchService.SearchNotebooksAsync(SearchQuery).Result.ToList();
            }

            // Apply folder/category filter
            switch (CurrentFilter)
            {
                case "All":
                    // No additional filtering
                    break;
                case "Favorites":
                    notebooks = notebooks.Where(n => n.IsFavorite).ToList();
                    break;
                case "Recent":
                    notebooks = notebooks.Where(n => n.IsInRecent).ToList();
                    break;
                case "Trash":
                    // TODO: Implement trash functionality
                    notebooks = new List<Notebook>();
                    break;
                default:
                    // Assume it's a folder name
                    notebooks = notebooks.Where(n => n.FolderId != null &&
                                               _folderService.GetFolderByIdAsync(n.FolderId).Result?.Name == CurrentFilter)
                                       .ToList();
                    break;
            }

            // Apply sorting
            switch (SortOption)
            {
                case "Date Modified":
                    notebooks = notebooks.OrderByDescending(n => n.ModifiedDate).ToList();
                    break;
                case "Date Created":
                    notebooks = notebooks.OrderByDescending(n => n.CreatedDate).ToList();
                    break;
                case "Name":
                    notebooks = notebooks.OrderBy(n => n.Title).ToList();
                    break;
            }

            // Update the observable collection
            Notebooks.Clear();
            foreach (var notebook in notebooks)
            {
                Notebooks.Add(notebook);
            }
        }

        [RelayCommand]
        private void ToggleView()
        {
            IsGridView = !IsGridView;
        }

        [RelayCommand]
        private void SetFilter(string filter)
        {
            CurrentFilter = filter;
        }

        [RelayCommand]
        private void SetSort(string sortOption)
        {
            SortOption = sortOption;
        }

        [RelayCommand]
        private async Task CreateNewNotebook()
        {
            // Show the new notebook dialog
            var dialog = new Views.Dialogs.NewNotebookDialog();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // User clicked Create - get the values from the dialog's ViewModel
                var newNotebook = new Notebook
                {
                    Title = ViewModel.Title.Trim(),
                    CoverColor = ViewModel.CoverColor,
                    FolderId = string.IsNullOrEmpty(ViewModel.SelectedFolderId) ? null : ViewModel.SelectedFolderId,
                    PaperTemplate = ViewModel.PaperTemplate,
                    PageSize = ViewModel.PageSize,
                    CreatedDate = System.DateTimeOffset.Now,
                    ModifiedDate = System.DateTimeOffset.Now
                };

                await _notebookService.AddNotebookAsync(newNotebook);
                await LoadNotebooksAsync(); // Reload to include the new notebook
            }
            // If result is Secondary (Cancel) or None, do nothing
        }

        [RelayCommand]
        private void ToggleFavorite(Notebook notebook)
        {
            if (notebook != null)
            {
                notebook.IsFavorite = !notebook.IsFavorite;
                _ = _notebookService.UpdateNotebookAsync(notebook);
                // Refresh the list to reflect the change
                _ = LoadNotebooksAsync();
            }
        }
    }
}