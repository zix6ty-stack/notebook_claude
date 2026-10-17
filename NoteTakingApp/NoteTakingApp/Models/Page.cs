using System;
using Windows.UI.Input.Inking;

namespace NoteTakingApp.Models
{
    /// <summary>
    /// Represents a page within a notebook.
    /// Contains ink data and references to overlay elements like text boxes and images.
    /// </summary>
    public class Page
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string NotebookId { get; set; } = string.Empty;
        public string Title { get; set; } = "Untitled";
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        // Ink data will be stored as ISF (Ink Serialized Format) in a byte array
        public byte[] InkData { get; set; } = Array.Empty<byte>();
        // Text boxes and images are stored as separate entities in the database
        // with foreign keys referencing this page
    }
}