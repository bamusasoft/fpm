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
    /// Interaction logic for MemberStatmentView.xaml
    /// </summary>
    public partial class MemberStatmentView : Window, INotifyPropertyChanged
    {
        #region "Fields"
        IUnitOfWork _unitOfWork;
        RepositoryBase<PeriodSetting> _repoPeriodSetting;

        MemberStatmentReport _statment;
        PeriodSetting _selectedYear;
        ObservableCollection<PeriodSetting> _years;

        RelayCommand _searchCommand;
        RelayCommand _printCommand;
        HashSet<MemberStatmentDetail> _internalStatementDetails;
        #endregion
        public MemberStatmentView()
        {
            InitializeComponent();
            _unitOfWork = new UnitOfWork();
            _repoPeriodSetting = _unitOfWork.PeriodSettings;
            Loaded += WindowLoaded;
            DataContext = this;
        }

        void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var r = _repoPeriodSetting.GetAll();
            Years = new ObservableCollection<PeriodSetting>(r);
        }

        

        #region "Properties"
        public MemberStatmentReport Statment
        {
            get 
            {
                return _statment;
            }
            set
            {
                _statment = value;
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
        public ObservableCollection<PeriodSetting> Years
        {
            get { return _years; }
            set
            {
                _years = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region "Commands"
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
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand(Print, CanPrint);
                }
                return _printCommand;
            }
        }
        #endregion
        #region "Commands Methods"
        void Search()
        {
            int code;
            string s = txtMemberCode.Text;
            if (int.TryParse(s, out code) && SelectedYear != null)
            {
               LoadStatment(code, SelectedYear);
               RaisePropertyChanged("");
            }
        }

        
        void Print()
        {
            
            ExcelMail exm = new ExcelMail();
            string path = Properties.Settings.Default.MemberStatmentTemplatePath;
            string pdfPath = Properties.Settings.Default.PdfsFolder;
            try
            {
                exm.SendMemberStatment(Statment, path, pdfPath);
            }
            catch (Exception ex)
            {

                Helper.LogAndShow(ex);
            }
            

            
        }
        bool CanPrint()
        {
            return Statment != null && Statment.DetailsTable != null && Statment.DetailsTable.Rows.Count > 0;
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



        private void LoadStatment(int memberCode, PeriodSetting year)
        {
            _internalStatementDetails = new HashSet<MemberStatmentDetail>();
            List<MemberStatmentDetail> msdList = new List<MemberStatmentDetail>();
            string statementYear = null;
            int searchedMemberCode = 0;
            string searchedMemberName = null;
            decimal netTotals = 0.0M;

            var yearPayments = _unitOfWork.Payments.Query(x => x.PeriodSetting.Id == year.Id);
            int fieldsCounter = 0;
            foreach (var yearPayment in yearPayments)
            {
                if(fieldsCounter == 0) //Assgin once
                {
                    statementYear = yearPayment.PeriodSetting.YearPart;
                }

                var pymentDetails = _unitOfWork.PaymentTransactions.Query
                    (
                        x => x.Payment.PaymentNo == yearPayment.PaymentNo
                        &&
                        x.FamilyMember.Code == memberCode
                    );
                foreach (var payDetail in pymentDetails)
                {
                    if (fieldsCounter == 0) //Assgin once
                    {
                        searchedMemberCode = payDetail.MemberCode;
                        searchedMemberName = payDetail.FamilyMember.FullName;
                        fieldsCounter++;
                    }
                    //Get loans paying history for this payment detail.
                    var loanHistory = _unitOfWork.LoanPayments.Query(
                        x => x.PaymentTransaction.TransNo == payDetail.TransNo
                            &&
                            x.FamilyMember.Code == payDetail.FamilyMember.Code).ToList();

                    int loansHisotoryCount = loanHistory.Count();
                    switch (loansHisotoryCount)
                    {
                        case 0:
                            AddNoLoansHeaderRow(yearPayment, payDetail);
                            netTotals += payDetail.NetPayments;
                            continue;
                        case 1 :
                            AddOneLoanRow(yearPayment, payDetail, loanHistory);
                            netTotals += payDetail.NetPayments;
                            continue;
                        default:
                            AddManyLoansRow(yearPayment, payDetail, loanHistory);
                            netTotals += payDetail.NetPayments;
                            break;
                    }
                }
            }
            Statment = new MemberStatmentReport(
                statementYear, searchedMemberCode, searchedMemberName, netTotals, _internalStatementDetails);

        }

        void AddNoLoansHeaderRow(Payment yearPayment, PaymentTransaction payDetail)
        {
            MemberStatmentDetail d = new MemberStatmentDetail(
                            "1433-01-01", yearPayment.PaymentSequence.SequenceDescription,
                            yearPayment.Amount, payDetail.ShareNumbers, 0.0M, string.Empty, payDetail.NetPayments, true);
            _internalStatementDetails.Add(d);
        }
        void AddOneLoanRow(Payment yearPayment, PaymentTransaction payDetail, List<LoanPayment> histories)
        {
            var loansTotal = histories.Sum(x => x.AmountPaid);
            MemberStatmentDetail msdHeader = new MemberStatmentDetail
                                    (
                                        "1433-01-01", yearPayment.PaymentSequence.SequenceDescription,
                                        yearPayment.Amount, payDetail.ShareNumbers, histories[0].AmountPaid, histories[0].Loan.LoanType.LoanDescription,
                                        payDetail.NetPayments, false
                                    );
            MemberStatmentDetail msdTotal = new MemberStatmentDetail(loansTotal, "الإجمالي", true);
            _internalStatementDetails.Add(msdHeader);
            _internalStatementDetails.Add(msdTotal);

        }
        void AddManyLoansRow(Payment yearPayment, PaymentTransaction payDetail, List<LoanPayment> histories)
        {
            bool isFirstRow = true;
            bool isLastRow = false;
            int counter = 0;
            var loansTotal = histories.Sum(x => x.AmountPaid);
            foreach (var history in histories)
            {

                if (isFirstRow)
                {
                    MemberStatmentDetail msd = new MemberStatmentDetail
                        (
                        "1433-01-01", yearPayment.PaymentSequence.SequenceDescription,
                        yearPayment.Amount, payDetail.ShareNumbers, history.AmountPaid,
                        history.Loan.LoanType.LoanDescription,
                        payDetail.NetPayments, false
                        );
                    _internalStatementDetails.Add(msd);
                    isFirstRow = false;
                    counter++;
                    continue;
                }

                isLastRow = ((histories.Count() - 1) == counter);

                if (!isLastRow)
                {
                    var m = new MemberStatmentDetail(history.AmountPaid, history.Loan.LoanType.LoanDescription, false);
                    _internalStatementDetails.Add(m);
                    counter++;
                    continue;
                }
                else
                {
                    var mm = new MemberStatmentDetail(history.AmountPaid, history.Loan.LoanType.LoanDescription, false);
                    var mmm = new MemberStatmentDetail(loansTotal, "الأجمالي", true);
                    _internalStatementDetails.Add(mm);
                    _internalStatementDetails.Add(mmm);
                }
            }
        }


    }
}
