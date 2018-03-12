using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.DomainModel.Repositories;
using Fbm.ViewsModel.Helpers;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for PaymentOrdersView.xaml
    /// </summary>
    public partial class PaymentOrdersView : Window, IModelOperations<PaymentInstruction>, ICommonOperations
    {
        public PaymentOrdersView()
        {
            InitializeComponent();
            txtPaymentNo.PreviewKeyDown += PaymentNoKeyDown;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.PaymentInstructions;
            _paymentsRepository = _unitOfWork.Payments;
            _loansRepository = _unitOfWork.Loans;
            CurrentPaymentOrders = new ObservableCollection<PaymentInstruction>();
            OldLoansOrders = new ObservableCollection<PaymentInstruction>();
            DataContext = this;
            _paidStatus = _unitOfWork.LoanStatuses.GetById(3);
            _closedYear = _unitOfWork.PeriodStatuses.GetById(1);
            _currentYear = _unitOfWork.PeriodStatuses.GetById(2);
            _futureYear = _unitOfWork.PeriodStatuses.GetById(3);
            
            
        }
        
        #region "Events"
        void PaymentNoKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SearchCommand.Execute(null);
            }
        } 
        #endregion

        #region "Fields"

        string _paymentNo;
        string _paymentYear;
        string _paymentSequence;
        
        bool ChangedFlage { get; set; }
        ModelState _modelState;
        IUnitOfWork _unitOfWork;
        RepositoryBase<Payment> _paymentsRepository;
        RepositoryBase<Loan> _loansRepository;
        RepositoryBase<PaymentInstruction> _repository;
        Payment _currentPayment;
        LoanStatus _paidStatus;
        PeriodStatus _closedYear;
        PeriodStatus _currentYear;
        PeriodStatus _futureYear;

        //
        RelayCommand _saveCommand;
        RelayCommand _addNewCommand;
        RelayCommand _searchCommand;
        //
        ObservableCollection<Loan> _paymentLoans;
        ObservableCollection<PaymentInstruction> _currentPaymentOrders;
        ObservableCollection<PaymentInstruction> _oldLoansOrders;

        //

        #endregion

        #region "Properties"
        
        public ObservableCollection<Loan> PaymentLoans
        {
            get { return _paymentLoans; }
            set
            {
                _paymentLoans = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<PaymentInstruction> CurrentPaymentOrders
        {
            get { return _currentPaymentOrders; }
            set
            {
                _currentPaymentOrders = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<PaymentInstruction> OldLoansOrders
        {
            get { return _oldLoansOrders; }
            set
            {
                _oldLoansOrders = value;
                RaisePropertyChanged();
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
        #endregion

        #region "IModelOperations"
        public void ReadModelValues(PaymentInstruction model)
        {
            throw new NotSupportedException();
        }

        public void WriteModelValues(PaymentInstruction model)
        {
            throw new NotSupportedException();
        }

        public bool ValidData()
        {
            throw new NotImplementedException();
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
        
        

        public void SetState(Helpers.ModelState state)
        {
            _modelState = state;
            switch (_modelState)
            {
                case ModelState.New:
                    Initialize();
                    RaisePropertyChanged();
                    break;
                case ModelState.Saved:
                    
                    break;
                case ModelState.Deleted:
                    break;
                default:
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

        public List<Helpers.RuleViolation> RulesViolations
        {
            get;
            set;
        }

        public void Initialize()
        {

        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region "Commands Methods"

        void Save()
        {
            var tempPaymentOrders = new List<PaymentInstruction>();

            foreach (var cpOrder in CurrentPaymentOrders)
            {
                tempPaymentOrders.Add(cpOrder);
            }
            foreach (var olOrders in OldLoansOrders)
            {
                tempPaymentOrders.Add(olOrders);
            }

            if (_modelState == ModelState.New)
            {
                foreach (var tpOrders in tempPaymentOrders)
                {
                    _repository.Add(tpOrders);
                }
            }
            else
            {
                WriteValues(tempPaymentOrders);
            }
            _unitOfWork.Save();
            SetState(ModelState.Saved);
        }

        private void WriteValues(List<PaymentInstruction> tempPaymentOrders)
        {
            foreach (var uiOrder in tempPaymentOrders)
            {
                var dbOrder = _repository.GetById(uiOrder.AutoKey);
                dbOrder.EarnPercent = uiOrder.EarnPercent;
            }
        }
        void AddNew() { }
        void Search() 
        {
            string s = txtPaymentNo.Text;
            if (string.IsNullOrEmpty(s)) return;
            try
            {
                _currentPayment = _paymentsRepository.GetById(s);
                PaymentNo = _currentPayment.PaymentNo;
                PaymentYear = _currentPayment.PeriodSetting.YearPart;
                PaymentSequence = _currentPayment.PaymentSequence.SequenceDescription;
                var ordersRegistred = _repository.Query(o => o.Payment.PaymentNo == _currentPayment.PaymentNo);
                if (ordersRegistred.Count() > 0)
                {
                    var cplOrders = ordersRegistred.Where(x => x.PeriodSetting.Id == _currentPayment.PeriodSetting.Id);
                    var oplOrders = ordersRegistred.Where(x => x.PeriodSetting.Id != _currentPayment.PeriodSetting.Id);
                    CurrentPaymentOrders = new ObservableCollection<PaymentInstruction>(cplOrders);
                    OldLoansOrders = new ObservableCollection<PaymentInstruction>(oplOrders);
                    SetState(ModelState.Saved);
                    return;
                }
                //First get the loans for this payment spicficlly
                var thisPaymentLoans = _loansRepository.Query
                                                   (
                                                     lon => lon.PeriodSetting.Id == _currentPayment.PeriodSetting.Id &&
                                                     lon.PaymentSequence.Id == _currentPayment.PaymentSequence.Id
                                                     )
                                                     .GroupBy(type => type.LoanTypeCode)
                                                     .Select(lo => lo.FirstOrDefault());
                foreach (var loan in thisPaymentLoans)
                {
                    CurrentPaymentOrders.Add(new PaymentInstruction()
                    {
                        EarnPercent = 0,
                        LoanType = loan.LoanType,
                        Payment = _currentPayment,
                        PeriodSetting = loan.PeriodSetting,
                        OldLoan = false
                    });
                }
                //Second: Get all loans that:
                //1 does not paid yet.
                //2 and does not belongs to a future year
                //3 and does not belongs to the past years.
                var oldDepitLoans = _loansRepository.Query
                    (
                        lon => lon.LoanStatus.Id != _paidStatus.Id
                               &&
                               lon.PeriodSetting.PeriodStatus.Id != _futureYear.Id
                               &&
                               lon.PaymentSequence.Id != _currentPayment.PaymentSequence.Id
                               
                    )
                    .GroupBy(type => type.LoanType)
                    .Select(lo => lo.FirstOrDefault());
                foreach (var oldLoan in oldDepitLoans)
                {
                    OldLoansOrders.Add(new PaymentInstruction()
                    {
                        EarnPercent = 0,
                        LoanType = oldLoan.LoanType,
                        Payment = _currentPayment,
                        PeriodSetting = oldLoan.PeriodSetting,
                        OldLoan =true
                    });
                }
                _modelState = ModelState.New;
                SetState(_modelState);
            }
            catch (InvalidOperationException)
            {
                string msg = "لا يوجد دفعة بهذا الرقم";
                Helper.ShowMessage(msg);
                
            }
           

            
        }
        void AddOrder()
        {

        }
        #endregion



    }
}
