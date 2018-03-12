    using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.Services;
using FlopManager.SettingsModule.Views;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManager.SettingsModule
{
    [ModuleExport(typeof(SettingsModule))]
    public class SettingsModule:IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MAIN_NAVIGATION_REGION, typeof (SettingsNavigationView));
        }
    }
}
