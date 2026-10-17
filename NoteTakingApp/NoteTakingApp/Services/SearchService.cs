using NoteTakingApp.Data;
using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for searching notebooks and pages.
    /// Provides functionality to search across notebook titles and text box content.
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly NoteTakingAppDbContext _dbContext;
        private readonly ITextBoxService _textBoxService;

        public SearchService()
        {
            _dbContext = DatabaseInitializer.GetContext();
            _textBoxService = ServiceLocator.TextBoxService;
        }

        public async Task<List<Notebook>> SearchNotebooksByTitleAsync(string searchTerm)
        {
            var notebooks = new List<Notebook>();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllNotebooksAsync(); // Return all if empty search
            }

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Title, CoverColor, FolderId, IsFavorite, IsInRecent,
                       PaperTemplate, PageSize, CreatedDate, ModifiedDate
                FROM Notebooks
                WHERE Title LIKE @SearchTerm
                ORDER BY ModifiedDate DESC";

            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                notebooks.Add(new Notebook
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    CoverColor = reader.GetString(2),
                    FolderId = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsFavorite = reader.GetBoolean(4),
                    IsInRecent = reader.GetBoolean(5),
                    PaperTemplate = reader.GetString(6),
                    PageSize = reader.GetString(7),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(8)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(9))
                });
            }

            return notebooks;
        }

        public async Task<List<Page>> SearchPagesByTextContentAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Page>();
            }

            var matchingPageIds = new HashSet<string>();

            // Get all text boxes that match the search term
            // Note: This is not the most efficient approach for large datasets
            // In a production app, we would use full-text search capabilities
            var allTextBoxes = await _textBoxService.GetAllTextBoxesAsync();

            foreach (var textBox in allTextBoxes)
            {
                if (textBox.TextContent.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    matchingPageIds.Add(textBox.PageId);
                }
            }

            // Get the pages for the matching IDs
            var pages = new List<Page>();
            foreach (var pageId in matchingPageIds)
            {
                var page = await _pageService.GetPageByIdAsync(pageId);
                if (page != null)
                {
                    pages.Add(page);
                }
            }

            return pages;
        }

        public async Task<List<Notebook>> SearchNotebooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllNotebooksAsync();
            }

            // Search in both notebook titles and text box content
            var notebooks = new List<Notebook>();

            // First, search by title
            var titleResults = await SearchNotebooksByTitleAsync(searchTerm);
            notebooks.AddRange(titleResults);

            // Then, search by text box content (find notebooks that contain pages with matching text)
            var matchingPages = await SearchPagesByTextContentAsync(searchTerm);
            var notebookIdsWithMatchingText = matchingPages
                .Select(p => p.NotebookId)
                .Distinct()
                .ToList();

            // Get notebooks that have pages with matching text
            foreach (var notebookId in notebookIdsWithMatchingText)
            {
                var notebook = await _notebookService.GetNotebookByIdAsync(notebookId);
                if (notebook != null && !notebooks.Any(n => n.Id == notebook.Id))
                {
                    notebooks.Add(notebook);
                }
            }

            // Remove duplicates based on ID (just in case)
            var uniqueNotebooks = new Dictionary<string, Notebook>();
            foreach (var notebook in notebooks)
            {
                if (!uniqueNotebooks.ContainsKey(notebook.Id))
                {
                    uniqueNotebooks[notebook.Id] = notebook;
                }
            }

            return new List<Notebook>(uniqueNotebooks.Values);
        }

        // Helper method to get all notebooks (used when search term is empty)
        private async Task<List<Notebook>> GetAllNotebooksAsync()
        {
            var notebooks = new List<Notebook>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Title, CoverColor, FolderId, IsFavorite, IsInRecent,
                       PaperTemplate, PageSize, CreatedDate, ModifiedDate
                FROM Notebooks
                ORDER BY ModifiedDate DESC";

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                notebooks.Add(new Notebook
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    CoverColor = reader.GetString(2),
                    FolderId = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsFavorite = reader.GetBoolean(4),
                    IsInRecent = reader.GetBoolean(5),
                    PaperTemplate = reader.GetString(6),
                    PageSize = reader.GetString(7),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(8)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(9))
                });
            }

            return notebooks;
        }
    }
}