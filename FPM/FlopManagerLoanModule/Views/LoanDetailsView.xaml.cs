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
using FlopManagerLoanModule.ViewModels;
using Prism.Regions;

namespace FlopManagerLoanModule.Views
{
    /// <summary>
    /// Interaction logic for LoanDetailsView.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoanDetailsView : UserControl
    {
        public LoanDetailsView()
        {
            InitializeComponent();
            Unloaded += OnLoanDetailsUnloaded;
        }

        private void OnLoanDetailsUnloaded(object sender, RoutedEventArgs e)
        {
            RegionManager.Regions.Remove(RegionNames.LOAN_DETAILS_REGION);
        }

        [Import]
        public LoanDetailsViewModel ViewModel
        {
            get { return DataContext as LoanDetailsViewModel;}
            set { DataContext = value; }
        }

        [Import]
        public IRegionManager RegionManager;
    }
}
