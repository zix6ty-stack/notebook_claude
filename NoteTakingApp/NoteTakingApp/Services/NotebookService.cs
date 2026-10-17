using NoteTakingApp.Data;
using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for managing notebook data using SQLite storage.
    /// </summary>
    public class NotebookService : INotebookService
    {
        private readonly NoteTakingAppDbContext _dbContext;

        public NotebookService()
        {
            // Initialize database context (will be initialized on first use)
            _dbContext = DatabaseInitializer.GetContext();
        }

        public async Task<List<Notebook>> GetAllNotebooksAsync()
        {
            var notebooks = new List<Notebook>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Title, CoverColor, FolderId, IsFavorite, IsInRecent,
                       PaperTemplate, PageSize, CreatedDate, ModifiedDate
                FROM Notebooks
                ORDER BY ModifiedDate DESC";

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                notebooks.Add(new Notebook
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    CoverColor = reader.GetString(2),
                    FolderId = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsFavorite = reader.GetBoolean(4),
                    IsInRecent = reader.GetBoolean(5),
                    PaperTemplate = reader.GetString(6),
                    PageSize = reader.GetString(7),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(8)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(9))
                });
            }

            return notebooks;
        }

        public async Task<Notebook> GetNotebookByIdAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Title, CoverColor, FolderId, IsFavorite, IsInRecent,
                       PaperTemplate, PageSize, CreatedDate, ModifiedDate
                FROM Notebooks
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", id);

            await using var reader = command.ExecuteReader();
            if (await reader.ReadAsync())
            {
                return new Notebook
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    CoverColor = reader.GetString(2),
                    FolderId = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsFavorite = reader.GetBoolean(4),
                    IsInRecent = reader.GetBoolean(5),
                    PaperTemplate = reader.GetString(6),
                    PageSize = reader.GetString(7),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(8)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(9))
                };
            }

            return null;
        }

        public async Task AddNotebookAsync(Notebook notebook)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Notebooks (Id, Title, CoverColor, FolderId, IsFavorite, IsInRecent,
                                      PaperTemplate, PageSize, CreatedDate, ModifiedDate)
                VALUES (@Id, @Title, @CoverColor, @FolderId, @IsFavorite, @IsInRecent,
                       @PaperTemplate, @PageSize, @CreatedDate, @ModifiedDate)";

            command.Parameters.AddWithValue("@Id", notebook.Id);
            command.Parameters.AddWithValue("@Title", notebook.Title);
            command.Parameters.AddWithValue("@CoverColor", notebook.CoverColor);
            command.Parameters.AddWithValue("@FolderId", (object)notebook.FolderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsFavorite", notebook.IsFavorite);
            command.Parameters.AddWithValue("@IsInRecent", notebook.IsInRecent);
            command.Parameters.AddWithValue("@PaperTemplate", notebook.PaperTemplate);
            command.Parameters.AddWithValue("@PageSize", notebook.PageSize);
            command.Parameters.AddWithValue("@CreatedDate", notebook.CreatedDate.ToString("o"));
            command.Parameters.AddWithValue("@ModifiedDate", notebook.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateNotebookAsync(Notebook notebook)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Notebooks
                SET Title = @Title,
                    CoverColor = @CoverColor,
                    FolderId = @FolderId,
                    IsFavorite = @IsFavorite,
                    IsInRecent = @IsInRecent,
                    PaperTemplate = @PaperTemplate,
                    PageSize = @PageSize,
                    ModifiedDate = @ModifiedDate
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", notebook.Id);
            command.Parameters.AddWithValue("@Title", notebook.Title);
            command.Parameters.AddWithValue("@CoverColor", notebook.CoverColor);
            command.Parameters.AddWithValue("@FolderId", (object)notebook.FolderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsFavorite", notebook.IsFavorite);
            command.Parameters.AddWithValue("@IsInRecent", notebook.IsInRecent);
            command.Parameters.AddWithValue("@PaperTemplate", notebook.PaperTemplate);
            command.Parameters.AddWithValue("@PageSize", notebook.PageSize);
            command.Parameters.AddWithValue("@ModifiedDate", notebook.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteNotebookAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Notebooks WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}