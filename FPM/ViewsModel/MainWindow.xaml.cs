using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Fbm.ViewsModel.Views;

namespace Fbm.ViewsModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PeriodSettingsViewClick(object sender, RoutedEventArgs e)
        {
            PeriodView v = new PeriodView();
            ShowView(v);
        }

        void ShowView(Window view)
        {
            view.Owner = this;
            view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            view.Show();
        }

        private void OpenLoanTypesViewClick(object sender, RoutedEventArgs e)
        {
            LoanTypesView v = new LoanTypesView();
            ShowView(v);
        }

        private void OpenLoansViewClick(object sender, RoutedEventArgs e)
        {
            LoansView v = new LoansView();
            ShowView(v);
        }

        private void OpenPaymentSequenceViewClick(object sender, RoutedEventArgs e)
        {
            PaymentSeqView v = new PaymentSeqView();
            ShowView(v);
        }

        private void OpenPaymentsViewClick(object sender, RoutedEventArgs e)
        {
            PaymentsView v = new PaymentsView();
            ShowView(v);
        }

        private void OpenPaymentDetailsViewClick(object sender, RoutedEventArgs e)
        {
            PaymentDetailsView v = new PaymentDetailsView();
            ShowView(v);
        }

        private void OpenSyncMembersViewClick(object sender, RoutedEventArgs e)
        {
            SyncMembersView v = new SyncMembersView();
            ShowView(v);
        }

        private void OpenPaymentOrdersViewClick(object sender, RoutedEventArgs e)
        {
            PaymentOrdersView v = new PaymentOrdersView();
            ShowView(v);
        }

        private void OpenMemberPaymentViewClick(object sender, RoutedEventArgs e)
        {
            MemberPaymentView v = new MemberPaymentView();
            ShowView(v);
        }

        private void OpenPostLoansViewClick(object sender, RoutedEventArgs e)
        {
            PostLoansView v = new PostLoansView();
            ShowView(v);
        }

        private void OpenMemberStatmentViewClick(object sender, RoutedEventArgs e)
        {
            MemberStatmentView v = new MemberStatmentView();
            ShowView(v);
        }

        private void OpenLoansStatementView(object sender, RoutedEventArgs e)
        {
            LoansStatementView v = new LoansStatementView();
            ShowView(v);
        }

        private void OpenOpetionsViewClick(object sender, RoutedEventArgs e)
        {
            OptionsView v = new OptionsView();
            ShowView(v);
        }
    }
}
