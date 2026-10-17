using NoteTakingApp.Services;

namespace NoteTakingApp.Services
{
    /// <summary>
    /// Simple service locator for accessing services.
    /// </>
    public static class ServiceLocator
    {
        public static INotebookService NotebookService { get; private set; }
        public static IPageService PageService { get; private set; }
        public static IFolderService FolderService { get; private set; }
        public static ITextBoxService TextBoxService { get; private set; }
        public static ISettingsService SettingsService { get; private set; }
        public static IBackupRestoreService BackupRestoreService { get; private set; }
        public static IImportExportService ImportExportService { get; private set; }
        public static ISearchService SearchService { get; private set; }

        public static void Initialize()
        {
            // Initialize services
            NotebookService = new NotebookService();
            PageService = new PageService();
            FolderService = new FolderService();
            TextBoxService = new TextBoxService();
            SettingsService = new SettingsService();
            BackupRestoreService = new BackupRestoreService();
            ImportExportService = new ImportExportService();
            SearchService = new SearchService();
        }
    }
}