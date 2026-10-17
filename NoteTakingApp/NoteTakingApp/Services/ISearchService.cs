using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface ISearchService
    {
        Task<List<Notebook>> SearchNotebooksByTitleAsync(string searchTerm);
        Task<List<Page>> SearchPagesByTextContentAsync(string searchTerm);
        Task<List<Notebook>> SearchNotebooksAsync(string searchTerm); // Search in title and content
    }
}