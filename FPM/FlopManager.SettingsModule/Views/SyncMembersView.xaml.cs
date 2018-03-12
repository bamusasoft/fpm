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
using FlopManager.SettingsModule.ViewModels;

namespace FlopManager.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for SyncMembersView.xaml
    /// </summary>
    [Export("SyncMembersView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SyncMembersView : UserControl
    {
        public SyncMembersView()
        {
            InitializeComponent();
        }
        [Import]
        public SyncMembersViewModel ViewModel
        {
            get { return DataContext as SyncMembersViewModel; }
            set { DataContext = value; }
        }
    }
}
