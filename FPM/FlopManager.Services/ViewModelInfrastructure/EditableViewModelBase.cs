using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public abstract class EditableViewModelBase : ViewModelBase, INavigationAware, INotifyDataErrorInfo
    {
        #region Fields

        private DelegateCommand _saveCommand;
        private DelegateCommand _deleteCommand;
        private DelegateCommand _printCommand;
        private DelegateCommand<object> _searchCommand;
        private DelegateCommand _addNewCommand;
        private bool _enableSave;
        private bool _enableDelete;
        private bool _enablePrint;
        private bool _enableSearch;
        private bool _enableAddNew;
        #endregion

        #region Constructor
        protected EditableViewModelBase()
        {
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            NotificationRequest = new InteractionRequest<Notification>();
        }

       
        #endregion

        #region Commands and its methods


        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new DelegateCommand(Save)); }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new DelegateCommand(Delete)); }
        }

        public ICommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new DelegateCommand(Print)); }
        }

        public ICommand SearchCommand
        {
            get { return (_searchCommand ?? (_searchCommand = new DelegateCommand<object>(Search))); }

        }

        public ICommand AddNewCommand
        {
            get { return _addNewCommand ?? (_addNewCommand = new DelegateCommand(AddNew)); }
        }

        protected abstract void Save();
        protected abstract void Delete();
        protected abstract void Print();
        protected abstract void Search(object criteria);
        protected abstract void AddNew();
        #endregion

        #region Prism
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; private set; }
        public InteractionRequest<Notification> NotificationRequest { get; set; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Initialize();
        }


        public abstract bool IsNavigationTarget(NavigationContext navigationContext);

        public abstract void OnNavigatedFrom(NavigationContext navigationContext);
        #endregion

        #region Abstract Functions
        public abstract void OnStateChanged(ViewModelState state);
        protected abstract void Initialize();

        #endregion

        #region Common Properties

        public bool EnableSave
        {
            get {return _enableSave; }
            set { SetProperty(ref _enableSave, value); }
        }
        public bool EnableDelete
        {
            get { return _enableDelete; }
            set { SetProperty(ref _enableDelete, value); }
        }
        public bool EnablePrint
        {
            get { return _enablePrint; }
            set { SetProperty(ref _enablePrint, value); }
        }
        public bool EnableSearch
        {
            get { return _enableSearch; }
            set { SetProperty(ref _enableSearch, value); }
        }
        public bool EnableAddNew
        {
            get { return _enableAddNew; }
            set { SetProperty(ref _enableAddNew, value); }
        }
        #endregion
        #region Common Functions


        public ViewModelState State { get; protected set; }
        /// <summary>
        /// This is complete CanExit, if you intend to override it, give your own full implementation and don't call base.CanExit.
        /// </summary>
        /// <returns></returns>
        public override bool CanExit()
        {
            bool canExit = true;
            if (State == ViewModelState.InEdit)
            {
                canExit = RaiseConfirmation(SettingsNames.CONFIRM_EXIST_MSG);
            }
            return canExit;
        }

       
        protected abstract bool IsValid();

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
        /// <summary>
        /// Insure that messages is dispached to the user in the approperiate time, thus it doesn't lost.
        /// </summary>
        /// <param name="logger"></param>
        protected void DispatchLoggedNotification(ILogger logger)
        {
            //DispatcherPriority.ContextIdle insures that notification will be raised after UI processed all it's work. 
            Action repalySavedLogs = () =>
            {
                logger.Callback = RaiseNotification;
                logger.ReplaySavedLogs();
            };
            RunActionOnUi(DispatcherPriority.ContextIdle, repalySavedLogs);
        }

        #endregion

        #region INotifyDataErrorInfo
        protected Dictionary<string, List<string>> Errors { get; set; }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !Errors.ContainsKey(propertyName))
                return null;
            return Errors[propertyName];
        }


        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }
        protected void AddError(string propertyName, string error)
        {
            if (!Errors.ContainsKey(propertyName))
                Errors[propertyName] = new List<string>();
            if (!Errors[propertyName].Contains(error))
            {
                Errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        protected void RemoveError(string propertyName, string error)
        {
            if (Errors.ContainsKey(propertyName) && Errors[propertyName].Contains(error))
            {
                Errors[propertyName].Remove(error);
                if (Errors[propertyName].Count == 0) Errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ResetErrors()
        {
            if (HasErrors)
            {
                List<string> errorProperties = Errors.Select(error => error.Key).ToList();
                Errors.Clear();
                foreach (var errorProperty in errorProperties)
                {
                    RaiseErrorsChanged(errorProperty);
                }
                errorProperties.Clear();
            }
        }
        #endregion

        

       

        
    }
}
