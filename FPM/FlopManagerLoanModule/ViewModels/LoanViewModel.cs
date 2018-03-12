using System.ComponentModel.Composition;
using FlopManager.Services;
using FlopManager.Services.ViewModelInfrastructure;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace FlopManagerLoanModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanViewModel : ContainerViewModelBase
    {
        public LoanViewModel()
        {
            Title = ViewModelsTitles.LOANS;
            CanClose = true;
        }

        #region "Fields"
        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
          
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public override bool CanExit()
        {
            var canExit = true;
            if (LoanDetailsViewModel.HasChanges)
            {
                canExit = RaiseConfirmation(SettingsNames.CONFIRM_EXIST_MSG);
            }
            
            return canExit;
        }

       
    }
}