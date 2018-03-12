using System.Data.Entity;
using System.Linq;

namespace FlopManager.Services.Helpers
{
    public static class ExtensionMethods
    {
        public static bool IsDigit(this string str)
        {
            foreach (var chr in str.ToCharArray())
            {
                int tempTest;
                if (! int.TryParse(chr.ToString(), out tempTest))
                {
                    return false;
                }
               
            }
            return true;
        }

        public static bool IsValidDate(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (str.Length != 8) return false;
            bool result = false;
            string d = str.Substring(6, 2);
            string m = str.Substring(4, 2);
            string y = str.Substring(0, 4);
            int day;
            int month;
            int year;
            if (int.TryParse(d, out day) &&
                int.TryParse(m, out month) &&
                int.TryParse(y, out year))
            {
                if (
                     (day > 0 && day < 31)
                     &&
                     (month > 0 && month <= 12)
                     &&
                     (year >= 1400 && year <= 1500))
                {
                    result = true;
                }
            }
            return result;
        }

        
    }
    

}
