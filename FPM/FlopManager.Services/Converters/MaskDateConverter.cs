using System;
using System.Windows.Data;

namespace FlopManager.Services.Converters
{
    class MaskDateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (string.IsNullOrEmpty(s) || s.Length != 8)
            {
                return "";
            }
            return Reverse(s);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (string.IsNullOrEmpty(s) || s.Length != 8)
            {
                return "";
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
            //Match this pattern yyymmdd
            string d = s.Substring(0, 2);
            string m = s.Substring(2, 2);
            string y = s.Substring(4, 4);

            return string.Format("{0}{1}{2}", y, m, d);
        }
    }
}
