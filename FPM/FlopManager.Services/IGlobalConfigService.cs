using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.Services
{
    public interface IGlobalConfigService
    {
        void Update(string settingName, object value);
        object Get(string settingName);
    }
}
