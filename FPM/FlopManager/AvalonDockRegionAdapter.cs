using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Regions;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace FlopManager
{
    public class AvalonDockRegionAdapter : RegionAdapterBase<DockingManager>
    {

        public AvalonDockRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }

        private IRegion _region;
        [Import(AllowRecomposition = false)]
        public IRegionManager RegionManager;

        [Import(AllowRecomposition = false)]
        public NavigationContext NavigationContext;
        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            _region = region;
            //regionTarget.Layout.ElementRemoved += Layout_ElementRemoved;
            
            region.Views.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        AddAnchorableDocument(regionTarget, e);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveAnchorableDocument(regionTarget, e); //use  to handle when view has closed
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
          

        }

        private void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
        {
            LayoutAnchorable an = e.Element as LayoutAnchorable;

            if (an != null) _region.Remove(an.Content);

            // LayoutRoot root = sender as LayoutRoot;
            //root.RemoveChild(e.Element);
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }

        private void AddAnchorableDocument(DockingManager regionTarget, NotifyCollectionChangedEventArgs e)
        {
            foreach (FrameworkElement element in e.NewItems)
            {
                var view = element as UIElement;

                LayoutDocumentPane documentPane = regionTarget.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                
                if ((view == null) || (documentPane == null))
                {
                    continue;
                }
                ViewModelBase vm = element.DataContext as ViewModelBase;
                var newContentPane = new LayoutAnchorable
                {
                    Content = view,
                    Title = vm?.Title,
                    CanHide = false,
                    CanClose = (vm?.CanClose ?? true),
                    CanFloat = true,
                    
                    
                };
                newContentPane.Closing += NewContentPane_Closing;
                newContentPane.Closed += NewContentPane_Closed;
                documentPane.Children.Add(newContentPane);
                regionTarget.Layout.ActiveContent = newContentPane;

            }
        }

        private void NewContentPane_Closing(object sender, CancelEventArgs e)
        {
            LayoutAnchorable lacn = (sender as LayoutAnchorable);
            FrameworkElement fe = lacn?.Content as FrameworkElement;
            ViewModelBase vm = fe?.DataContext as ViewModelBase;
            e.Cancel = ((!(vm?.CanExit())) ?? false);
        }

        private void NewContentPane_Closed(object sender, EventArgs e)
        {
            LayoutAnchorable layoutAnchorable = sender as LayoutAnchorable;
            _region.Remove(layoutAnchorable?.Content);
        }

        private void RemoveAnchorableDocument(DockingManager regionTarget, NotifyCollectionChangedEventArgs e)
        {

            foreach (FrameworkElement element in e.OldItems)
            {
                //var s = regionTarget.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                //var n =s.Children.Count;
            }
        }
      

    }
}