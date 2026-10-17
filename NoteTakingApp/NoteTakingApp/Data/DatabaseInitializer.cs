using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace NoteTakingApp.Data
{
    /// <summary>
    /// Initializes the database and ensures it's ready for use.
    /// Handles creating the database file in the app's local data folder.
    /// </summary>
    public static class DatabaseInitializer
    {
        private static NoteTakingAppDbContext _dbContext;
        private static bool _isInitialized;

        /// <summary>
        /// Initializes the database asynchronously.
        /// </summary>
        public static async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            try
            {
                // Get the local app data folder
                string localFolderPath = ApplicationData.Current.LocalFolder.Path;
                string databasePath = Path.Combine(localFolderPath, "NoteTakingApp.db");

                // Create the database context
                _dbContext = new NoteTakingAppDbContext(databasePath);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                // In a real app, you might want to log this or show an error to the user
                System.Diagnostics.Debug.WriteLine($"Failed to initialize database: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        public static NoteTakingAppDbContext GetContext()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Database has not been initialized. Call InitializeAsync() first.");
            }

            return _dbContext;
        }
    }
}