using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbm.ViewsModel.Helpers
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
        
    }
}
