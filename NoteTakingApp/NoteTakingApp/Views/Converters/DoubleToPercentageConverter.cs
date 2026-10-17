using Microsoft.UI.Xaml.Data;
using System;

namespace NoteTakingApp.Views.Converters
{
    /// <summary>
    /// Converts double values to percentage strings (e.g., 1.5 -> "150%").
    /// </summary>
    public class DoubleToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                return $"{doubleValue * 100:F0}%";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string stringValue && stringValue.EndsWith("%"))
            {
                if (double.TryParse(stringValue.TrimEnd('%'), out double result))
                {
                    return result / 100.0;
                }
            }
            return 0.0;
        }
    }
}