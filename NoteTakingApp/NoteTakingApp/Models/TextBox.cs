using System;

namespace NoteTakingApp.Models
{
    /// <summary>
    /// Represents a rich text overlay on a notebook page.
    /// Supports formatting options like font, size, color, and alignment.
    /// </summary>
    public class TextBox
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PageId { get; set; } = string.Empty; // Foreign key to Page
        public string TextContent { get; set; } = string.Empty;
        public string FontFamily { get; set; } = "Segoe UI";
        public double FontSize { get; set; } = 12.0;
        public string FontWeight { get; set; } = "Normal"; // Normal, Bold, etc.
        public string FontStyle { get; set; } = "Normal"; // Normal, Italic, etc.
        public string TextColor { get; set; } = "#FF000000"; // Black
        public string BackgroundColor { get; set; } = "#00000000"; // Transparent
        public string TextAlignment { get; set; } = "Left"; // Left, Center, Right, Justified
        public double X { get; set; } = 0; // Position on page
        public double Y { get; set; } = 0; // Position on page
        public double Width { get; set; } = 200; // Width of text box
        public double Height { get; set; } = 20; // Height of text box
        public double Rotation { get; set; } = 0; // Rotation in degrees
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}