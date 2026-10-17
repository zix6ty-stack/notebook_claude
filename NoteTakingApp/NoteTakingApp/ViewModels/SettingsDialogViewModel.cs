using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteTakingApp.Services;
using System.Threading.Tasks;

namespace NoteTakingApp.ViewModels
{
    public class SettingsDialogViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private bool _useSystemTheme = true;

        [ObservableProperty]
        private bool _isDarkModeEnabled = false;

        [ObservableProperty]
        private bool _palmRejectionEnabled = true;

        [ObservableProperty]
        private string _defaultTool = "Pen";

        [ObservableProperty]
        private string _defaultPaperTemplate = "Blank";

        [ObservableProperty]
        private int _autoSaveIntervalSeconds = 30;

        [ObservableProperty]
        private bool _showTooltips = true;

        public SettingsDialogViewModel()
        {
            _settingsService = ServiceLocator.SettingsService;
            LoadSettingsAsync();
        }

        private async void LoadSettingsAsync()
        {
            await _settingsService.LoadSettingsAsync();
            UseSystemTheme = _settingsService.UseSystemTheme;
            IsDarkModeEnabled = _settingsService.IsDarkModeEnabled;
            PalmRejectionEnabled = _settingsService.PalmRejectionEnabled;
            DefaultTool = _settingsService.DefaultTool;
            DefaultPaperTemplate = _settingsService.DefaultPaperTemplate;
            AutoSaveIntervalSeconds = _settingsService.AutoSaveIntervalSeconds;
            ShowTooltips = _settingsService.ShowTooltips;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            // Update the settings service with current values
            _settingsService.UseSystemTheme = UseSystemTheme;
            _settingsService.IsDarkModeEnabled = IsDarkModeEnabled;
            _settingsService.PalmRejectionEnabled = PalmRejectionEnabled;
            _settingsService.DefaultTool = DefaultTool;
            _settingsService.DefaultPaperTemplate = DefaultPaperTemplate;
            _settingsService.AutoSaveIntervalSeconds = AutoSaveIntervalSeconds;
            _settingsService.ShowTooltips = ShowTooltips;

            await _settingsService.SaveSettingsAsync();
            // TODO: Close dialog
        }

        [RelayCommand]
        private async Task ResetToDefaultAsync()
        {
            UseSystemTheme = true;
            IsDarkModeEnabled = false;
            PalmRejectionEnabled = true;
            DefaultTool = "Pen";
            DefaultPaperTemplate = "Blank";
            AutoSaveIntervalSeconds = 30;
            ShowTooltips = true;

            await _settingsService.ResetToDefaultAsync();
            // Reload the values from the service
            await LoadSettingsAsync();
        }

        [RelayCommand]
        private void Cancel()
        {
            // TODO: Close dialog without saving
        }
    }
}