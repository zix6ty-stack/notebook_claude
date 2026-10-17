using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;
using NoteTakingApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace NoteTakingApp.ViewModels
{
    public class NewNotebookDialogViewModel : ObservableObject
    {
        private readonly INotebookService _notebookService;
        private readonly IFolderService _folderService;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _coverColor = "#FF007ACC"; // Default teal

        [ObservableProperty]
        private string _selectedFolderId = string.Empty; // null for no folder

        [ObservableProperty]
        private string _paperTemplate = "Blank"; // Blank, Lined, Grid, Dotted, Cornell, Todo, WeeklyPlanner

        [ObservableProperty]
        private string _pageSize = "A4"; // A4, Letter, Square, etc.

        [ObservableProperty]
        private bool _isFolderSelectionEnabled = true;

        public IEnumerable<Folder> AvailableFolders { get; private set; } = new List<Folder>();
        public IEnumerable<string> PaperTemplates { get; } = new List<string> { "Blank", "Lined", "Grid", "Dotted", "Cornell", "Todo", "WeeklyPlanner" };
        public IEnumerable<string> PageSizes { get; } = new List<string> { "A4", "Letter", "Square" };

        public NewNotebookDialogViewModel()
        {
            _notebookService = ServiceLocator.NotebookService;
            _folderService = ServiceLocator.FolderService;
            _ = LoadAvailableFoldersAsync();
        }

        private async Task LoadAvailableFoldersAsync()
        {
            var folders = await _folderService.GetAllFoldersAsync();
            AvailableFolders = folders;
        }

        [RelayCommand]
        private async Task CreateNotebookAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                // TODO: Show validation error
                return;
            }

            var newNotebook = new Notebook
            {
                Title = Title.Trim(),
                CoverColor = CoverColor,
                FolderId = string.IsNullOrEmpty(SelectedFolderId) ? null : SelectedFolderId,
                PaperTemplate = PaperTemplate,
                PageSize = PageSize,
                CreatedDate = System.DateTimeOffset.Now,
                ModifiedDate = System.DateTimeOffset.Now
            };

            await _notebookService.AddNotebookAsync(newNotebook);
            // TODO: Close dialog and return result
        }

        [RelayCommand]
        private void Cancel()
        {
            // TODO: Close dialog without creating
        }
    }
}