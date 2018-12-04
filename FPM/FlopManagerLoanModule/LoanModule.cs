using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.Services;
using FlopManagerLoanModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManagerLoanModule
{
    public class LoanModule:IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {

            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.MAIN_NAVIGATION_REGION, typeof(LoanNavigationView));
            regionManager.RegisterViewWithRegion(RegionNames.LAONS_LIST_REGION, typeof(LoansListView));
            regionManager.RegisterViewWithRegion(RegionNames.LOAN_DETAILS_REGION, typeof(LoanDetailsView));
        }
    }
}
