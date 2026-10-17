using Microsoft.Data.Sqlite;
using NoteTakingApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NoteTakingApp.Data
{
    /// <summary>
    /// SQLite database context for the note-taking application.
    /// Manages database connection and provides methods for data access.
    /// </summary>
    public class NoteTakingAppDbContext
    {
        private readonly string _connectionString;

        public NoteTakingAppDbContext(string databasePath)
        {
            _connectionString = $"Data Source={databasePath}";
            InitializeDatabaseAsync().GetAwaiter().GetResult(); // Initialize synchronously for simplicity
        }

        /// <summary>
        /// Initializes the database by creating tables if they don't exist.
        /// </summary>
        private async Task InitializeDatabaseAsync()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Create notebooks table
            var notebooksTable = connection.CreateCommand();
            notebooksTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS Notebooks (
                    Id TEXT PRIMARY KEY,
                    Title TEXT NOT NULL,
                    CoverColor TEXT NOT NULL,
                    FolderId TEXT,
                    IsFavorite INTEGER DEFAULT 0,
                    IsInRecent INTEGER DEFAULT 0,
                    PaperTemplate TEXT DEFAULT 'Blank',
                    PageSize TEXT DEFAULT 'A4',
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL
                );";
            await notebooksTable.ExecuteNonQueryAsync();

            // Create folders table
            var foldersTable = connection.CreateCommand();
            foldersTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS Folders (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    ParentFolderId TEXT,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL
                );";
            await foldersTable.ExecuteNonQueryAsync();

            // Create pages table
            var pagesTable = connection.CreateCommand();
            pagesTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS Pages (
                    Id TEXT PRIMARY KEY,
                    NotebookId TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    InkData BLOB,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL,
                    FOREIGN KEY (NotebookId) REFERENCES Notebooks(Id) ON DELETE CASCADE
                );";
            await pagesTable.ExecuteNonQueryAsync();

            // Create text boxes table
            var textBoxesTable = connection.CreateCommand();
            textBoxesTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS TextBoxes (
                    Id TEXT PRIMARY KEY,
                    PageId TEXT NOT NULL,
                    TextContent TEXT NOT NULL,
                    FontFamily TEXT NOT NULL,
                    FontSize REAL NOT NULL,
                    FontWeight TEXT NOT NULL,
                    FontStyle TEXT NOT NULL,
                    TextColor TEXT NOT NULL,
                    BackgroundColor TEXT,
                    TextAlignment TEXT NOT NULL,
                    X REAL NOT NULL,
                    Y REAL NOT NULL,
                    Width REAL NOT NULL,
                    Height REAL NOT NULL,
                    Rotation REAL DEFAULT 0,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL,
                    FOREIGN KEY (PageId) REFERENCES Pages(Id) ON DELETE CASCADE
                );";
            await textBoxesTable.ExecuteNonQueryAsync();

            // Create image overlays table
            var imageOverlaysTable = connection.CreateCommand();
            imageOverlaysTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS ImageOverlays (
                    Id TEXT PRIMARY KEY,
                    PageId TEXT NOT NULL,
                    ImagePath TEXT NOT NULL,
                    X REAL NOT NULL,
                    Y REAL NOT NULL,
                    Width REAL NOT NULL,
                    Height REAL NOT NULL,
                    Rotation REAL DEFAULT 0,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL,
                    FOREIGN KEY (PageId) REFERENCES Pages(Id) ON DELETE CASCADE
                );";
            await imageOverlaysTable.ExecuteNonQueryAsync();

            // Create index for faster queries
            var indexNotebookId = connection.CreateCommand();
            indexNotebookId.CommandText = "CREATE INDEX IF NOT IX_Pages_NotebookId ON Pages(NotebookId);";
            await indexNotebookId.ExecuteNonQueryAsync();

            var indexPageIdTextBox = connection.CreateCommand();
            indexPageIdTextBox.CommandText = "CREATE INDEX IF NOT IX_TextBoxes_PageId ON TextBoxes(PageId);";
            await indexPageIdTextBox.ExecuteNonQueryAsync();

            var indexPageIdImage = connection.CreateCommand();
            indexPageIdImage.CommandText = "CREATE INDEX IF NOT IX_ImageOverlays_PageId ON ImageOverlays(PageId);";
            await indexPageIdImage.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Gets a new SQLite connection.
        /// </summary>
        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}