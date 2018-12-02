using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.DTOs;
using Prism.Commands;
using Prism.Regions;
using WordMail = FlopManagerLoanModule.ModuleServices.WordMail;

namespace FlopManagerLoanModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanStatementViewModel:ListViewModelBase
    {
        #region "Fields"
        private readonly ILogger _logger;
        private readonly ISettings _settings;
        private string _memberCode;
        private ObservableCollection<PaymentSequence> _paymentSequences;
        private PaymentSequence _selectedSequence;
        private ObservableCollection<PeriodYear> _years;
        private PeriodYear _selectedYear;
        private ObservableCollection<LoanStatementDto> _statement;
        private bool _includePaidLoans;

        #endregion

        public LoanStatementViewModel(ILogger logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;

        }

        #region "Proeprties"


        public string MemberCode
        {
            get { return _memberCode; }
            set { SetProperty(ref _memberCode, value); }
        }
        public ObservableCollection<PeriodYear> Years
        {
            get { return _years; }
            set
            {
                SetProperty(ref _years, value);
            }
        }

        public PeriodYear SelectedYear
        {
            get { return _selectedYear; }
            set { SetProperty(ref _selectedYear, value); }
        }
        public ObservableCollection<PaymentSequence> PaymentSequences
        {
            get { return _paymentSequences; }
            set { SetProperty(ref _paymentSequences, value); }
        }

        public PaymentSequence SelectedSequence
        {
            get { return _selectedSequence; }
            set { SetProperty(ref _selectedSequence, value); }
        }
        public ObservableCollection<LoanStatementDto> Statement
        {
            get { return _statement; }
            set
            {
                SetProperty(ref _statement, value);
            }
        }

        public bool IncludePaidLoans
        {
            get { return _includePaidLoans; }
            set { SetProperty(ref _includePaidLoans, value); }
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
            try
            {
                int code = (int) term;
                FamilyContext db = new FamilyContext();
                var member = db.FamilyMembers.Single(m => m.Code == code);
                if (member != null)
                {
                    var yearsOfMemberLoans = db.Loans.Where(x => x.MemberCode == member.Code).Select(m =>
                        new 
                        {
                            LoanYear = m.Year
                        });
                    foreach (var yearsOfMemberLoan in yearsOfMemberLoans)
                    {
                        var y = db.PeriodYears.SingleOrDefault(x => x.Year == yearsOfMemberLoan.LoanYear);
                        if (y != null)
                        {
                            Years.Add(y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.HandleUiException(ex, _logger, RaiseNotification);
            }
            
        }

        protected override bool CanSearch(object term)
        {
            return true;
        }

        protected override void Print()
        {
            string fileName = ""; //Properties.Settings.Default.LoansStatementTemplatePath;
            string pdf = ""; //Properties.Settings.Default.PdfsFolder;
            WordMail wm = new WordMail();
            try
            {
                //wm.Send(Statement, fileName, pdf);
            }
            catch (Exception ex)
            {

                Helper.HandleUiException(ex, _logger, RaiseNotification);
            }


        }

        protected override bool CanPrint()
        {
            return true;
        }

        #endregion

        #region SQL QUERIES

        private static string _sqlQeury =
            @"With P AS
                (Select LoanPayments.LoanNo, Sum(LoanPayments.AmountPaid) As AmountPaid 
                 FROM LoanPayments GROUP BY LoanPayments.LoanNo)	
                 SELECT Loans.LoanNo, Loans.Description, Loans.MemberCode, FamilyMembers.FullName, Loans.LoanTypeCode, LoanTypes.LoanDescription,
	                    Loans.Year, Loans.PaySeqDue, PaymentSequences.SequenceDescription,
                        Loans.Amount, P.AmountPaid, Loans.Remarks, Loans.Status
                FROM Loans
                INNER JOIN FamilyMembers on Loans.MemberCode = FamilyMembers.Code
                INNER JOIN PaymentSequences on Loans.PaySeqDue = PaymentSequences.Id
                INNER JOIN LoanTypes on Loans.LoanTypeCode = LoanTypes.Code
                INNER JOIN P on Loans.LoanNo = P.LoanNo";

        private Dictionary<string, SqlParameter> BuildQuery()
        {
            Dictionary<String, SqlParameter> query = new Dictionary<string, SqlParameter>();
            if (!string.IsNullOrEmpty(MemberCode))
            {
                query.Add("Loans.MemberCode = @MemberCode ", new SqlParameter("@MemberCode", MemberCode));
            }
            if (!string.IsNullOrEmpty(Year))
            {
                query.Add("Loans.Year = @Year", new SqlParameter("@Year", Year));

            }
            if (SelectedSequence != null)
            {
                query.Add("Loans.PaySeqDue = @PaySeqDue", new SqlParameter("@PaySeqDue", SelectedSequence.Id));
            }
            if (IncludePaidLoans)
            {
                query.Add("Loans.Status = @Status", new SqlParameter("@Status", 1));

            }
            else
            {
                query.Add("Loans.Status = @Status", new SqlParameter("@Status", 0));
            }
            return query;

        }
        private string BuildOrderBy()
        {
            string orderBy = " ORDER BY Loans.Year, Loans.PaySeqDue";
            return orderBy;
        }
        #endregion


    }
}
