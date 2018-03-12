using System;
using System.Windows;
using System.Windows.Data;

namespace FlopManager.Services.Converters
{
    public class MaskLiteralsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (string.IsNullOrEmpty(s))
            {
                return DependencyProperty.UnsetValue;
            }
            return Reverse(s);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (string.IsNullOrEmpty(s) || s.Length != 12)
            {
               return DependencyProperty.UnsetValue;
            }
            return UnReverse(s);
        }

        private static string Reverse(string s)
        {
            //Match this pattern ddmmyyyy
            string d = s.Substring(6, 2);
            string m = s.Substring(4, 2);
            string y = s.Substring(0, 4);

            return string.Format("{0}{1}{2}", d, m, y);
        }
        private static string UnReverse(string s)
        {
            //Note: s length is 12
            //Match this pattern yyymmdd
            string d = s.Substring(0, 2);
            string m = s.Substring(4, 2);
            string y = s.Substring(8, 4);

            return string.Format("{0}{1}{2}", y, m, d);
        }
    }
}
