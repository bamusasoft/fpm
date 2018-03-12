using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.DomainModel.Repositories;
using Fbm.ViewsModel.Helpers;
using Fbm.ViewsModel.Properties;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for PaymentDetailsView.xaml
    /// </summary>
    public partial class PaymentDetailsView : Window, ICommonOperations, IModelOperations<PaymentTransaction>
    {
        public PaymentDetailsView()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.PaymentTransactions;
            _historyRepository = _unitOfWork.LoanPayments;
            _uiLoansHistory = new List<LoanPayment>();
            txtPaymentNo.PreviewKeyDown += PaymentNoKeyDown;
            _settings = Settings.Default;
            DataContext = this;
            PaymentDetails = new ObservableCollection<PaymentDetailsReport>();
            _seacrhablePaymentDetails = new HashSet<SearchablePaymentDetails>();

            //Enable the new feature of WPF 4.5 CollectionSyncronization instead of the old
            //way that required posting through dispatch.
            BindingOperations.EnableCollectionSynchronization(PaymentDetails, PaymentDetailsLock);
        }

        #region "Events"

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            progCounter.Visibility = Visibility.Hidden;
            txtProgCurrent.Visibility = Visibility.Hidden;
        }

        private void PaymentNoKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SearchCommand.Execute(null);
            }
        }

        #endregion

        #region "Fields"

        private static readonly object PaymentDetailsLock = new object();
        private readonly RepositoryBase<LoanPayment> _historyRepository;
        private readonly HashSet<SearchablePaymentDetails> _seacrhablePaymentDetails;
        private readonly Settings _settings;
        private readonly List<LoanPayment> _uiLoansHistory;
        private readonly IUnitOfWork _unitOfWork;
        private RelayCommand _addNewCommand;
        private PaymentDetailsFilter _criteria;

        private Payment _currentPayment;
        private RelayCommand _deleteCommand;
        private ObservableCollection<PaymentDetailsReport> _paymentDetails;
        private string _paymentNo;
        private string _paymentSequence;
        private string _paymentYear;
        private RelayCommand _printCommand;
        private string _progCurrent;
        private double _progressCounter;
        private RepositoryBase<PaymentTransaction> _repository;
        //
        private RelayCommand _saveCommand;
        private RelayCommand _searchCommand;
        private bool ChangedFlage { get; set; }

        #endregion.

        #region "Properties"

        public PaymentDetailsFilter Criteria
        {
            get
            {
                if (_criteria == null)
                {
                    _criteria = new PaymentDetailsFilter();
                }
                return _criteria;
            }
        }

        public string PaymentNo
        {
            get { return _paymentNo; }
            set
            {
                _paymentNo = value;
                RaisePropertyChanged();
            }
        }

        public string PaymentYear
        {
            get { return _paymentYear; }
            set
            {
                _paymentYear = value;
                RaisePropertyChanged();
            }
        }

        public string PaymentSequence
        {
            get { return _paymentSequence; }
            set
            {
                _paymentSequence = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PaymentDetailsReport> PaymentDetails
        {
            get { return _paymentDetails; }
            set
            {
                _paymentDetails = value;
                RaisePropertyChanged();
                SetChangedFlag();
            }
        }

        public double ProgressCounter
        {
            get { return _progressCounter; }
            set
            {
                _progressCounter = value;
                RaisePropertyChanged();
            }
        }

        public string ProgCurrent
        {
            get { return _progCurrent; }
            set
            {
                _progCurrent = value;
                RaisePropertyChanged();
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
                    _addNewCommand = new RelayCommand(AddNew);
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
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(Delete);
                }
                return _deleteCommand;
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand(Print);
                }
                return _printCommand;
            }
        }

        public ICommand EditCommand
        {
            get { throw new NotImplementedException(); }
        }


        public void SetState(ModelState state)
        {
            throw new NotImplementedException();
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
            return !ChangedFlage;
        }

        public List<RuleViolation> RulesViolations { get; set; }

        public void Initialize()
        {
            _currentPayment = null;
            if (PaymentDetails != null)
            {
                PaymentDetails.Clear();
            }
            if (_seacrhablePaymentDetails != null)
            {
                _seacrhablePaymentDetails.Clear();
            }
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

        #region "IModelOperations"

        public void ReadModelValues(PaymentTransaction model)
        {
            throw new NotImplementedException();
        }

        public void WriteModelValues(PaymentTransaction model)
        {
            throw new NotImplementedException();
        }

        public bool ValidData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Commands Methods"

        private void Save()
        {
            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
            }
        }

        private bool CanSave()
        {
            return (_currentPayment != null &&
                    _currentPayment.PaymentStatus.Id != 3
                   );
        }

        private void AddNew()
        {
            Func<SearchablePaymentDetails, bool> s = Criteria.BuildCriteria();
            if (s != null)
            {
                IEnumerable<SearchablePaymentDetails> result = _seacrhablePaymentDetails.Where(s);
                PaymentDetails = ShowFilterResult(result);
            }
        }


        private async void Search()
        {
            string s = txtPaymentNo.Text;
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            try
            {
                Payment result = _unitOfWork.Payments.Query(x => x.PaymentNo == s).Single();
                PaymentNo = result.PaymentNo;
                PaymentYear = result.PeriodSetting.YearPart;
                PaymentSequence = result.PaymentSequence.SequenceDescription;
                if (result.PaymentStatus.Id == 1) //Initiated
                {
                    progCounter.Visibility = Visibility.Visible;
                    txtProgCurrent.Visibility = Visibility.Visible;
                    Task tsk = CreatePaymentDetails(result);
                    await tsk;
                    progCounter.Visibility = Visibility.Hidden;
                    txtProgCurrent.Visibility = Visibility.Hidden;
                }
                else
                {
                    ShowPaymentDetails(result);

                }
                _currentPayment = result;
            }
            catch (InvalidOperationException ex)
            {
                Helper.LogOnly(ex);
                string msg = Properties.Resources.InvalidPaymentNo;
                Helper.ShowMessage(msg);
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
            }
        }

        private void Print()
        {
            string templatePath = _settings.PaymentDetailsTemplatePath;
            string pdfPath = _settings.PdfsFolder;
            try
            {
                List<PaymentDetailsReport> report = PaymentDetails.ToList();
                DataTable table = PaymentDetailsReport.FillData(report);
                var mail = new ExcelMail();

                mail.SendTable(table, templatePath, pdfPath);
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
            }
        }

        #endregion

        #region "Helper Methods"

        private async Task CreatePaymentDetails(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException("payment");
            await Task.Run(() =>
                               {
                                   bool isFirstLine = true;
                                   RepositoryBase<FamilyMember> familyMemberRepos = _unitOfWork.FamilyMembers;
                                   RepositoryBase<PaymentSequence> sequRepos = _unitOfWork.PaymentSequences;
                                   RepositoryBase<PeriodSetting> periodRepos = _unitOfWork.PeriodSettings;
                                   RepositoryBase<PaymentStatus> payStatusRepos = _unitOfWork.PaymentStatuses;
                                   RepositoryBase<PaymentStatus> paymRepos = _unitOfWork.PaymentStatuses;
                                   PaymentSequence paySequ = payment.PaymentSequence;
                                   PeriodSetting currentYear = payment.PeriodSetting;
                                   RepositoryBase<PaymentTransaction> detailsRepos = _unitOfWork.PaymentTransactions;

                                   int counter = 0;
                                   //Start:
                                   _uiLoansHistory.Clear();
                                   List<FamilyMember> eligibleMembers = GetEligibleMembers();
                                   //List<LoansHistory> tempLoansHistory = new List<LoansHistory>();

                                   foreach (FamilyMember familyMember in eligibleMembers)
                                   {
                                       isFirstLine = true;
                                       decimal memberLoansPayment = 0.0M;
                                       PaymentTransaction detail = CreateMemberDetail(payment, familyMember);

                                       #region "Old"

                                       //detail.DetailNo = Guid.NewGuid();
                                       //detail.PaymentNo = payment.PaymentNo;
                                       //detail.FamilyMember = familyMember;
                                       //detail.ShareNumbers = familyMember.Shares;

                                       #endregion

                                       List<Loan> memberLoans = GetMemberLoans(payment, familyMember);
                                       foreach (Loan loan in memberLoans)
                                       {
                                           #region "Old"

                                           //var order = GetLoanOrder(payment, loan);
                                           //if (order == null) throw new InvalidOperationException("You must register payment orders for all types of loans");
                                           //paymentAmountDue = PayMemberLoans(detail, tempLoansHistory, familyMember, paymentAmountDue, loan, order);

                                           #endregion

                                           memberLoansPayment += PayLoan(loan, detail);
                                       }
                                       detail.AmountDue = memberLoansPayment;
                                       detail.NetPayments = detail.NetPayments - detail.AmountDue;
                                           //((payment.Amount * detail.ShareNumbers) - detail.AmountDue);
                                       detailsRepos.Add(detail);
                                       var progMeta = new Tuple<double, double, string>(counter, eligibleMembers.Count,
                                                                                        familyMember.FullName);
                                       ShowData(detail, isFirstLine, progMeta);

                                       #region "Old algorithm "

                                       //if (holderLoans.Count == 0)
                                       //{
                                       //    //******* Warning: Get an official confirmation for this policy.********//

                                       //    // If no loans found; either this ShareHolder has no loans completely,
                                       //    //or has no loans for just this payment.
                                       //    //and in the latter, we will retrive all INPAYING loans.

                                       //    holderLoans = loansRepos.Query
                                       //        (
                                       //          x => x.ShareHolder.Code == sharHolder.Code &&
                                       //          x.StatusId == 2
                                       //        ).ToList();
                                       //}

                                       //decimal paymentAmountDue = 0.0M;
                                       //if (holderLoans.Count() > 0) //If no loans, terminate.
                                       //{
                                       //    //Get the payment orders of this payment.
                                       //    var paymentOrders = ordersRepos.Query(x => x.Payment.PaymentNo == payment.PaymentNo);

                                       //    foreach (var loan in holderLoans)
                                       //    {
                                       //        decimal tempAmountDue = 0.0M;
                                       //        //Get the admin order regrad this specific loan.
                                       //        var order = paymentOrders.Where(x => x.LoanType.Code == loan.LoanType.Code).SingleOrDefault();

                                       //        if (order != null)
                                       //        {
                                       //            tempAmountDue = (loan.Amount * order.EarnPercent) / 100;
                                       //            paymentAmountDue += (loan.Amount * order.EarnPercent) / 100;
                                       //        }
                                       //        else
                                       //        {
                                       //            tempAmountDue = loan.Amount;
                                       //            paymentAmountDue += loan.Amount;
                                       //        }
                                       //        loan.Paid = tempAmountDue;
                                       //        loan.Balance = loan.Amount - loan.Paid;

                                       //    }
                                       //    //amountDue = holderLoans.Sum(x => x.Amount);
                                       //}

                                       #endregion

                                       counter++;
                                   }
                                   PaymentStatus status = payStatusRepos.GetById(2); //Under Review Status.
                                   payment.PaymentStatus = status;
                               }
                );
        }

        private decimal PayLoan(Loan loan, PaymentTransaction detail)
        {
            RepositoryBase<LoanStatus> statusRepos = _unitOfWork.LoanStatuses;
            PaymentInstruction order = GetLoanOrder(detail.Payment, loan);
            if (order == null)
                throw new InvalidOperationException("You must register payment orders for all types of loans");
            decimal amountToPaid = (loan.Amount*order.EarnPercent)/100;
            //In case if the amountToPaid is greater than the actual balance
            //adjust the amountToPaid to be just the acutal balance.
            //This behvior is accour due to loans that is INPAYING status
            //and the payment order earn percent will exceed the remaining balance.
            if (amountToPaid > loan.Balance) amountToPaid = loan.Balance;
            var history = new LoanPayment
                              {
                                  Id = Guid.NewGuid(),
                                  Loan = loan,
                                  FamilyMember = detail.FamilyMember,
                                  PaymentTransaction = detail,
                                  AmountPaid = amountToPaid
                              };
            loan.Paid = (loan.Paid + amountToPaid);
            loan.Balance = loan.Amount - loan.Paid;
            if (loan.Balance == 0.0M)
            {
                loan.LoanStatus = statusRepos.GetById(3);
            }
            else
            {
                loan.LoanStatus = statusRepos.GetById(2);
            }
            _historyRepository.Add(history);
            _uiLoansHistory.Add(history);
            return amountToPaid;
        }

        private List<FamilyMember> GetEligibleMembers()
        {
            //Only 'Alive and 'Independent' members should get payments
            return _unitOfWork.FamilyMembers.Query
                (
                    x => x.Alive == 1 & x.Independent != 2
                ).ToList();
        }

        private PaymentTransaction CreateMemberDetail(Payment payment, FamilyMember member)
        {
            return new PaymentTransaction
                       {
                           TransNo = Guid.NewGuid(),
                           Payment = payment,
                           FamilyMember = member,
                           ShareNumbers = member.Shares,
                           AmountDue = 0.0M,
                           NetPayments = (payment.Amount*member.Shares)
                       };
        }

        private PaymentInstruction GetLoanOrder(Payment payment, Loan loan)
        {
            PaymentInstruction order = null;
            RepositoryBase<PaymentInstruction> ordersRepos = _unitOfWork.PaymentInstructions;
            //First check that this loans is belong to the current payment period and sequence.
            bool currentPaymentLoan = loan.PeriodSetting.Id == payment.PeriodSetting.Id
                                      &&
                                      loan.PaymentSequence.Id == payment.PaymentSequence.Id;

            //If so
            if (!currentPaymentLoan)
            {
                //Get the order of this loan type which is not belong to this payment sequnce
                order = ordersRepos.Query(
                    x => x.Payment.PaymentNo == payment.PaymentNo
                         &&
                         x.LoanType.Code == loan.LoanType.Code
                         &&
                         x.PeriodSetting.Id == loan.PeriodSetting.Id
                         &&
                         x.OldLoan
                    ).Single();
            }
            else
            {
                order = ordersRepos.Query(
                    x => x.Payment.PaymentNo == payment.PaymentNo
                         &&
                         x.LoanType.Code == loan.LoanType.Code
                         &&
                         x.PeriodSetting.Id == loan.PeriodSetting.Id
                         &&
                         x.OldLoan == false
                    ).Single();
            }

            return order;
        }

        private List<Loan> GetMemberLoans(Payment payment, FamilyMember familyMember)
        {
            //Pay loans of this member.
            //Get loans of this member that must be paid in this payment.
            RepositoryBase<Loan> loansRepos = _unitOfWork.Loans;
            List<Loan> holderLoans =
                loansRepos.Query(
                    x => x.FamilyMember.Code == familyMember.Code
                         &&
                         x.PeriodSetting.Id == payment.PeriodSetting.Id
                         &&
                         x.PaymentSequence.Id == payment.PaymentSequence.Id
                         ||
                         x.FamilyMember.Code == familyMember.Code
                         &&
                         x.LoanStatus.Id == 2
                    ).OrderBy(lo => lo.LoanNo).ToList();
            return holderLoans;
        }

        private void ShowData(PaymentTransaction detail, bool isFirstLine, Tuple<double, double, string> progMetadata)
        {
            double p = (progMetadata.Item1/progMetadata.Item2)*100;
            string currMember = progMetadata.Item3;
            //Action action = () =>
            //    {
            if (_uiLoansHistory.Count > 0)
            {
                foreach (LoanPayment history in _uiLoansHistory)
                {
                    if (isFirstLine)
                    {
                        //This for show.
                        var pdrHeader = new PaymentDetailsReport(
                            detail.TransNo, detail.FamilyMember.Code, detail.FamilyMember.FullName,
                            detail.Payment.PeriodSetting.YearPart, detail.Payment.PaymentSequence.SequenceDescription,
                            detail.Payment.Amount, detail.FamilyMember.Shares,
                            (detail.Payment.Amount*detail.FamilyMember.Shares),
                            history.AmountPaid, history.Loan.LoanType.LoanDescription,
                            history.Loan.Remarks, detail.AmountDue, detail.NetPayments, detail.FamilyMember.Sex);

                        PaymentDetails.Add(pdrHeader);
                        //This for search.
                        var spdHeader = new SearchablePaymentDetails
                            (
                            detail.TransNo, detail.FamilyMember.Code, detail.FamilyMember.FullName,
                            detail.Payment.PeriodSetting.YearPart, detail.Payment.PaymentSequence.SequenceDescription,
                            detail.Payment.Amount, detail.FamilyMember.Shares,
                            (detail.Payment.Amount*detail.FamilyMember.Shares),
                            history.AmountPaid, history.Loan.LoanType.LoanDescription,
                            history.Loan.Remarks, detail.AmountDue, detail.NetPayments, detail.FamilyMember.Sex, true
                            );
                        _seacrhablePaymentDetails.Add(spdHeader);
                        isFirstLine = false;
                        continue;
                    }
                    //This for show.
                    var pdrDetail = new PaymentDetailsReport
                        (
                        detail.TransNo, history.AmountPaid,
                        history.Loan.LoanType.LoanDescription, history.Loan.Remarks, detail.FamilyMember.Sex
                        );
                    PaymentDetails.Add(pdrDetail);
                    //This for search.
                    var spdDetail = new SearchablePaymentDetails
                        (
                        detail.TransNo, detail.FamilyMember.Code, detail.FamilyMember.FullName,
                        detail.Payment.PeriodSetting.YearPart, detail.Payment.PaymentSequence.SequenceDescription,
                        detail.Payment.Amount, detail.FamilyMember.Shares,
                        (detail.Payment.Amount*detail.FamilyMember.Shares),
                        history.AmountPaid, history.Loan.LoanType.LoanDescription,
                        history.Loan.Remarks, detail.AmountDue, detail.NetPayments, detail.FamilyMember.Sex, false
                        );
                    _seacrhablePaymentDetails.Add(spdDetail);
                }
                _uiLoansHistory.Clear();
            }
            else
            {
                var pdr = new PaymentDetailsReport
                    (
                    detail.TransNo, detail.FamilyMember.Code, detail.FamilyMember.FullName,
                    detail.Payment.PeriodSetting.YearPart, detail.Payment.PaymentSequence.SequenceDescription,
                    detail.Payment.Amount, detail.FamilyMember.Shares,
                    (detail.Payment.Amount*detail.FamilyMember.Shares), 0.00M, null,
                    null, detail.AmountDue, detail.NetPayments, detail.FamilyMember.Sex
                    );
                PaymentDetails.Add(pdr);
                var spd = new SearchablePaymentDetails
                    (
                    detail.TransNo, detail.FamilyMember.Code, detail.FamilyMember.FullName,
                    detail.Payment.PeriodSetting.YearPart, detail.Payment.PaymentSequence.SequenceDescription,
                    detail.Payment.Amount, detail.FamilyMember.Shares,
                    (detail.Payment.Amount*detail.FamilyMember.Shares), 0.00M, null,
                    null, detail.AmountDue, detail.NetPayments, detail.FamilyMember.Sex, true
                    );
                _seacrhablePaymentDetails.Add(spd);
            }
            ProgressCounter = p;
            ProgCurrent = currMember;

            //};
            //Dispatcher.Invoke(action, null);
        }

        //List<PaymentDetailsReport> reportList = new List<PaymentDetailsReport>();

        private void ShowPaymentDetails(Payment p)
        {
            var holdersRepos = _unitOfWork.FamilyMembers;
            var periodsRepos = _unitOfWork.PeriodSettings;
            var sequenceRepos = _unitOfWork.PaymentSequences;
            var paymentsRepos = _unitOfWork.Payments;
            var detailsRepos = _unitOfWork.PaymentTransactions;
            var historyRepos = _unitOfWork.LoanPayments;
            var loansRepos = _unitOfWork.Loans;

            IList<FamilyMember> holders = holdersRepos.GetAll();
            IList<PeriodSetting> perods = periodsRepos.GetAll();
            IList<PaymentSequence> sequences = sequenceRepos.GetAll();
            IList<Payment> payments = paymentsRepos.GetAll();
            IList<PaymentTransaction> details = detailsRepos.GetAll();
            //
            var results =
                from payment in payments
                join detail in details
                    on payment.PaymentNo equals detail.PaymentNo
                join holder in holders
                    on detail.MemberCode equals holder.Code
                join period in perods
                    on payment.PayYear equals period.Id
                join sequ in sequences
                    on payment.PaySequence equals sequ.Id
                where p.PaymentNo == payment.PaymentNo
                //new Added
                select new
                           {
                               detail.TransNo,
                               ShareHolderCode = holder.Code,
                               holder.FullName,
                               PeriodSettingsId = period.Id,
                               Year = period.YearPart,
                               SequenId = sequ.Id,
                               SequenDesc = sequ.SequenceDescription,
                               SharesNo = detail.ShareNumbers,
                               PaymentAmount = payment.Amount,
                               TotalPayments = (payment.Amount*detail.ShareNumbers),
                               LoansTatal = detail.AmountDue,
                               detail.NetPayments,
                               MemberSex = holder.Sex
                           };

            foreach (var result in results)
            {
                //var loans = loansRepos.Query(
                //     lo => lo.ShareHolder.Code == result.ShareHolderCode 
                //         &&
                //         lo.PaymentSequence.Id == result.SequenId
                //         && 
                //         lo.PeriodSetting.Id == result.PeriodSettingsId
                //    ).ToList();

                var resultCopy = result; //Copy foreach variable to avoid accessing it in clousre
                List<LoanPayment> loansPaymentHistory = historyRepos.Query(
                    hist => hist.FamilyMember.Code == resultCopy.ShareHolderCode
                            &&
                            hist.PaymentTransaction.Payment.PaymentSequence.Id == resultCopy.SequenId
                            &&
                            hist.PaymentTransaction.Payment.PeriodSetting.Id == resultCopy.PeriodSettingsId).ToList();

                if (loansPaymentHistory.Count != 0)
                {
                    bool isFirstLine = true;
                    foreach (LoanPayment history in loansPaymentHistory)
                    {
                        if (isFirstLine)
                        {
                            //For Show
                            var pdrHeader = new PaymentDetailsReport
                                (
                                result.TransNo, result.ShareHolderCode, result.FullName, result.Year,
                                result.SequenDesc, result.PaymentAmount, result.SharesNo,
                                result.TotalPayments, history.AmountPaid, history.Loan.LoanType.LoanDescription,
                                history.Loan.Remarks, result.LoansTatal, result.NetPayments, result.MemberSex
                                );
                            PaymentDetails.Add(pdrHeader);
                            var spdHeader = new SearchablePaymentDetails
                                (
                                result.TransNo, result.ShareHolderCode, result.FullName, result.Year,
                                result.SequenDesc, result.PaymentAmount, result.SharesNo,
                                result.TotalPayments, history.AmountPaid, history.Loan.LoanType.LoanDescription,
                                history.Loan.Remarks, result.LoansTatal, result.NetPayments, result.MemberSex, true
                                );
                            _seacrhablePaymentDetails.Add(spdHeader);
                            isFirstLine = false;
                            continue;
                        }
                        var pdrDetail = new PaymentDetailsReport
                            (
                            result.TransNo, history.AmountPaid,
                            history.Loan.LoanType.LoanDescription, history.Loan.Remarks, result.MemberSex
                            );
                        PaymentDetails.Add(pdrDetail);

                        var spdDetail = new SearchablePaymentDetails
                            (
                            result.TransNo, result.ShareHolderCode, result.FullName, result.Year,
                            result.SequenDesc, result.PaymentAmount, result.SharesNo,
                            result.TotalPayments, history.AmountPaid, history.Loan.LoanType.LoanDescription,
                            history.Loan.Remarks, result.LoansTatal, result.NetPayments, result.MemberSex, false
                            );
                        _seacrhablePaymentDetails.Add(spdDetail);
                    }
                }
                else
                {
                    var pdr = new PaymentDetailsReport(
                        result.TransNo,
                        result.ShareHolderCode, result.FullName, result.Year,
                        result.SequenDesc, result.PaymentAmount, result.SharesNo,
                        result.TotalPayments, 0.00M, null,
                        null, result.LoansTatal, result.NetPayments, result.MemberSex
                        );
                    PaymentDetails.Add(pdr);
                    var spd = new SearchablePaymentDetails
                        (
                        result.TransNo,
                        result.ShareHolderCode, result.FullName, result.Year,
                        result.SequenDesc, result.PaymentAmount, result.SharesNo,
                        result.TotalPayments, 0.00M, null,
                        null, result.LoansTatal, result.NetPayments, result.MemberSex, true
                        );
                    _seacrhablePaymentDetails.Add(spd);
                }
            }
        }

        private void Delete()
        {
            RepositoryBase<PaymentTransaction> detailsRepos = _unitOfWork.PaymentTransactions;
            RepositoryBase<LoanPayment> historyRepos = _unitOfWork.LoanPayments;
            RepositoryBase<Loan> loansRepos = _unitOfWork.Loans;
            RepositoryBase<LoanStatus> statusRepos = _unitOfWork.LoanStatuses;
            foreach (PaymentDetailsReport detail in PaymentDetails)
            {
                IQueryable<PaymentTransaction> dbDetails = detailsRepos.Query(x => x.TransNo == detail.DetailNo);
                IQueryable<LoanPayment> dbHisotry = historyRepos.Query(x => x.PaymentTransaction.TransNo == detail.DetailNo);
                foreach (PaymentTransaction det in dbDetails)
                {
                    detailsRepos.Delete(det);
                }
                foreach (LoanPayment hist in dbHisotry)
                {
                    Loan loan = loansRepos.Query(x => x.LoanNo == hist.LoanNo).SingleOrDefault();
                    if (loan != null)
                    {
                        loan.Paid = loan.Paid - hist.AmountPaid;
                        loan.Balance = loan.Balance + hist.AmountPaid;
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
                    historyRepos.Delete(hist);
                }
            }
            //If you deleted the payment details that means it's no longer in InReview State. so Revert to Initiated State.
            PaymentStatus status = _unitOfWork.PaymentStatuses.GetById(1);

            _currentPayment.PaymentStatus = status;
            //_unitOfWork.Save();
        }

        private ObservableCollection<PaymentDetailsReport> ShowFilterResult(IEnumerable<SearchablePaymentDetails> result)
        {
            var ocpdr = new ObservableCollection<PaymentDetailsReport>();
            foreach (SearchablePaymentDetails searchResult in result)
            {
                if (searchResult.IsHeader)
                {
                    var pdr = new PaymentDetailsReport
                        (
                        searchResult.DetailNo, searchResult.MemberCode, searchResult.MemberName,
                        searchResult.PaymentYear, searchResult.PaymentSequence,
                        searchResult.PaymentAmount, searchResult.ShareNo, searchResult.TotalPayment,
                        searchResult.LoanAmount, searchResult.LoanDescription, searchResult.LoanRemarks,
                        searchResult.LoansTotal, searchResult.NetPayments, searchResult.Sex
                        );
                    ocpdr.Add(pdr);
                }
                else
                {
                    var pdr = new PaymentDetailsReport
                        (
                        searchResult.DetailNo, searchResult.LoanAmount,
                        searchResult.LoanDescription, searchResult.LoanRemarks, searchResult.Sex
                        );
                    ocpdr.Add(pdr);
                }
            }
            return ocpdr;
        }

        #region "Old"

        //private decimal PayMemberLoans(PaymentDetail detail, List<LoansHistory> tempLoansHistory, FamilyMember familyMember, decimal paymentAmountDue, Loan loan, PaymentOrder order)
        //{
        //    var statusRepos = _unitOfWork.LoanStatuses;
        //    var historyRepost = _unitOfWork.LoanPayments;
        //    var amountToPaid = (loan.Amount * order.EarnPercent) / 100;
        //    //In case if the amountToPaid is greater than the actual balance
        //    //adjust the amountToPaid to be just the acutal balance.
        //    //This behvior is accour due to loans that is INPAYING status
        //    //and the payment order earn percent will exceed the remaining balance.
        //    if (amountToPaid > loan.Balance) amountToPaid = loan.Balance;
        //    paymentAmountDue += amountToPaid;
        //    loan.Paid = (loan.Paid + amountToPaid);
        //    loan.Balance = loan.Amount - loan.Paid;
        //    if (loan.Balance == 0.0M)
        //    {
        //        loan.LoanStatu = statusRepos.GetById(3);
        //    }
        //    else
        //    {
        //        loan.LoanStatu = statusRepos.GetById(2);
        //    }
        //    LoansHistory history = new LoansHistory();
        //    history.Id = Guid.NewGuid();
        //    history.Loan = loan;
        //    history.FamilyMember = familyMember;
        //    history.PaymentDetail = detail;
        //    history.AmountPaid = amountToPaid;
        //    historyRepost.Add(history);
        //    tempLoansHistory.Add(history);
        //    return paymentAmountDue;
        //}

        #endregion

        #endregion
    }
}