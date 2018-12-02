using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public abstract class ListViewModelBase : ViewModelBase, INavigationAware
    {

        #region Fields
        private DelegateCommand<object> _searchCommand;
        private DelegateCommand _printCommand;

        #endregion
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; private set; }
        public InteractionRequest<Notification> NotificationRequest { get; set; }
        #region INavigation
        public abstract void OnNavigatedTo(NavigationContext navigationContext);


        public abstract bool IsNavigationTarget(NavigationContext navigationContext);


        public abstract void OnNavigatedFrom(NavigationContext navigationContext);

        #endregion
        #region Commands

        public ICommand SearchCommand
        {
            get { return _searchCommand ?? ( _searchCommand = new DelegateCommand<object>(Search, CanSearch)); }
        }

        public ICommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new DelegateCommand(Print, CanPrint)); }
        }
        protected abstract void Search(object term);
        protected abstract bool CanSearch(object term);
        protected abstract void Print();
        protected abstract bool CanPrint();


        #endregion

        protected void RaiseNotification(string msg)
        {
            // By invoking the Raise method we are raising the Raised event and triggering any InteractionRequestTrigger that
            // is subscribed to it.
            // As parameters we are passing a Notification, which is a default implementation of INotification provided by Prism
            // and a callback that is executed when the interaction finishes.
            this.NotificationRequest.Raise(
                new Notification
                {
                    Content = msg,
                    Title = SettingsNames.NOTIFY_ACTION_TITLE
                });

        }
        protected bool RaiseConfirmation(string msg)
        {
            // By invoking the Raise method we are raising the Raised event and triggering any InteractionRequestTrigger that
            // is subscribed to it.
            // As parameters we are passing a Confirmation, which is a default implementation of IConfirmation (which inherits
            // from INotification) provided by Prism and a callback that is executed when the interaction finishes.
            bool confirmed = false;
            this.ConfirmationRequest.Raise(
                new Confirmation
                {
                    Content = msg,
                    Title = SettingsNames.CONFIRM_ACTION_TITLE
                },
                c =>
                {
                    confirmed = c.Confirmed;
                });
            return confirmed;
        }
    }
}
