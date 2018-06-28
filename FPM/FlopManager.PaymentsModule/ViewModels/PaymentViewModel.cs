using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Commands;
using Prism.Logging;
using Prism.Regions;

namespace FlopManager.PaymentsModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentViewModel : EditableViewModelBase, IEntityMapper<Payment>
    {
        [ImportingConstructor]
        public PaymentViewModel(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            CanClose = true;
            Title = ViewModelsTitles.CREATE_PAYMENT;
        }

        #region "Fields"

        private readonly ILogger _logger;
        private string _paymentNo;
        protected string _paymentDate;
        private decimal _paymentAmount;
        private FamilyContext _unitOfWork;
        private DbSet<Payment> _repository;
        private ObservableCollection<PeriodYear> _years;
        private ObservableCollection<PaymentSequence> _paymentSequences;
        private DelegateCommand _postPaymentCommand;
        private DelegateCommand _generatePaymentNoCommand;
        private PeriodYear _selectedYear;
        private PaymentSequence _selectedSequence;
        private bool _posted;
        private ICollectionView _currnetYearPayments;
        private Payment _selectedPayment;
        #endregion

        #region "Properties"

        public PeriodYear CurrentYear
        {
            get
            {
                PeriodYear p = null;
                try
                {
                    p = _unitOfWork.PeriodYears.Single(x => x.Status == YearStatus.Present);
                }
                catch (Exception ex) //Fail silently
                {
                    var exception = Helper.ProcessExceptionMessages(ex);
                    _logger.Log(exception.DetialsMsg, Category.Exception, Priority.Low);
                }
                return p;
            }
        }

        public PeriodYear SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
                if (SelectedYear != null)
                {
                    UpdatePaymentSequences(SelectedYear);
                }
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        private void UpdatePaymentSequences(PeriodYear selectedYear)
        {
            PaymentSequences = new ObservableCollection<PaymentSequence>(GetSequences(selectedYear));
        }

        public PaymentSequence SelectedSequence
        {
            get { return _selectedSequence; }
            set
            {
                SetProperty(ref _selectedSequence, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                SetProperty(ref _paymentAmount, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        public string PaymentNo
        {
            get { return _paymentNo; }
            set
            {
                SetProperty(ref _paymentNo, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        public string PaymentDate
        {
            get { return _paymentDate; }
            set
            {
                SetProperty(ref _paymentDate, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }
        public bool Posted
        {
            get { return _posted; }
            set
            {
                SetProperty(ref _posted, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        public ObservableCollection<PeriodYear> Years
        {
            get { return _years; }
            set { SetProperty(ref _years, value); }
        }

        public ObservableCollection<PaymentSequence> PaymentSequences
        {
            get { return _paymentSequences; }
            set { SetProperty(ref _paymentSequences, value); }
        }

        public ICollectionView CurrenYearPayments
        {
            get { return _currnetYearPayments; }
            set { SetProperty(ref _currnetYearPayments, value); }
        }
        public Payment SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                OnSelectedPaymentChanged();
            }
        }
        #endregion

        #region "Base"

        protected override void Save()
        {
            try
            {
                if (IsValid())
                {
                    var payment = _repository.Find(PaymentNo);
                    if (payment == null)
                    {
                        payment = new Payment();
                        MapTo(payment, false);
                        _repository.Add(payment);
                    }
                    _unitOfWork.SaveChanges();
                    OnStateChanged(ViewModelState.Saved);
                }
            }
            catch (Exception ex)
            {
                var exception = Helper.ProcessExceptionMessages(ex);
                _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                RaiseNotification(exception.UserMsg);
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
                    break;
                case ViewModelState.InEdit:
                    break;
                case ViewModelState.Saved:
                    LoadCurrentYearPayments(CurrentYear);
                    break;
                case ViewModelState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected override bool IsValid()
        {
            var isValid = true;
            if (string.IsNullOrEmpty(PaymentNo))
            {
                AddError(nameof(PaymentNo), ValidationErrorsMessages.PAYMENT_NO_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(PaymentNo), ValidationErrorsMessages.PAYMENT_NO_MUST_SUPPLIED);
            }
            if (!PaymentDate.IsValidDate())
            {
                AddError(nameof(PaymentDate), ValidationErrorsMessages.INVALID_DATE);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(PaymentDate), ValidationErrorsMessages.INVALID_DATE);
            }

            if (SelectedYear == null)
            {
                AddError(nameof(SelectedYear), ValidationErrorsMessages.YEAR_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedYear), ValidationErrorsMessages.YEAR_MUST_SUPPLIED);
            }
            if (!AllPastPaymentsPosted(SelectedYear))
            {
                AddError(nameof(SelectedYear), ValidationErrorsMessages.PAST_PAYMENTS_MUST_POSTED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedYear), ValidationErrorsMessages.PAST_PAYMENTS_MUST_POSTED);
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
            if (SequenceAlreadyPaid(SelectedSequence))
            {
                AddError(nameof(SelectedSequence), ValidationErrorsMessages.SEQUENCE_MUST_NOT_ALREADY_PAID);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedSequence), ValidationErrorsMessages.SEQUENCE_MUST_NOT_ALREADY_PAID);
            }
            if (PaymentAmount <= 0)
            {
                AddError(nameof(PaymentAmount), ValidationErrorsMessages.AMOUNT_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(PaymentAmount), ValidationErrorsMessages.AMOUNT_MUST_SUPPLIED);
            }
            return isValid;
        }

        #endregion

        #region "Helper"

        protected override void Initialize()
        {
            try
            {
                _unitOfWork = new FamilyContext();
                _repository = _unitOfWork.Payments;
                Errors = new Dictionary<string, List<string>>();
                Years = GetYears();
                PaymentSequences = new ObservableCollection<PaymentSequence>();
                LoadCurrentYearPayments(CurrentYear);
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

        private void LoadCurrentYearPayments(PeriodYear currentYear)
        {
           
            var currentYearPayments = _repository.Where(p => p.Year == currentYear.Year);
            var list = new ObservableCollection<Payment>(currentYearPayments);
            CurrenYearPayments = new ListCollectionView(list);
            
        }

        private void OnSelectedPaymentChanged()
        {
            if (SelectedPayment != null) //If fired during initialization do not react to it.
            {
                
                MapFrom(SelectedPayment);
                OnStateChanged(ViewModelState.Saved);
            }
           
        }


        private string DecorateNo(int i)
        {
            var s = i.ToString(CultureInfo.InvariantCulture);
            switch (s.Length)
            {
                case 1:
                    return "000" + s;
                case 2:
                    return "00" + s;
                case 3:
                    return "0" + s;
                case 4:
                    return s;
                default:
                    throw new IndexOutOfRangeException("Schedule No. can't be greater than 9999");
            }
        }

        private ObservableCollection<PeriodYear> GetYears()
        {
            return new ObservableCollection<PeriodYear>(_unitOfWork.PeriodYears.Where(x => x.Status == YearStatus.Present)); 
        }

       
        private ObservableCollection<PaymentSequence> GetSequences(PeriodYear selectedYear)
        {
            var yearSeqeunces =
                _unitOfWork.PaymentSequences.Where(ps => ps.PeriodYear.Year == selectedYear.Year).ToList();
            
            return new ObservableCollection<PaymentSequence>(yearSeqeunces);
        }
        /// <summary>
        /// Check whether Selected Sequece is not already paid.
        /// </summary>
        /// <param name="periodYear"></param>
        /// <returns>True is already withdrawled otherwise false.</returns>
        private bool SequenceAlreadyPaid(PaymentSequence selectedSequence)
        {
            if (selectedSequence == null) return false;
            var payments = _repository.Where(p => p.Year == selectedSequence.PeriodYear.Year);
            return payments.Any(p => p.PaymentSequence.Id == selectedSequence.Id);
        }
        private bool AllPastPaymentsPosted(PeriodYear currentYear)
        {
            if (currentYear == null) return false;
            var payments = _repository.Where(p => p.PeriodYear.Year == currentYear.Year);
            return !(payments.Any(p => p.Posted));
        }
        private void ClearView()
        {
            PaymentNo = "";
            PaymentDate = "";
            SelectedYear = null;
            SelectedSequence = null;
            PaymentAmount = 0.00m;
            CurrenYearPayments.MoveCurrentToPosition(-1);
            

        }
        #endregion

        #region Commands

        public ICommand PostPaymentCommand
        {
            get
            {
                if (_postPaymentCommand == null)
                {
                    _postPaymentCommand = new DelegateCommand(PostPayment, CanPost);
                }
                return _postPaymentCommand;
            }
        }

        private void PostPayment()
        {
            var msg = SettingsNames.CONFIRM_POST_MSG;
            if (RaiseConfirmation(msg))
            {
                var current = _unitOfWork.Payments.Single(p => p.PaymentNo == PaymentNo);
                current.Posted = true;
                Posted = true;
            }
        }

        private bool CanPost()
        {
            return !(Posted);
        }

        public ICommand GeneratePaymentNoCommand
        {
            get
            {
                return _generatePaymentNoCommand ?? (_generatePaymentNoCommand = new DelegateCommand(GeneratePaymentNo));
            }
        }

        private void GeneratePaymentNo()
        {
            var currentYear = CurrentYear?.Year;
            var currentYearPortion = currentYear?.Substring(2, 2);
            var dbMaxNo = Payment.MaxNo;

            if (!string.IsNullOrEmpty(dbMaxNo))
            {
                var dbYearPortion = dbMaxNo.Substring(0, 2);
                if (dbYearPortion.Equals(currentYearPortion))
                {
                    var incrementedPortion = dbMaxNo.Substring(3, 3);
                    int incrementedNo;

                    if (int.TryParse(incrementedPortion, out incrementedNo))
                    {
                        incrementedNo++;
                    }
                    PaymentNo = currentYearPortion + DecorateNo(incrementedNo);
                }
                else
                {
                    PaymentNo = Helper.StartNewIncrement(currentYearPortion).ToString(CultureInfo.InvariantCulture);
                }

            }
            else
            {
                PaymentNo = Helper.StartNewIncrement(currentYearPortion).ToString(CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region IEntityMapper

        public void MapTo(Payment entity, bool ignoreKey)
        {
            if (!ignoreKey)
            {
                entity.PaymentNo = PaymentNo;
            }
            entity.PaymentDate = PaymentDate;
            entity.PeriodYear = SelectedYear;
            entity.PaymentSequence = SelectedSequence;
            entity.Amount = PaymentAmount;
        }

        public void MapFrom(Payment entity)
        {
            PaymentNo = entity.PaymentNo;
            PaymentDate = entity.PaymentDate;
            SelectedYear = entity.PeriodYear;
            SelectedSequence = entity.PaymentSequence;
            PaymentAmount = entity.Amount;
            Posted = entity.Posted;
        }

        #endregion

        
    }
}