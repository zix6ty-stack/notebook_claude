using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface INotebookService
    {
        Task<List<Notebook>> GetAllNotebooksAsync();
        Task<Notebook> GetNotebookByIdAsync(string id);
        Task AddNotebookAsync(Notebook notebook);
        Task UpdateNotebookAsync(Notebook notebook);
        Task DeleteNotebookAsync(string id);
    }
}