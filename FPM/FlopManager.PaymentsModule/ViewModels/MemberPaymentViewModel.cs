using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.DTOs;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Commands;
using Prism.Logging;
using Prism.Regions;

namespace FlopManager.PaymentsModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MemberPaymentViewModel : EditableViewModelBase
    {
        #region "Fields"

        private readonly DbSet<PaymentTransaction> _detailRepository;
        private readonly DbSet<LoanPayment> _loanPaymentsRepository;
        private readonly DbSet<Loan> _loanRepository;
        private readonly DbSet<FamilyMember> _membersRepository;
        private readonly DbSet<Payment> _paymentRepository;
        private readonly FamilyContext _unitOfWork;
        private readonly ILogger _logger;
        private readonly ISettings _settings;
        private readonly IResouece _resource;
        private decimal _loansTotals;
        //
        private int _memberCode;
        private ObservableCollection<MemberLoan> _memberLoans;
        private string _memberName;
        private int _memberShares;
        private decimal _netPayment;
        private Payment _payment;
        private decimal _paymentAmount;
        private PaymentTransaction _paymentDetail;
        private ObservableCollection<PaymentSequence> _paymentSequences;
        private decimal _paymentTotal;
        private FamilyMember _selectedMember;
        private PaymentSequence _selectedSeqeunce;
        private PeriodYear _selectedYear;
        private ObservableCollection<PeriodYear> _years;
        private ViewState _viewState;
        private List<LoanPayment> LoansHistory { get; set; }
        //
        private bool ChangedFlage { get; set; }
        private bool _canEdit;
        //Commands
        private DelegateCommand<object> _searchMemberCommand;
        #endregion
        [ImportingConstructor]
        public MemberPaymentViewModel(FamilyContext dbContext, ILogger logger, ISettings settings, IResouece resouece)
        {
            _unitOfWork = dbContext;
            _logger = logger;
            _settings = settings;
            _resource = resouece;
            _paymentRepository = _unitOfWork.Payments;
            _detailRepository = _unitOfWork.PaymentTransactions;
            _loanRepository = _unitOfWork.Loans;
            _loanPaymentsRepository = _unitOfWork.LoanPayments;
            _membersRepository = _unitOfWork.FamilyMembers;
            LoansHistory = new List<LoanPayment>();
            Initialize();
        }

        #region "Events"

        private void MemberCodeKeyDown(object code)
        {
            Search(code);
        }


        public ICommand SearchMemberCommand
        {
            get { return _searchMemberCommand ?? (_searchMemberCommand = new DelegateCommand<object>(MemberCodeKeyDown)); }
        }



        protected override void Initialize()
        {
            MemberLoans = new ObservableCollection<MemberLoan>();
            MemberLoans.CollectionChanged += MemberLoansChanged;
            CanEdit = true;
            Errors = new Dictionary<string, List<string>>();
            Title = ViewModelsTitles.MEMBER_PAYMENT;
            CanClose = true;
        }

        #endregion



        #region "Properties"
        public int MemberCode
        {
            get { return _memberCode; }
            set { SetProperty(ref _memberCode, value); }
        }

        public string MemberName
        {
            get { return _memberName; }
            set
            {
                SetProperty(ref _memberName, value);
            }
        }

        public int MemberShares
        {
            get { return _memberShares; }
            set
            {
                SetProperty(ref _memberShares, value);
            }
        }

        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                SetProperty(ref _paymentAmount, value);
            }
        }

        public decimal PaymentTotal
        {
            get { return _paymentTotal; }
            set
            {
                SetProperty(ref _paymentTotal, value);
            }
        }

        public decimal LoansTotal
        {
            get { return _loansTotals; }
            set
            {
                SetProperty(ref _loansTotals, value);
            }
        }

        public decimal NetPayment
        {
            get { return _netPayment; }
            set
            {
                SetProperty(ref _netPayment, value);
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

        public ObservableCollection<MemberLoan> MemberLoans
        {
            get { return _memberLoans; }
            set
            {
                SetProperty(ref _memberLoans, value);
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
            PaymentSequences = new ObservableCollection<PaymentSequence>(LoadYearPayments(selectedYear));
        }
        public PaymentSequence SelectedSeqeunce
        {
            get { return _selectedSeqeunce; }
            set
            {
                SetProperty(ref _selectedSeqeunce, value);
                if(SelectedSeqeunce != null)
                {
                    SeqeunceSelectionChnaged(SelectedYear, SelectedSeqeunce);
                }
            }
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                SetProperty(ref _canEdit, value);
            }
        }


        #endregion

        #region "ICommonOperations"
        public void SetState(ModelState state)
        {

        }

        public void SetChangedFlag()
        {
            ChangedFlage = true;
        }

        public void ResetChangedFalg()
        {
            ChangedFlage = false;
        }

        public bool OkClose()
        {
            if (!ChangedFlage) return true;
            string msg = "";//Properties.Resources.PromptForSaveMsg;
            return Helper.UserConfirmed(msg);
        }



        public List<RuleViolation> RulesViolations
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }







        #endregion

        #region "Commands Methods"



        protected override void Save()
        {
            if (_viewState == ViewState.New) return;
            try
            {
                WriteValues();
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                Helper.HandleUiException(ex, _logger, RaiseNotification);
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

            int code;
            if (int.TryParse(criteria.ToString(), out code))
            {

                if (code < 1) return;

                try
                {
                    _selectedMember = _membersRepository.Single(mem => mem.Code == code);
                    MemberName = _selectedMember.FullName;
                    MemberShares = _selectedMember.Shares;
                    IQueryable<IGrouping<string, PeriodYear>> result = _detailRepository.Where
                        (
                            x => x.FamilyMember.Code == _selectedMember.Code
                        )
                        .Select(n => n.Payment.PeriodYear)
                        .GroupBy(g => g.Year);
                    if (Years == null) Years = new ObservableCollection<PeriodYear>();
                    //Years.Clear();
                    foreach (var items in result)
                    {
                        var k = items.First();
                        //foreach (PeriodYear key in items)
                        //{
                        Years.Add(k);
                        RaisePropertyChanged(nameof(Years));
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Helper.HandleUiException(ex, _logger, RaiseNotification);

                }
            }

        }

        protected override void AddNew()
        {
            throw new NotImplementedException();
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
            
        }

        protected override bool IsValid()
        {
            throw new NotImplementedException();
        }

        private void ClearView()
        {
            if (!OkClose()) return;
            SetState(ViewState.New);

        }

        #endregion


        #region "Helpers"

        private void SetState(ViewState state)
        {
            _viewState = state;
            switch (_viewState)
            {
                case ViewState.New:
                    MemberLoans.Clear();
                    MemberCode = 0;
                    MemberName = string.Empty;
                    _selectedMember = null;
                    Years = null;
                    PaymentSequences = null;
                    PaymentTotal = 0.0M;
                    MemberShares = 0;
                    PaymentAmount = 0.0M;
                    LoansTotal = 0.0M;
                    NetPayment = 0.0M;
                    CanEdit = true;
                    ResetChangedFalg();
                    break;
                case ViewState.Saved:
                    ResetChangedFalg();
                    break;
                case ViewState.Edited:
                    SetChangedFlag();
                    break;
                case ViewState.HasErrors:
                    break;
                case ViewState.Deleted:
                    ResetChangedFalg();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RaisePropertyChanged("");
        }
        private void MemberLoansChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                foreach (MemberLoan memLoan in e.NewItems)
                {
                    //Get notified for any changes.
                    memLoan.PropertyChanged += OnMemeberLoanChanged;
                    //_schedule.ScheduleDetails.Add(memLoan);
                }
            }
            if (e.OldItems != null && e.OldItems.Count != 0)
            {
                foreach (MemberLoan memLoan in e.OldItems)
                {
                    //Remove notification event handler.
                    memLoan.PropertyChanged -= OnMemeberLoanChanged;
                    //_schedule.ScheduleDetails.Remove(memLoan);
                }
            }

        }

        private void OnMemeberLoanChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdatePayments();
        }

        private void UpdatePayments()
        {
            LoansTotal = MemberLoans.Sum(x => x.LoanAmount);
            NetPayment = ((PaymentAmount * MemberShares) - LoansTotal);
            SetState(ViewState.Edited);

        }


        private IList<PaymentSequence> LoadYearPayments(PeriodYear selectedYear)
        {
            var yearPayments = new ObservableCollection<PaymentSequence>();
            try
            {
                var result =
                 _unitOfWork.Payments
                            .Where(x => x.PeriodYear.Year == selectedYear.Year)
                            .Select(x => x.PaymentSequence).ToList();
                if (!result.Any())
                {
                    string msg = _resource["SelectedYearHasNoPayments"];
                    RaiseNotification(msg);
                    return yearPayments;
                }
                yearPayments = new ObservableCollection<PaymentSequence>(result);
            }
            catch (Exception ex)
            {

                var exception = Helper.ProcessExceptionMessages(ex);
                _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                RaiseNotification(exception.UserMsg);
            }
            return yearPayments;
        }

        private void SeqeunceSelectionChnaged(PeriodYear paymentYear, PaymentSequence paymentSequence)
        {
            if (paymentSequence == null) return;

            _payment = _paymentRepository.Single
                (
                    pay => pay.PeriodYear.Year == paymentYear.Year
                           &&
                           pay.PaymentSequence.Id == paymentSequence.Id
                );

            _paymentDetail =
                _detailRepository.Single
                (
                    det => det.FamilyMember.Code == _selectedMember.Code
                        &&
                           det.Payment.PaymentNo == _payment.PaymentNo
                );

            LoansHistory =
                _loanPaymentsRepository.Where
                (
                    hist => hist.PaymentTransaction.TransNo == _paymentDetail.TransNo
                ).ToList();

            MemberLoans.Clear();

            foreach (LoanPayment loanPayment in LoansHistory)
            {
                var memLoan = new MemberLoan
                                       (
                                         loanPayment.DocNo, loanPayment.Loan.LoanNo, loanPayment.AmountPaid,
                                         loanPayment.Loan.Description, loanPayment.Loan.Remarks
                                       );
                MemberLoans.Add(memLoan);
            }

            MemberShares = _paymentDetail.ShareNumbers;
            PaymentAmount = _payment.Amount;
            PaymentTotal = (MemberShares * PaymentAmount);
            LoansTotal = MemberLoans.Sum(x => x.LoanAmount);
            NetPayment = _paymentDetail.NetPayments;
            if (_payment.Posted)
            {
                CanEdit = false;
            }
            SetState(ViewState.Saved);

        }

        private void WriteValues()
        {
            foreach (MemberLoan memberLoan in MemberLoans)
            {
                LoanPayment loanPayment = LoansHistory.Find(x => x.DocNo == memberLoan.DocNo);
                if (loanPayment.AmountPaid == memberLoan.LoanAmount) continue;

                Loan loan = _loanRepository.Single(lo => lo.LoanNo == loanPayment.Loan.LoanNo);


                loanPayment.AmountPaid = memberLoan.LoanAmount;

                SetLoanStatus(loan, loanPayment.AmountPaid);


                decimal newLoansTotal = MemberLoans.Sum(x => x.LoanAmount);
                _paymentDetail.NetPayments = (_payment.Amount * _paymentDetail.ShareNumbers) - newLoansTotal;
            }
        }

        private void SetLoanStatus(Loan loan, decimal amountPaid)
        {
            if (loan.Amount == amountPaid)
            {
                loan.Status = LoanStatus.Paid;
            }
            else if (loan.Amount > amountPaid)
            {
                loan.Status = LoanStatus.NotPaid;
            }
            else
            {
                throw new InvalidOperationException("Amount paid cannot be greater than the loan amount");
            }

        }

        #endregion
    }
}
