using NoteTakingApp.Data;
using NoteTakingApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for managing text box data using SQLite storage.
    /// </summary>
    public class TextBoxService : ITextBoxService
    {
        private readonly NoteTakingAppDbContext _dbContext;

        public TextBoxService()
        {
            // Initialize database context (will be initialized on first use)
            _dbContext = DatabaseInitializer.GetContext();
        }

        public async Task<List<TextBox>> GetTextBoxesByPageIdAsync(string pageId)
        {
            var textBoxes = new List<TextBox>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, PageId, TextContent, FontFamily, FontSize, FontWeight, FontStyle,
                       TextColor, BackgroundColor, TextAlignment, X, Y, Width, Height, Rotation,
                       CreatedDate, ModifiedDate
                FROM TextBoxes
                WHERE PageId = @PageId
                ORDER BY CreatedDate";

            command.Parameters.AddWithValue("@PageId", pageId);

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                textBoxes.Add(new TextBox
                {
                    Id = reader.GetString(0),
                    PageId = reader.GetString(1),
                    TextContent = reader.GetString(2),
                    FontFamily = reader.GetString(3),
                    FontSize = reader.GetDouble(4),
                    FontWeight = reader.GetString(5),
                    FontStyle = reader.GetString(6),
                    TextColor = reader.GetString(7),
                    BackgroundColor = reader.GetString(8),
                    TextAlignment = reader.GetString(9),
                    X = reader.GetDouble(10),
                    Y = reader.GetDouble(11),
                    Width = reader.GetDouble(12),
                    Height = reader.GetDouble(13),
                    Rotation = reader.GetDouble(14),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(15)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(16))
                });
            }

            return textBoxes;
        }

        public async Task<TextBox> GetTextBoxByIdAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, PageId, TextContent, FontFamily, FontSize, FontWeight, FontStyle,
                       TextColor, BackgroundColor, TextAlignment, X, Y, Width, Height, Rotation,
                       CreatedDate, ModifiedDate
                FROM TextBoxes
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", id);

            await using var reader = command.ExecuteReader();
            if (await reader.ReadAsync())
            {
                return new TextBox
                {
                    Id = reader.GetString(0),
                    PageId = reader.GetString(1),
                    TextContent = reader.GetString(2),
                    FontFamily = reader.GetString(3),
                    FontSize = reader.GetDouble(4),
                    FontWeight = reader.GetString(5),
                    FontStyle = reader.GetString(6),
                    TextColor = reader.GetString(7),
                    BackgroundColor = reader.GetString(8),
                    TextAlignment = reader.GetString(9),
                    X = reader.GetDouble(10),
                    Y = reader.GetDouble(11),
                    Width = reader.GetDouble(12),
                    Height = reader.GetDouble(13),
                    Rotation = reader.GetDouble(14),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(15)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(16))
                };
            }

            return null;
        }

        public async Task AddTextBoxAsync(TextBox textBox)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO TextBoxes (Id, PageId, TextContent, FontFamily, FontSize, FontWeight, FontStyle,
                                      TextColor, BackgroundColor, TextAlignment, X, Y, Width, Height, Rotation,
                                      CreatedDate, ModifiedDate)
                VALUES (@Id, @PageId, @TextContent, @FontFamily, @FontSize, @FontWeight, @FontStyle,
                       @TextColor, @BackgroundColor, @TextAlignment, @X, @Y, @Width, @Height, @Rotation,
                       @CreatedDate, @ModifiedDate)";

            command.Parameters.AddWithValue("@Id", textBox.Id);
            command.Parameters.AddWithValue("@PageId", textBox.PageId);
            command.Parameters.AddWithValue("@TextContent", textBox.TextContent);
            command.Parameters.AddWithValue("@FontFamily", textBox.FontFamily);
            command.Parameters.AddWithValue("@FontSize", textBox.FontSize);
            command.Parameters.AddWithValue("@FontWeight", textBox.FontWeight);
            command.Parameters.AddWithValue("@FontStyle", textBox.FontStyle);
            command.Parameters.AddWithValue("@TextColor", textBox.TextColor);
            command.Parameters.AddWithValue("@BackgroundColor", textBox.BackgroundColor);
            command.Parameters.AddWithValue("@TextAlignment", textBox.TextAlignment);
            command.Parameters.AddWithValue("@X", textBox.X);
            command.Parameters.AddWithValue("@Y", textBox.Y);
            command.Parameters.AddWithValue("@Width", textBox.Width);
            command.Parameters.AddWithValue("@Height", textBox.Height);
            command.Parameters.AddWithValue("@Rotation", textBox.Rotation);
            command.Parameters.AddWithValue("@CreatedDate", textBox.CreatedDate.ToString("o"));
            command.Parameters.AddWithValue("@ModifiedDate", textBox.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateTextBoxAsync(TextBox textBox)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE TextBoxes
                SET PageId = @PageId,
                    TextContent = @TextContent,
                    FontFamily = @FontFamily,
                    FontSize = @FontSize,
                    FontWeight = @FontWeight,
                    FontStyle = @FontStyle,
                    TextColor = @TextColor,
                    BackgroundColor = @BackgroundColor,
                    TextAlignment = @TextAlignment,
                    X = @X,
                    Y = @Y,
                    Width = @Width,
                    Height = @Height,
                    Rotation = @Rotation,
                    ModifiedDate = @ModifiedDate
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", textBox.Id);
            command.Parameters.AddWithValue("@PageId", textBox.PageId);
            command.Parameters.AddWithValue("@TextContent", textBox.TextContent);
            command.Parameters.AddWithValue("@FontFamily", textBox.FontFamily);
            command.Parameters.AddWithValue("@FontSize", textBox.FontSize);
            command.Parameters.AddWithValue("@FontWeight", textBox.FontWeight);
            command.Parameters.AddWithValue("@FontStyle", textBox.FontStyle);
            command.Parameters.AddWithValue("@TextColor", textBox.TextColor);
            command.Parameters.AddWithValue("@BackgroundColor", textBox.BackgroundColor);
            command.Parameters.AddWithValue("@TextAlignment", textBox.TextAlignment);
            command.Parameters.AddWithValue("@X", textBox.X);
            command.Parameters.AddWithValue("@Y", textBox.Y);
            command.Parameters.AddWithValue("@Width", textBox.Width);
            command.Parameters.AddWithValue("@Height", textBox.Height);
            command.Parameters.AddWithValue("@Rotation", textBox.Rotation);
            command.Parameters.AddWithValue("@ModifiedDate", textBox.ModifiedDate.ToString("o"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteTextBoxAsync(string id)
        {
            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TextBoxes WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<TextBox>> GetAllTextBoxesAsync()
        {
            var textBoxes = new List<TextBox>();

            await using var connection = _dbContext.GetConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, PageId, TextContent, FontFamily, FontSize, FontWeight, FontStyle,
                       TextColor, BackgroundColor, TextAlignment, X, Y, Width, Height, Rotation,
                       CreatedDate, ModifiedDate
                FROM TextBoxes
                ORDER BY CreatedDate";

            await using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                textBoxes.Add(new TextBox
                {
                    Id = reader.GetString(0),
                    PageId = reader.GetString(1),
                    TextContent = reader.GetString(2),
                    FontFamily = reader.GetString(3),
                    FontSize = reader.GetDouble(4),
                    FontWeight = reader.GetString(5),
                    FontStyle = reader.GetString(6),
                    TextColor = reader.GetString(7),
                    BackgroundColor = reader.GetString(8),
                    TextAlignment = reader.GetString(9),
                    X = reader.GetDouble(10),
                    Y = reader.GetDouble(11),
                    Width = reader.GetDouble(12),
                    Height = reader.GetDouble(13),
                    Rotation = reader.GetDouble(14),
                    CreatedDate = DateTimeOffset.Parse(reader.GetString(15)),
                    ModifiedDate = DateTimeOffset.Parse(reader.GetString(16))
                });
            }

            return textBoxes;
        }
    }
}