using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.DTOs;
using Prism.Commands;
using Prism.Regions;

namespace FlopManagerLoanModule.ViewModels
{
    internal class PostLoansViewModel:EditableViewModelBase
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
        ObservableCollection<PeriodYear> _years;
        ObservableCollection<PeriodYear> _futureYears;
        ObservableCollection<LoanType> _loanTypes;
        ObservableCollection<PostLoan> _currentLoans;
        ObservableCollection<PostLoan> _postedLoans;
        ObservableCollection<PaymentSequence> _paymentSequences;
        PeriodYear _selectedYear;
        PeriodYear _selectedFuture;
        LoanType _selectedLoanType;
        PaymentSequence _selectedSequence;

        //
        FamilyContext _unitOfWork;
        DbSet<Loan> _loansRepository;
        DbSet<PeriodYear> _yearsRepository;
        //
        DelegateCommand _closeErrorCommand;
        public event RoutedEventHandler ViewStateChanged;
        ErrorTemplate _errorTemplate;
        dynamic _errorMembers;
        ViewState _viewState;
        PeriodYear _currentYear;
        #endregion

        public PostLoansViewModel()
        {
            _unitOfWork = new FamilyContext();
            _loansRepository = _unitOfWork.Loans;
            _yearsRepository = _unitOfWork.PeriodYears;
            WindowLoaded();
            ViewStateChanged += OnViewStateChanged;
            RulesViolations = new List<RuleViolation>();
            Initialize();

        }

        void OnViewStateChanged(object sender, RoutedEventArgs e)
        {
            switch (_viewState)
            {
                case ViewState.New:
                    //VisualStateManager.GoToElementState(layoutRoot, "HidePostedGrid", false);
                    //VisualStateManager.GoToElementState(layoutRoot, "HideError", false);
                    break;
                case ViewState.Saved:
                    break;
                case ViewState.Edited:
                    //VisualStateManager.GoToElementState(layoutRoot, "HideError", false);
                   // VisualStateManager.GoToElementState(layoutRoot, "ShowPostedGrid", false);
                    break;
                case ViewState.HasErrors:
                    //VisualStateManager.GoToElementState(layoutRoot, "ShowError", false);
                    break;
                case ViewState.Deleted:
                    break;
                default:
                    break;
            }
        }


        #region "Events"
        void WindowLoaded()
        {
            Initialize();
        }
        #endregion

        #region "Properties"
        public PaymentSequence SelectedSeqeunce
        {
            get { return _selectedSequence; }
            set { SetProperty(ref _selectedSequence, value); }
        }
        public ObservableCollection<PaymentSequence> PaymentSequences
        {
            get { return _paymentSequences; }
            set
            {
                SetProperty(ref _paymentSequences, value);
            }
        }
        private PeriodYear CurrentYear
        {
            get
            {
                if (_currentYear == null)
                {
                    try
                    {
                        _currentYear = _yearsRepository.Single(ye => ye.Status == YearStatus.Present);
                    }
                    catch (Exception e)
                    {
                        string msg = Helper.ProcessDbError(e);
                        //Helper.ShowMessage(msg);

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
                SetProperty(ref _errorTemplate, value);
            }
        }
        public PeriodYear SelectedFuture
        {
            get { return _selectedFuture; }
            set
            {
                SetProperty(ref _selectedFuture, value);
            }
        }
        public PeriodYear SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
            }
        }
        public LoanType SelectedLoanType
        {
            get { return _selectedLoanType; }
            set
            {
                SetProperty(ref _selectedLoanType, value);
            }
        }
        public ObservableCollection<PeriodYear> FutureYears
        {
            get { return _futureYears; }
            set
            {
                SetProperty(ref _futureYears, value);
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
        public ObservableCollection<LoanType> LoanTypes
        {
            get { return _loanTypes; }
            set
            {
                SetProperty(ref _loanTypes, value);
            }
        }
        public ObservableCollection<PostLoan> CurrentLoans
        {
            get { return _currentLoans; }
            set
            {
                SetProperty(ref _currentLoans, value);
            }
        }
        public ObservableCollection<PostLoan> PostedLoans
        {
            get { return _postedLoans; }
            set
            {
                SetProperty(ref _postedLoans, value);
            }
        }
        #endregion

        #region "ICommonOperations"
        

      

       

        public List<RuleViolation> RulesViolations
        {
            get;
            set;
        }

        protected override void Initialize()
        {
            Years = LoadCurrentYear();

            ToggleState(ViewState.New);
        }

       
        #endregion

        #region "Helpers"
        ObservableCollection<PeriodYear> LoadCurrentYear()
        {
            var curretYear = CurrentYear;
            var result = _yearsRepository.Where(x => x.Status == curretYear.Status);
            return new ObservableCollection<PeriodYear>(result);
        }

        private ObservableCollection<PostLoan> LoadCurrentLoans(LoanType selected)
        {
            ObservableCollection<PostLoan> psList = new ObservableCollection<PostLoan>();
            var result = _loansRepository.Where(
                                                x => x.LoanType.Code == selected.Code
                                                    &&
                                                    x.PeriodYear.Year == SelectedYear.Year
                                                );
            foreach (var loan in result)
            {
                PostLoan pl = new PostLoan(true, loan.FamilyMember.Code, loan.FamilyMember.FullName,
                                         loan.LoanType.Code, loan.LoanType.LoanDescription,
                                         loan.PeriodYear.Year, loan.PeriodYear.Year,
                                         loan.PaymentSequence.Id, loan.PaymentSequence.SequenceDescription,
                                         loan.Amount);
                psList.Add(pl);
            }
            return psList;
        }
        private ObservableCollection<LoanType> LoadLoanTypes(PeriodYear year)
        {
            ObservableCollection<LoanType> ltList = new ObservableCollection<LoanType>();
            var result = _loansRepository.Where(x => x.PeriodYear.Year == year.Year)
                                                .Select(s => s.LoanType)
                                                .GroupBy(g => g.Code)
                                                .Select(p => p.FirstOrDefault());
            foreach (var item in result)
            {
                ltList.Add(item);
            }
            return ltList;

        }
        private ObservableCollection<PeriodYear> LoadFutureYears()
        {
            ObservableCollection<PeriodYear> futList = new ObservableCollection<PeriodYear>();
            var result = _yearsRepository.Where(x => x.Status == YearStatus.Future);
            return new ObservableCollection<PeriodYear>(result);
        }
        private ObservableCollection<PostLoan> CreatePostedLoans(PeriodYear future, PaymentSequence sequence)
        {
            ObservableCollection<PostLoan> posLoansList = new ObservableCollection<PostLoan>();
            foreach (var loan in CurrentLoans)
            {
                if (loan.Selected)
                {
                    PostLoan pl = new PostLoan(loan.Selected, loan.MemberCode, loan.MemberName,
                                                loan.LoanTypeId, loan.LoanTypeDescription,
                                                future.Year, future.Year, sequence.Id, sequence.SequenceDescription,
                                                loan.LoanAmount);
                    posLoansList.Add(pl);
                }
            }
            return posLoansList;
        }

        private bool ValidData()
        {
            bool result = true;
            _errorMembers = new List<PostLoan>();
            var postedToYearLoans = _loansRepository.Where
                (
                    lo => lo.PeriodYear.Year == SelectedFuture.Year
                    &&
                    lo.PaymentSequence.Id == SelectedSeqeunce.Id

                );
            foreach (var item in PostedLoans)
            {
                var typeExist = postedToYearLoans.Any(x => x.FamilyMember.Code == item.MemberCode
                                         &&
                                         x.LoanType.Code == item.LoanTypeId);
                if (typeExist)
                {
                    string msg = "There are members aleady register for this type of loans!";
                    RulesViolations.Add(new RuleViolation(msg));
                    result = false;
                    _errorMembers.Add(item);
                    //break;

                }

            }
            return result;
        }
        private ObservableCollection<PaymentSequence> LoadSeqeunces(PeriodYear year)
        {
            var yearSeqeunces = _unitOfWork.PaymentSequences.Where(ps => ps.PeriodYear.Year == year.Year);
            return new ObservableCollection<PaymentSequence>(yearSeqeunces);
        }
        #endregion
        #region "Commands Methods"
        protected override void Save()
        {
            if (!ValidData())
            {
                string msg = null;
                foreach (var viol in RulesViolations)
                {
                    msg += viol.ErrorMessage;

                }
                RulesViolations.Clear();
                //Helper.ShowMessage(msg);
                Action ac = () => CloseError();
                RegistredErrorTemplate = new FlopManager.Services.Helpers.ErrorTemplate(_errorMembers, ac);
                ToggleState(ViewState.HasErrors);
                //RaisePropertyChanged("");
                return;
            }
            try
            {
                WriteModelValues(PostedLoans);
                _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {

                throw;
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

        

        private void WriteModelValues(IList<PostLoan> loansToPost)
        {

            string max = "";//Helper.GenerateLoanNo(SelectedFuture.Year);
            int counter = 0;
            foreach (var loan in loansToPost)
            {
                var member = _unitOfWork.FamilyMembers.Single(x => x.Code == loan.MemberCode);
                var loanType = _unitOfWork.LoanTypes.Single(x => x.Code == loan.LoanTypeId);
                var loanYear = SelectedFuture;
                var paySequ = _unitOfWork.PaymentSequences.Single(x => x.Id == loan.PaymentSequenceId);

                Loan postedLoan = new Loan()
                {
                    LoanNo = Helper.AutoIncrement(max, counter),
                    LoanType = loanType,
                    FamilyMember = member,
                    PeriodYear = loanYear,
                    Status = LoanStatus.NotPaid,
                    PaymentSequence = paySequ,
                    Amount = loan.LoanAmount,
                };
                _loansRepository.Add(postedLoan);
                counter++;
            }
        }


        protected override void AddNew()
        {
            Initialize();
        }

        
        

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public override void OnStateChanged(ViewModelState state)
        {
            throw new NotImplementedException();
        }

        protected override bool IsValid()
        {
            throw new NotImplementedException();
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
                    _closeErrorCommand = new DelegateCommand(CloseError);
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
                    //cmbYears.SelectedIndex = -1;
                    LoanTypes = new ObservableCollection<LoanType>();
                    CurrentLoans = new ObservableCollection<PostLoan>();
                    PostedLoans = new ObservableCollection<PostLoan>();
                    FutureYears = new ObservableCollection<PeriodYear>();
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