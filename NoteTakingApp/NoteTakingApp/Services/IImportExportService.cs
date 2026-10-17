using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface IImportExportService
    {
        // Import functionality
        Task<Notebook> ImportPdfAsNotebookAsync(string pdfFilePath);

        // Export functionality
        Task<string> ExportNotebookAsPdfAsync(string notebookId);
        Task<string> ExportPageAsPngAsync(string pageId);
        Task ShareItemAsync(string itemId, bool isNotebook);
    }
}