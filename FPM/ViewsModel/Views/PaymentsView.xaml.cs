using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.DomainModel.Repositories;
using Fbm.ViewsModel.Helpers;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for PaymentsView.xaml
    /// </summary>
    public partial class PaymentsView : ICommonOperations, IModelOperations<Payment>
    {
        public PaymentsView()
        {
            InitializeComponent();
            Loaded += WindowLoad;
            DataContext = this;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.Payments;
            RulesViolations = new List<RuleViolation>();
        }
        #region "Fields"
        PeriodSetting _paymentYear;
        PaymentSequence _paymentSequence;
        decimal _paymentAmount;
        string _paymentNo;
        bool ChangedFlage { get; set; }
        ModelState _modelState;
        readonly IUnitOfWork _unitOfWork;
        readonly RepositoryBase<Payment> _repository;
        Payment _currnetModel;
        //
        RelayCommand _saveCommand;
        RelayCommand _addNewCommand;
        RelayCommand _searchCommand;
        RelayCommand _flagCommand;
        //
        ObservableCollection<PeriodSetting> _paymentYears;
        ObservableCollection<PaymentSequence> _paymentSequences;
        ObservableCollection<Payment> _yearPayments;
        //

        #endregion

        #region "Properties"
        public PeriodSetting CurrentPeriod
        {
            get
            {
                PeriodSetting p = null;
                try
                {
                    p = _unitOfWork.PeriodSettings.Query(x => x.PeriodStatus.Id == 2).Single();
                }
                catch (Exception ex)//Fail silently
                {
                    string msg = Helper.ProcessExceptionMessages(ex);
                    Logger.Log(LogMessageTypes.Error, msg, ex.TargetSite.Name, ex.StackTrace);

                }
                return p;
            }
        }

        public PeriodSetting PaymentYear
        {
            
            get { return _paymentYear; }
            set
            {
                _paymentYear = value;
                RaisePropertyChanged();
                SetChangedFlag();
            }
        }
        public PaymentSequence PaymentSequence
        {
            get { return _paymentSequence; }
            set
            {
                _paymentSequence = value;
                RaisePropertyChanged();
                SetChangedFlag();
            }
        }
        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                _paymentAmount = value;
                RaisePropertyChanged();
                SetChangedFlag();
            }
        }
        public string PaymentNo
        {
            get
            {
                return _paymentNo;
            }
            set
            {
                _paymentNo = value;
                RaisePropertyChanged();
            }

        }
        public ObservableCollection<PeriodSetting> PaymentYears
        {
            get { return _paymentYears; }
            set
            {
                _paymentYears = value;
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
        public ObservableCollection<Payment> YearPayments
        {
            get { return _yearPayments; }
            set
            {
                _yearPayments = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region "Events"
        private void WindowLoad(object sender, RoutedEventArgs e)
        {
            SetState(ModelState.New);
        }
        #endregion

        #region "ICommonOperations"
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
            _modelState = state;
            switch (_modelState)
            {
                case ModelState.New:
                    Initialize();
                    ResetChangedFalg();
                    break;
                case ModelState.Saved:
                    PaymentNo = _currnetModel.PaymentNo;
                    ResetChangedFalg();
                    break;
                case ModelState.Deleted:
                    break;
            }
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

        public List<RuleViolation> RulesViolations
        {
            get;
            set;
        }

        public void Initialize()
        {
            _currnetModel = new Payment();
            PaymentYears = GetYears();
            cmbYears.SelectedIndex = -1;
            cmbSequences.SelectedIndex = -1;
            PaymentSequences = new ObservableCollection<PaymentSequence>();
            PaymentNo = string.Empty;
            PaymentAmount = 0.0M;
            RaisePropertyChanged("");
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

        #region "IMdoelOperations"
        public void ReadModelValues(Payment model)
        {
            if (model == null) throw new ArgumentNullException("model");
            PaymentNo = model.PaymentNo;
            PaymentYear = model.PeriodSetting;
            PaymentSequence = model.PaymentSequence;
            PaymentAmount = model.Amount;
        }

        public void WriteModelValues(Payment model)
        {
            if(model == null) throw new ArgumentNullException("model");
            BindingExpression be = txtPaymentAmount.GetBindingExpression(TextBox.TextProperty);
            if (be != null) be.UpdateSource();
            string currentYear = CurrentPeriod.YearPart;
            if (_modelState == ModelState.New)
            {
                model.PaymentNo = GeneratePaymentNo(currentYear);
            }
            model.PeriodSetting = PaymentYear;
            model.PaymentSequence = PaymentSequence;
            model.Amount = PaymentAmount;

        }

        public bool ValidData()
        {
            if (PaymentYear == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PaymentView_PayYearMissing));
                return false;
            }
            if (PaymentSequence == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PaymentView_PaySeqMissing));
                return false;
            }
            return true;

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
                return;

            }

            try
            {
                WriteModelValues(_currnetModel);
                if (_modelState == ModelState.New)
                {
                    PaymentStatus status = _unitOfWork.PaymentStatuses.GetById(1); //Initiated
                    _currnetModel.PaymentStatus = status;
                    _repository.Add(_currnetModel);
                }
                _unitOfWork.Save();
                SetState(ModelState.Saved);

            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
                
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
        void Search()
        {
            string s = txtSearchedYear.Text;
            if (string.IsNullOrEmpty(s)) return;
            var result = _repository.Query(paym => paym.PeriodSetting.YearPart == s);
            YearPayments = new ObservableCollection<Payment>(result);
            

        }
        void Flag()
        { 
            var flagStatus = _unitOfWork.PaymentStatuses.GetById(3);
            _currnetModel.PaymentStatus = flagStatus;
            
        }
        bool CanFlag()
        {
            return (
                    _currnetModel != null && 
                    _currnetModel.PaymentStatus != null &&
                    _currnetModel.PaymentStatus.Id == 2
                    );
        }
        #endregion

        #region "Helper"
        string GeneratePaymentNo(string activeYear)
        {
            if (string.IsNullOrEmpty(activeYear)) throw new ArgumentNullException("activeYear");
            string currentYearPortion = activeYear.Substring(2, 2);
            string dbMaxNo = Payment.MaxNo;

            if (!string.IsNullOrEmpty(dbMaxNo))
            {
                string dbYearPortion = dbMaxNo.Substring(0, 2);
                if (dbYearPortion.Equals(currentYearPortion))
                {
                    string incrementedPortion = dbMaxNo.Substring(3, 3);
                    int incrementedNo;

                    if (int.TryParse(incrementedPortion, out incrementedNo))
                    {
                        incrementedNo++;
                    }
                    return currentYearPortion + DecorateNo(incrementedNo);
                }
                
            }
            return Helper.StartNewIncrement(currentYearPortion).ToString(CultureInfo.InvariantCulture);
        }
        private string DecorateNo(int i)
        {
            string s = i.ToString(CultureInfo.InvariantCulture);
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

        ObservableCollection<PeriodSetting> GetYears()
        {
            if (PaymentYears != null) return PaymentYears;
            return new ObservableCollection<PeriodSetting>(_unitOfWork.PeriodSettings.GetAll());
        }
        ObservableCollection<PaymentSequence> GetSequences(PeriodSetting year)
        {
            var yearSeqeunces = _unitOfWork.PaymentSequences.Query(ps => ps.PeriodSetting.Id == year.Id);
            return new ObservableCollection<PaymentSequence>(yearSeqeunces);
        }
        #endregion

        private void SearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SearchCommand.Execute(null);
            }
        }

        private void PaymentsChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lstPyaments.SelectedItem as Payment;
            if (selected != null)
            {
                _currnetModel = selected;
                ReadModelValues(_currnetModel);
                SetState(ModelState.Saved);
            }
        }
        public ICommand FlagCommand
        {
            get
            {
                if (_flagCommand == null)
                {
                    _flagCommand = new RelayCommand(Flag, CanFlag);
                }
                return _flagCommand;
            }
        }

        private void PaymentYearsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentYear != null)
            {
                PaymentSequences = GetSequences(PaymentYear);
            }
        }
    }
        
}
