using Microsoft.UI.Xaml.Data;
using System;

namespace NoteTakingApp.Views.Converters
{
    /// <summary>
    /// Converts DateTimeOffset to a relative time string (e.g., "2 hours ago").
    /// </summary>
    public class DateTimeOffsetToRelativeTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                var timeSpan = DateTimeOffset.Now - dateTimeOffset;

                if (timeSpan.TotalSeconds < 60)
                {
                    return "just now";
                }
                else if (timeSpan.TotalMinutes < 60)
                {
                    return $"{Math.Floor(timeSpan.TotalMinutes)} minutes ago";
                }
                else if (timeSpan.TotalHours < 24)
                {
                    return $"{Math.Floor(timeSpan.TotalHours)} hours ago";
                }
                else if (timeSpan.TotalDays < 30)
                {
                    return $"{Math.Floor(timeSpan.TotalDays)} days ago";
                }
                else if (timeSpan.TotalDays < 365)
                {
                    return $"{Math.Floor(timeSpan.TotalDays / 30)} months ago";
                }
                else
                {
                    return $"{Math.Floor(timeSpan.TotalDays / 365)} years ago";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}