using System;
using System.Windows;
using System.Windows.Data;

namespace FlopManager.Services.Converters
{
    public class NumberToEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is int)
            {
                var i = (int)value;
                if (i == 0)
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            if (value is decimal)
            {
                var i = (decimal)value;
                if (i == 0.0M)
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
