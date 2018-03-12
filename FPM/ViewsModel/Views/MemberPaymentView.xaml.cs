using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.DomainModel.Repositories;
using Fbm.ViewsModel.Helpers;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for MemberPaymentView.xaml
    /// </summary>
    public partial class MemberPaymentView : ICommonOperations
    {
        public MemberPaymentView()
        {
            InitializeComponent();
            _unitOfWork = new UnitOfWork();
            _paymentRepository = _unitOfWork.Payments;
            _detailRepository = _unitOfWork.PaymentTransactions;
            _loanRepository = _unitOfWork.Loans;
            _hisotryRepository = _unitOfWork.LoanPayments;
            _membersRepository = _unitOfWork.FamilyMembers;
            LoansHistory = new List<LoanPayment>();
            DataContext = this;
            Loaded += WindowLoaded;
            txtMemberCode.KeyDown += MemberCodeKeyDown;
            Initialize();
        }

        #region "Events"

        private void MemberCodeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SearchCommand.Execute(null);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txtMemberCode.Focus();
        }

        #endregion

        #region "Fields"

        private readonly RepositoryBase<PaymentTransaction> _detailRepository;
        private readonly RepositoryBase<LoanPayment> _hisotryRepository;
        private readonly RepositoryBase<Loan> _loanRepository;
        private readonly RepositoryBase<FamilyMember> _membersRepository;
        private readonly RepositoryBase<Payment> _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
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
        private RelayCommand _saveCommand;
        private RelayCommand _searchCommand;
        private FamilyMember _selectedMember;
        private PaymentSequence _selectedSeqeunce;
        private PeriodSetting _selectedYear;
        private ObservableCollection<PeriodSetting> _years;
        private ViewState _viewState;
        private RelayCommand _addNewCommand;
        private List<LoanPayment> LoansHistory { get; set; }
        //
        private bool ChangedFlage { get; set; }
        private bool _canEdit;

        #endregion
        
        #region "Properties"
        public int MemberCode
        {
            get { return _memberCode; }
            set
            {
                _memberCode = value;
                RaisePropertyChanged();
            }
        }

        public string MemberName
        {
            get { return _memberName; }
            set
            {
                _memberName = value;
                RaisePropertyChanged();
            }
        }

        public int MemberShares
        {
            get { return _memberShares; }
            set
            {
                _memberShares = value;
                RaisePropertyChanged();
            }
        }

        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                _paymentAmount = value;
                RaisePropertyChanged();
            }
        }

        public decimal PaymentTotal
        {
            get { return _paymentTotal; }
            set
            {
                _paymentTotal = value;
                RaisePropertyChanged();
            }
        }

        public decimal LoansTotal
        {
            get { return _loansTotals; }
            set
            {
                _loansTotals = value;
                RaisePropertyChanged();
            }
        }

        public decimal NetPayment
        {
            get { return _netPayment; }
            set
            {
                _netPayment = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PeriodSetting> Years
        {
            get { return _years; }
            set
            {
                _years = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PaymentSequence> PaymentSequences
        {
            get { return _paymentSequences; }
            set
            {
                _paymentSequences = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MemberLoan> MemberLoans
        {
            get { return _memberLoans; }
            set
            {
                _memberLoans = value;
                RaisePropertyChanged();
            }
        }

       

        public PeriodSetting SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                RaisePropertyChanged();
            }
        }

        public PaymentSequence SelectedSeqeunce
        {
            get { return _selectedSeqeunce; }
            set
            {
                _selectedSeqeunce = value;
                RaisePropertyChanged();
            }
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set 
            {
                _canEdit = value;
                RaisePropertyChanged();
            }
        }

        private PaymentStatus PaymentInReviewStatus
        {
            get { 
                PaymentStatus ps = null;

                try
                {
                    ps = _unitOfWork.PaymentStatuses.GetById(2);
                }
                catch (Exception ex)
                {
                    
                    Helper.LogOnly(ex);
                }
                return ps;
            }

        }

        #endregion

        #region "ICommonOperations"

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(Save, CanSave);
                }
                return _saveCommand;
            }
        }

        public ICommand AddNewCommand
        {
            get
            {
                if (_addNewCommand == null)
                {
                    _addNewCommand = new RelayCommand(ClearView);
                }
                return _addNewCommand;
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(Search);
                }
                return _searchCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand PrintCommand
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand EditCommand
        {
            get { throw new NotImplementedException(); }
        }

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
            string msg = Properties.Resources.PromptForSaveMsg;
            return Helper.UserConfirmed(msg);
        }

        

        public List<RuleViolation> RulesViolations
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Initialize()
        {
            MemberLoans = new ObservableCollection<MemberLoan>();
            MemberLoans.CollectionChanged +=MemberLoansChanged;
            CanEdit = true;
        }

        

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region "Commands Methods"

        private void Search()
        {
            string s = txtMemberCode.Text;
            if (string.IsNullOrEmpty(s)) return;
            int code;
            if (int.TryParse(s, out code))
            {
                try
                {
                    _selectedMember = _membersRepository.Query(mem => mem.Code == code).Single();
                    MemberName = _selectedMember.FullName;
                    IQueryable<IGrouping<int, PeriodSetting>> result = _detailRepository.Query
                        (
                            x => x.FamilyMember.Code == _selectedMember.Code
                        )
                        .Select(n => n.Payment.PeriodSetting)
                        .GroupBy(g => g.Id);
                    if (Years == null) Years = new ObservableCollection<PeriodSetting>();
                    //Years.Clear();
                    foreach (var items in result)
                    {
                        var k = items.First();
                        //foreach (PeriodSetting key in items)
                        //{
                            Years.Add(k);
                            RaisePropertyChanged("Years");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Helper.LogAndShow(ex);
                }
            }
        }

        private void Save()
        {
            if (_viewState == ViewState.New) return;
            try
            {
                WriteValues();
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                
                Helper.LogAndShow(ex);
            }
            
        }
        bool CanSave()
        {
            return CanEdit;
        }

        private  void ClearView()
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


        private void YearSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedYear != null)
            {
                LoadYearPayments(SelectedYear);
            }
        }

        private void LoadYearPayments(PeriodSetting selectedYear)
        {
            IQueryable<PaymentSequence> result =
                _unitOfWork.Payments.Query(x => x.PeriodSetting.Id == selectedYear.Id).Select(x => x.PaymentSequence);
            if (!result.Any())
            {
                string msg = Properties.Resources.MemberPaymentView_NoPaymentSequ;
                Helper.ShowMessage(msg);
                return;
            }
            PaymentSequences = new ObservableCollection<PaymentSequence>(result);
        }

        private void SeqeunceSelectionChnaged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedSeqeunce == null) return;

            _payment = _paymentRepository.Query
                (
                    pay => pay.PeriodSetting.Id == SelectedYear.Id
                           &&
                           pay.PaymentSequence.Id == SelectedSeqeunce.Id
                ).Single();

            _paymentDetail =
                _detailRepository.Query
                (
                    det => det.FamilyMember.Code == _selectedMember.Code
                        &&
                           det.Payment.PaymentNo == _payment.PaymentNo
                ).Single();

            LoansHistory =
                _hisotryRepository.Query
                (
                    hist => hist.PaymentTransaction.TransNo == _paymentDetail.TransNo
                ).ToList();

            MemberLoans.Clear();

            foreach (LoanPayment lohis in LoansHistory)
            {
                var memLoan = new MemberLoan
                                       (
                                         lohis.Id, lohis.Loan.LoanNo, lohis.AmountPaid,
                                         lohis.Loan.Description, lohis.Loan.Remarks
                                       );
                MemberLoans.Add(memLoan);
            }

            MemberShares = _paymentDetail.ShareNumbers;
            PaymentAmount = _payment.Amount;
            PaymentTotal = (MemberShares * PaymentAmount);
            LoansTotal = MemberLoans.Sum(x => x.LoanAmount);
            NetPayment = _paymentDetail.NetPayments;
            if(_payment.PaymentStatus.Id != PaymentInReviewStatus.Id)
            {
                CanEdit = false;
            }
            SetState(ViewState.Saved);

        }

        private void WriteValues()
        {
            foreach (MemberLoan memberLoan in MemberLoans)
            {
                LoanPayment hist = LoansHistory.Find(x => x.Id == memberLoan.Id);
                if (hist.AmountPaid == memberLoan.LoanAmount) continue;
                Loan loan = _loanRepository.Query(lo => lo.LoanNo == hist.Loan.LoanNo).Single();
                loan.Paid = (loan.Paid - hist.AmountPaid);
                loan.Paid = (loan.Paid + memberLoan.LoanAmount);
                loan.Balance = loan.Amount - loan.Paid;
                SetLoanStatus(loan);
                hist.AmountPaid = memberLoan.LoanAmount;
                decimal newLoansTotal = MemberLoans.Sum(x => x.LoanAmount);
                _paymentDetail.AmountDue = newLoansTotal;
                _paymentDetail.NetPayments = (_payment.Amount * _paymentDetail.ShareNumbers) - newLoansTotal;
            }
        }

        private void SetLoanStatus(Loan loan)
        {
            RepositoryBase<LoanStatus> statusRepos = _unitOfWork.LoanStatuses;
            if (loan.Balance == 0.0M)
            {
                loan.LoanStatus = statusRepos.GetById(3);
            }
            else if (loan.Paid == 0.0M)
            {
                loan.LoanStatus = statusRepos.GetById(1);
            }
            else
            {
                loan.LoanStatus = statusRepos.GetById(2);
            }
        }

        #endregion

    }
}