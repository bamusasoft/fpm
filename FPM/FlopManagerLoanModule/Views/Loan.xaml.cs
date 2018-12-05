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
   
    public partial class Loan : UserControl
    {
        public Loan(IRegionManager regionManager)
        {
            InitializeComponent();
            Loaded += OnLoanViewLoaded;
            _regionManager = regionManager;
        }

        private void OnLoanViewLoaded(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate(RegionNames.LAONS_LIST_REGION, _loansListView);
            _regionManager.RequestNavigate(RegionNames.LOAN_DETAILS_REGION, _loanDeailsView);
        }

        private static readonly string _loansListView = "LoansListView";
        private static readonly string _loanDeailsView = "LoanDetailsView";
        private readonly IRegionManager _regionManager;
    }
}
