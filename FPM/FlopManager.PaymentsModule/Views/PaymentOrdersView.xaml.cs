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
using FlopManager.PaymentsModule.ViewModels;

namespace FlopManager.PaymentsModule.Views
{
    /// <summary>
    /// Interaction logic for PaymentOrdersView.xaml
    /// </summary>
    [Export("PaymentOrdersView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PaymentOrdersView : UserControl
    {
        public PaymentOrdersView()
        {
            InitializeComponent();
        }
        [Import]
        public PaymentOrderViewModel ViewModel
        {
            get { return DataContext as PaymentOrderViewModel;}
            set { DataContext = value; }
        }
    }
}
