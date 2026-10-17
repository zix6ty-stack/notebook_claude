using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface IFolderService
    {
        Task<List<Folder>> GetAllFoldersAsync();
        Task<Folder> GetFolderByIdAsync(string id);
        Task<List<Folder>> GetSubFoldersAsync(string parentFolderId);
        Task AddFolderAsync(Folder folder);
        Task UpdateFolderAsync(Folder folder);
        Task DeleteFolderAsync(string id);
    }
}