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
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for PostLoansView.xaml
    /// </summary>
    public partial class PostLoansView : Window, ICommonOperations
    {
        //Gools:
        //1- Year must be the current year.
        //1- Show loan types of the selected year.
        //1- Show members of the selected loan type for the specified year.
        //2 Eable selecting the desired members.
        //3-Enable editing the loan amount of each member.
        //- Generate new loans of the loan type for the specific year just for the selected members .
        //
        #region "Fields"
        ObservableCollection<PeriodSetting> _years;
        ObservableCollection<PeriodSetting> _futureYears;
        ObservableCollection<LoanType> _loanTypes;
        ObservableCollection<PostLoan> _currentLoans;
        ObservableCollection<PostLoan> _postedLoans;
        ObservableCollection<PaymentSequence> _paymentSequences;
        PeriodSetting _selectedYear;
        PeriodSetting _selectedFuture;
        LoanType _selectedLoanType;
        PaymentSequence _selectedSequence;

        //
        IUnitOfWork _unitOfWork;
        RepositoryBase<Loan> _loansRepository;
        RepositoryBase<PeriodSetting> _yearsRepository;
        //
        RelayCommand _addNewCommand;
        RelayCommand _saveCommand;
        RelayCommand _closeErrorCommand;
        public event RoutedEventHandler ViewStateChanged;
        ErrorTemplate _errorTemplate;
        dynamic _errorMembers;
        ViewState _viewState;
        PeriodSetting _currentYear;
        #endregion

        public PostLoansView()
        {
            InitializeComponent();
            _unitOfWork = new UnitOfWork();
            _loansRepository = _unitOfWork.Loans;
            _yearsRepository = _unitOfWork.PeriodSettings;
            Loaded += WindowLoaded;
            ViewStateChanged += OnViewStateChanged;
            DataContext = this;
            RulesViolations = new List<RuleViolation>();
            
        }

        void OnViewStateChanged(object sender, RoutedEventArgs e)
        {
            switch (_viewState)
            {
                case ViewState.New:
                    VisualStateManager.GoToElementState(layoutRoot, "HidePostedGrid", false);
                    VisualStateManager.GoToElementState(layoutRoot, "HideError", false);
                    break;
                case ViewState.Saved:
                    break;
                case ViewState.Edited:
                    VisualStateManager.GoToElementState(layoutRoot, "HideError", false);
                    VisualStateManager.GoToElementState(layoutRoot, "ShowPostedGrid", false);
                    break;
                case ViewState.HasErrors:
                    VisualStateManager.GoToElementState(layoutRoot, "ShowError", false);
                    break;
                case ViewState.Deleted:
                    break;
                default:
                    break;
            }
        }
        
        
        #region "Events"
        void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
        #endregion

        #region "Properties"
        public PaymentSequence SelectedSeqeunce
        {
            get { return _selectedSequence; }
            set
            {
                _selectedSequence = value;
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
        private PeriodSetting CurrentYear
        {
            get 
            {
                if (_currentYear == null)
                {
                    try
                    {
                        _currentYear = _yearsRepository.Query(ye => ye.PeriodStatus.Id == 2).Single();
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
        private bool SaveEnabled
        {
            get;
            set;
        }
      
        public ErrorTemplate RegistredErrorTemplate
        {
            get { return _errorTemplate; }
            set
            {
                _errorTemplate = value;
                RaisePropertyChanged();
            }
        }
        public PeriodSetting SelectedFuture
        {
            get { return _selectedFuture; }
            set
            {
                _selectedFuture = value;
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
        public LoanType SelectedLoanType
        {
            get { return _selectedLoanType; }
            set
            {
                _selectedLoanType = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<PeriodSetting> FutureYears
        {
            get { return _futureYears; }
            set
            {
                _futureYears = value;
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
        public ObservableCollection<LoanType> LoanTypes
        {
            get { return _loanTypes; }
            set
            {
                _loanTypes = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<PostLoan> CurrentLoans
        {
            get { return _currentLoans; }
            set
            {
                _currentLoans = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<PostLoan> PostedLoans
        {
            get { return _postedLoans; }
            set
            {
                _postedLoans = value;
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
            get { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
        }

        public void SetChangedFlag()
        {
            throw new NotImplementedException();
        }

        public void ResetChangedFalg()
        {
            throw new NotImplementedException();
        }

        public bool OkClose()
        {
            throw new NotImplementedException();
        }

        public List<RuleViolation> RulesViolations
        {
            get;
            set;
        }

        public void Initialize()
        {
            Years = LoadCurrentYear();

            ToggleState(ViewState.New);
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

        #region "Helpers"
        ObservableCollection<PeriodSetting> LoadCurrentYear()
        {
            var curretYear = CurrentYear;
            var result = _yearsRepository.Query(x => x.PeriodStatus.Id == curretYear.PeriodStatus.Id);
            return new ObservableCollection<PeriodSetting>(result);
        }

        private ObservableCollection<PostLoan> LoadCurrentLoans(LoanType selected)
        {
            ObservableCollection<PostLoan> psList = new ObservableCollection<PostLoan>();
            var result = _loansRepository.Query(
                                                x => x.LoanType.Code == selected.Code
                                                    &&
                                                    x.PeriodSetting.Id == SelectedYear.Id
                                                );
            foreach (var loan in result)
            {
                PostLoan pl = new PostLoan(true, loan.FamilyMember.Code, loan.FamilyMember.FullName,
                                         loan.LoanType.Code, loan.LoanType.LoanDescription,
                                         loan.PeriodSetting.YearPart, loan.PeriodSetting.YearPart,
                                         loan.PaymentSequence.Id,loan.PaymentSequence.SequenceDescription, 
                                         loan.Amount);
                psList.Add(pl);
            }
            return psList;
        }
        private ObservableCollection<LoanType> LoadLoanTypes(PeriodSetting year)
        {
            ObservableCollection<LoanType> ltList = new ObservableCollection<LoanType>();
            var result = _loansRepository.Query(x => x.PeriodSetting.Id == year.Id)
                                                .Select(s => s.LoanType)
                                                .GroupBy(g => g.Code)
                                                .Select(p => p.FirstOrDefault());
            foreach (var item in result)
            {
                ltList.Add(item);
            }
            return ltList;
                                            
        }
        private ObservableCollection<PeriodSetting> LoadFutureYears()
        {
            ObservableCollection<PeriodSetting> futList = new ObservableCollection<PeriodSetting>();
            var result = _yearsRepository.Query(x => x.PeriodStatus.Id == 3);
            return new ObservableCollection<PeriodSetting>(result);
        }
        private ObservableCollection<PostLoan> CreatePostedLoans(PeriodSetting future, PaymentSequence sequence)
        {
            ObservableCollection<PostLoan> posLoansList = new ObservableCollection<PostLoan>();
            foreach (var loan in CurrentLoans)
            {
                if (loan.Selected)
                {
                    PostLoan pl = new PostLoan(loan.Selected, loan.MemberCode, loan.MemberName, 
                                                loan.LoanTypeId, loan.LoanTypeDescription, 
                                                future.YearPart, future.YearPart, sequence.Id, sequence.SequenceDescription,
                                                loan.LoanAmount);
                    posLoansList.Add(pl);
                }
            }
            return posLoansList;
        }

        private bool ValidData()
        {
            bool result =true;
            _errorMembers = new List<PostLoan>();
            var postedToYearLoans = _loansRepository.Query
                (
                    lo => lo.PeriodSetting.Id == SelectedFuture.Id
                    &&                                 
                    lo.PaymentSequence.Id == SelectedSeqeunce.Id
                
                );
            foreach (var item in PostedLoans)
            {
                var typeExist= postedToYearLoans.Any(x => x.FamilyMember.Code == item.MemberCode 
                                        &&
                                        x.LoanType.Code == item.LoanTypeId);
                if (typeExist)
                { 
                    string msg = "There are members aleady register for this type of loasn!";
                    RulesViolations.Add(new RuleViolation(msg));
                    result = false;
                    _errorMembers.Add(item);
                    //break;

                }

            }
            return result;
        }
        private ObservableCollection<PaymentSequence> LoadSeqeunces(PeriodSetting year)
        {
            var yearSeqeunces = _unitOfWork.PaymentSequences.Query(ps => ps.PeriodSetting.Id == year.Id);
            return new ObservableCollection<PaymentSequence>(yearSeqeunces);
        }
        #endregion
        #region "Commands Methods"
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
                Action ac = () => CloseError();
                RegistredErrorTemplate = new Helpers.ErrorTemplate(_errorMembers, ac);
                ToggleState(ViewState.HasErrors);
                //RaisePropertyChanged("");
                return;
            }
            try
            {
                WriteModelValues(PostedLoans);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }
        bool CanSave()
        {
            return SaveEnabled;
        }
        private void WriteModelValues(IList<PostLoan> loansToPost)
        {
             
            string max = Helper.GenerateLoanNo(SelectedFuture.YearPart); 
            int counter= 0;
            foreach (var loan in loansToPost)
            {
                var member = _unitOfWork.FamilyMembers.GetById(loan.MemberCode);
                var loanStatus = _unitOfWork.LoanStatuses.GetById(1);
                var loanTypo = _unitOfWork.LoanTypes.GetById(loan.LoanTypeId);
                var loanYear = SelectedFuture;
                var paySequ = _unitOfWork.PaymentSequences.GetById(loan.PaymentSequenceId);

                Loan postedLoan = new Loan()
                {
                    LoanNo = Helper.AutoIncrement(max, counter),
                    LoanType = loanTypo,
                    FamilyMember = member,
                    PeriodSetting = loanYear,
                    LoanStatus = loanStatus,
                    PaymentSequence = paySequ,
                    Amount = loan.LoanAmount,
                    Paid = 0.0M,
                    Balance = loan.LoanAmount

                };
                _loansRepository.Add(postedLoan);
                counter++;
            }
        }

        
        void AddNew()
        {
            Initialize();
        }
        #endregion

        private void LoanTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedLoanType != null)
            {
                CurrentLoans = LoadCurrentLoans(SelectedLoanType);
                FutureYears = LoadFutureYears();
            }
        }


        private void YearSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedYear != null)
            {
                LoanTypes = LoadLoanTypes(SelectedYear);
            }
            
        }

        private void FutureSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedFuture != null)
            {
                PaymentSequences = LoadSeqeunces(SelectedFuture);
            }
           
        }


        public ICommand CloseErrorCommand
        {
            get
            {
                if (_closeErrorCommand == null)
                {
                    _closeErrorCommand = new RelayCommand(CloseError);
                }
                return _closeErrorCommand;
            }
        }
        void CloseError()
        {
            ToggleState(ViewState.Edited);
        }

        void ToggleState(ViewState state)
        {
            _viewState = state;
            switch (_viewState)
            {
                case ViewState.New:
                    RaiseViewStateChanged();
                    cmbYears.SelectedIndex = -1;
                    LoanTypes = new ObservableCollection<LoanType>();
                    CurrentLoans = new ObservableCollection<PostLoan>();
                    PostedLoans = new ObservableCollection<PostLoan>();
                    FutureYears = new ObservableCollection<PeriodSetting>();
                    PaymentSequences = new ObservableCollection<PaymentSequence>();
                    SaveEnabled = true;
                    break;
                case ViewState.Saved:
                    RaiseViewStateChanged();

                    SaveEnabled = true;
                    break;
                case ViewState.Edited:
                    RaiseViewStateChanged();
                    SaveEnabled = true;
                    break;
                case ViewState.HasErrors:
                    RaiseViewStateChanged();
                    SaveEnabled = false;
                    break;
                case ViewState.Deleted:
                    RaiseViewStateChanged();
                    SaveEnabled = true;
                    break;
                default:
                    break;
            }
        }
        void RaiseViewStateChanged()
        {
            if (ViewStateChanged != null)
            {
                ViewStateChanged(this, new RoutedEventArgs());
            }
        }

        private void SequencesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedFuture != null && SelectedSeqeunce != null)
            {
                PostedLoans = CreatePostedLoans(SelectedFuture, SelectedSeqeunce);
                ToggleState(ViewState.Edited);
            }
        }

        

        

        
    }
}
