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
    /// Interaction logic for LoansListView.xaml
    /// </summary>
    //[Export("LoansListView")]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoansListView : UserControl
    {
        public LoansListView()
        {
            InitializeComponent();
            Unloaded += OnLoansListUnloaded;
        }

        [Import]
        public LoanListViewModel ViewModel
        {
            get { return DataContext as LoanListViewModel; }
            set { DataContext = value; }
        }

        private void OnLoansListUnloaded(object sender, RoutedEventArgs e)
        {
           RegionManager.Regions.Remove(RegionNames.LAONS_LIST_REGION);
        }
        [Import]
        public IRegionManager RegionManager;

    }
}
