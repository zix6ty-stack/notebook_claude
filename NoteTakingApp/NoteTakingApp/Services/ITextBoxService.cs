using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface ITextBoxService
    {
        Task<List<TextBox>> GetTextBoxesByPageIdAsync(string pageId);
        Task<TextBox> GetTextBoxByIdAsync(string id);
        Task<List<TextBox>> GetAllTextBoxesAsync();
        Task AddTextBoxAsync(TextBox textBox);
        Task UpdateTextBoxAsync(TextBox textBox);
        Task DeleteTextBoxAsync(string id);
    }
}