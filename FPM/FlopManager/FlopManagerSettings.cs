using FlopManager.Services.ViewModelInfrastructure;
using System.ComponentModel.Composition;

namespace FlopManager
{
    [Export(typeof(ISettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FlopManagerSettings:ISettings
    {
        public object this[string propertyName]
        {
            get { return Properties.Settings.Default[propertyName]; }
            set { Properties.Settings.Default[propertyName] = value; }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
