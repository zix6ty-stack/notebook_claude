using NoteTakingApp.Data;
using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for managing page data using SQLite storage.
    /// </summary>
    public class PageService : IPageService
    {
        private readonly NoteTakingAppDbContext _dbContext;

        public PageService()
        {
            // Initialize database context (will be initialized on first use)
            _dbContext = DatabaseInitializer.GetContext();
        }

        public async Task<List<Page>> GetPagesByNotebookIdAsync(string notebookId)
        {
            var pages = new List<Page>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, NotebookId, Title, InkData, CreatedDate, ModifiedDate
                FROM Pages
                WHERE NotebookId = @NotebookId
                ORDER BY CreatedDate";

            command.Parameters.AddWithValue("@NotebookId", notebookId);

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                var inkDataBlob = reader.GetValue(3);
                byte[] inkData = inkDataBlob is DBNull ? Array.Empty<byte>() : (byte[])inkDataBlob;

                pages.Add(new Page
                {
                    Id = reader.GetString(0),
                    NotebookId = reader.GetString(1),
                    Title = reader.GetString(2),
                    InkData = inkData,
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(4)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(5))
                });
            }

            return pages;
        }

        public async Task<Page> GetPageByIdAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, NotebookId, Title, InkData, CreatedDate, ModifiedDate
                FROM Pages
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", id);

            await using var reader = command.ExecuteReader();
            if (await reader.ReadAsync())
            {
                var inkDataBlob = reader.GetValue(3);
                byte[] inkData = inkDataBlob is DBNull ? Array.Empty<byte>() : (byte[])inkDataBlob;

                return new Page
                {
                    Id = reader.GetString(0),
                    NotebookId = reader.GetString(1),
                    Title = reader.GetString(2),
                    InkData = inkData,
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(4)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(5))
                };
            }

            return null;
        }

        public async Task AddPageAsync(Page page)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Pages (Id, NotebookId, Title, InkData, CreatedDate, ModifiedDate)
                VALUES (@Id, @NotebookId, @Title, @InkData, @CreatedDate, @ModifiedDate)";

            command.Parameters.AddWithValue("@Id", page.Id);
            command.Parameters.AddWithValue("@NotebookId", page.NotebookId);
            command.Parameters.AddWithValue("@Title", page.Title);
            command.Parameters.AddWithValue("@InkData", page.InkData.Length > 0 ? (object)page.InkData : DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", page.CreatedDate.ToString("o"));
            command.Parameters.AddWithValue("@ModifiedDate", page.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdatePageAsync(Page page)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Pages
                SET NotebookId = @NotebookId,
                    Title = @Title,
                    InkData = @InkData,
                    ModifiedDate = @ModifiedDate
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", page.Id);
            command.Parameters.AddWithValue("@NotebookId", page.NotebookId);
            command.Parameters.AddWithValue("@Title", page.Title);
            command.Parameters.AddWithValue("@InkData", page.InkData.Length > 0 ? (object)page.InkData : DBNull.Value);
            command.Parameters.AddWithValue("@ModifiedDate", page.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeletePageAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Pages WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}