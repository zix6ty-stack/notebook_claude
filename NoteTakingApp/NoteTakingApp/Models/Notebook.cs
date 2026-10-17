using System;

namespace NoteTakingApp.Models
{
    /// <summary>
    /// Represents a notebook in the note-taking application.
    /// Contains metadata about the notebook and references to its pages.
    /// </summary>
    public class Notebook
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string CoverColor { get; set; } = "#FF007ACC"; // Default teal color
        public string FolderId { get; set; } = null; // For organizing notebooks in folders
        public bool IsFavorite { get; set; } = false;
        public bool IsInRecent { get; set; } = false;
        public string PaperTemplate { get; set; } = "Blank"; // Blank, Lined, Grid, Dotted, Cornell, Todo, WeeklyPlanner
        public string PageSize { get; set; } = "A4"; // A4, Letter, Square, etc.
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        // We can add more properties like page count, etc.
    }
}