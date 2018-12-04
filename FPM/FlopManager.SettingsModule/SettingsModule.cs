using System;
using System.ComponentModel.Composition;
using FlopManager.Services;
using FlopManager.SettingsModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManager.SettingsModule
{
    public class SettingsModule : IModule
    {
        [Import] public IRegionManager RegionManager;

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var reegionManager = containerProvider.Resolve<IRegionManager>();
            reegionManager.RegisterViewWithRegion(RegionNames.MAIN_NAVIGATION_REGION, typeof(SettingsNavigationView));
        }

        public void Initialize()
        {
        }
    }
}