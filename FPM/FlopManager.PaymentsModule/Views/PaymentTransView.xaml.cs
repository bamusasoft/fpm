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
    /// Interaction logic for PaymentTransView.xaml
    /// </summary>
    [Export("PaymentTransView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PaymentTransView : UserControl
    {
        public PaymentTransView()
        {
            InitializeComponent();
        }
        [Import]
        public PaymentTransViewModel ViewModel
        {
            get { return DataContext as PaymentTransViewModel; }
            set { DataContext = value; }
        }
    }
}
