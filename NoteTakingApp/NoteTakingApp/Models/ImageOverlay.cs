using System;

namespace NoteTakingApp.Models
{
    /// <summary>
    /// Represents an image overlay on a notebook page.
    /// Supports positioning, sizing, and rotation of images.
    /// </summary>
    public class ImageOverlay
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PageId { get; set; } = string.Empty; // Foreign key to Page
        public string ImagePath { get; set; } = string.Empty; // Path to image file in app's local storage
        public double X { get; set; } = 0; // Position on page
        public double Y { get; set; } = 0; // Position on page
        public double Width { get; set; } = 100; // Width of image
        public double Height { get; set; } = 100; // Height of image
        public double Rotation { get; set; } = 0; // Rotation in degrees
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}