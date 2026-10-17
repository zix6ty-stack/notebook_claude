using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface ISettingsService
    {
        // Theme settings
        bool UseSystemTheme { get; set; }
        bool IsDarkModeEnabled { get; set; }

        // Palm rejection
        bool PalmRejectionEnabled { get; set; }

        // Default tools and templates
        string DefaultTool { get; set; } // Pen, Pencil, Highlighter, Eraser, Lasso, Ruler
        string DefaultPaperTemplate { get; set; } // Blank, Lined, Grid, Dotted, Cornell, Todo, WeeklyPlanner

        // Auto-save
        int AutoSaveIntervalSeconds { get; set; } // In seconds

        // General
        bool ShowTooltips { get; set; }

        // Methods
        Task LoadSettingsAsync();
        Task SaveSettingsAsync();
        Task ResetToDefaultAsync();
    }
}