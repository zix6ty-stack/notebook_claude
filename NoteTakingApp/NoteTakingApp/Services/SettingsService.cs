using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Service for managing application settings.
    /// Stores settings in a JSON file in the app's local data folder.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "appsettings.json";
        private StorageFile _settingsFile;

        // Theme settings
        public bool UseSystemTheme { get; set; } = true;
        public bool IsDarkModeEnabled { get; set; } = false;

        // Palm rejection
        public bool PalmRejectionEnabled { get; set; } = true;

        // Default tools and templates
        public string DefaultTool { get; set; } = "Pen";
        public string DefaultPaperTemplate { get; set; } = "Blank";

        // Auto-save
        public int AutoSaveIntervalSeconds { get; set; } = 30; // 30 seconds

        // General
        public bool ShowTooltips { get; set; } = true;

        public SettingsService()
        {
            // Initialize with default values
        }

        private async Task EnsureSettingsFileExistsAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            _settingsFile = await localFolder.CreateFileAsync(
                SettingsFileName,
                CreationCollisionOption.OpenIfExists);
        }

        public async Task LoadSettingsAsync()
        {
            await EnsureSettingsFileExistsAsync();

            try
            {
                string jsonContent = await FileIO.ReadTextAsync(_settingsFile);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    var settings = JsonSerializer.Deserialize<SettingsDto>(jsonContent);
                    if (settings != null)
                    {
                        UseSystemTheme = settings.UseSystemTheme;
                        IsDarkModeEnabled = settings.IsDarkModeEnabled;
                        PalmRejectionEnabled = settings.PalmRejectionEnabled;
                        DefaultTool = settings.DefaultTool;
                        DefaultPaperTemplate = settings.DefaultPaperTemplate;
                        AutoSaveIntervalSeconds = settings.AutoSaveIntervalSeconds;
                        ShowTooltips = settings.ShowTooltips;
                    }
                }
            }
            catch (Exception ex)
            {
                // If loading fails, use default values
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
                // Keep default values
            }
        }

        public async Task SaveSettingsAsync()
        {
            await EnsureSettingsFileExistsAsync();

            var settings = new SettingsDto
            {
                UseSystemTheme = UseSystemTheme,
                IsDarkModeEnabled = IsDarkModeEnabled,
                PalmRejectionEnabled = PalmRejectionEnabled,
                DefaultTool = DefaultTool,
                DefaultPaperTemplate = DefaultPaperTemplate,
                AutoSaveIntervalSeconds = AutoSaveIntervalSeconds,
                ShowTooltips = ShowTooltips
            };

            string jsonContent = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await FileIO.WriteTextAsync(_settingsFile, jsonContent);
        }

        public async Task ResetToDefaultAsync()
        {
            UseSystemTheme = true;
            IsDarkModeEnabled = false;
            PalmRejectionEnabled = true;
            DefaultTool = "Pen";
            DefaultPaperTemplate = "Blank";
            AutoSaveIntervalSeconds = 30;
            ShowTooltips = true;

            await SaveSettingsAsync();
        }

        private class SettingsDto
        {
            public bool UseSystemTheme { get; set; }
            public bool IsDarkModeEnabled { get; set; }
            public bool PalmRejectionEnabled { get; set; }
            public string DefaultTool { get; set; }
            public string DefaultPaperTemplate { get; set; }
            public int AutoSaveIntervalSeconds { get; set; }
            public bool ShowTooltips { get; set; }
        }
    }
}