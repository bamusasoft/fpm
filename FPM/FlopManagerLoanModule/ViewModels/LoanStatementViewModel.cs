using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;
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
        private FamilyMember _member;
        private ObservableCollection<PaymentSequence> _paymentSequences;
        private PaymentSequence _selectedSequence;
        private ObservableCollection<PeriodYear> _years;
        private PeriodYear _selectedYear;
        private ObservableCollection<LoanStatementDto> _statement;
        private bool _includePaidLoans;
        //
        private DelegateCommand _showLoanStatementCommand;
        #endregion
        [ImportingConstructor]
        public LoanStatementViewModel(ILogger logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;
            CanClose = true;

        }

        #region "Proeprties"


        public FamilyMember Member
        {
            get { return _member; }
            set { SetProperty(ref _member, value); }
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
            set
            {
                SetProperty(ref _selectedYear, value);
                try
                {
                    PaymentSequences = new ObservableCollection<PaymentSequence>(GetYearSequences(SelectedYear));
                }
                catch (Exception ex)
                {
                    Helper.HandleUiException(ex, _logger, RaiseNotification);
                }
            }
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

        public ICommand ShowLoanStatementCommand
        {
            get { return _showLoanStatementCommand ?? (_showLoanStatementCommand = new DelegateCommand(LoadLoanStatement)); }
        }



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
                string s = term as string;
                if(string.IsNullOrEmpty(s))return;
                if (int.TryParse(s, out var code))
                {
                    FamilyContext db = new FamilyContext();
                    Member = db.FamilyMembers.Single(m => m.Code == code);
                    if (Member != null)
                    {
                        var yearsOfMemberLoans = db.Loans.Where(x => x.MemberCode == Member.Code).Select(x => x.PeriodYear).Distinct();
                        Years = new ObservableCollection<PeriodYear>(yearsOfMemberLoans);
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

        private void LoadLoanStatement()
        {
            try
            {
               string sqlQuery = @"With P AS
                (Select LoanPayments.LoanNo, Sum(LoanPayments.AmountPaid) As AmountPaid 
                 FROM LoanPayments GROUP BY LoanPayments.LoanNo)	
                 SELECT Loans.LoanNo, Loans.Description, Loans.MemberCode, FamilyMembers.FullName, Loans.LoanTypeCode, LoanTypes.LoanDescription,
	                    Loans.Year, Loans.PaySeqDue, PaymentSequences.SequenceDescription,
                        Loans.Amount, P.AmountPaid, Loans.Remarks, Loans.Status
                FROM Loans
                INNER JOIN FamilyMembers on Loans.MemberCode = FamilyMembers.Code
                INNER JOIN PaymentSequences on Loans.PaySeqDue = PaymentSequences.Id
                INNER JOIN LoanTypes on Loans.LoanTypeCode = LoanTypes.Code
                LEFT JOIN P on Loans.LoanNo = P.LoanNo";
                var query = BuildQuery();
                string whereClause = "";
                object[] parameters = query.Values.ToArray();
                int counter = 0;
                foreach (var item in query)
                {
                    if (counter == 0)
                    {
                        whereClause = " WHERE " + item.Key;
                    }
                    else
                    {
                        whereClause += " AND " + item.Key;
                    }
                    counter++;
                }
                sqlQuery += whereClause;
                sqlQuery += BuildOrderBy();
                FamilyContext db = new FamilyContext();
                var s = db.Database.SqlQuery<LoanStatementDto>(sqlQuery, parameters).ToList();
                Statement = new ObservableCollection<LoanStatementDto>(s);
            }
            catch (Exception ex)
            {
                Helper.HandleUiException(ex, _logger, RaiseNotification);
            }
        }
        #region Helpers

        private IList<PaymentSequence> GetYearSequences(PeriodYear year)
        {
            if (year == null) throw new ArgumentException("year");
            FamilyContext db = new FamilyContext();
            return db.PaymentSequences.Where(x => x.Year == year.Year).ToList();

        }

        #endregion
        #endregion

        #region SQL QUERIES
        private Dictionary<string, SqlParameter> BuildQuery()
        {
            Dictionary<String, SqlParameter> query = new Dictionary<string, SqlParameter>();
            if (Member != null)
            {
                query.Add("Loans.MemberCode = @MemberCode ", new SqlParameter("@MemberCode", Member.Code));
            }
            if (SelectedYear != null)
            {
                query.Add("Loans.Year = @Year", new SqlParameter("@Year", SelectedYear.Year));

            }
            if (SelectedSequence != null)
            {
                query.Add("Loans.PaySeqDue = @PaySeqDue", new SqlParameter("@PaySeqDue", SelectedSequence.Id));
            }
            if (IncludePaidLoans)
            {
                byte paid = 1;
                query.Add("Loans.Status = @Status", new SqlParameter("@Status", paid));

            }
            else
            {
                byte notPaid = 0;
                query.Add("Loans.Status = @Status", new SqlParameter("@Status", notPaid));
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
