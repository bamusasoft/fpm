using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.PaymentsModule.Views;
using FlopManager.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManager.PaymentsModule
{
    public class PaymentModule:IModule
    {

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager reegionManager = containerProvider.Resolve<IRegionManager>();
            reegionManager.RegisterViewWithRegion(RegionNames.MAIN_NAVIGATION_REGION, typeof(PaymentNavigationView));

        }
    }
}
