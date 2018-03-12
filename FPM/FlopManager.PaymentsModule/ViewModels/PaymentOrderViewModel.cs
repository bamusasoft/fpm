using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
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
    public class PaymentOrderViewModel : EditableViewModelBase
    {
        [ImportingConstructor]
        public PaymentOrderViewModel(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            CanClose = true;
            Title = ViewModelsTitles.PAYMENT_ORDERS;
            Errors = new Dictionary<string, List<string>>();

        }

        #region "Fields"

        private readonly ILogger _logger;
        private string _paymentNo;
        private string _paymentYear;
        private string _paymentSequence;
        private FamilyContext _unitOfWork;
        private DbSet<PaymentInstruction> _instructionsRepository;
        private ObservableCollection<PaymentInstruction> _currentPaymentLoansInstructions;
        private ObservableCollection<PaymentInstruction> _oldOpenLoansOrders;
        private DelegateCommand<string> _createPaymentOrderCommand;

        #endregion

        #region "Commands"

        public ICommand CreatePaymentOrderCommand
            =>
                _createPaymentOrderCommand ??
                (_createPaymentOrderCommand = new DelegateCommand<string>(CreatePaymentOrder));

        private void CreatePaymentOrder(string paymentNo)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentNo)) return;
                if (!PaymentExist(paymentNo))
                {
                    var s = SettingsNames.PAYMENT_NOT_EXIST_MSG;
                    RaiseNotification(s);
                    return;
                }
                var currentPayment = GetPayment(paymentNo);
                SetPaymentValues(currentPayment);
                //1- Check if there are already instructions registred for currentPayment.
                var existingPaymentInstructions = GetExistingInstructions(currentPayment);
                if (existingPaymentInstructions.Any())
                {
                    AddPaymentInstructions(existingPaymentInstructions, currentPayment);
                    OnStateChanged(ViewModelState.Saved);
                    return;
                }
                //2- If no instrcution registere yet, Get current payment loans to add insturctios for them.
                var currentPaymentLoans = FindCurrentPaymentLoans(currentPayment);
                AddToCurrentPaymentLoansInstructions(currentPayment, currentPaymentLoans);
                //3-Then get any unpaid loan that is due previous payment sequences of this year or
                //any unpaid loan that is due previous years to add instructions for them.
                var oldDueLoans = FindOldDueLoans(currentPayment);
                AddToOldDueLoansInstructions(currentPayment, oldDueLoans);
                OnStateChanged(ViewModelState.AddNew);
            }
            catch (Exception ex)
            {
                var exception = Helper.ProcessExceptionMessages(ex);
                _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                RaiseNotification(exception.UserMsg);
            }
        }

        private IList<PaymentInstruction> GetExistingInstructions(Payment currentPayment)
        {
            return _instructionsRepository.Where(o => o.Payment.PaymentNo == currentPayment.PaymentNo).ToList();
        }

        private void AddPaymentInstructions(IList<PaymentInstruction> instructions, Payment currentPayment)
        {
            var currentYearLoans = instructions.Where(x => x.PeriodYear.Year == currentPayment.PeriodYear.Year);
            var oldDueLoans = instructions.Where(x => x.PeriodYear.Year != currentPayment.PeriodYear.Year);
            AddToCurrentPaymentLoansInstructions(currentYearLoans);
            AddToOldDueLoansInstructions(oldDueLoans);
        }

        #endregion

        #region "Properties"

        public ObservableCollection<PaymentInstruction> CurrentPaymentLoansInstructions
        {
            get { return _currentPaymentLoansInstructions; }
            set { SetProperty(ref _currentPaymentLoansInstructions, value); }
        }

        public ObservableCollection<PaymentInstruction> OldDueLoansInstructions
        {
            get { return _oldOpenLoansOrders; }
            set { SetProperty(ref _oldOpenLoansOrders, value); }
        }

        public string PaymentNo
        {
            get { return _paymentNo; }
            set { SetProperty(ref _paymentNo, value); }
        }

        public string PaymentYear
        {
            get { return _paymentYear; }
            set { SetProperty(ref _paymentYear, value); }
        }

        public string PaymentSequence
        {
            get { return _paymentSequence; }
            set { SetProperty(ref _paymentSequence, value); }
        }

        
        #endregion

        #region Helpers

        private void SetPaymentValues(Payment payment)
        {
            PaymentNo = payment.PaymentNo;
            PaymentYear = payment.Year;
            PaymentSequence = payment.PaymentSequence.SequenceDescription;
        }

        private bool PaymentExist(string paymentNo)
        {
            return _unitOfWork.Payments.Any(p => p.PaymentNo.Equals(paymentNo) && !(p.Posted));
        }

        private IEnumerable<Loan> FindCurrentPaymentLoans(Payment currentPayment)
        {
            return SearchForCurrentPaymentLoans(currentPayment);
        }

        private Payment GetPayment(string paymentNo)
        {
            return _unitOfWork.Payments.Single(p => p.PaymentNo.Equals(paymentNo));
        }

        private void AddToCurrentPaymentLoansInstructions(Payment currentPayment, IEnumerable<Loan> currentPaymentLoans)
        {
            foreach (var loan in currentPaymentLoans)
            {
                var instruction = new PaymentInstruction
                {
                    EarnPercent = 0,
                    LoanType = loan.LoanType,
                    Payment = currentPayment,
                    PeriodYear = loan.PeriodYear,
                    OldLoan = false
                };
                AddToCurrentPaymentLoansInstructions(instruction);
            }
        }

        private void AddToCurrentPaymentLoansInstructions(IEnumerable<PaymentInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                AddToCurrentPaymentLoansInstructions(instruction);
            }
        }

        private void AddToCurrentPaymentLoansInstructions(PaymentInstruction instruction)
        {
            CurrentPaymentLoansInstructions.Add(instruction);
        }

        private IEnumerable<Loan> SearchForCurrentPaymentLoans(Payment currentPayment)
        {
            return _unitOfWork.Loans.Where(
                lon => lon.Year.Equals(currentPayment.Year)
                       &&
                       lon.PaymentSequence.Id == currentPayment.PaymentSequence.Id)
                .GroupBy(type => type.LoanTypeCode)
                .Select(lon => lon.FirstOrDefault());
        }

        private IList<Loan> FindOldDueLoans(Payment currentPayment)
        {
            return SearchForOldDueLoans(currentPayment);
        }

        private void AddToOldDueLoansInstructions(Payment currentPayment, IList<Loan> oldDepitLoans)
        {
            foreach (var oldLoan in oldDepitLoans)
            {
                var instruction = new PaymentInstruction
                {
                    EarnPercent = 0,
                    LoanType = oldLoan.LoanType,
                    Payment = currentPayment,
                    PeriodYear = oldLoan.PeriodYear,
                    OldLoan = true
                };
                AddToOldDueLoansInstructions(instruction);
            }
        }

        private void AddToOldDueLoansInstructions(IEnumerable<PaymentInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                AddToOldDueLoansInstructions(instruction);
            }
        }

        private void AddToOldDueLoansInstructions(PaymentInstruction instruction)
        {
            OldDueLoansInstructions.Add(instruction);
        }

        private IList<Loan> SearchForOldDueLoans(Payment currentPayment)
        {
            // Get all loans that:
            //1 does not paid yet.
            //2 and does not belongs to a future year
            //3 and does not belongs to the this payment sequence.
            //Note: In case of current year due loans we, by old loan we mean that the loan is due in this year
            //but its payment sequence is older than the curren payment sequence ( its sequence No is less than current payment sequence No).
            var pastYearsOldLoans = _unitOfWork.Loans.Where(
                lon => lon.Status != LoanStatus.Paid
                       &&
                       lon.PeriodYear.Status == YearStatus.Past
                )
                .GroupBy(type => type.LoanType)
                .Select(lo => lo.FirstOrDefault());
            var currentYearOldLoans = _unitOfWork.Loans.Where(
                lon => lon.Year == currentPayment.Year
                       &&
                       lon.Status != LoanStatus.NotPaid
                       &&
                       lon.PaymentSequence.SequenceNo < currentPayment.PaymentSequence.SequenceNo
                )
                .GroupBy(type => type.LoanType)
                .Select(lo => lo.FirstOrDefault());
            return MergeOldLoans(pastYearsOldLoans, currentYearOldLoans);
        }

        private IList<Loan> MergeOldLoans(IQueryable<Loan> pastYearsOldLoans, IQueryable<Loan> currentYearOldLoans)
        {
            var oldLoans = new List<Loan>();
            oldLoans.AddRange(pastYearsOldLoans);
            oldLoans.AddRange(currentYearOldLoans);

            return oldLoans;
        }

        private IList<PaymentInstruction> MergeInstructions()
        {
            var instructions = new List<PaymentInstruction>();
            instructions.AddRange(CurrentPaymentLoansInstructions);
            instructions.AddRange(OldDueLoansInstructions);
            return instructions;
        }
        private void MapToRepository(IList<PaymentInstruction> instructions)
        {
            foreach (var paymentInstruction in instructions)
            {
               var storeInstruction = _instructionsRepository.Single(x => x.AutoKey == paymentInstruction.AutoKey);
                storeInstruction.EarnPercent = paymentInstruction.EarnPercent;
            }
        }
        #endregion

        #region "Base"

        protected override void Initialize()
        {
            try
            {
                _unitOfWork = new FamilyContext();
                _instructionsRepository = _unitOfWork.PaymentInstructions;

                CurrentPaymentLoansInstructions = new ObservableCollection<PaymentInstruction>();
                OldDueLoansInstructions = new ObservableCollection<PaymentInstruction>();
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
            
            try
            {
                var instructions = MergeInstructions();
                if (State == ViewModelState.AddNew)
                {

                    _instructionsRepository.AddRange(instructions);
                }
                else
                {
                    MapToRepository(instructions);
                }
                _unitOfWork.SaveChanges();
                OnStateChanged(ViewModelState.Saved);
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
                    EnableSave = true;
                    
                    break;
                case ViewModelState.Saved:
                    break;
            }
           
        }

        protected override bool IsValid()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}