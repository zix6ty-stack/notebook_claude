using NoteTakingApp.Data;
using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for managing folder data using SQLite storage.
    /// </summary>
    public class FolderService : IFolderService
    {
        private readonly NoteTakingAppDbContext _dbContext;

        public FolderService()
        {
            // Initialize database context (will be initialized on first use)
            _dbContext = DatabaseInitializer.GetContext();
        }

        public async Task<List<Folder>> GetAllFoldersAsync()
        {
            var folders = new List<Folder>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, ParentFolderId, CreatedDate, ModifiedDate
                FROM Folders
                ORDER BY Name";

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                folders.Add(new Folder
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    ParentFolderId = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(3)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(4))
                });
            }

            return folders;
        }

        public async Task<Folder> GetFolderByIdAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, ParentFolderId, CreatedDate, ModifiedDate
                FROM Folders
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", id);

            await using var reader = command.ExecuteReader();
            if (await reader.ReadAsync())
            {
                return new Folder
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    ParentFolderId = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(3)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(4))
                };
            }

            return null;
        }

        public async Task<List<Folder>> GetSubFoldersAsync(string parentFolderId)
        {
            var folders = new List<Folder>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, ParentFolderId, CreatedDate, ModifiedDate
                FROM Folders
                WHERE ParentFolderId = @ParentFolderId
                ORDER BY Name";

            command.Parameters.AddWithValue("@ParentFolderId", (object)parentFolderId ?? DBNull.Value);

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                folders.Add(new Folder
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    ParentFolderId = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(3)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(4))
                });
            }

            return folders;
        }

        public async Task AddFolderAsync(Folder folder)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Folders (Id, Name, ParentFolderId, CreatedDate, ModifiedDate)
                VALUES (@Id, @Name, @ParentFolderId, @CreatedDate, @ModifiedDate)";

            command.Parameters.AddWithValue("@Id", folder.Id);
            command.Parameters.AddWithValue("@Name", folder.Name);
            command.Parameters.AddWithValue("@ParentFolderId", (object)folder.ParentFolderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", folder.CreatedDate.ToString("o"));
            command.Parameters.AddWithValue("@ModifiedDate", folder.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateFolderAsync(Folder folder)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Folders
                SET Name = @Name,
                    ParentFolderId = @ParentFolderId,
                    ModifiedDate = @ModifiedDate
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", folder.Id);
            command.Parameters.AddWithValue("@Name", folder.Name);
            command.Parameters.AddWithValue("@ParentFolderId", (object)folder.ParentFolderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModifiedDate", folder.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteFolderAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Folders WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}