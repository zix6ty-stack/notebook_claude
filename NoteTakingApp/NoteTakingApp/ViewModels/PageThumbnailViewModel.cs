using System;
using Windows.UI.Xaml.Media.Imaging;

namespace NoteTakingApp.ViewModels
{
    /// <summary>
    /// ViewModel for a page thumbnail in the page sidebar.
    /// </summary>
    public class PageThumbnailViewModel : ObservableObject
    {
        private ImageSource _thumbnailSource;
        private int _pageNumber;

        public ImageSource ThumbnailSource
        {
            get => _thumbnailSource;
            set => SetProperty(ref _thumbnailSource, value);
        }

        public int PageNumber
        {
            get => _pageNumber;
            set => SetProperty(ref _pageNumber, value);
        }

        // In a full implementation, this would have a reference to the actual page
        // and methods to generate/update the thumbnail
    }
}