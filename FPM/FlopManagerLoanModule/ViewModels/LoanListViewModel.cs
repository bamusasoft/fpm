using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Windows.Data;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.Events;
using Prism.Events;
using Prism.Regions;

namespace FlopManagerLoanModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanListViewModel : ListViewModelBase
    {
        [ImportingConstructor]
        public LoanListViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<SaveCompletedEvent>().Subscribe(OnSaveCompleted);
            Initialize();

        }

        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private ICollectionView _loans;
        private FamilyContext _unitOfWork;
        private DbSet<Loan> _repository;
        #endregion
        #region Properties

        public ICollectionView Loans
        {
            get { return _loans; }
            set { SetProperty(ref _loans, value); }
        }
        #endregion

        #region Helpers

        private void Initialize()
        {
            _unitOfWork = new FamilyContext();
            _repository = _unitOfWork.Loans;
            LoadLoans();
        }

        private void OnSelectedLoanChanged(object sender, EventArgs e)
        {
            Loan selected = Loans.CurrentItem as Loan;
            if (selected != null)
            {
                _eventAggregator.GetEvent<LoanSelectedEvent>().Publish(selected);
            }
        }
        private void LoadLoans()
        {
            if (Loans != null) Loans.CurrentChanged -= OnSelectedLoanChanged;
            var list = new ObservableCollection<Loan>(_repository.ToList());
            Loans = new ListCollectionView(list);
            Loans.CurrentChanged += OnSelectedLoanChanged;
        }
        #endregion

        #region Base


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

        protected override void Search(object term)
        {
            throw new NotImplementedException();
        }

        protected override bool CanSearch(object term)
        {
            throw new NotImplementedException();
        }

        protected override void Print()
        {
            throw new NotImplementedException();
        }

        protected override bool CanPrint()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Events

        private void OnSaveCompleted(object source)
        {
            LoadLoans();
        }
        #endregion
    }
}
