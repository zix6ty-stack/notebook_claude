using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Models;
using NoteTakingApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.ViewModels
{
    /// <summary>
    /// Represents an operation that can be undone or redone.
    /// </summary>
    public enum OperationType
    {
        InkChanged,
        TextAdded,
        TextUpdated,
        TextDeleted,
        ImageAdded,
        ImageUpdated,
        ImageDeleted,
        PageAdded,
        PageDeleted,
        PageReordered
    }

    /// <summary>
    /// Represents an operation in the undo/redo stack.
    /// </summary>
    public class EditorOperation
    {
        public OperationType Type { get; set; }
        public string Description { get; set; }
        public object Data { get; set; } // Serialized state before the operation
        public DateTimeOffset Timestamp { get; set; }
    }

    public partial class EditorPageViewModel : ObservableObject
    {
        private readonly IPageService _pageService;
        private readonly INotebookService _notebookService;

        [ObservableProperty]
        private Page _currentPage = new Page();

        [ObservableProperty]
        private string _notebookId = string.Empty;

        // Toolbar state
        [ObservableProperty]
        private string _currentTool = "Pen"; // Pen, Pencil, Highlighter, Eraser, Lasso, Ruler

        [ObservableProperty]
        private double _penThickness = 2.0;

        [ObservableProperty]
        private Windows.UI.Color _penColor = Windows.UI.Colors.Black;

        [ObservableProperty]
        private double _zoomFactor = 1.0; // 1.0 = 100%

        [ObservableProperty]
        private bool _isSaved = true;

        // Undo/redo stacks
        private readonly Stack<EditorOperation> _undoStack = new Stack<EditorOperation>();
        private readonly Stack<EditorOperation> _redoStack = new Stack<EditorOperation>();

        // Autosave
        private System.Threading.Timer _autosaveTimer;
        private bool _hasUnsavedChanges = false;

        public EditorPageViewModel()
        {
            _pageService = ServiceLocator.PageService;
            _notebookService = ServiceLocator.NotebookService;

            // Setup autosave timer (30 seconds interval)
            _autosaveTimer = new System.Threading.Timer(AutosaveCallback, null,
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        // This method would be called when navigating to the page, e.g., from OnNavigatedTo in the code-behind
        public async Task InitializeAsync(string notebookId, string pageId = null)
        {
            NotebookId = notebookId;
            if (!string.IsNullOrEmpty(pageId))
            {
                CurrentPage = await _pageService.GetPageByIdAsync(pageId);
            }
            else
            {
                // Create a new page for this notebook
                CurrentPage = new Page
                {
                    NotebookId = notebookId,
                    Title = "New Page",
                    CreatedDate = System.DateTimeOffset.Now,
                    ModifiedDate = System.DateTimeOffset.Now
                };
                await _pageService.AddPageAsync(CurrentPage);
            }
            // TODO: Load ink data into the InkCanvas
            // Clear operation stacks when navigating to a new page
            _undoStack.Clear();
            _redoStack.Clear();
            HasUnsavedChanges = false;
            IsSaved = true;
        }

        private void AutosaveCallback(object state)
        {
            // This runs on a thread pool thread
            if (HasUnsavedChanges)
            {
                // We need to marshal back to the UI thread to access the ViewModel properties
                // In a real implementation, we would use DispatcherQueue or similar
                // For simplicity, we'll just save directly (though this isn't ideal)
                _ = SavePageAsync();
            }
        }

        /// <summary>
        /// Updates the ink data of the current page and records the operation for undo/redo.
        /// </summary>
        /// <param name="inkData">The ink data in ISF format.</param>
        public async Task UpdateInkDataAsync(byte[] inkData)
        {
            // Save current state for undo
            var operation = new EditorOperation
            {
                Type = OperationType.InkChanged,
                Description = "Changed ink",
                Data = CurrentPage.InkData, // Save previous ink data
                Timestamp = System.DateTimeOffset.Now
            };
            _undoStack.Push(operation);
            _redoStack.Clear(); // Clear redo stack when new operation is performed

            // Update the page
            CurrentPage.InkData = inkData;
            CurrentPage.ModifiedDate = System.DateTimeOffset.Now;
            await _pageService.UpdatePageAsync(CurrentPage);

            HasUnsavedChanges = true;
            IsSaved = false;
            // TODO: Show subtle "Saving..." indicator
        }

        [RelayCommand]
        private async Task SavePageAsync()
        {
            CurrentPage.ModifiedDate = System.DateTimeOffset.Now;
            await _pageService.UpdatePageAsync(CurrentPage);

            HasUnsavedChanges = false;
            IsSaved = true;
            // TODO: Show subtle "Saved" indicator with debounce
        }

        /// <summary>
        /// Undo the last operation.
        /// </summary>
        [RelayCommand]
        private async Task UndoAsync()
        {
            if (_undoStack.Count > 0)
            {
                var operation = _undoStack.Pop();
                _redoStack.Push(operation);

                // Restore previous state based on operation type
                switch (operation.Type)
                {
                    case OperationType.InkChanged:
                        if (operation.Data is byte[] previousInkData)
                        {
                            CurrentPage.InkData = previousInkData;
                            CurrentPage.ModifiedDate = System.DateTimeOffset.Now;
                            await _pageService.UpdatePageAsync(CurrentPage);
                        }
                        break;
                    // TODO: Implement other operation types
                }

                HasUnsavedChanges = true;
                IsSaved = false;
            }
        }

        /// <summary>
        /// Redo the last undone operation.
        /// </summary>
        [RelayCommand]
        private async Task RedoAsync()
        {
            if (_redoStack.Count > 0)
            {
                var operation = _redoStack.Pop();
                _undoStack.Push(operation);

                // Reapply the operation based on type
                switch (operation.Type)
                {
                    case OperationType.InkChanged:
                        if (operation.Data is byte[] inkData)
                        {
                            CurrentPage.InkData = inkData;
                            CurrentPage.ModifiedDate = System.DateTimeOffset.Now;
                            await _pageService.UpdatePageAsync(CurrentPage);
                        }
                        break;
                    // TODO: Implement other operation types
                }

                HasUnsavedChanges = true;
                IsSaved = false;
            }
        }

        [RelayCommand]
        private void SetTool(string tool)
        {
            CurrentTool = tool;
        }

        [RelayCommand]
        private void SetPenThickness(double thickness)
        {
            PenThickness = thickness;
        }

        [RelayCommand]
        private void SetPenColor(Windows.UI.Color color)
        {
            PenColor = color;
        }

        [RelayCommand]
        private void SetZoomFactor(double zoom)
        {
            ZoomFactor = Math.Max(0.25, Math.Min(5.0, zoom)); // Clamp between 25% and 500%
        }

        /// <summary>
        /// Creates a new page in the current notebook.
        /// </summary>
        [RelayCommand]
        private async Task CreateNewPageAsync()
        {
            var newPage = new Page
            {
                NotebookId = NotebookId,
                Title = "New Page",
                CreatedDate = System.DateTimeOffset.Now,
                ModifiedDate = System.DateTimeOffset.Now
            };

            // Save current state for undo
            var operation = new EditorOperation
            {
                Type = OperationType.PageAdded,
                Description = "Added new page",
                Data = null, // No previous state for new page
                Timestamp = System.DateTimeOffset.Now
            };
            _undoStack.Push(operation);
            _redoStack.Clear();

            await _pageService.AddPageAsync(newPage);
            // TODO: Select the new page

            HasUnsavedChanges = true;
            IsSaved = false;
        }

        /// <summary>
        /// Deletes the current page.
        /// </summary>
        [RelayCommand]
        private async Task DeleteCurrentPageAsync()
        {
            // Save current state for undo
            var operation = new EditorOperation
            {
                Type = OperationType.PageDeleted,
                Description = "Deleted page",
                Data = CurrentPage, // Save the page data
                Timestamp = System.DateTimeOffset.Now
            };
            _undoStack.Push(operation);
            _redoStack.Clear();

            await _pageService.DeletePageAsync(CurrentPage.Id);
            // TODO: Select another page or create a new one

            HasUnsavedChanges = true;
            IsSaved = false;
        }

        // TODO: Implement other page operations (duplicate, reorder)
        // TODO: Implement text box operations
        // TODO: Implement image operations
        // TODO: Implement paper template functionality
    }
}