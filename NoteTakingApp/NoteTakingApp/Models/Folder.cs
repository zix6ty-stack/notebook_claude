using System;

namespace NoteTakingApp.Models
{
    /// <summary>
    /// Represents a folder for organizing notebooks.
    /// Supports hierarchical organization through parent folders.
    /// </>
    public class Folder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string ParentFolderId { get; set; } = null; // For subfolders
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}