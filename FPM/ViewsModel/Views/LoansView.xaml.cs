using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Fbm.ViewsModel.Properties;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for LoansView.xaml
    /// </summary>
    public partial class LoansView : Window, INotifyPropertyChanged
    {
        public LoansView()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += WindowLoaded;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.Loans;
            _settings = Settings.Default;
            RulesViolations = new List<RuleViolation>();
        }


        #region "Fields"
        FamilyMember _member;
        string _searchField;
        LoanType _loanType;
        PeriodSetting _loanYear;
        PaymentSequence _paymentSequence;
        LoanStatus _status;
        PeriodSetting _currentYear;
        string _description;
        decimal _amount;
        decimal _paid;
        string _remarks;
        ObservableCollection<LoanType> _loansTypes;
        ObservableCollection<PeriodSetting> _years;
        ObservableCollection<PaymentSequence> _paymentSequences;
        ObservableCollection<Loan> _memberLoans;
        ModelState _modelState;
        IUnitOfWork _unitOfWork;
        RepositoryBase<Loan> _repository;
        Loan _currnetModel;
        bool ChangedFlage { get; set; }
        const string Undefined = "لا يوجد";
        //Commands
        RelayCommand _saveCommand;
        RelayCommand _searchCommand;
        RelayCommand _addNewcommand;
        RelayCommand _addMemberCommand;
        //
        Settings _settings;
        List<RuleViolation> RulesViolations { get; set; }
        IEnumerable<LoanStatus> LoansStatuses { get; set; }
        #endregion

        #region "Events"
        void WindowLoaded(object sender, RoutedEventArgs e)
        {

            SetState(ModelState.New);

        }
        private void MemberCodeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                AddMemberCommand.Execute(null);
            }
        }
        #endregion

        #region "Properties"
        private PeriodSetting CurrentYear
        {
            get
            {
                if (_currentYear == null)
                {
                    try
                    {
                        _currentYear = _unitOfWork.PeriodSettings.Query(ye => ye.PeriodStatus.Id == 2).Single();
                    }
                    catch (Exception e)
                    {
                        string msg = Helper.ProcessDbError(e);
                        Helper.ShowMessage(msg);

                    }

                }
                return _currentYear;
            }
        }

        public FamilyMember Member
        {
            get { return _member; }
            set
            {
                _member = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }

        public LoanType LoanType
        {
            get { return _loanType; }
            set
            {
                _loanType = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }
        public PeriodSetting LoanYear
        {
            get { return _loanYear; }
            set
            {
                _loanYear = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }
        public PaymentSequence PaymentSequence
        {
            get { return _paymentSequence; }
            set
            {
                _paymentSequence = value;
                RaisePropertyChanged();
                SetChangedFlage();
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
                _description = value;
                RaisePropertyChanged();
            }
        }
        public LoanStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Balance");
                SetChangedFlage();
            }
        }
        public decimal Paid
        {
            get { return _paid; }
            set
            {
                _paid = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Balance");
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
                _remarks = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }
        public ObservableCollection<LoanType> LoansTypes
        {
            get { return _loansTypes; }
            set
            {
                _loansTypes = value;
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

       

        public string SearchField
        {
            get { return _searchField; }
            set
            {
                _searchField = value;
                RaisePropertyChanged();
            }
        }
        public string LoanStatus
        {
            get
            {
                if (_currnetModel == null) return Undefined;
                if (_currnetModel.LoanStatus == null) return Undefined;
                return _currnetModel.LoanStatus.Status;
            }
        }
        public ObservableCollection<Loan> MemberLoans
        {
            get 
            {
                return _memberLoans;
            }
            private set
            {
                _memberLoans = value;
                RaisePropertyChanged();
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

        #region "INotifyPropertyChanged Members"
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region "Helpers"
        void SetState(ModelState state)
        {
            _modelState = state;
            switch (_modelState)
            {
                case ModelState.New:
                    Initialize();
                    ResetChangedFlage();
                    break;
                case ModelState.Saved:
                    txtMemberCode.IsEnabled = false;
                    RaisePropertyChanged("");
                    ResetChangedFlage();
                    break;
                case ModelState.Deleted:
                    ResetChangedFlage();
                    break;
                default:
                    break;
            }
        }

        private void Initialize()
        {

            _currnetModel = new Loan();
            ReadModelValues(_currnetModel);
            LoansTypes = GetLoansTypes();
            Years = GetYears();
            
            LoansStatuses = GetLoansStatuses();
            cmbLoansTypes.SelectedIndex = -1;
            cmbLoansSeq.SelectedIndex = -1;
            cmbLoansYears.SelectedIndex = -1;
            txtMemberCode.IsEnabled = true;
            txtMemberCode.Focus();
            txtMemberCode.SelectAll();
        }

        private void ReadModelValues(Loan model)
        {
            if (model == null) throw new ArgumentNullException("model");
            Member = model.FamilyMember;
            this.LoanType = model.LoanType;
            this.LoanYear = model.PeriodSetting;
            this.PaymentSequence = model.PaymentSequence;
            this.Status = model.LoanStatus;
            Amount = model.Amount;
            Paid = model.Paid;
            Description = model.Description;
            Remarks = model.Remarks;
            RaisePropertyChanged("");
        }
        void WriteModelValues(Loan model)
        {
            string currentYear = CurrentYear.YearPart;
            if (model == null) throw new ArgumentNullException("model");
            if (_modelState == ModelState.New)
            {
                model.LoanNo = Helper.GenerateLoanNo(currentYear);
            }
            model.FamilyMember = Member;
            model.LoanType = LoanType;
            model.PeriodSetting = LoanYear;
            model.PaymentSequence = PaymentSequence;
            model.Amount = Amount;
            model.Paid = Paid;
            model.Balance = Balance;
            model.Description = Description;
            model.Remarks = Remarks;

        }


        void SetChangedFlage()
        {
            ChangedFlage = true;
        }
        void ResetChangedFlage()
        {
            ChangedFlage = false;
        }

        bool ValidData()
        {
            bool valid = true;

            if (Member == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_MemberMissing));
                return false;
            }
            if (LoanType == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_LoanTypeMissing));
                return false;
            }
            if (LoanYear == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_LoanYearMissing));
                return false;
            }
            if (PaymentSequence == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_PaySeqMissing));
                return false;
            }
            if (Balance < 0)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_InsufBalance));
                return false;
            }
            if (Amount == 0)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_ZeroloanAmount));
                return false;
            }
            if (_modelState == ModelState.New)
            {
                var currYearLoans = _repository.Query(
                                                       lo => lo.PeriodSetting.Id == LoanYear.Id
                                                       &&
                                                       lo.PaymentSequence.Id == PaymentSequence.Id
                                                       );
                bool exist = currYearLoans.Any(
                                                lo => lo.FamilyMember.Code == Member.Code
                                                 &&
                                                lo.LoanType.Code == LoanType.Code
                                                );
                if (exist)
                {
                    RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_ExistMemberLoan));
                    return false;
                }
            }
            if (string.IsNullOrEmpty(Description))
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoansView_DescriptionMissing));
                return false;
            }
            return valid;
        }
        bool OkClose()
        {
            return !ChangedFlage;
        }
        ObservableCollection<LoanType> GetLoansTypes()
        {
            if (LoansTypes != null) return LoansTypes;
            return new ObservableCollection<LoanType>(_unitOfWork.LoanTypes.GetAll());
        }
        ObservableCollection<PeriodSetting> GetYears()
        {
            if (Years != null) return Years;
            return new ObservableCollection<PeriodSetting>(_unitOfWork.PeriodSettings.GetAll());
        }
        ObservableCollection<PaymentSequence> GetSequences(PeriodSetting year)
        {
            var yearSeqeunces = _unitOfWork.PaymentSequences.Query(ps => ps.PeriodSetting.Id == year.Id);
            return new ObservableCollection<PaymentSequence>(yearSeqeunces);
        }
        IEnumerable<LoanStatus> GetLoansStatuses()
        {
            if (LoansStatuses != null) return LoansStatuses;
            return _unitOfWork.LoanStatuses.GetAll();
        }
        FamilyMember GetFamilyMember(int code)
        {
           return _unitOfWork.FamilyMembers.GetById(code);
        }
        
        
        #endregion

        #region "Commands"
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(Save);
                }
                return _saveCommand;
            }
        }
        void Save()
        {
            if (!ValidData())
            {
                string msg = null;
                foreach (var viol in RulesViolations)
                {
                    msg += viol.ErrorMessage;

                }
                RulesViolations.Clear();
                Helper.ShowMessage(msg);
                return;

            }

            try
            {
                WriteModelValues(_currnetModel);
                if (_modelState == ModelState.New)
                {
                    _currnetModel.LoanStatus = LoansStatuses.Single(x => x.Id == 1);
                    _repository.Add(_currnetModel);
                }
                _unitOfWork.Save();
                SetState(ModelState.Saved);

            }
            catch (Exception ex)
            {
                string msg = Helper.ProcessExceptionMessages(ex);
                Helper.ShowMessage(msg);
                
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
        void Search()
        {
            string s = txtSearchedMember.Text;
            if (string.IsNullOrEmpty(s)) return;
            int code;
            if (int.TryParse(s, out code))
            {
                try
                {
                    var tempResult =_repository.Query(lo => lo.FamilyMember.Code == code);
                    MemberLoans = new ObservableCollection<Loan>(tempResult);
                }
                catch (Exception ex)
                {
                    string msg = Helper.ProcessExceptionMessages(ex);
                    Logger.Log(LogMessageTypes.Error, 
                        msg, ex.TargetSite.Name, ex.StackTrace);
                    MessageBox.Show(msg);
                }
               
                
            }
        }
        public ICommand AddNewCommand
        {
            get
            {
                if (_addNewcommand == null)
                {
                    _addNewcommand = new RelayCommand(AddNew);
                }
                return _addNewcommand;
            }
        }
        void AddNew()
        {
            if (!OkClose())
            {
                string msg = Properties.Resources.PromptForSaveMsg;
                if (!Helper.UserConfirmed(msg)) return;

            }
            SetState(ModelState.New);

        }
        public ICommand AddMemberCommand
        {
            get
            {
                if (_addMemberCommand == null)
                {
                    _addMemberCommand = new RelayCommand(AddMember);
                }
                return _addMemberCommand;
            }
        }
        void AddMember()
        { 
            string s = txtMemberCode.Text;
            if (string.IsNullOrEmpty(s)) return;
            int code;
            if(int.TryParse(s, out code))
            {
                try
                {
                    Member = GetFamilyMember(code);

                }
                catch (InvalidOperationException)
                {
                    string msg = Properties.Resources.InvalidFamilyMemberCode;
                    Helper.ShowMessage(msg);
                }
            }
        }
        #endregion

        private void SearchMemberKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SearchCommand.Execute(null);
            }
        }

        private void SelectedLoanChanged(object sender, SelectionChangedEventArgs e)
        {
            Loan temp = lstMemeberLoans.SelectedItem as Loan;
            if (temp != null)
            {
                _currnetModel = temp;
                ReadModelValues(_currnetModel);
                SetState(ModelState.Saved);
            }
        }

        private void YearsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoanYear != null)
            {
                PaymentSequences = GetSequences(LoanYear);
            }
        }

        
    }
}
