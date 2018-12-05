using FlopManager.Services;
using FlopManagerLoanModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManagerLoanModule
{
    public class LoanModule:IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoanTypes>();
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
