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
using Prism.Regions;

namespace FlopManager.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for SetttingsNavigationView.xaml
    /// </summary>
    [Export]
    [ViewSortHint("03")]
    public partial class SettingsNavigationView : UserControl
    {
        #region Fields
        private static readonly Uri SyncMembersViewUri = new Uri("/SyncMembersView", UriKind.Relative);
        private static readonly Uri PeriodViewViewUri = new Uri("/PeriodView", UriKind.Relative);
        private static readonly Uri OptionsViewUri = new Uri("/OptionsView", UriKind.Relative);
        [Import]
        public IRegionManager RegionManager;
        #endregion
        public SettingsNavigationView()
        {
            InitializeComponent();
        }

        private void OnNavigateToSyncMember(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, SyncMembersViewUri);   
        }

        private void OnNavigateToPeriod(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, PeriodViewViewUri);
        }
        private void OnNavigateToOptions(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate(RegionNames.MAIN_CONTENT_REGION, OptionsViewUri);
        }
        
    }
}
