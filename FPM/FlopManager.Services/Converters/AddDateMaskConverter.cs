using System;
using System.Windows;
using System.Windows.Data;

namespace FlopManager.Services.Converters
{
    public class AddDateMaskConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if(string.IsNullOrEmpty(s) || s.Length != 8)
            {
            return DependencyProperty.UnsetValue;
            }
            return AddMask(s);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static string AddMask(string s)
        {
            string d = s.Substring(6, 2);
            string m = s.Substring(4, 2);
            string y = s.Substring(0, 4);
            var mask = y + "/" + m + "/" + d;
            return mask;
        }
    }
}
