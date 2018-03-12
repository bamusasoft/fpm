using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.Services;
using FlopManagerLoanModule.Views;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;

namespace FlopManagerLoanModule
{
    [ModuleExport(typeof(LoanModule))]
    public class LoanModule:IModule
    {
        [Import]
        public IRegionManager RegionManager;
        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MAIN_NAVIGATION_REGION, typeof (LoanNavigationView));

            RegionManager.RegisterViewWithRegion(RegionNames.LAONS_LIST_REGION, typeof(LoansListView));
            RegionManager.RegisterViewWithRegion(RegionNames.LOAN_DETAILS_REGION, typeof(LoanDetailsView));
        }
    }
}
