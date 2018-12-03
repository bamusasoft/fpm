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
using FlopManagerLoanModule.ViewModels;

namespace FlopManagerLoanModule.Views
{
    /// <summary>
    /// Interaction logic for LoanPaymentsView.xaml
    /// </summary>
    [Export("LoanPaymentsView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoanPaymentsView : UserControl
    {
       
        public LoanPaymentsView()
        {
            InitializeComponent();
        }

        [Import]
        public LoanPaymentsViewModel ViewModel
        {
            get { return DataContext as LoanPaymentsViewModel; }
            set { DataContext = value; }
        }
    }
}

