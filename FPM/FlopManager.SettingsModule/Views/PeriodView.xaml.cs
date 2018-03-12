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
using FlopManager.SettingsModule.ViewModels;

namespace FlopManager.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for PeriodView.xaml
    /// </summary>
    [Export("PeriodView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PeriodView : UserControl
    {
        public PeriodView()
        {
            InitializeComponent();
        }

        [Import]
        public PeriodViewModel ViewModel
        {
            get { return DataContext as PeriodViewModel; }
            set { DataContext = value; }
        }
    }
}
