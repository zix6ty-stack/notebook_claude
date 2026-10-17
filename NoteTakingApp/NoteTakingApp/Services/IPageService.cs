using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface IPageService
    {
        Task<List<Page>> GetPagesByNotebookIdAsync(string notebookId);
        Task<Page> GetPageByIdAsync(string id);
        Task AddPageAsync(Page page);
        Task UpdatePageAsync(Page page);
        Task DeletePageAsync(string id);
    }
}