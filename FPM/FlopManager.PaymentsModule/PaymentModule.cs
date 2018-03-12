using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.PaymentsModule.Views;
using FlopManager.Services;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManager.PaymentsModule
{
    [ModuleExport(typeof(PaymentModule))]
    public class PaymentModule:IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MAIN_NAVIGATION_REGION, typeof(PaymentNavigationView));

            //RegionManager.RegisterViewWithRegion(RegionNames.LAONS_LIST_REGION, typeof(LoansListView));
            //RegionManager.RegisterViewWithRegion(RegionNames.LOAN_DETAILS_REGION, typeof(LoanDetailsView));
        }
    }
}
