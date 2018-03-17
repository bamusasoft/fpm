using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.OfficePrintingService.Excel;
using FlopManager.OfficePrintingService.Excel.Templates;
using FlopManager.PaymentsModule.DTOs;
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
    public class PaymentTransViewModel : EditableViewModelBase
    {
        [ImportingConstructor]
        public PaymentTransViewModel(ILogger logger, ISettings settings)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _logger = logger;
            _settings = new GlobalConfigService(settings);
            CanClose = true;
            Title = ViewModelsTitles.PAYMENT_TRANS;
            Errors = new Dictionary<string, List<string>>();
        }

        #region "Helper Methods"

        private Payment GetPayment(string paymentNo)
        {
            return _unitOfWork.Payments.Find(paymentNo);
        }

        #endregion

        #region "Fields"

        private readonly ILogger _logger;
        private readonly IGlobalConfigService _settings;
        private List<PaymentTransaction> _paymentTransactions;
        private List<LoanPayment> _loanPayments;
        private FamilyContext _unitOfWork;
        private PaymentDetailsFilter _criteria;
        private string _paymentNo;
        private string _paymentSequence;
        private string _paymentYear;
        private string _progCurrent;
        private double _progressCounter;
        private int _maxPaymentTransactionKey;
        private int _maxLoanPaymentsKey;
        private int _checkSerial;
        private int _trasferSerial;
        private DelegateCommand<string> _generatePaymentTransCommand;
        private CollectionView _uiReport;
        private List<PaymentDetailsReport> _uiReportList;
        private Dictionary<Sex, string> _membersGenders;
        private KeyValuePair<Sex, string> _selectedGender;
        private bool _endableGenderFilter;
        private bool _enableUi;
        private string _searchedPaymentNo;
        private bool _isSearchEnabled;
        private string _paymentStatus;
     
        #endregion

        #region "Properties"

        public PaymentDetailsFilter Criteria
        {
            get { return _criteria ?? (_criteria = new PaymentDetailsFilter()); }
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


        public double ProgressCounter
        {
            get { return _progressCounter; }
            set { SetProperty(ref _progressCounter, value); }
        }

        public string ProgCurrent
        {
            get { return _progCurrent; }
            set { SetProperty(ref _progCurrent, value); }
        }

        public int CheckSerial
        {
            get { return _checkSerial; }
            set { SetProperty(ref _checkSerial, value); }
        }

        public int TransferSerial
        {
            get { return _trasferSerial; }
            set { SetProperty(ref _trasferSerial, value); }
        }

        public CollectionView UiReport
        {
            get { return _uiReport; }
            set { SetProperty(ref _uiReport, value); }
        }

        public Dictionary<Sex, string> MembersGenders
        {
            get { return _membersGenders; }
            set { SetProperty(ref _membersGenders, value); }
        }

        public KeyValuePair<Sex, string> SelectedGender
        {
            get { return _selectedGender; }
            set
            {
                SetProperty(ref _selectedGender, value);
                if (UiReport != null)
                {
                    Filter(SelectedGender);
                }
            }
        }

        public bool EnableGenderFilter
        {
            get { return _endableGenderFilter; }
            set { SetProperty(ref _endableGenderFilter, value); }
        }

        public bool EnableUi
        {
            get { return _enableUi; }
            set { SetProperty(ref _enableUi, value); }
        }

        public string PaymentStatus
        {
            get { return _paymentStatus; }
            set
            {
                SetProperty(ref _paymentStatus, value);
            }
        }
        #endregion

        #region Commands

        public ICommand GeneratePaymentTransCommand
        {
            get
            {
                return _generatePaymentTransCommand ??
                       (_generatePaymentTransCommand = new DelegateCommand<string>(GeneratePaymentTransactions));
            }
        }

        private async void GeneratePaymentTransactions(string paymentNo)
        {
            if (string.IsNullOrEmpty(paymentNo)) return;
            try
            {
                OnStateChanged(ViewModelState.Busy);
                var payment = GetPayment(paymentNo);

                if (payment != null)
                {
                    if (PaymentHasTransactions(payment))
                    {
                        _paymentTransactions = GetPaymentTransactions(payment);
                        _loanPayments = GetPaymentLoanPayments(payment);
                        ShowUiReport();
                        OnStateChanged(ViewModelState.Saved);
                    }
                    else
                    {
                        var t = GeneratePaymentTransactionsAsync(payment);
                        await t;
                        ShowUiReport();
                        OnStateChanged(ViewModelState.InEdit);
                    }
                }
            }
            catch (Exception ex)
            {
                if (State == ViewModelState.Busy)
                {
                    OnStateChanged(ViewModelState.AddNew);
                }
                var exception = Helper.ProcessExceptionMessages(ex);
                _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                RaiseNotification(exception.UserMsg);
            }
        }

        private void ShowUiReport()
        {
            foreach (var paymentTransaction in _paymentTransactions)
            {
                var memberLoanPayments =
                    _loanPayments.Where(x => x.PaymentTransaction.MemberCode == paymentTransaction.MemberCode)
                        .ToList();
                decimal loansPaymentsMemberPaid = memberLoanPayments.Sum(x => x.AmountPaid);
                AddPaymentTransactionRowToReport(paymentTransaction, loansPaymentsMemberPaid);
                if (memberLoanPayments.Any())
                {
                    foreach (var memberLoanPayment in memberLoanPayments)
                    {
                        AddLoanPaymentToReport(paymentTransaction, memberLoanPayment);
                    }
                }
            }
            UiReport = new ListCollectionView(_uiReportList);
        }

        private void AddPaymentTransactionRowToReport(PaymentTransaction paymentTransaction, decimal loansPaymentsTotal)
        {
            var row = new PaymentDetailsReport(
                paymentTransaction.TransNo, paymentTransaction.MemberCode,
                paymentTransaction.FamilyMember.FullName,
                paymentTransaction.Payment.Year, paymentTransaction.Payment.PaymentSequence.SequenceNo,
                paymentTransaction.AmountDue, paymentTransaction.ShareNumbers,
                paymentTransaction.AmountDue, 0.00M,
                string.Empty, string.Empty,
                loansPaymentsTotal, paymentTransaction.NetPayments,
                paymentTransaction.FamilyMember.Sex,
                TranslatePaymentMethods(paymentTransaction.PayMethod), paymentTransaction.BankDocNo
                );
            _uiReportList.Add(row);
        }

        private void AddLoanPaymentToReport(PaymentTransaction paymentTransaction, LoanPayment loanPayment)
        {
            var row = new PaymentDetailsReport(
                paymentTransaction.TransNo, paymentTransaction.MemberCode, loanPayment.AmountPaid,
                loanPayment.Loan.Description,
                loanPayment.Loan.Remarks, paymentTransaction.FamilyMember.Sex);
            _uiReportList.Add(row);
        }

        private string TranslatePaymentMethods(PayMethod method)
        {
            switch (method)
            {
                case PayMethod.Check:
                    return SettingsNames.CHECK_IN_ARABIC;
                case PayMethod.BankTransfer:
                    return SettingsNames.TRANSFER_IN_ARABIC;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        #endregion

        #region Helpers

        private void Filter(KeyValuePair<Sex, string> predicate)
        {
            switch (predicate.Key)
            {
                case Sex.Male | Sex.Female:
                    UiReport.Filter = null;
                    break;
                case Sex.Male:
                    UiReport.Filter = FilterByMaleGender;
                    break;
                case Sex.Female:
                    UiReport.Filter = FilterByFamleGender;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool FilterByMaleGender(object obj)
        {
            var temp = obj as PaymentDetailsReport;
            return temp?.Sex == Sex.Male;
        }

        private bool FilterByFamleGender(object obj)
        {
            var temp = obj as PaymentDetailsReport;
            return temp?.Sex == Sex.Female;
        }


        private void RevertLoanStatus(List<LoanPayment> loanPayments)
        {
            foreach (var loanPayment in loanPayments)
            {
                var loan = _unitOfWork.Loans.Single(x => x.LoanNo == loanPayment.LoanNo);
                loan.Status = LoanStatus.NotPaid;
            }
        }

        private void DeletePaymentLoanPayments()
        {
            _unitOfWork.LoanPayments.RemoveRange(_loanPayments);
        }

        private void DeletePaymentTransactions()
        {
            _unitOfWork.PaymentTransactions.RemoveRange(_paymentTransactions);
        }

        private List<LoanPayment> GetPaymentLoanPayments(Payment payment)
        {
            return _unitOfWork.LoanPayments.Where(x => x.PaymentTransaction.PaymentNo == payment.PaymentNo).ToList();
        }

        private List<PaymentTransaction> GetPaymentTransactions(Payment payment)
        {
            return _unitOfWork.PaymentTransactions.Where(x => x.PaymentNo == payment.PaymentNo).ToList();
        }

        private bool PaymentHasTransactions(Payment payment)
        {
            return _unitOfWork.PaymentTransactions.Any(x => x.PaymentNo == payment.PaymentNo);
        }

        private void AddLoanPaymentsToRepositry()
        {
            _unitOfWork.LoanPayments.AddRange(_loanPayments);
        }

        private void AddPaymentTrasactionsToRepository()
        {
            _unitOfWork.PaymentTransactions.AddRange(_paymentTransactions);
        }

        private void ReportProgress(double count, double progress, string currentMember = "")
        {
            double c = ((progress/count)*100);
            ProgressCounter = c;
            ProgCurrent = currentMember;
        }

        #endregion

        #region Base

        protected override void Initialize()
        {
            _unitOfWork = new FamilyContext();
            _paymentTransactions = new List<PaymentTransaction>();
            _loanPayments = new List<LoanPayment>();
            CheckSerial = 10;
            TransferSerial = 20;
            _maxPaymentTransactionKey = GetPaymentTrasactionsMaxKey();
            _maxLoanPaymentsKey = GetLoanPaymentsMaxKey();
            _uiReportList = new List<PaymentDetailsReport>();
            MembersGenders = CreateGenders();
            SelectedGender = MembersGenders.FirstOrDefault();
            OnStateChanged(ViewModelState.AddNew);
        }

        private Dictionary<Sex, string> CreateGenders()
        {
            return new Dictionary<Sex, string>
            {
                {Sex.Male | Sex.Female, "الكل"},
                {Sex.Male, "رجال"},
                {Sex.Female, "سيدات"}
            };
        }


        protected override void Save()
        {
            try
            {
                if (IsValid())
                {
                    AddPaymentTrasactionsToRepository();
                    AddLoanPaymentsToRepositry();
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
            string msg = SettingsNames.CONFIRM_DELETE_MSG;
            if (RaiseConfirmation(msg))
            {
                //var payment = _unitOfWork.Payments.Single(x => x.PaymentNo == "360001");
                //if (PaymentHasTransactions(payment))
                //{
                //_paymentTransactions = GetPaymentTransactions(payment);
                //_loanPayments = GetPaymentLoanPayments(payment);
                try
                {
                    RevertLoanStatus(_loanPayments);

                    DeletePaymentTransactions();
                    DeletePaymentLoanPayments();
                    _unitOfWork.SaveChanges();
                    OnStateChanged(ViewModelState.Deleted);
                }
                catch (Exception ex)
                {
                    var exception = Helper.ProcessExceptionMessages(ex);
                    _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                    RaiseNotification(exception.UserMsg);
                }

                //}
            }
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
                    EnableSave = false;
                    EnableDelete = false;
                    EnableGenderFilter = false;
                    EnableUi = true;
                    EnablePrint = false;
                    break;
                case ViewModelState.InEdit:
                    EnableSave = true;
                    EnableDelete = false;
                    EnableGenderFilter = false;
                    EnableUi = false;
                    EnablePrint = false;

                    break;
                case ViewModelState.Busy:
                    EnableSave = false;
                    EnableDelete = false;
                    EnableGenderFilter = false;
                    EnableUi = false;
                    EnablePrint = false;

                    break;
                case ViewModelState.Saved:
                    EnableSave = false;
                    EnableDelete = true;
                    EnableGenderFilter = true;
                    EnableUi = false;
                    EnablePrint = true;

                    break;
                case ViewModelState.Deleted:
                    EnableSave = false;
                    EnableDelete = false;
                    EnableGenderFilter = false;
                    EnablePrint = false;
                    _uiReportList.Clear();
                    UiReport.Refresh();
                    EnableUi = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected override bool IsValid()
        {
            return true;
        }


        protected override void Print()
        {
            string path =(string) _settings.Get(SettingsNames.MEMBER_STATEMENT_PATH);
            ExcelFileProperties prop = new ExcelFileProperties();
            prop.PrintDirectly = false;
            prop.Path = path;
            prop.StartRowIndex = 2;
            prop.StartcolumnIndex = 1;
            prop.Source = PaymentDetailsReport.FillData(_uiReportList);
            try
            {
              ExcelPrinterBase printer = new PaymentTransTempPrinter();
                printer.Print(prop);
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
            }
        }


        protected override void Search(object criteria)
        {
            if (criteria == null) return;
            var payment = _unitOfWork.Payments.Find(criteria);
            if(payment != null)
            {
                PaymentNo = payment.PaymentNo;
                PaymentYear = payment.Year;
                PaymentSequence = payment.PaySequence.ToString();
                PaymentStatus = payment.Posted ? "مرحلة" : "غير مرحلة";

            }


        }

        #endregion

        #region Generate payment instructions and loan payments 

        private async Task GeneratePaymentTransactionsAsync(Payment payment)
        {
            await Task.Run(() =>
            {
                var elligibleMembers = GetEligibleMembers();
                int count = elligibleMembers.Count;
                int progress = 0;
                foreach (var elligibleMember in elligibleMembers)
                {
                    var paymentTransaction = CreatePaymentTransactionForMember(elligibleMember, payment);
                    if (MemberHasLoansDueThisPayment(elligibleMember, payment))
                    {
                        var loansDueCurrentPayment = GetMemberLoansDueCurrentPayment(elligibleMember, payment);
                        PayMemberLoans(loansDueCurrentPayment, paymentTransaction, payment);
                    }
                    if (MemberHasOldLoansDue(elligibleMember, payment))
                    {
                        var oldDueLoans = GetMemberOldDueLoans(elligibleMember, payment);
                        PayMemberLoans(oldDueLoans, paymentTransaction, payment);
                    }
                    _paymentTransactions.Add(paymentTransaction);
                    ReportProgress(count, progress, elligibleMember.FullName);
                    progress++;
                }
            });
        }

        private IEnumerable<Loan> GetMemberOldDueLoans(FamilyMember elligibleMember, Payment payment)
        {
            return
                _unitOfWork.Loans.Where(
                    loan =>
                        loan.MemberCode == elligibleMember.Code && loan.Status == LoanStatus.NotPaid &&
                        loan.PeriodYear.Status == YearStatus.Past ||
                        loan.MemberCode == elligibleMember.Code && loan.Status == LoanStatus.NotPaid &&
                        loan.PaymentSequence.SequenceNo < payment.PaymentSequence.SequenceNo);
        }

        private void PayMemberLoans(IEnumerable<Loan> loans, PaymentTransaction paymentTransaction, Payment payment)
        {
            foreach (var loan in loans)
            {
                var loanPayment = CreateLoanPayment(payment, paymentTransaction, loan);
                paymentTransaction.NetPayments = paymentTransaction.NetPayments - loanPayment.AmountPaid;
                UpdateLoanStatus(loan, loanPayment);
                _loanPayments.Add(loanPayment);
            }
        }

        private void UpdateLoanStatus(Loan loan, LoanPayment loanPayment)
        {
            decimal previousLoanPayments =
                _unitOfWork.LoanPayments.Where(x => x.LoanNo == loan.LoanNo).Sum(s => (decimal?) s.AmountPaid) ?? 0;
            decimal totalAmountPaidSoFar = previousLoanPayments + loanPayment.AmountPaid;
            if (loan.Amount == totalAmountPaidSoFar)
            {
                //Remark: this will update entity in unitOfWork, Make sure to save changes on the same unitOfWork in order for change to be reflected in database.
                loan.Status = LoanStatus.Paid;
            }
        }

        private LoanPayment CreateLoanPayment(Payment payment, PaymentTransaction paymentTransaction, Loan loan)
        {
            return new LoanPayment
            {
                DocNo = GenerateLoanPaymentDocNo(),
                DocDate = paymentTransaction.TransDate,
                LoanNo = loan.LoanNo,
                PaymentNo = payment.PaymentNo,
                TransNo = paymentTransaction.TransNo,
                AmountPaid = CalculateAmountToPaid(payment, loan),
                //Add newly after testing
                Loan = loan,
                PaymentTransaction = paymentTransaction
            };
        }

        private int GenerateLoanPaymentDocNo()
        {
            _maxLoanPaymentsKey++;
            return _maxLoanPaymentsKey;
        }

        private decimal CalculateAmountToPaid(Payment payment, Loan loan)
        {
            var instruction = GetInstructionForThisLoan(payment, loan);
            return ((loan.Amount*instruction.EarnPercent)/100);
        }

        private PaymentInstruction GetInstructionForThisLoan(Payment payment, Loan loan)
        {
            return
                _unitOfWork.PaymentInstructions.Single(
                    x => x.PaymentNo == payment.PaymentNo
                    && x.LoanType.Code == loan.LoanType.Code
                    && x.Year == loan.Year
                    );
        }

        private IEnumerable<Loan> GetMemberLoansDueCurrentPayment(FamilyMember elligibleMember, Payment payment)
        {
            return
                _unitOfWork.Loans.Where(
                    x =>
                        x.MemberCode == elligibleMember.Code && x.Year == payment.Year &&
                        x.PaymentSequence.Id == payment.PaymentSequence.Id);
        }

        private bool MemberHasOldLoansDue(FamilyMember elligibleMember, Payment payment)
        {
            return
                _unitOfWork.Loans.Any(
                    loan =>
                        loan.MemberCode == elligibleMember.Code && loan.Status == LoanStatus.NotPaid &&
                        loan.PeriodYear.Status == YearStatus.Past ||
                        loan.MemberCode == elligibleMember.Code && loan.Status == LoanStatus.NotPaid &&
                        loan.PaymentSequence.SequenceNo < payment.PaymentSequence.SequenceNo);
        }

        private bool MemberHasLoansDueThisPayment(FamilyMember elligibleMember, Payment payment)
        {
            return
                _unitOfWork.Loans.Any(
                    x =>
                        x.MemberCode == elligibleMember.Code && x.Year == payment.Year &&
                        x.PaymentSequence.Id == payment.PaymentSequence.Id);
        }

        private PaymentTransaction CreatePaymentTransactionForMember(FamilyMember elligibleMember, Payment payment)
        {
            var transaction = new PaymentTransaction
            {
                TransNo = GenerateTransNo(elligibleMember.PayMethod),
                TransDate = payment.PaymentDate,
                PaymentNo = payment.PaymentNo,
                MemberCode = elligibleMember.Code,
                ShareNumbers = elligibleMember.Shares,
                AmountDue = (payment.Amount*elligibleMember.Shares),
                NetPayments = (payment.Amount*elligibleMember.Shares),
                PayMethod = elligibleMember.PayMethod,
                BankDocNo = GenerateBankDocNo(elligibleMember.PayMethod),
                //Add newly after testing
                FamilyMember = elligibleMember,
                Payment = payment
            };
            return transaction;
        }

        private int GenerateBankDocNo(PayMethod payMethod)
        {
            int docNo;
            switch (payMethod)
            {
                case PayMethod.Check:
                    CheckSerial++;
                    docNo = CheckSerial;
                    break;
                case PayMethod.BankTransfer:
                    TransferSerial++;
                    docNo = TransferSerial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(payMethod), payMethod, null);
            }
            return docNo;
        }

        private int GenerateTransNo(PayMethod payMethod)
        {
            _maxPaymentTransactionKey++;
            return _maxPaymentTransactionKey;
        }

        private int GetLoanPaymentsMaxKey()
        {
            return _unitOfWork.LoanPayments.Max(x => (int?) x.DocNo) ?? 0;
        }

        private int GetPaymentTrasactionsMaxKey()
        {
            return _unitOfWork.PaymentTransactions.Max(x => (int?) x.TransNo) ?? 0;
        }

        private List<FamilyMember> GetEligibleMembers()
        {
            //Only 'Alive and 'Independent' members should get payments
            return _unitOfWork.FamilyMembers.Where(x => x.Alive == 1 & x.Independent != 2).ToList();
        }

        #endregion
    }
}