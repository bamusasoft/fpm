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
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;

namespace FlopManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
            Loaded += OnShellLoaded;
        }

        private void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            //This is necessary because AvalonDock's DocumentPanTabPanel will be set to
            //FlowDirection.LeftToRight when initialize the DockingManager control, and it will not respect
            //FlowDirection.RightToLeft that I set in XAML of it is parent. Therefore, if we need the 
            //tabs of the DockingManager to be shown from right instead of left we have to get a reference to 
            //DocumentPaneTabPanel in code, I couldn't figure out how to access it in XAML if it could be,
            //Then manually set the FlowDirection.RightToLeft.
            //I used Dispacher.BeginInvok and ContextIdle to insure the all visual aspects are created and loaded,
            //so I can get reference to DocumentPaneTabPanel.
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(
                () =>
                {
                    var s = DockingManager.FindVisualChildren<DocumentPaneTabPanel>().FirstOrDefault();
                    if (s != null) s.FlowDirection = FlowDirection.RightToLeft;
                })
                );
        }

        
    }
}
