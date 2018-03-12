using System;
using Prism.Mvvm;
using System.Windows.Threading;
using System.Windows;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public abstract class ViewModelBase : BindableBase
    {
        public bool CanClose { get; protected set; }
        public string Title { get; protected set; }
        public virtual bool CanExit()
        {
            return true;
        }

        private Dispatcher CurrentDispatcher
        {
            get { return Application.Current.Dispatcher; }
        }

        protected void RunActionOnUi(DispatcherPriority priority, Action action)
        {
            CurrentDispatcher.Invoke(priority,  action);
        }
        protected void RunActionOnUi(Action action)
        {
           RunActionOnUi(DispatcherPriority.Normal, action);
        }

    }
}
