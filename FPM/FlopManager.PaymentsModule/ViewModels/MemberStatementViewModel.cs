using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.PaymentsModule.DTOs;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Regions;

namespace FlopManager.PaymentsModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MemberStatementViewModel:ListViewModelBase
    {
        #region "Fields"
        FamilyContext _dbContext;
        private ILogger _logger;
        private ISettings _setting;
        DbSet<PeriodYear> _repoPeriodSetting;

        MemberStatmentReport _statment;
        PeriodYear _selectedYear;
        ObservableCollection<PeriodYear> _years;
        HashSet<MemberStatmentDetail> _internalStatementDetails;
        #endregion
        [ImportingConstructor]
        public MemberStatementViewModel(FamilyContext dbContext, ILogger logger, ISettings settings)
        {

            _dbContext = dbContext;
            _logger = logger;
            _setting = settings;
            _repoPeriodSetting = _dbContext.PeriodYears;
            CanClose = true;
            Title = ViewModelsTitles.MEMBER_STATMENT;

            WindowLoaded();
        }

        void WindowLoaded()
        {
            var r = _repoPeriodSetting.ToList();
            Years = new ObservableCollection<PeriodYear>(r);
        }



        #region "Properties"
        public MemberStatmentReport Statment
        {
            get
            {
                return _statment;
            }
            set { SetProperty(ref _statment, value); }
        }
        public PeriodYear SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
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
        #endregion

        #region "Commands"
        
        #endregion
        #region "Commands Methods"

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        protected override void Search(object term)
        {
            int code;
            string s = "";//txtMemberCode.Text;
            if (int.TryParse(s, out code) && SelectedYear != null)
            {
                LoadStatment(code, SelectedYear);
                OnPropertyChanged("");
            }
        }

        protected override bool CanSearch(object term)
        {
            throw new NotImplementedException();
        }


        protected override void Print()
        {

            //ExcelMail exm = new ExcelMail();
            string path = "";//Properties.Settings.Default.MemberStatmentTemplatePath;
            string pdfPath = "";//Properties.Settings.Default.PdfsFolder;
            try
            {
                //exm.SendMemberStatment(Statment, path, pdfPath);
            }
            catch (Exception ex)
            {

                Helper.LogAndShow(ex);
            }



        }

        protected override bool CanPrint()
        {
            return Statment != null && Statment.DetailsTable != null && Statment.DetailsTable.Rows.Count > 0;
        }
        #endregion

        



        private void LoadStatment(int memberCode, PeriodYear year)
        {
            _internalStatementDetails = new HashSet<MemberStatmentDetail>();
            List<MemberStatmentDetail> msdList = new List<MemberStatmentDetail>();
            string statementYear = null;
            int searchedMemberCode = 0;
            string searchedMemberName = null;
            decimal netTotals = 0.0M;

            var yearPayments = _dbContext.Payments.Where(x => x.PeriodYear.Year == year.Year);
            int fieldsCounter = 0;
            foreach (var yearPayment in yearPayments)
            {
                if (fieldsCounter == 0) //Assgin once
                {
                    statementYear = yearPayment.PeriodYear.Year;
                }

                var pymentDetails = _dbContext.PaymentTransactions.Where
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
                    //var loanHistory = _unitOfWork.LoanPayments.Where(
                    //    x => x.PaymentTransaction.TransNo == payDetail.TransNo
                    //        &&
                    //        x.FamilyMember.Code == payDetail.FamilyMember.Code).ToList();

                    //int loansHisotoryCount = loanHistory.Count();
                    //switch (loansHisotoryCount)
                    //{
                    //    case 0:
                    //        AddNoLoansHeaderRow(yearPayment, payDetail);
                    //        netTotals += payDetail.NetPayments;
                    //        continue;
                    //    case 1:
                    //        AddOneLoanRow(yearPayment, payDetail, loanHistory);
                    //        netTotals += payDetail.NetPayments;
                    //        continue;
                    //    default:
                    //        AddManyLoansRow(yearPayment, payDetail, loanHistory);
                    //        netTotals += payDetail.NetPayments;
                    //        break;
                   // }
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