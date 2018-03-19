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
using Prism.Regions;

namespace FlopManager.PaymentsModule.Views
{
    /// <summary>
    /// Interaction logic for PaymentNavigationView.xaml
    /// </summary>
    [Export]
    [ViewSortHint("02")]
    public partial class PaymentNavigationView : UserControl
    {
        public PaymentNavigationView()
        {
            InitializeComponent();
        }
        #region Fields
        private static readonly Uri CreatePaymentViewUri = new Uri("/PaymentView", UriKind.Relative);
        private static readonly Uri PaymentOrderViewUri = new Uri("/PaymentOrdersView", UriKind.Relative);
        private static readonly Uri PaymentTransViewUri = new Uri("/PaymentTransView", UriKind.Relative);
        private static readonly Uri MembStatmViewUri = new Uri("/MemberStatmentView", UriKind.Relative);
        [Import]
        public IRegionManager RegionManager;
        #endregion
        private void OnNavigateToCreatePayment(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, CreatePaymentViewUri);
        }

        private void OnNavigateToPaymentOrder(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, PaymentOrderViewUri);
        }

        private void OnNavigateToPaymentTrans(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, PaymentTransViewUri);
        }
        private void OnNavigateToMembStatm(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, MembStatmViewUri);
        }
    }
}
