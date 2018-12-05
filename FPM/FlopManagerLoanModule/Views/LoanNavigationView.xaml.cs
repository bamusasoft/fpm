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
        IRegionManager _regionManager;
        public LoanNavigationView(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
        }
        #region Fields
        private static readonly string _loanView = "Loan";
        private static readonly string _loanTypesView = "LoanTypes";
        private static readonly string _loanStatemView = "LoanStatement";
        private static readonly string _loanPaymentsView = "LoanPayments";

        #endregion
        private void OnNavigateToLoan(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, _loanView);
        }

        private void OnNavigateToLoanTypes(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, _loanTypesView);
        }
        private void OnNavigateToLoanStatement(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, _loanStatemView);
        }

        private void OnNavigateToLoanPayments(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, _loanStatemView);
        }
    }
}
