using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlopManager.Services;
using FlopManagerLoanModule.ViewModels;
using Prism.Regions;

namespace FlopManagerLoanModule.Views
{
    /// <summary>
    /// Interaction logic for LoanView.xaml
    /// </summary>
    [Export("LoanView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoanView : UserControl
    {
        public LoanView()
        {
            InitializeComponent();
            Loaded += OnLoanViewLoaded;
            
        }

        private void OnLoanViewLoaded(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.LAONS_LIST_REGION, LoansListViewUri);
            RegionManager.RequestNavigate(RegionNames.LOAN_DETAILS_REGION, LoanDeailsViewUri);
        }

        private static readonly Uri LoansListViewUri = new Uri("/LoansListView", UriKind.Relative);
        private static readonly Uri LoanDeailsViewUri = new Uri("/LoanDetailsView", UriKind.Relative);

        [Import]
        public IRegionManager RegionManager;

        [Import]
        public LoanViewModel ViewModel
        {
            get { return DataContext as LoanViewModel;}
            set { DataContext = value; }
        }

    }
}
