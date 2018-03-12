using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Regions;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public abstract class ListViewModelBase : ViewModelBase, INavigationAware
    {

        #region Fields
        private DelegateCommand<object> _searchCommand;
        private DelegateCommand _printCommand;
        #endregion

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
    }
}
