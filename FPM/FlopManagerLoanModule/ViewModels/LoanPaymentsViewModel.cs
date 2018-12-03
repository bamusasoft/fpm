using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.Services;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.DTOs;
using FlopManagerLoanModule.Views;
using Prism.Regions;

namespace FlopManagerLoanModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanPaymentsViewModel:EditableViewModelBase
    {
        private readonly ILogger _logger;
        private readonly ISettings _settings;
        private readonly LoanStatementDto _loanStatement;
         
        [ImportingConstructor]
        public LoanPaymentsViewModel(ILogger logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;
            CanClose = true;
            Errors = new Dictionary<string, List<string>>();
            Title = ViewModelsTitles.LOAN_PAYMENTS;
        }

        #region Propertis

        private string _loanNo;
        public string LoanNo
        {
            get { return _loanNo; }
            set { SetProperty(ref _loanNo, value); }
        }

        #endregion
        #region "Base"
        protected override void Save()
        {
            throw new NotImplementedException();
        }

        protected override void Delete()
        {
            throw new NotImplementedException();
        }

        protected override void Print()
        {
            throw new NotImplementedException();
        }

        protected override void Search(object criteria)
        {
            throw new NotImplementedException();
        }

        protected override void AddNew()
        {
            throw new NotImplementedException();
        }


        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {

            if (navigationContext.Parameters.Count() != 0)
            {
             string loanNo = navigationContext.Parameters["LoanNo"].ToString();
              //Note: you can also pass object
              LoanNo = loanNo;
              //navigationContext.NavigationService.Region.Activate(navigationContext.Uri);
            }

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public override void OnStateChanged(ViewModelState state)
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override bool IsValid()
        {
            throw new NotImplementedException();
        }
        #endregion
        [Import]
        public IRegionManager RegionManager;
    }
}
