using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for backing up and restoring the note library.
    /// Creates a ZIP file containing the database and associated files.
    /// </summary>
    public class BackupRestoreService : IBackupRestoreService
    {
        public async Task BackupLibraryAsync(string backupFilePath)
        {
            try
            {
                // Get the local app data folder
                string localFolderPath = ApplicationData.Current.LocalFolder.Path;

                // Create a temporary directory for the backup
                string tempBackupPath = Path.Combine(Path.GetTempPath(), $"NoteTakingApp_Backup_{Guid.NewGuid()}");
                Directory.CreateDirectory(tempBackupPath);

                try
                {
                    // Copy the database file
                    string databasePath = Path.Combine(localFolderPath, "NoteTakingApp.db");
                    if (File.Exists(databasePath))
                    {
                        File.Copy(databasePath, Path.Combine(tempBackupPath, "NoteTakingApp.db"), overwrite: true);
                    }

                    // Copy any image files stored in the app's local folder
                    // Images are stored with GUID names in the local folder
                    string imagesDir = Path.Combine(localFolderPath, "Images");
                    if (Directory.Exists(imagesDir))
                    {
                        string tempImagesDir = Path.Combine(tempBackupPath, "Images");
                        Directory.CreateDirectory(tempImagesDir);

                        foreach (string imageFile in Directory.GetFiles(imagesDir))
                        {
                            File.Copy(imageFile, Path.Combine(tempImagesDir, Path.GetFileName(imageFile)), overwrite: true);
                        }
                    }

                    // Create ZIP archive
                    ZipFile.CreateFromDirectory(tempBackupPath, backupFilePath);
                }
                finally
                {
                    // Clean up temporary directory
                    if (Directory.Exists(tempBackupPath))
                    {
                        Directory.Delete(tempBackupPath, recursive: true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to create backup: {ex.Message}", ex);
            }
        }

        public async Task RestoreLibraryAsync(string backupFilePath)
        {
            try
            {
                // Get the local app data folder
                string localFolderPath = ApplicationData.Current.LocalFolder.Path;

                // Create a temporary directory for extraction
                string tempRestorePath = Path.Combine(Path.GetTempPath(), $"NoteTakingApp_Restore_{Guid.NewGuid()}");
                Directory.CreateDirectory(tempRestorePath);

                try
                {
                    // Extract ZIP archive
                    ZipFile.ExtractToDirectory(backupFilePath, tempRestorePath);

                    // Restore the database file
                    string backupDatabasePath = Path.Combine(tempRestorePath, "NoteTakingApp.db");
                    string localDatabasePath = Path.Combine(localFolderPath, "NoteTakingApp.db");

                    if (File.Exists(backupDatabasePath))
                    {
                        // Ensure the database is not in use (in a real app, you might need to handle this better)
                        File.Copy(backupDatabasePath, localDatabasePath, overwrite: true);
                    }

                    // Restore image files
                    string backupImagesDir = Path.Combine(tempRestorePath, "Images");
                    string localImagesDir = Path.Combine(localFolderPath, "Images");

                    if (Directory.Exists(backupImagesDir))
                    {
                        // Ensure the images directory exists
                        if (!Directory.Exists(localImagesDir))
                        {
                            Directory.CreateDirectory(localImagesDir);
                        }

                        // Copy each image file
                        foreach (string imageFile in Directory.GetFiles(backupImagesDir))
                        {
                            string fileName = Path.GetFileName(imageFile);
                            string destPath = Path.Combine(localImagesDir, fileName);
                            File.Copy(imageFile, destPath, overwrite: true);
                        }
                    }
                }
                finally
                {
                    // Clean up temporary directory
                    if (Directory.Exists(tempRestorePath))
                    {
                        Directory.Delete(tempRestorePath, recursive: true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to restore from backup: {ex.Message}", ex);
            }
        }
    }
}