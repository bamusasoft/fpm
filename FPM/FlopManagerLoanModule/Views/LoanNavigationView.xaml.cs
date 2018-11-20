using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using Prism.Regions;

namespace FlopManagerLoanModule.Views
{
    /// <summary>
    /// Interaction logic for LoanNavigationView.xaml
    /// </summary>
    [Export]
    [ViewSortHint("01")]
    public partial class LoanNavigationView : UserControl
    {
       
        public LoanNavigationView()
        {
            InitializeComponent();
        }
        #region Fields
        private static readonly Uri LoanViewUri= new Uri("/LoanView", UriKind.Relative);
        private static readonly Uri LoanTypesViewUri = new Uri("/LoanTypesView", UriKind.Relative);
        private static readonly Uri MemberLoansViewUri = new Uri("/LoanStatementView", UriKind.Relative);
        [Import]
        public IRegionManager RegionManager;
        #endregion
        private void OnNavigateToLoan(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, LoanViewUri);
        }

        private void OnNavigateToLoanTypes(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, LoanTypesViewUri);
        }
        private void OnNavigateToMemberLoans(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, MemberLoansViewUri);
        }
    }
}
