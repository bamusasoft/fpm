using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;

namespace FlopManagerLoanModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanDetailsViewModel:EditableViewModelBase, IEntityMapper<Loan>
    {
        [ImportingConstructor]
        public LoanDetailsViewModel(ISettings settings, IEventAggregator eventAggregator, ILogger logger)
        {
            if(settings == null)throw new ArgumentNullException(nameof(settings));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            _eventAggregator = eventAggregator;
            _settings = new GlobalConfigService(settings);
            _logger = logger;
            _selectedLoanFiredOnInitialzation = true;
            eventAggregator.GetEvent<LoanSelectedEvent>().Subscribe(OnLoanSelected);
            Errors = new Dictionary<string, List<string>>();
           

        }

        private void OnLoanSelected(Loan selected)
        {
            if (!_selectedLoanFiredOnInitialzation) //If fired during initialization do not react to it.
            {
                MapFrom(selected);
                OnStateChanged(ViewModelState.Saved);
            }
            else
            {
                _selectedLoanFiredOnInitialzation = false;
            }
        }
        

        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private IGlobalConfigService _settings;
        private readonly ILogger _logger;
        private string _loanNo;
        FamilyMember _member;
        LoanType _selectedloanType;
        PeriodYear _selectedloanYear;
        PaymentSequence _selectedSequence;
        LoanStatus _status;
        PeriodYear _currentYear;
        string _description;
        decimal _amount;
        decimal _paid;
        string _remarks;
        ObservableCollection<LoanType> _loansTypes;
        ObservableCollection<PeriodYear> _years;
        ObservableCollection<PaymentSequence> _paymentSequences;
        ObservableCollection<Loan> _memberLoans;
        FamilyContext _unitOfWork;
        DbSet<Loan> _repository;
        const string Undefined = "لا يوجد";
        private LoanStatus _loanStatus;
        private DelegateCommand<string> _addMemberCommand;
        private DelegateCommand _generateLoanNoCommand;
        IEnumerable<LoanStatus> LoansStatuses { get; set; }
        private bool _selectedLoanFiredOnInitialzation;
        #endregion

        #region "Properties"

        public string LoanNo
        {
            get { return _loanNo; }
            set
            {
                SetProperty(ref _loanNo, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }
        public FamilyMember Member
        {
            get { return _member; }
            set
            {
                SetProperty(ref _member, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        public LoanType SelectedLoanType
        {
            get { return _selectedloanType; }
            set
            {
                SetProperty(ref _selectedloanType, value);
                OnStateChanged(ViewModelState.InEdit);


            }
        }
        public PeriodYear SelectedLoanYear
        {
            get { return _selectedloanYear; }
            set
            {
                SetProperty(ref _selectedloanYear, value);
                if (SelectedLoanYear != null)
                {
                    PaymentSequences = GetSequences(SelectedLoanYear);
                }
                OnStateChanged(ViewModelState.InEdit); ;

            }
        }
        public PaymentSequence SelectedSequence
        {
            get { return _selectedSequence; }
            set
            {
                SetProperty(ref _selectedSequence, value);
                SetChangedFlag(true);

            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
                SetChangedFlag(true);
            }
        }
        public LoanStatus Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                SetProperty(ref _amount, value);
                OnPropertyChanged(nameof(Balance));
                OnStateChanged(ViewModelState.InEdit);

            }
        }
        public decimal Paid
        {
            get { return _paid; }
            private set
            {
                SetProperty(ref _paid, value);
                OnPropertyChanged(nameof(Balance));
            }
        }
        public decimal Balance
        {
            get { return (Amount - Paid); }

        }
        public string Remarks
        {
            get { return _remarks; }
            set
            {
                SetProperty(ref _remarks, value);
                OnStateChanged(ViewModelState.InEdit);

            }
        }
        public ObservableCollection<LoanType> LoansTypes
        {
            get { return _loansTypes; }
            set
            {
                SetProperty(ref _loansTypes, value);
            }
        }
        public ObservableCollection<PeriodYear> Years
        {
            get { return _years; }
            set
            {
                SetProperty(ref _years, value);
            }
        }
        public ObservableCollection<PaymentSequence> PaymentSequences
        {
            get { return _paymentSequences; }
            set
            {
                SetProperty(ref _paymentSequences, value);
            }
        }
        public LoanStatus LoanStatus
        {
            get { return _loanStatus; }
            set { SetProperty(ref _loanStatus, value); }
        }
        public ObservableCollection<Loan> MemberLoans
        {
            get
            {
                return _memberLoans;
            }
            private set
            {
                SetProperty(ref _memberLoans, value);
            }
        }
        public bool CanEditLoan
        {
            get
            {
                return Paid == 0;
            }
        }
        #endregion

        #region Commands

        public ICommand AddMemberCommand
        {
            get { return _addMemberCommand ?? (_addMemberCommand = new DelegateCommand<string>(AddMember)); }
        }
        private void AddMember(string str)
        {
            if (string.IsNullOrEmpty(str)) return;
            int code;
            if (int.TryParse(str, out code))
            {
                try
                {
                    Member = GetFamilyMember(code);

                }
                catch (InvalidOperationException ex)
                {
                    var exception = Helper.ProcessExceptionMessages(ex);
                    _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                    RaiseNotification(exception.UserMsg);

                }
            }
        }

        public ICommand GenerateLoanNoCommand
        {
            get { return _generateLoanNoCommand ?? (_generateLoanNoCommand = new DelegateCommand(GenerateLoanNo)); }
        }

        private void GenerateLoanNo()
        {
            try
            {
                string maxNo = _repository.Max(lon => lon.LoanNo);
                LoanNo = Helper.GenerateLoanNo(_currentYear.Year, maxNo);
            }
            catch (Exception ex)
            {
                var exception = Helper.ProcessExceptionMessages(ex);
                _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                RaiseNotification(exception.UserMsg);
            }
           
        }
        
        #endregion
        #region Helpers
        
        private ObservableCollection<LoanType> GetLoansTypes()
        {
            if (LoansTypes != null) return LoansTypes;
            return new ObservableCollection<LoanType>(_unitOfWork.LoanTypes.ToList());
        }
        private ObservableCollection<PeriodYear> GetYears()
        {
            if (Years != null) return Years;
            return new ObservableCollection<PeriodYear>(_unitOfWork.PeriodYears.ToList());
        }
        ObservableCollection<PaymentSequence> GetSequences(PeriodYear year)
        {
            var yearSeqeunces = _unitOfWork.PaymentSequences.Where(ps => ps.PeriodYear.Year == year.Year);
            return new ObservableCollection<PaymentSequence>(yearSeqeunces);
        }
        
        FamilyMember GetFamilyMember(int code)
        {
            return _unitOfWork.FamilyMembers.Single(fm => fm.Code == code);
        }

        private void ClearView()
        {
            LoanNo = string.Empty;
            Member = null;
            SelectedLoanType = null;
            SelectedLoanYear = null;
            SelectedSequence = null;
            Description = string.Empty;
            Amount = 0;
            Remarks = string.Empty;
            Paid = 0;
        }
     
        #endregion

        #region Base

        protected override void Initialize()
        {
            try
            {
                _unitOfWork = new FamilyContext();
                _repository = _unitOfWork.Loans;
                LoansTypes = GetLoansTypes();
                Years = GetYears();
                _currentYear = _unitOfWork.PeriodYears.Single(x => x.Status == YearStatus.Present);
                OnStateChanged(ViewModelState.AddNew);

            }
            catch (Exception ex)
            {
                var exception = Helper.ProcessExceptionMessages(ex);
                _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                _logger.LogUiNotifications(exception.UserMsg);
                DispatchLoggedNotification(_logger);
                
            }
        }

        protected override void Save()
        {
            if (IsValid())
            {
                try
                {
                    Loan loan = _repository.Find(LoanNo);
                    if (loan == null)
                    {
                        loan = new Loan();
                        MapTo(loan, false);
                        _repository.Add(loan);
                    }
                    else
                    {
                        MapTo(loan, true);
                    }
                    _unitOfWork.SaveChanges();
                    OnStateChanged(ViewModelState.Saved);
                    _eventAggregator.GetEvent<SaveCompletedEvent>().Publish(this);
                }
                catch (Exception ex)
                {
                    var exception = Helper.ProcessExceptionMessages(ex);
                    _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                    RaiseNotification(exception.UserMsg);
                }
            }
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
            if (CanExit())
            {
                ClearView();
                OnStateChanged(ViewModelState.AddNew);
            }
        }

       
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public override void OnStateChanged(ViewModelState state)
        {
            State = state;
            switch (state)
            {
                case ViewModelState.AddNew:
                    State = ViewModelState.AddNew;
                    ResetErrors();
                    SetChangedFlag(false);
                    break;
                case ViewModelState.InEdit:
                    SetChangedFlag(true);
                    break;
                case ViewModelState.Saved:
                    SetChangedFlag(false);
                    break;
                case ViewModelState.Deleted:
                    State = ViewModelState.Deleted;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected override bool IsValid()
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(LoanNo))
            {
                AddError(nameof(LoanNo), ValidationErrorsMessages.LOAN_NO_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(LoanNo), ValidationErrorsMessages.LOAN_NO_MUST_SUPPLIED);
            }
            if (Member == null)
            {
                AddError(nameof(Member), ValidationErrorsMessages.MEMBER_MUST_SUUPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(Member), ValidationErrorsMessages.MEMBER_MUST_SUUPLIED);
            }
            if (SelectedLoanType == null)
            {
                AddError(nameof(SelectedLoanType), ValidationErrorsMessages.LOAN_TYPE_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedLoanType), ValidationErrorsMessages.LOAN_TYPE_MUST_SUPPLIED);
            }
            if (SelectedLoanYear == null)
            {
                AddError(nameof(SelectedLoanYear), ValidationErrorsMessages.YEAR_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedLoanYear), ValidationErrorsMessages.YEAR_MUST_SUPPLIED);
            }
            if (SelectedSequence == null)
            {
                AddError(nameof(SelectedSequence), ValidationErrorsMessages.SEQUENCE_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedSequence), ValidationErrorsMessages.SEQUENCE_MUST_SUPPLIED);
            }
            if (string.IsNullOrEmpty(Description))
            {
                AddError(nameof(Description), ValidationErrorsMessages.LOAN_DESCR_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(Description), ValidationErrorsMessages.LOAN_DESCR_MUST_SUPPLIED);
            }
            if (Amount <= 0)
            {
                AddError(nameof(Amount), ValidationErrorsMessages.AMOUNT_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(Amount), ValidationErrorsMessages.AMOUNT_MUST_SUPPLIED);
            }

            return isValid;
        }

        #endregion

        #region IEntityMapper

        public void MapTo(Loan entity, bool ignoreKey)
        {
            if (!ignoreKey)
            {
                entity.LoanNo = LoanNo;
            }
            entity.FamilyMember = Member;
            entity.LoanType = SelectedLoanType;
            entity.PeriodYear = SelectedLoanYear;
            entity.PaymentSequence = SelectedSequence;
            entity.Amount = Amount;
            entity.Description = Description;
            entity.Remarks = Remarks;
        }

        public void MapFrom(Loan entity)
        {
            LoanNo = entity.LoanNo;
            Member = entity.FamilyMember;
            SelectedLoanType = entity.LoanType;
            SelectedLoanYear = entity.PeriodYear;
            SelectedSequence = entity.PaymentSequence;
            Status = entity.Status;
            Amount = entity.Amount;
            Paid = entity.Paid;
            Description = entity.Description;
            Remarks = entity.Remarks;
        }

        #endregion

        #region Custom Change Observation
        public static bool HasChanges { get; private set; }

        private void SetChangedFlag(bool hasChanges)
        {
            HasChanges = hasChanges;
        }

        public override bool CanExit()
        {
            bool canExit = true;
            if (HasChanges)
            {
                canExit = RaiseConfirmation(SettingsNames.CONFIRM_EXIST_MSG);
            }
            return canExit;
        }

        #endregion
    }

   
}
