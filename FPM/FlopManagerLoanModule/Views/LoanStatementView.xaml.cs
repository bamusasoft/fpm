using FlopManagerLoanModule.ViewModels;
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

namespace FlopManagerLoanModule.Views
{
    /// <summary>
    /// Interaction logic for LoanStatementView.xaml
    /// </summary>
    [Export("LoanStatementView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoanStatementView : UserControl
    {
        public LoanStatementView()
        {
            InitializeComponent();
        }

        [Import]
        public LoanStatementViewModel ViewModel
        {
            get { return DataContext as LoanStatementViewModel; }
            set { DataContext = value; }
        }
    }
}
