using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.Services.ViewModelInfrastructure;

namespace FlopManager.Services
{
    [Export(typeof(IGlobalConfigService))]
    public class GlobalConfigService:IGlobalConfigService
    {
        protected ISettings Settings;

        public GlobalConfigService(ISettings settings)
        {
            Settings = settings;
        }

        public void Update(string settingName, object value)
        {
            if(string.IsNullOrEmpty(settingName)) throw new ArgumentNullException(nameof(settingName));
            var setting = Settings[settingName];
            if (setting == null)
            {
                throw new ArgumentException("Setting " + settingName + " not found");
            }
            else if(setting.GetType() != value.GetType())
            {
                throw new InvalidCastException("Unable to cast value to " + setting.GetType());
            }
            else
            {
                Settings[settingName] = value;
                Settings.Save();
            }


        }

        public object Get(string settingName)
        {
            if (String.IsNullOrEmpty(settingName))
                throw new ArgumentNullException(nameof(settingName));
            return Settings[settingName];
        }
    }
}
