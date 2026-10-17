using Microsoft.UI.Xaml.Data;
using System;

namespace NoteTakingApp.Views.Converters
{
    /// <summary>
    /// Converts enum values to boolean values for comparison.
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Enum enumValue && parameter is string paramString)
            {
                return enumValue.ToString() == paramString;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue && parameter is string paramString && boolValue)
            {
                return Enum.Parse(targetType, paramString);
            }
            return Activator.CreateInstance(targetType);
        }
    }
}