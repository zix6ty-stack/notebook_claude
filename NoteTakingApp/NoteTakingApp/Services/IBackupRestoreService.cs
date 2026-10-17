using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface IBackupRestoreService
    {
        Task BackupLibraryAsync(string backupFilePath);
        Task RestoreLibraryAsync(string backupFilePath);
    }
}